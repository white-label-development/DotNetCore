using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Infrastructure;
using SportsStore.Models;

namespace SportsStore.Controllers
{
    public class NeilController : Controller
    {
        private readonly Neil _neilModel;

        public NeilController(Neil neilModel)
        {
            _neilModel = neilModel;
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