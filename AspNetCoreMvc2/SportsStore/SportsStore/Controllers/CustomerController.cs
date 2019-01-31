using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;

namespace SportsStore.Controllers
{
    public class CustomerController : Controller
    {
        [Route("myroute")]
        public ViewResult Index() => View("Result",
            new Result
            {
                Controller = nameof(CustomerController),
                Action = nameof(Index)
            });

        public ViewResult List(string id)
        {
            Result r = new Result
            {
                Controller = nameof(RoutingHomeController),
                Action = nameof(List),
            };
            r.Data["id"] = id ?? "<no value>";
            r.Data["catchall"] = RouteData.Values["catchall"];
            return View("Result", r);
        }
    }
}