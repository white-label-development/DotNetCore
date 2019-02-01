using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using SportsStore.Controllers;
using Xunit;

namespace SportsStore.Tests
{
    public class ControllerAndActionsTests
    {

        [Fact]
        public void NamedViewSelected()
        {   // Arrange
            var controller = new NeilController();

            // Act
            ViewResult result = controller.ReceiveFormAndResultAViewResult("Adam", "London");
            
            // Assert
            Assert.Equal("StringResult", result.ViewName);

            //some extras for reference
            Assert.IsType<string>(result.ViewData.Model);

            //viewbag
            Assert.IsType<string>(result.ViewData["Message"]);
            Assert.Equal("Hello", result.ViewData["Message"]);

            //test for literal redirect
            //Assert.False(result.Permanent);
            //Assert.Equal("Example", result.RouteValues["controller"]);
            //Assert.Equal("Index", result.RouteValues["action"]);
            //Assert.Equal("MyID", result.RouteValues["ID"]); 

            //redirect to action
            // Act
            // RedirectToActionResult result = controller.Redirect();
            // // Assert
            // Assert.False(result.Permanent);
            // Assert.Equal("Index", result.ActionName);


            //status codes
            // Act
            // StatusCodeResult result = controller.Index();
            // Assert
            // Assert.Equal(404, result.StatusCode); 
        }

        [Fact]
        public void AssumedViewSelected()
        {   
            var controller = new NeilController();

            ViewResult result = controller.StringResult(); //Annoyingly, this form "public ViewResult StringResult() => View();" returns a null ViewName

            Assert.Null(result.ViewName); //so this test only proves we are not getting a named view.
                                          //it's a pretty crap test.
                                          //edit: "a null value is how the ViewResult object signals to MVC that the default view associated with the action method has been selected"
        }
    }
}
