using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SportsStore.Infrastructure;
using SportsStore.Models;

namespace SportsStore.Controllers
{
    public class NeilController : Controller
    {
        private readonly Neil _neilModel;
        private ILogger<NeilController> logger;

        public NeilController(Neil neilModel, ILogger<NeilController> lo)
        {
            _neilModel = neilModel;
            logger.LogDebug($"Handled {Request.Path} at uptime {_neilModel.Uptime}");
            //logger.LogCritical("ouch");
        }

        public IActionResult Index()
        {
            return View();
        }

        public static int DescendingOrder(int num)
        {
            if (num < 0) throw new ArgumentException("num must be non-negative");

            // Bust a move right here
            var numbers = num.ToString().Select(c => (int)Char.GetNumericValue(c));


            string numStr = numbers.OrderByDescending(x => x).Aggregate("", (current, i) => current + i);


            return int.Parse(numStr);
        }


        public string Uptime() => _neilModel.Uptime;
    }
}