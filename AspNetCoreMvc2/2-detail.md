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

#### Web Artifacts

 The convention is that third-party CSS and JavaScript packages are installed into the wwwroot/lib folder. Bootstrap etc..

#### Startup 

Learn more about these changes, such as ` services.AddMvc()`, `app.UseMvcWithDefaultRoute();`

`app.UseDeveloperExceptionPage();` is self explanatory.

`app.UseBrowserLink();` enables BrowserLink (which used to be awful. Revisit this?)


#### Quick Razor Notes

 You can specify a set of namespaces that should be searched for types by adding a view imports file to the project. The view imports file is placed in the Views folder and is named `_ViewImports.cshtml`

#### Managing Software Packages 

the Microsoft.AspNetCore.All package is a meta-package, which contains all the individual nuget packages required by asp .net Core and the MVC framework, which means you don’t need to add packages one by one. When you publish your application, any individual packages that are part of the meta-package but not used by the application will be removed, ensuring that you don’t deploy more packages that you need.
The NuGet tool keeps track of the project’s packages in the \{projectname\}.csproj file, eg:

```
<ItemGroup>  
	<PackageReference Include="Microsoft.AspNetCore.All" Version="2.0.0" /> 
</ItemGroup> 
```
 You can also edit the .csproj file directly and Visual Studio will detect changes and download and install the packages added (cool!)


#### Bower

Bower packages are specified through the bower.json file.  the repository for Bower packages is http://bower.io/search, where you can search for packages to add to your project.


#### Static Content Delivery 

add `app.UseStaticFiles();` to the Startup class for delivering static files from the wwwroot folder to clients.

#### Bundling and Minifying

add extension:  Tools > Extensions and Updates > Bundler & Minifier

Select files in the order they should be.
As you perform bundling and minification operations, 
the extension keeps a record of the files that have been processed in a file called bundleconfig.json in the root folder of the project.

The extension automatically monitors the input files for changes and regenerates the output files when there are changes, ensuring that any edits you make are reflected in the bundled and minified files. 
(versioning ??? v1.1.2 etc)




