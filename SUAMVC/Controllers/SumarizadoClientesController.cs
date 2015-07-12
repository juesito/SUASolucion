using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;
using System.Data.SqlClient;

namespace SUAMVC.Controllers
{
    public class SumarizadoClientesController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: SumarizadoClientes
        public ActionResult Index(String plazasId, String patronesId, String periodoId, 
            String ejercicioId, String clientesId, String usuarioId)
        {
            if (!String.IsNullOrEmpty(clientesId))
            {
                ViewBag.filtered = true;
                int clienteId = int.Parse(clientesId.Trim());
                int userId = int.Parse(usuarioId.Trim());

                int result = db.Database.ExecuteSqlCommand("sp_SumarizadoClientes @usuarioId, @clienteId", new SqlParameter("@usuarioId", userId), new SqlParameter("@clienteId", clienteId));
                var sumarizadoClientes = db.SumarizadoClientes.Include(s => s.Cliente).Include(s => s.Patrone).Include(s => s.Usuario);
                if (!String.IsNullOrEmpty(plazasId))
                {
                    int plazaTempId = int.Parse(plazasId.Trim());
                    sumarizadoClientes = sumarizadoClientes.Where(s => s.Patrone.Plaza_id.Equals(plazaTempId));
                }
                if (!String.IsNullOrEmpty(patronesId))
                {
                    int patronesTempId = int.Parse(patronesId);
                    sumarizadoClientes = sumarizadoClientes.Where(s => s.Patrone.Id.Equals(patronesTempId));
                }
                if (!String.IsNullOrEmpty(periodoId))
                {
                    sumarizadoClientes = sumarizadoClientes.Where(s => s.mes.Trim().Equals(periodoId.Trim()));
                }
                if (!String.IsNullOrEmpty(ejercicioId))
                {
                    sumarizadoClientes = sumarizadoClientes.Where(s => s.anno.Trim().Equals(ejercicioId));
                }

                sumarizadoClientes = sumarizadoClientes.OrderBy(p => p.Patrone.registro);
                return View(sumarizadoClientes.ToList());
            }
            return View();
        }

        // GET: SumarizadoClientes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SumarizadoCliente sumarizadoCliente = db.SumarizadoClientes.Find(id);
            if (sumarizadoCliente == null)
            {
                return HttpNotFound();
            }
            return View(sumarizadoCliente);
        }

        // GET: SumarizadoClientes/Create
        public ActionResult Create()
        {
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente");
            ViewBag.patronId = new SelectList(db.Patrones, "Id", "registro");
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario");
            return View();
        }

        // POST: SumarizadoClientes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,patronId,anno,mes,imss,rcv,infonavit,total,nt,usuarioId,fechaCreacion,clienteId")] SumarizadoCliente sumarizadoCliente)
        {
            if (ModelState.IsValid)
            {
                db.SumarizadoClientes.Add(sumarizadoCliente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", sumarizadoCliente.clienteId);
            ViewBag.patronId = new SelectList(db.Patrones, "Id", "registro", sumarizadoCliente.patronId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", sumarizadoCliente.usuarioId);
            return View(sumarizadoCliente);
        }

        // GET: SumarizadoClientes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SumarizadoCliente sumarizadoCliente = db.SumarizadoClientes.Find(id);
            if (sumarizadoCliente == null)
            {
                return HttpNotFound();
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", sumarizadoCliente.clienteId);
            ViewBag.patronId = new SelectList(db.Patrones, "Id", "registro", sumarizadoCliente.patronId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", sumarizadoCliente.usuarioId);
            return View(sumarizadoCliente);
        }

        // POST: SumarizadoClientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,patronId,anno,mes,imss,rcv,infonavit,total,nt,usuarioId,fechaCreacion,clienteId")] SumarizadoCliente sumarizadoCliente)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sumarizadoCliente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", sumarizadoCliente.clienteId);
            ViewBag.patronId = new SelectList(db.Patrones, "Id", "registro", sumarizadoCliente.patronId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", sumarizadoCliente.usuarioId);
            return View(sumarizadoCliente);
        }

        // GET: SumarizadoClientes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SumarizadoCliente sumarizadoCliente = db.SumarizadoClientes.Find(id);
            if (sumarizadoCliente == null)
            {
                return HttpNotFound();
            }
            return View(sumarizadoCliente);
        }

        // POST: SumarizadoClientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SumarizadoCliente sumarizadoCliente = db.SumarizadoClientes.Find(id);
            db.SumarizadoClientes.Remove(sumarizadoCliente);
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
