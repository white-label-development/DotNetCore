## Blazor Intro

WebAssembly (WASM) | Server

Write for server and convert to client-side later.

### Lifecycle
OnInitializedAsync, OnParametersSetAsync, OnAfterRender

### Codel along for the Pie Shop Demo Notes

### 2

Note: intellisense was not working for me. Added a `global.json' (to same location as .sln) that specifiec the current SDK. Restarted VS = fixed.

Razor pages supports "mixed" approach = inline via `@code { ... }` for trivial code, and code behind (`xBase:CompnentBase` )for non-trivial.

In `EmployeeOverviewBase`` used `protected override Task OnInitializedAsync()` - NOT ctor.

`services.AddServerSideBlazor().AddCircuitOptions(op => { op.DetailedErrors = true; });` // in browser

### 3 Data

Use `IHttpClientFactory` rather than `System.Net.Http.HttpClient`.
There is also (I think) a client side HttpClient (comae back to this once WASM stuff is 1.0)

`services.AddHttpClient();` for normal ctor injection but also component injection

```
// typed httpclient via Factory
services.AddHttpClient<IEmployeeDataService, EmployeeDataService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:44340/");
});
```

```
 public class EmployeeOverviewBase : ComponentBase
    {
        [Inject]
        public IEmployeeDataService EmployeeDataService { get; set; }
```

One-way binding: `hello @Employee.FirstName`

Two-way binding: `<input @bind="@Employee.LastName" ... />` - value gets update when user taps out of the input. Can override using `@bind-value:event="oninput"` in this example any client value would update while typing.

Baked in Input Components, eg: InputText, InputDatem InputSelect etc..


```
[Inject]
public NavigationManager NavigationManager { get; set; } //built in component
...
 NavigationManager.NavigateTo("/employeeoverview");
```

Validation is built in.
Added System.ComponentModel.Annotation Nuget pck to .Shared (where the models are)

### 4 Adding features

Component can be nested. Parent can pass parameters to child. child can raise events.

Components can be in-project or in a separate library (and re-used)

Use of `StateHasChanged()` to inform Base of state change

#### Using Javascript components

Javascript InterOp - Call into JS from Blazor (and reverse). (There is a blazor interop course)

[Inject] the `IJSRuntime`

```
<script>
    window.DoSomething = () => { // do something}
</script>

var result = await JsRuntime.InvokeAsync<object>("DoSomething","");
```

See Map example in EmployeeDetail for an example.
Note us of `_content` in `_Host.cshtml` to access resources in the compnent library

`<link href="_content/BethanysPieShopHRM.ComponentsLibrary/leaflet/leaflet.css" rel="stylesheet" />`


### 5 Client Side / WASM Blazor

Convert server side app to client side app.

.Server really becomes .Core (name does not change in demo) and two 'anemic' project .ServerApp and 
.ClientApp (new project of type Blazor WebAssembly App) are made, referencing .Server.

.Server has _Host.cshtml and project type changed (how?) to a Razor Class Library

Currently IHttpClientFactory can't be used in ClientApp. Just used HttpClient directly


### 6 Deployment

Azure SignalR Service (free. Needs Ms.Azure.SignalR nuget, startup services updated, connection string), x2 Webapps (API and ServerApp)