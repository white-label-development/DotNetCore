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


        //second example showing extension that DOES NOT use Lambdas
        public static IEnumerable<Product> FilterByName(this IEnumerable<Product> productEnum, char firstLetter)
        {
            foreach (Product prod in productEnum)
            {
                if (prod?.Name?[0] == firstLetter) { yield return prod; }
            }
        }

        // A single extension method that filters an enumeration of Product objects but that delegates the decision about which ones are included in the results to a separate function.
        // The second argument to the Filter method is a function that accepts a Product object and that returns a bool value.
        // The Filter method calls the function for each Product object and includes it in the result if the function returns true. 
        public static IEnumerable<Product> Filter(this IEnumerable<Product> productEnum, Func<Product, bool> selector)
        {
            foreach (Product prod in productEnum)
            {
                if (selector(prod)) { yield return prod; }
            }
        }

        //eg: method
        // bool FilterByPrice(Product p) { return (p?.Price ?? 0) >= 20; }
        //decimal priceFilterTotal = productArray.Filter(FilterByPrice).TotalPrices();

        //eg: function
        //Func<Product, bool> nameFilter = delegate (Product prod) { return prod?.Name?[0] == 'S'; };
        //decimal nameFilterTotal = productArray.Filter(nameFilter).TotalPrices()

        //eg: lambda (inline, anon)
        //decimal priceFilterTotal = productArray.Filter(p => (p?.Price ?? 0) >= 20).TotalPrices();

        //note / reminder:
        //if i need a lambda expression for a delegate that has multiple parameters, i must wrap the parameters in parentheses, like this:
        //(prod, count) => prod.Price > 20 && count > 0

    }
}
