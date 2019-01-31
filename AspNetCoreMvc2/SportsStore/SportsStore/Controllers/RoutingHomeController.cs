using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;

namespace SportsStore.Controllers
{
    public class RoutingHomeController : Controller
    {
        public ViewResult Index() => View("Result",
            new Result
            {
                Controller = nameof(RoutingHomeController),
                Action = nameof(Index)
            });

        public ViewResult CustomVariable(string id)
        {            
            Result r = new Result
            {
                Controller = nameof(RoutingHomeController),
                Action = nameof(CustomVariable),
            };
            r.Data["Id"] = id ?? "<no value>";
            return View("Result", r);
        }
    }
}