using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Palette_Project;
using Palette_Project.Controllers;
using Palette_Project.Models;

namespace Palette_Project.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {




        [TestMethod]
        public void CheckSite()
        {
            // Arrange
            HomeController controller = new HomeController();

            Website testSite = new Website();
            testSite.url = "http://thepiratebay.org";

            // Act
            ViewResult result = controller.CheckSite(testSite) as ViewResult;

            // Assert
            Assert.IsTrue(testSite.websiteColors.Count != 0);
        }











        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
