## Blazor Components

https://github.com/GillCleeren/BethanysPieShopHR/tree/CreatingComponents


```
//startup (server version) - define blazor middleware and set host
app.UseEndpoints(endpoints =>
{
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
}); // _Host.cshtml is the root razor page
```

//startup (WASM)
```
app.AddComponent<App>("app"); // one line in Configure()
```

From Host define App component
```
<app>
    @(await Html.RenderComponentAsync<App>(RenderMode.ServerPrerendered))
</app>
```

Render Modes

+ Static - no dynamic update
+ Server - placeholder marker tag set, then updated by SignalR
+ ServerPrerendered - intially static, then updated

App Component contains the chile `Router` component. It will scan the given assembly for pages and is reponsponsible the rendering pages that match a route.
```
//  App.razor
<Router AppAssembly="@typeof(Program).Assembly">
```

route -> page + MainLayout

Libraries (for components) via 'Razor Class Library' template. Can be packed as Nuget packages.

`_Imports.razor` used by all component in the  project. (inline using also works)

### Events (required .AspNetCore.Components.Web namespace to be using)

@onX EventCallBack is a wrapper around a delegate. 

Can take a lambda: 
```
@for (var i=1; i<4; i++>{
    var num = i;
    <button @onclick="@(x => UpdateHeading(x, num))" ...
})
```

### Child Content

In example, in ProfilePictureBase added 

```
[Parameter]
public RenderFragment ChildContent {get;set;} // must be called ChildContent!
```

Then in ProfilePicture.razor
`<div>@ChildContent</div>`

Then in Index.razor
```
<ProfilePicture>This text gets injected down</ProfilePicture>
```

Tip: In a for each statement, as soon as State is involved. move from inline to a component.
```
@foreach (var employee in Employees)
{
    <EmployeeRow Employee="@employee"></EmployeeRow>
}
```

Components can use IDisposable. A razor IF will create and destroy. Consider **hiding** if we don't want to create a new instance on each event.

### @key

Help out diffing mechanism by assigning keys to elements in a list (that you will be changing)
```
 @foreach (var benefit in Benefits)
{
    <tr @key="benefit">
```
or `<tr @key="benefit.Id">`

### two way binding

`<input type="checkbox" checked="@benefit.Selected" @onchange="e => CheckBoxChanged(e, benefit)" />`

```
public async Task CheckBoxChanged(ChangeEventArgs e, BenefitModel benefit)
{
    var newValue = (bool)e.Value;
    benefit.Selected = newValue;
    ...
```

Also "simple" two way (directly to underlying object). Cannot use this with @onchange

`<input type="checkbox" @bind="benefit.Selected" />`

Also, `@bind-value="obj.valX"` and `@bind-value:event="onX"`


### events

```
[Parameter]
public EventCallback<bool> OnPremiumToggle { get; set; }
...
await OnPremiumToggle.InvokeAsync(Benefits.Any(b => b.Premium && b.Selected));
```

and in parent
```
<BenefitSelector Employee="Employee" OnPremiumToggle="PremiumToggle" />

//in parent Base, raised event sets  property on Employee
public void PremiumToggle(bool premiumBenefit)
{
    Employee.HasPremiumBenefits = premiumBenefit;
}
```

### chained binds

binding a child components property to a property of the parent component.

DateField component code along as I don't get it atm:

// BenefitSelector.razor 2 way direct binding
`<input type="date" @bind="benefit.EndDate" />

Requirement to addd a 'revert' button to the rhs of the input that will reset the date. The two being together suggests we should make a component for this. (see DateField in component library).

// DateField.razor can't use direct two way as a)don't want to couple with benefit, b) we want to do more than just bind, so @onchange

`<input type="date" value="@Date?.ToString(dateFormatString)" format-value="@dateFormatString" @onchange="OnDateChanged" />`

```
// DateField.razor

[Parameter]
public DateTime? Date { get; set; }

[Parameter]
public EventCallback<DateTime?> DateChanged { get; set; } // MUST be called {Propertyname}Changed

public async Task OnDateChanged(ChangeEventArgs e)
{
    var date = (string)e.Value;
    var newDate = DateTime.Parse(date);
    Date = newDate;
    await DateChanged.InvokeAsync(newDate); //invoke event callback to notify parent that the date has changed
}
```

then in BenefitSelector.razor (parent). There is now 2 way binding between the event property in the date field and benefit.EndDate

`<DateField @bind-Date="benefit.EndDate" />` This is convention based - @bind-x tells blazor the property is called date and the callback therefore is DateChanged

EditForm. InputText etc. Theses all use chained binds
`<InputText id="lastName" class="form-control" @bind-Value="@Employee.LastName" placeholder="Enter last name"></InputText>`
The value property of each component is bound to a property in the model

### atribute splatting
Can be splatted using a dictionary

```
// child 
<input id="useAttributesDict" @attributes="InputAttributes" />

@code {
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> InputAttributes { get; set; }
}

// parent            
var inputAttributes = new Dictionary<string, object>()
{
    { "maxlength", "10" },
    { "placeholder", "Input placeholder text" },
    { "required", "required" },
    { "size", "50" }
};   

<AttributeSplatInput type="date" InputAttributes="inputAttributes"></AttributeSplatInput>
// date is a custom attribute that will be added to the dictionary (if we have CaptureUnmatchedValues = true)
```

### Cascading Values

Makes it possible to make values and objects available to all components without the need to pass them on specifically

//App.razor
```
<CascadingValue Value="@buttonClass" IsFixed="true">
    <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
</CascadingValue>

private string buttonClass = "btn-danger"l
```
`IsFixed` disables change tracking / increases performance.

xBase.cs
```
[CascadingParameter]
public string BtnClass { get; set; } //match is not done on name, but the type of the value and it's position in the value hierarchy. Simpler to put string buttonClass into a class where the type can be better matched, eg:

[CascadingParameter]
public MyThemeDto Theme { get; set; } 
```


### ref

//EmpoyeeOverviews needs a reference to AddEmployeeDialog to call the Show and Close methods. ref="AddEmployeeDialog" sets the variable name in the code
`<AddEmployeeDialog @ref="AddEmployeeDialog" CloseEventCallback="@AddEmployeeDialog_OnDialogClose"></AddEmployeeDialog>`

//in the base
` protected AddEmployeeDialog AddEmployeeDialog { get; set; }` set from ref I think

### Templated Components


```
@button

@dateInput(Date)

@code {
    private RenderFragment button = @<button ... />;

    private RenderFragment<DateTime?> dateInput = (date) => @<input type="date" value="@xxxx" ... /> <-- incomplete example neil! non-generic is prefered -->
}
```

#### Component Library TableTemplate:

` public partial class TableTemplate<TItem>`


//BenefitSelector.razor 

swap out html `<table>` for

```
<TableTemplate Items="Benefits" IsSmall="true"  TItem="BenefitModel" Context="benefit">
    <TableHeader>
        <th class="w-auto">Benefit</th>
        <th class="w-25">Start date</th>
        <th class="w-25">End date</th>
    </TableHeader>
    <RowTemplate>
        <td>
            <input type="checkbox" checked="@benefit.Selected"
                    @onchange="e => CheckBoxChanged(e, benefit)" />
        </td>
        <td>@benefit.Description</td>

        @if (benefit.Selected)
        {
            <td>@benefit.StartDate</td>
            <td>
                <DateField @bind-Date="benefit.EndDate" />
            </td>
        }
        else
        {
            <td></td>
            <td></td>
        }
    </RowTemplate>
</TableTemplate>
```

got lost here. revisit templates and generic templates. Done for now.
