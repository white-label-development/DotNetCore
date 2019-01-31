### URL Routing

 There are two ways to create routes in an MVC application: convention-based routing and attribute routing. 

##### URLs to actions

not much has changed.

###### convention-based routing

` routes.MapRoute(name: "", template: "Public/{controller=Home}/{action=Index}"); ` inline default values.

`routes.MapRoute("", "X{controller}/{action}");` matches XHome/Index

Segment variables are put into RouteData `RouteData.Values["myVal"]; `

```
 app.UseMvc(routes => { 
    routes.MapRoute(name: "MyRoute",
    template: "{controller=Home}/{action=Index}/{id?}/{*catchall}");
 }); 
```
/Customer/List/All/Delete/Perm => controller = Customer, action = List, id = All, catchall = Delete/Perm

inliine contraints (see :int) ` template: "{controller=Home}/{action=Index}/{id:int?}");` 
replace the old `,constraints: new { id = new IntRouteConstraint() }`

regexs: `template: "{controller:regex(^H.*)=Home}/{action=Index}/{id?}");`

Note: Default values are applied before constraints are checked. 
So, for example, if i request the UrL /, the default value for controller, which is Home, is applied. 
the constraints are then checked, and since the controller value begins with H, 
the default UrL will match the route.

Can define custom constraints via `IRouteConstraint`.

###### Attribute Routing

Attribute routing is enabled when you call the UseMvc method in the Startup.cs `app.UseMvcWithDefaultRoute()`. 
Appears to support both convention and attribute routing at the same time (check this).

` [Route("[controller]/MyAction")]`

`[Route("app/[controller]/actions/[action]/{id?}")`

`[Route("app/[controller]/actions/[action]/{id:weekday?}")]`

 Multiple constraints can be applied by chaining them together and separating them with colons.

 ##### generate outgoing URLs 



