using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;

namespace SportsStore.Controllers
{
    public class ProductController : Controller
    {
        public int PageSize = 4;

        private IProductRepository repository;
        public ProductController(IProductRepository repo) { repository = repo; }


        public IActionResult Index()
        {
            return View();
        }

        public ViewResult List(int productPage = 1) => View(new ProductsListViewModel
        {
            Products = repository.Products.OrderBy(p => p.ProductID).Skip((productPage - 1) * PageSize).Take(PageSize),
            PagingInfo = new PagingInfo { CurrentPage = productPage, ItemsPerPage = PageSize, TotalItems = repository.Products.Count() }
        });
    }
}