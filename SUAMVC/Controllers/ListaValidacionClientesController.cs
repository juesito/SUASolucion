using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;

namespace SUAMVC.Controllers
{
    public class ListaValidacionClientesController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: ListaValidacionClientes
        public ActionResult Index()
        {
            var listaValidacionClientes = db.ListaValidacionClientes.Include(l => l.Cliente).Include(l => l.Usuario);
            return View(listaValidacionClientes.ToList());
        }

        // GET: ListaValidacionClientes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ListaValidacionCliente listaValidacionCliente = db.ListaValidacionClientes.Find(id);
            if (listaValidacionCliente == null)
            {
                return HttpNotFound();
            }
            return View(listaValidacionCliente);
        }

        // GET: ListaValidacionClientes/Create
        public ActionResult Create()
        {
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente");
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario");
            return View();
        }

        // POST: ListaValidacionClientes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,clienteId,validador,emailValidador,autorizador,emailAutorizador,listaEmailAux,fechaCreacion,usuarioId")] ListaValidacionCliente listaValidacionCliente)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                listaValidacionCliente.fechaCreacion = DateTime.Now;
                listaValidacionCliente.usuarioId = usuario.Id;
                db.ListaValidacionClientes.Add(listaValidacionCliente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", listaValidacionCliente.clienteId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", listaValidacionCliente.usuarioId);
            return View(listaValidacionCliente);
        }

        // GET: ListaValidacionClientes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ListaValidacionCliente listaValidacionCliente = db.ListaValidacionClientes.Find(id);
            if (listaValidacionCliente == null)
            {
                return HttpNotFound();
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", listaValidacionCliente.clienteId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", listaValidacionCliente.usuarioId);
            return View(listaValidacionCliente);
        }

        // POST: ListaValidacionClientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,clienteId,validador,emailValidador,autorizador,emailAutorizador,listaEmailAux,fechaCreacion,usuarioId")] ListaValidacionCliente listaValidacionCliente)
        {
            if (ModelState.IsValid)
            {
                db.Entry(listaValidacionCliente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", listaValidacionCliente.clienteId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", listaValidacionCliente.usuarioId);
            return View(listaValidacionCliente);
        }

        // GET: ListaValidacionClientes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ListaValidacionCliente listaValidacionCliente = db.ListaValidacionClientes.Find(id);
            if (listaValidacionCliente == null)
            {
                return HttpNotFound();
            }
            return View(listaValidacionCliente);
        }

        // POST: ListaValidacionClientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ListaValidacionCliente listaValidacionCliente = db.ListaValidacionClientes.Find(id);
            db.ListaValidacionClientes.Remove(listaValidacionCliente);
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
    }
}
