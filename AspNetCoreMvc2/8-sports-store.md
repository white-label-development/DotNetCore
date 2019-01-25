### 8- SportsStore "sing along"

#### Setup

Built in DI via Startup is new. eg: `services.AddTransient<IProductRepository, FakeProductRepository>()`

Startup routes eg: `routes.MapRoute(  name: "default", template: "{controller=Product}/{action=List}/{id?}");`

"The UseMvc method sets up the MVC middleware, and one of the configuration options is the scheme that will be used to map URLs to controllers and action methods."