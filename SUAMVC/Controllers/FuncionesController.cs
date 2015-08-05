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
    public class FuncionesController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Funciones
        public ActionResult Index(String moduloId, String tipo)
        {
            var funcions = db.Funcions.Include(f => f.Modulo);
            if (!String.IsNullOrEmpty(moduloId)) {
                int moduloIntId = int.Parse(moduloId);
                funcions = funcions.Where(f => f.moduloId.Equals(moduloIntId));
            }
            if (!String.IsNullOrEmpty(tipo)) {
                funcions = funcions.Where(f => f.tipo.Equals(tipo.Trim()));
            }
            
            return View(funcions.ToList());
        }

        // GET: Funciones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Funcion funcion = db.Funcions.Find(id);
            if (funcion == null)
            {
                return HttpNotFound();
            }
            return View(funcion);
        }

        // GET: Funciones/Create
        public ActionResult Create()
        {
            ViewBag.moduloId = new SelectList(db.Modulos, "id", "descripcionCorta");
            return View();
        }

        // POST: Funciones/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,moduloId,descripcionCorta,descripcionLarga,accion,controlador,estatus,usuarioId,fechaCreacion,tipo")] Funcion funcion)
        {
            if (ModelState.IsValid)
            {
                //Usuario loggeado
                Usuario usuario = Session["UsuarioData"] as Usuario;

                funcion.fechaCreacion = DateTime.Now;
                funcion.usuarioId = usuario.Id;
                funcion.estatus = "A";
                db.Funcions.Add(funcion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.moduloId = new SelectList(db.Modulos, "id", "descripcionCorta", funcion.moduloId);
            return View(funcion);
        }

        // GET: Funciones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Funcion funcion = db.Funcions.Find(id);
            if (funcion == null)
            {
                return HttpNotFound();
            }
            ViewBag.moduloId = new SelectList(db.Modulos, "id", "descripcionCorta", funcion.moduloId);
            return View(funcion);
        }

        // POST: Funciones/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,moduloId,descripcionCorta,descripcionLarga,accion,controlador,estatus,tipo")] Funcion funcion)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                funcion.fechaCreacion = DateTime.Now;
                funcion.usuarioId = usuario.Id;
                funcion.estatus = "A";
                db.Entry(funcion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.moduloId = new SelectList(db.Modulos, "id", "descripcionCorta", funcion.moduloId);
            return View(funcion);
        }

        // GET: Funciones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Funcion funcion = db.Funcions.Find(id);
            if (funcion == null)
            {
                return HttpNotFound();
            }
            return View(funcion);
        }

        // POST: Funciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Funcion funcion = db.Funcions.Find(id);
            db.Funcions.Remove(funcion);
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

        public void GetExcel(string funcionId)
        {

            //traigo de la base de datos Funciones todos los registros

            var funciones = from f in db.Funcions
                              select f;

            //Valida que la variable no sea nula
            if (!String.IsNullOrEmpty(funcionId))
            {
                int funcionIdTemp = int.Parse(funcionId);
                funciones = funciones.Where(f => f.id.Equals(funcionIdTemp));
            }

            List<Funcion> allCust = new List<Funcion>();

            allCust = funciones.ToList();

            WebGrid grid = new WebGrid(source: allCust, canPage: false, canSort: false);

            List<WebGridColumn> gridColumns = new List<WebGridColumn>();
            gridColumns.Add(grid.Column("descripcionCorta", "Descripción Corta"));
            gridColumns.Add(grid.Column("descripcionLarga", "Descripción Larga"));
            gridColumns.Add(grid.Column("accion", "Acción"));
            gridColumns.Add(grid.Column("controlador", "Controlador"));
            gridColumns.Add(grid.Column("estatus", "Estatus"));
            gridColumns.Add(grid.Column("usuarioId", "Usuario Alta"));
            gridColumns.Add(grid.Column("fechaCreacion", "Fecha de Creación"));
            gridColumns.Add(grid.Column("tipo", "Tipo"));
            gridColumns.Add(grid.Column("Modulo.descripcionCorta", "Módulo Descripción"));
            

            string gridData = grid.GetHtml(
                columns: grid.Columns(gridColumns.ToArray())
                    ).ToString();

            Response.ClearContent();
            DateTime date = DateTime.Now;
            String fileName = "Funciones" + date.ToString("ddMMyyyyHHmm") + ".xls";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();
        }
    }
}
