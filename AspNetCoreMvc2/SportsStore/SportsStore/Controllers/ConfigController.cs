using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SportsStore.Controllers
{
    public class ConfigController : Controller
    {
        public ViewResult Index() => View(new Dictionary<string, string> {["Message"] = "This is the Index action"});
    }
}