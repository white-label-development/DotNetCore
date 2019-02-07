﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Users.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ViewResult Index() => View(new Dictionary<string, object> { ["Placeholder"] = "Placeholder" });
    }
}