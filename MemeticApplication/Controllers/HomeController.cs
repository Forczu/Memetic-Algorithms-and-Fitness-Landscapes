using MemeticApplication.MemeticLibrary.Model;
using MemeticApplication.MemeticLibrary.Readers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MemeticApplication.Controllers
{
    public class HomeController : Controller
    {
        private static readonly string FILENAME = @"C101.txt";

        private static SolomonProblemReader reader = new SolomonProblemReader();

        // GET: Home
        public ActionResult Index()
        {
            string filePath = Server.MapPath("~/Data/Solomon/solomon_25/" + FILENAME);
            VrptwProblem problem = reader.ReadFromFile(filePath);
            return View(problem);
        }
    }
}