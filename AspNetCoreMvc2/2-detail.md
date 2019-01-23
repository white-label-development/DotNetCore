## 2 - Detail

#### New Tag Helpers, eg:
`<a asp-action="RsvpForm">RSVP Now</a>`

`<label asp-for="Name">Your name:</label>`            


see Jon Hilton's Cheat Sheet at <https://jonhilton.net/aspnet-core-forms-cheat-sheet/>


#### Controller return types

Original MVC controller actions return a `IActionResult` wheras the demo returns a `ViewResult`. 

Todo: be clear about the difference

#### Validation (same so far)
" MVC supports declarative validation rules defined with attributes from the System.ComponentModel.DataAnnotations namespace, meaning that validation constraints are expressed using the standard C# attribute features"

MVC automatically detects the attributes and uses them to validate data during the model-binding process
