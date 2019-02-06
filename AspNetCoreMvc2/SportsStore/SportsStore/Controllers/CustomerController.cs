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

        //Remote Validation Action example
        //used as an attribute in the model, eg:

        /*
         * [Remote("ValidateDate", "Customer")]
         *  public DateTime Date { get; set;}
         *
         *  the validation action method will be called when the user first submits the form and then again each time the data is edited.
         * For text input elements, every keystroke will lead to a call to the server
         */

        public JsonResult ValidateDate(string Date)
        {
            DateTime parsedDate;
            if (!DateTime.TryParse(Date, out parsedDate))
            {
                return Json("Please enter a valid date (mm/dd/yyyy)");
            }
            else if (DateTime.Now > parsedDate)
            {
                return Json("Please enter a date in the future");
            }
            else
            {
                return Json(true);
            }
        }
    }
}