﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using SportsStore.Infrastructure;
using SportsStore.Models;

namespace SportsStore.Controllers
{
    public class NeilController : Controller
    {
        private readonly Neil _neilModel;
        private ILogger<NeilController> logger;

        public NeilController()
        {
            
        }

        //public NeilController(Neil neilModel, ILogger<NeilController> log)
        //{
        //    _neilModel = neilModel;
        //    logger = log;
        //    logger.LogDebug("logger ok");
        //    logger.LogCritical("ouch");
        //}

        public IActionResult Index()
        {
            //logger.LogDebug($"Handled {Request.Path} at uptime {_neilModel.Uptime}");
            return View();
        }

        public string Uptime()
        {
            return _neilModel.Uptime;
        }

        //manual construction of ViewResult
        //View(myModel)  is a shortcut for setting ViewData.Model explicitly
        public ViewResult PocoAction()
        {
            return new ViewResult()
            {
                ViewName = "Result",
                ViewData =
                    new ViewDataDictionary(
                        new EmptyModelMetadataProvider(),
                        new ModelStateDictionary())
                    {
                        Model = new Result
                        {
                            Action = "a",
                            Controller = "b"
                        }
                    }
            };
        }

        //manual response via  HttpResponse context object
        public void ReceiveForm(string name, string city)
        {
            Response.StatusCode = 200;
            Response.ContentType = "text/html";
            byte[] content = Encoding.ASCII.GetBytes($"<html><body>{name} lives in {city}</body>");
            Response.Body.WriteAsync(content, 0, content.Length);
        }

        public ViewResult ReceiveFormAndResultAViewResult(string name, string city)
        {
            ViewBag.Message = "Hello";
            return View("StringResult", $"{name} lives in {city}");
        }

        //return a custom IActionResult (as opposed to a concrete ViewResult or RedirectResult etc)
        public IActionResult ReceiveFormAndReturnCustomHtmlResult(string name, string city)
        {
            return new CustomHtmlResult {Content = $"{name} lives in {city}"};
        }

        public ViewResult StringResult()
        {
            return View();
        }

        public ViewResult Result()
        {
            return View((object) "Hello World");
        }


        //PRG
        //Can use TempData to provide the Get with data
        //set by  TempData["name"] = name; 
        //read by string name = TempData["name"] as string; 
        [HttpPost] public RedirectToActionResult ReceiveFormPrgExample(string name, string city) => RedirectToAction(nameof(GetPrgExample));
        public ViewResult GetPrgExample() => View("Result");
    }
}