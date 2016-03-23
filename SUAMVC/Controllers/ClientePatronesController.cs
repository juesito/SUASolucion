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
    public class ClientePatronesController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: ClientePatrones
        public ActionResult Index(String clienteId)
        {
            if (!String.IsNullOrEmpty(clienteId))
            {

                var ctePatrones = db.ClientePatrones.ToList();

                if (!String.IsNullOrEmpty(clienteId))
                {
                    int clienteTempId = int.Parse(clienteId.Trim());
                    ctePatrones = ctePatrones.Where(p => p.clienteId.Equals(clienteTempId)).OrderBy(p => p.Patrone.nombre).ToList();
                    ViewBag.clienteId = clienteId;
                }

                return View(ctePatrones.ToList());
            }

            List<ClientePatrone> list = new List<ClientePatrone>();
            return View(list);
        }

        // GET: ClientePatrones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientePatrone clientePatrone = db.ClientePatrones.Find(id);
            if (clientePatrone == null)
            {
                return HttpNotFound();
            }
            return View(clientePatrone);
        }

        // GET: ClientePatrones/Create
        public ActionResult Create(String clienteId)
        {
            ViewBag.clienteId = clienteId;
            ViewBag.patronId = new SelectList(db.Patrones, "Id", "registro");
            return View();
        }

        // POST: ClientePatrones/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,patronesId,fechaCreacion,usuarioId,clienteId")] ClientePatrone clientePatrone)
        {
            if (ModelState.IsValid)
            {
                var patronTemp = from b in db.ClientePatrones
                                 where b.clienteId.Equals(clientePatrone.clienteId)
                                 && b.patronesId.Equals(clientePatrone.patronesId)
                                 select b;

                if (patronTemp.Equals(null) || patronTemp.Count() == 0)
                {
                    Usuario usuario = Session["UsuarioData"] as Usuario;

                    clientePatrone.fechaCreacion = DateTime.Now;
                    clientePatrone.usuarioId = usuario.Id;

                    db.ClientePatrones.Add(clientePatrone);
                    db.SaveChanges();
                }
                return RedirectToAction("Index", new { clienteId = clientePatrone.clienteId });
            }

            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", clientePatrone.clienteId);
            ViewBag.patronId = new SelectList(db.Patrones, "Id", "registro", clientePatrone.patronesId);
            return View(clientePatrone);
        }

        // GET: ClientePatrones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientePatrone clientePatrone = db.ClientePatrones.Find(id);
            if (clientePatrone == null)
            {
                return HttpNotFound();
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", clientePatrone.clienteId);
            ViewBag.patronId = new SelectList(db.Patrones, "Id", "registro", clientePatrone.patronesId);
            return View(clientePatrone);
        }

        // POST: ClientePatrones/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,patronesId,fechaCreacion,usuarioId,clienteId")] ClientePatrone clientePatrone)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                clientePatrone.fechaCreacion = DateTime.Now;
                clientePatrone.usuarioId = usuario.Id;
                db.Entry(clientePatrone).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", clientePatrone.clienteId);
            ViewBag.patronId = new SelectList(db.Patrones, "Id", "registro", clientePatrone.patronesId);
            return View(clientePatrone);
        }

        // GET: ClientePatrones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientePatrone clientePatrone = db.ClientePatrones.Find(id);
            if (clientePatrone == null)
            {
                return HttpNotFound();
            }
            ViewBag.clienteId = clientePatrone.clienteId;
            return View(clientePatrone);
        }

        // POST: ClientePatrones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClientePatrone clientePatrone = db.ClientePatrones.Find(id);
            int clienteIdTmp = clientePatrone.clienteId;
            ViewBag.clienteId = clienteIdTmp;
            db.ClientePatrones.Remove(clientePatrone);
            db.SaveChanges();
            return RedirectToAction("Index", new { clienteId = clienteIdTmp });
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
