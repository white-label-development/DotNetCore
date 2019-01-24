using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LanguageFeatures.Models
{
    public static class ExtensionMethods
    {
        public static decimal TotalPrices(this ShoppingCart cartParam)
        {
            decimal total = 0;
            foreach (Product prod in cartParam.Products)
            {
                total += prod?.Price ?? 0;
            }
            return total;

            //eg:
            //ShoppingCart cart = new ShoppingCart { Products = Product.GetProducts() };
            //decimal cartTotal = cart.TotalPrices();
        }


        //Extension applied to IEnumerable interface
        //so it works on the ShppingCart (which implements this interface)
        //but also works on others things, 
        public static decimal TotalPrices(this IEnumerable<Product> products)
        {
            decimal total = 0;
            foreach (Product prod in products)
            {
                total += prod?.Price ?? 0;
            }
            return total;

            //also works on an array 
            //Product[] productArray = { new Product { Name = "Kayak", Price = 275M }, new Product { Name = "Lifejacket", Price = 48.95M } };
            //decimal arrayTotal = productArray.TotalPrices();
        }


        //This extension method, called FilterByPrice, takes an additional parameter that allows me to filter products
        //so that Product objects whose Price property matches or exceeds the parameter are returned in the resul
        public static IEnumerable<Product> FilterByPrice(this IEnumerable<Product> productEnum, decimal minimumPrice)
        {
            foreach (Product prod in productEnum)
            {
                if ((prod?.Price ?? 0) >= minimumPrice) { yield return prod; }
            }

            //eg: decimal arrayTotal = productArray.FilterByPrice(20).TotalPrices()
        }
    }
}
