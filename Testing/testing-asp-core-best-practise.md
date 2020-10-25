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