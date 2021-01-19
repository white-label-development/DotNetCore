# .NET Unit Testing with AutoFixture

Simplify and reduce Arrange phase.

Anonymous test data.

## 2 Getting Started

`AutoFixture.Xunit2` also `.seedExtensions`, `.AutoMoq` etc

### Demo

new xUnit Test project DemoCode.Tests + nugets

```c#
var fixture = new Fixture();
var anonInt = fixture.Create<int>();
```

## 3 Creating anonymous test data and objects

add AutoFixture.SeedExtensions to allow  better names eg:
`var firstName = fixture.Create<string>("First_");`

```c#
 var enum = fixture.Create<EmailMessageType>() // create an enum in same way
 var email = fixture.Create<EmailAddressLocalPart>().LocalPart; // EmailAddressLocalPart is part of AutoFixture (email up to the @ = localpart)
// also <DomainName>().Domain;
MailAddress email = fixture.Create<MailAddress>(); //.Address for string result
```

## sequences

```c#
IEnumerable<string> messages = fixture.CreateMany<string>(6); // 6 strings

fixture.AddManyTo<string>(sut.Messages, 10); // add 10  to existing

fixture.AddManyTo<string>(sut.Messages, () => "hi"); // add 3(default number) hi string
```

## generate entire object

```c#
MyClass foo = fixture.Create<MyClass>(); // AF will auto pop properties, including entire graph of child types.
```

AF respects annotations, eg; `[StringLength(20)]`, `[Range(0, 100)]`

## 4 Customizing Creation

```c#
var fixture = new Fixture();
fixture.Inject<string>("LHR"); // when asked for a string, will return LHR
var flight = fixture.Create<FlightDetails>();
```

```c#
fixture.Register<string>(() => DateTime.Now.Ticks.ToString()); // use a function
```

```c#
var id = fixture.Freeze<int>(); // sets id and injects into fixture (so requsted Id from fixture will === id)
```

```c#
var flight = fixture.Build<FlightDetails>()
  .Without(x => x.AirportCode)
  .With(x => x.Active, true)
  .Do(x => x.MealOptions.Add("Fish"))
  .Do(x => x.MealOptions.Add("Rice"))
  .Create(); // AirportCode = null, Active = true. Do
```

`.OmitAutoProperties // won't set any props`

```c#
var fixture = new Fixture();

fixture.Customize<FlightDetails>(fd => fd
    .With(x => x.Active, true))); // note no Create()

var flight = fixture.Create<FlightDetails>(); // both build according to template defn above
var flight2 = fixture.Create<FlightDetails>();
```

### specimen

An individual value for a specfic data type used as an example of that data type. `ISpecimenBuilder`

Fixture Pipeline: Customizations (has collection of ISpecimenBuilder), Default Builders ("engine"), Residue Collectors (fall back if no other matches). Goes through chain until a specimen is returned.

Customizations overrride default specimen builders.

```c#
var fixture = new Fixture();

fixture.Customize(new CurrentDateTimeCustomization()); // use an inbuilt customz to create DateTime as now. Essentially adding a customization

// lets make a custom one
public class AirportCodeStringPropertyGenerator: ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context){
        // see if we are trying to create a value for a property
        var propertyInfo = request as PropertyInfo; // via reflection
        if(propertInfo is null){
            // this builder is not relevent to current request. will move to next one.
            return new NoSpecimen(); // as nullis a valid specimen
        }

        var isAirportCodeProperty = propertyInfo.Name.Contains("AirportCode");
        var isString = propertyInfo.PropertyType == typeof(string);
        if(isAirportCodeProperty && isString){
            return RandomAirportCode(); // our custom bit. implemntation not imp.
        }
        return new NoSpecimen();
    }
}

fixture.Customizations.Add(new AirportCodeStringPropertyGenerator());
var flight = fixture.Create<FlightDetails>();
```

^^ Seems like a lot of work to me.

## 5 Less code and better test maintenance

xunit data driven tests can be combine with AF nuget: AutoFixture.NUNit2

```c#
[Theory]
[InlineData (1, 7)]

//InlineData can become
[InlineAutoData]

// InlineData can become
[AutoData] // passes 2 ints and the SUT!
public void xxx( int a, int b, Calculator sut){
    sut.Add(a);
    sut.Add(b);
    //assert ...
}
```

### moq

add nuget AutoFixture.AutoMoq

```c#
fixture.Customize(new AutoMoqCustomization()); // will automake interface dependencies
var mockEmailGateway = fixture.Freeze<Mock<EmailGateway>>(); // get a frozen/injected reference

var sut = fixture.Create<EmailMessageBuffer>();
...
mockEmailGateway.Verify(x => x.Send(), Time.Once());
```

FIN