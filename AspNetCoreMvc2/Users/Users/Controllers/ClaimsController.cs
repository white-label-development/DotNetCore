using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Users.Controllers
{
    public class ClaimsController : Controller
    {
        [Authorize]
        public ViewResult Index() => View(User?.Claims);
    }
}