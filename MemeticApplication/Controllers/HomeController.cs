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
        private static VrptwProblemReader reader = new VrptwProblemReader();

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult LoadProblemInstance(string instanceId)
        {
            string filePath = Server.MapPath("~/Data/Homberger/" + instanceId + @".txt");
            VrptwProblem problem = reader.ReadFromFile(filePath);
            return new JsonResult
            {
                Data = problem,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }


    }
}