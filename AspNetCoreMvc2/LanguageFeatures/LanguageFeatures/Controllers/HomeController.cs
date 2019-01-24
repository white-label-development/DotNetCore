using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageFeatures.Models;
using Microsoft.AspNetCore.Mvc;

namespace LanguageFeatures.Controllers
{
    public class HomeController : Controller
    {
        public ViewResult Index()
        {
            //Using the Null Conditional Operator 
            List<string> results = new List<string>();
            foreach (Product p in Product.GetProducts())
            {
                string name = p?.Name;
                decimal? price = p?.Price;
                string relatedName = p?.Related?.Name; //chained null conditional

                //results.Add(string.Format("Name: {0}, Price: {1}, Related: {2}", name, price, relatedName));
                results.Add($"Name: {name}, Price: {price}, Related: {relatedName}"); //string interpolation

                //  Collection Initializer Syntax 
                Dictionary<string, Product> products = new Dictionary<string, Product>
                {
                    ["Kayak"] = new Product { Name = "Kayak", Price = 275M },
                    ["Lifejacket"] = new Product { Name = "Lifejacket", Price = 48.95M }
                };

            }
            return View(results);
        }


        public ViewResult PatternMatching()
        {
            object[] data = new object[] { 275M, 29.95M, "apple", "orange", 100, 10 };
            decimal total = 0;

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] is decimal d) { total += d; } //The is keyword performs a type check
            }

            return View("Index", new string[] { $"Total: {total:C2}" });
        }

        public ViewResult PatternMatchingInSwitch()
        {
            object[] data = new object[] { 275M, 29.95M, "apple", "orange", 100, 10 };
            decimal total = 0;
            for (int i = 0; i < data.Length; i++)
            {
                // To match any value of a specific type, use the type and variable name in the case statement,
                switch (data[i])
                {
                    case decimal decimalValue:
                        total += decimalValue;
                        break;
                    case int intValue when intValue > 50:
                        total += intValue; break;
                }
            }

            return View("Index", new string[] { $"Total: {total:C2}" });
        }


        public async Task<ViewResult> TestAsyncAwait()
        {
            long? length = await AsyncMethods.GetPageLengthAsync();
            return View("Index", new string[] { $"Length: {length}" });
        }

        public ViewResult UsingNameOf()
        {
            var products = new[]
            {
                new { Name = "Kayak", Price = 275M },
                new { Name = "Lifejacket", Price = 48.95M },
                new { Name = "Soccer ball", Price = 19.50M },
                new { Name = "Corner flag", Price = 34.95M }
            };

            //return View("Index", products.Select(p => $"Name: {p.Name}, Price: {p.Price}")); //string Name and Price are hardcoded

            return View("Index", products.Select(p => $"{nameof(p.Name)}: {p.Name}, {nameof(p.Price)}: {p.Price}")); //using nameof Expression we pull "Name" and "Price" from the object,
        }
    }
}
