# Testing asp.net core applications (Integration Tests)

unit tests (mocks and fakes) > integration > ui tests

integration tests: test multiple components work together, often with in-memory data.

## 2. Demo

Easier to see logs in kestrel.

Add [test folder] TennisBookings.Merchandise.ntegrationTEsts

xUnit test project (.NET Core) prpject type. add `MS.ASPNetCore.Mvc.Testing`. Modify cs.proj SDK to be `MS.NET.Sdk.Web` (gain launchsettings.json etc). Disable shadow copying by adding xUnit configuration file (test execure in different dir to build output - we don't want that). add `xunit.runner.json`

```json
{
    "shaowCopy": false
}
```

add ref to TennisBookings.Merchandise.Api project from test proj.

in TennisBookings.Merchandise.Api startup we have `services.AddHealthChecks();` and `endpoints.MapHealthChecks(/healthcheck)`

(/healthcheck should return 200 ok "Healthy")

add

```c#
public class HealthCheckTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _httpClient;

        public HealthCheckTests(WebApplicationFactory<Startup> factory)
        {
            _httpClient = factory.CreateDefaultClient();
        }

        [Fact]
        public async Task HealthCheck_ReturnsOk()
        {
            var response = await _httpClient.GetAsync("/healthcheck");

            response.EnsureSuccessStatusCode();

            //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
```

In VS: Test Explorer run, or from codelens etc, In CLI: `dotnet test -c Release` (test release with config)

The `WebApplicationFactory` is the class fixture type that setups up stuff for all test methods. Build and run a host. cleans up at end. IClassFixture is xUNit. WAF is a MS.

## 3

```c#
[Fact]
public async Task GetAll_ReturnsExpectedResponse()
{
    var expected = new List<string> { "Accessories", "Bags", "Balls", "Clothing", "Rackets" };

    var model = await _client.GetFromJsonAsync<ExpectedCategoriesModel>(""); // note base url set in factory options

    Assert.NotNull(model?.AllowedCategories);
    Assert.Equal(expected.OrderBy(s => s), model.AllowedCategories.OrderBy(s => s));
}
```

`factory.CreateDefaultClient` default base address is [http://localhost]

`factory.CreateClient` follows redirects and cookies. configured in `WebApplicationFactory.ClientOptions`.

`factory.ClientOptions.BaseAddress = new Uri("http://localhost/api/categories");` in test class ctor.

mod project file with new (.net 5) package:
`<PackageReference Include="System.Net.Http.Json" Version="3.2.1" />`

CategoriesController has `[ResponseCache(Duration = 300)]` can test this

```c#
[Fact]
public async Task GetAll_SetsExpectedCacheControlHeader()
{
    var response = await _client.GetAsync("");
    var header = response.Headers.CacheControl;
    Assert.True(header.MaxAge.HasValue);
    Assert.Equal(TimeSpan.FromMinutes(5), header.MaxAge);
    Assert.True(header.Public);
}
```

## 4

Use of WAF and host builder to inject a fake (ICloudDatabase) into services, to be used by code under test, then tested.

```c#
[Fact]
public async Task GetStockTotal_ReturnsExpectedStockQuantity()
{
    var cloudDatabase = new FakeCloudDatabase(new[]
    {
        new ProductDto{ StockCount = 200},
        new ProductDto{ StockCount = 500},
        new ProductDto{ StockCount = 300}
    });

    var client = _factory.WithWebHostBuilder(builder =>
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<ICloudDatabase>(cloudDatabase);
        });
    }).CreateClient();

    var model = await client.GetFromJsonAsync<ExpectedStockTotalOutputModel>("total");

    Assert.Equal(1000, model.StockItemTotal);
}
```

The real ConfigureServices runs first, registering it's services, and  ConfigureTestServices runs last. This creates a duplicate reg for ICloudDatabase, but the MS DO uses the last one when this happens - which is our fake.

NOTE: rather than StockController returning an IAction result (which it could) being more specific helps swagger

`public async Task<ActionResult<StockTotalOutputModel>> GetStockTotal()`

## 5

create a custom WebApplicationFactory to be shared across test project. Does FakeCloudDatabase registration.

```c#
public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where
    TStartup : class
{
    public FakeCloudDatabase FakeCloudDatabase { get; }
    public CustomWebApplicationFactory()
    {
        FakeCloudDatabase = FakeCloudDatabase.WithDefaultProducts();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<ICloudDatabase>(FakeCloudDatabase);
            services.AddSingleton<IMetricRecorder>(new FakeMetricRecorder());
        });
    }
}
```

neat use of Action on a clone (eg: get Model, then remove name, to set ShouldHaveName)

```c#
public TestProductInputModel CloneWith(Action<TestProductInputModel> changes)
{
    var clone = (TestProductInputModel)MemberwiseClone();
    changes(clone);
    return clone;
}

var foo = model.CloneWith(x => x.Name = null);
```

Use of Theory (method return an Ienumerable)

```c#
[Theory]
[MemberData(nameof(GetInvalidInputs))]
public async Task Post_WithoutName_ReturnsBadRequest(TestProductInputModel productInputModel)
{
    var response = await _client.PostAsJsonAsync("", productInputModel, JsonSerializerHelper.DefaultSerialisationOptions);
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
}

public static IEnumerable<object[]> GetInvalidInputs()
{
    return GetInvalidInputsAndProblemDetailsErrorValidator().Select(x => new[] { x[0] }); // included cos i like this linq which gets the first aray of two in the source
}

// Products POST is interesting too
[HttpPost]
[ProducesResponseType(StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status409Conflict)]        
public async Task<IActionResult> Post(ProductInputModel model)
{
    var product = model.ToProduct();
    var addResult = await _productDataRepository.AddProductAsync(product);

    if (addResult.IsSuccess)
    {
        return CreatedAtAction(nameof(Get), new { id = product.Id }, ProductOutputModel.FromProduct(product));
    }

    if (addResult.IsDuplicate)
    {
        var existingUrl = Url.Action(nameof(Get),
            ControllerContext.ActionDescriptor.ControllerName,
            new { id = product.Id },
            Request.Scheme,
            Request.Host.ToUriComponent());

        HttpContext.Response.Headers.Add("Location", existingUrl);
        return StatusCode(StatusCodes.Status409Conflict);
    }

    ModelState.AddValidationResultErrors(addResult.ValidationResult.Errors);
    return ValidationProblem();
}
```

### Side Effects (like messages sent to a queue, something written to a file)

Given the CloudBasedProductDataRepository it's AddProductasync calls

```c#
 await _cloudDatabase.InsertAsync(product.Id.ToString(), ProductDto.FromProduct(product)); //db
 await _cloudQueue.SendAsync(CreateSendRequest(product)); // Q
```

just fake it as testing the cloud provider is not our shizz

```c#
public class FakeCloudQueue : ICloudQueue
{
    public List<SendRequest> Requests = new List<SendRequest>();

    public Task<SendResponse> SendAsync(SendRequest request)
    {
        Requests.Add(request);
        return Task.FromResult(new SendResponse { IsSuccess = true, StatusCode = 200 });
    }
}
```

### testing custom middleware

see MetricsMiddleware in demo code and FakeMetricRecorder and MetricMiddlewareTests and CustomWebApplicationFactory.

add headers and register another  IcloudDatabse that overrided the other test one

```c#
var factory = _factory.WithWebHostBuilder(builder =>
{
    builder.ConfigureTestServices(services =>
    {
        services.AddSingleton<ICloudDatabase>(new FakeCloudDatabase { ShouldThrow = true });
    });
});

await factory.Server.CreateRequest("/api/products")
    .AddHeader("User-Agent", userAgent)
    .AddHeader("Correlation-Id", correlationId)
    .GetAsync();
```

## 6 ui apps

skipped

## 7 auth in the ui

pretty much skipped.

Disable redirect so we can assert the redirct on challenge is issued.


AdminHomeContollerTests

### ef

SQLLite provider is more feature rich than in-memory database (no raw sql, transactions)

pimped CustomWebApplicationFactory.














