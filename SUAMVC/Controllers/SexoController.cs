using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;
using System.Web.Helpers;

namespace SUAMVC.Controllers
{
    public class SexoController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Sexoes
        public ActionResult Index()
        {
            var sexos = db.Sexos.Include(s => s.Usuario);
            return View(sexos.ToList());
        }

        // GET: Sexoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sexo sexo = db.Sexos.Find(id);
            if (sexo == null)
            {
                return HttpNotFound();
            }
            return View(sexo);
        }

        // GET: Sexoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Sexoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion")] Sexo sexo)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                sexo.fechaCreacion = DateTime.Now;
                sexo.usuarioId = usuario.Id;
                db.Sexos.Add(sexo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", sexo.usuarioId);
            return View(sexo);
        }

        // GET: Sexoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sexo sexo = db.Sexos.Find(id);
            if (sexo == null)
            {
                return HttpNotFound();
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", sexo.usuarioId);
            return View(sexo);
        }

        // POST: Sexoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion")] Sexo sexo)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                sexo.fechaCreacion = DateTime.Now;
                sexo.usuarioId = usuario.Id;
                db.Entry(sexo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", sexo.usuarioId);
            return View(sexo);
        }

        // GET: Sexoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sexo sexo = db.Sexos.Find(id);
            if (sexo == null)
            {
                return HttpNotFound();
            }
            return View(sexo);
        }

        // POST: Sexoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sexo sexo = db.Sexos.Find(id);
            db.Sexos.Remove(sexo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        public void GetExcel(String sexoId)
        {

                    
            //traigo de la base de datos Sexo los registros
            var generos = from s in db.Sexos
                           select s;

            //Valida que la variable no sea nula
            if (!String.IsNullOrEmpty(sexoId))
            {
               //Convierte la variable de string a entero
                int sexoIdTemp = int.Parse(sexoId);
                generos = generos.Where(p => p.id.Equals(sexoIdTemp));
            }

            List<Sexo> allCust = new List<Sexo>();

            allCust = generos.ToList();

            WebGrid grid = new WebGrid(source: allCust, canPage: false, canSort: false);

            List<WebGridColumn> gridColumns = new List<WebGridColumn>();
            gridColumns.Add(grid.Column("descripcion", "Descripcion"));
            gridColumns.Add(grid.Column("fechaCreacion", "Fecha Alta"));
            gridColumns.Add(grid.Column("Usuario.nombreUsuario", "Usuario Alta"));
           
            string gridData = grid.GetHtml(
                columns: grid.Columns(gridColumns.ToArray())
                    ).ToString();

            Response.ClearContent();
            DateTime date = DateTime.Now;
            String fileName = "Generos" + date.ToString("ddMMyyyyHHmm") + ".xls";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();
        }
    }
}
