using SUADATOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SUAMVC.Controllers
{
    public class RoleModulosController : Controller
    {
        private suaEntities db = new suaEntities();
        // GET: RoleModulos
        public ActionResult Index()
        {
            ViewBag.roleId = new SelectList(db.Roles, "id", "descripcion");
            return View();
        }
    }
}