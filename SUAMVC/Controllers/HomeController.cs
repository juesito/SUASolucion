using SUAMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SUAMVC.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {

            SetupModel setup = new SetupModel();
            setup.setUpSystem();

            return View();
        }

        public ActionResult Home() {
            return View();
        }
    }
}