using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SportsStore.Infrastructure
{
    public class ContentMiddleware
    {
        // Middleware components don’t implement an interface or derive from a common base class.
        // Instead, they define a constructor that takes a RequestDelegate object and define an Invoke method.
        // The RequestDelegate object represents the next middleware component in the chain, and the Invoke method is called when ASP.NET receives an HTTP request.

        private RequestDelegate nextDelegate;
        private UptimeService uptime;

        public ContentMiddleware(RequestDelegate next, UptimeService up)
        {
            nextDelegate = next;
            uptime = up;
        }


        // Information about the HTTP request and the response that will be returned to the client is provided through the HttpContext argument to the Invoke method
        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path.ToString().ToLower() == "/middleware")
            {
                await httpContext.Response.WriteAsync(
                    "This is from the content middleware " +
                    $"(uptime: {uptime.Uptime}ms)", Encoding.UTF8);

                await nextDelegate.Invoke(httpContext); //test. continue must be explicit. this passes to ContentMiddleware2 ok
            }
            else
            {
                await nextDelegate.Invoke(httpContext);
            }
        }
    }

    public class ContentMiddleware2
    {
        // Ensuring we can chain these

        private RequestDelegate nextDelegate;
        

        public ContentMiddleware2(RequestDelegate next) => nextDelegate = next;            
        


        // Information about the HTTP request and the response that will be returned to the client is provided through the HttpContext argument to the Invoke method
        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path.ToString().ToLower() == "/middleware")
            {
                await httpContext.Response.WriteAsync("This is from ContentMiddleware2");
            }
            else
            {
                await nextDelegate.Invoke(httpContext);
            }
        }
    }
}
