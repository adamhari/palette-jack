using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime;
using System.Web.Mvc;
using Palette_Project.Models;
using HtmlAgilityPack;
using ExCSS;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Palette_Project.Controllers
{
    public class HomeController : Controller
    {

        private WebsiteDBContext db = new WebsiteDBContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Index(string error)
        {
            ViewBag.Error = error;
            return View();
        }

        [HttpPost]
        public ActionResult CheckSite(Website website)
        {
            ViewBag.JumbotronPhrase = "";
            
            try
            {
                website.FixUrl();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return RedirectToAction("Index", "Home", new { error = "The provided URL was invalid." });
            }

            website.GetStyleRules();

            if (website.rules == null)
            {
                return RedirectToAction("Index", "Home", new { error = "The provided URL did not contain any parseable stylesheets." });
            }

            website.FilterColorRules();

            return View(website);
        }
    }
}