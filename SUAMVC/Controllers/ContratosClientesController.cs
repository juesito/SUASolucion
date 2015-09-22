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
    public class ContratosClientesController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: ContratosClientes
        public ActionResult Index()
        {
            var contratosClientes = db.ContratosClientes.Include(c => c.Cliente).Include(c => c.Usuario);
            return View(contratosClientes.ToList());
        }

        // GET: ContratosClientes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContratosCliente contratosCliente = db.ContratosClientes.Find(id);
            if (contratosCliente == null)
            {
                return HttpNotFound();
            }
            return View(contratosCliente);
        }

        // GET: ContratosClientes/Create
        public ActionResult Create()
        {
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente");
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario");
            return View();
        }

        // POST: ContratosClientes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,clienteId,descripcion,archivo1,archivo2,archivo3,actaConstitutivaEmpresa,poderRepresentanteLegal,ifeRepresentanteLegal,comprobanteDomicilio,fechaInicioVigencia,fechaFinalVigencia,estatus,fechaCreacion,usuarioId")] ContratosCliente contratosCliente)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;
                contratosCliente.fechaCreacion = DateTime.Now;
                contratosCliente.usuarioId = usuario.Id;
                contratosCliente.estatus = "A";
                db.ContratosClientes.Add(contratosCliente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", contratosCliente.clienteId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", contratosCliente.usuarioId);
            return View(contratosCliente);
        }

        // GET: ContratosClientes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Clientes");
            }
            ContratosCliente contratosCliente = db.ContratosClientes.Find(id);
            if (contratosCliente == null)
            {
                return HttpNotFound();
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", contratosCliente.clienteId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", contratosCliente.usuarioId);
            return View(contratosCliente);
        }

        // POST: ContratosClientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,clienteId,descripcion,archivo1,archivo2,archivo3,actaConstitutivaEmpresa,poderRepresentanteLegal,ifeRepresentanteLegal,comprobanteDomicilio,fechaInicioVigencia,fechaFinalVigencia,estatus,fechaCreacion,usuarioId")] ContratosCliente contratosCliente)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                contratosCliente.fechaCreacion = DateTime.Now;
                contratosCliente.usuarioId = usuario.Id;
                contratosCliente.estatus = "A";
                db.Entry(contratosCliente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = contratosCliente.clienteId });
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", contratosCliente.clienteId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", contratosCliente.usuarioId);
            return View(contratosCliente);
        }

        // GET: ContratosClientes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContratosCliente contratosCliente = db.ContratosClientes.Find(id);
            if (contratosCliente == null)
            {
                return HttpNotFound();
            }
            return View(contratosCliente);
        }

        // POST: ContratosClientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ContratosCliente contratosCliente = db.ContratosClientes.Find(id);
            db.ContratosClientes.Remove(contratosCliente);
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
