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
    public class ArmadoPrenominasController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: ArmadoPrenominas
        public ActionResult Index()
        {
            var armadoPrenominas = db.ArmadoPrenominas.Include(a => a.Cliente).Include(a => a.ConceptosPrenómina).Include(a => a.EsquemasPago).Include(a => a.Usuario);
            return View(armadoPrenominas.ToList());
        }

        // GET: ArmadoPrenominas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArmadoPrenomina armadoPrenomina = db.ArmadoPrenominas.Find(id);
            if (armadoPrenomina == null)
            {
                return HttpNotFound();
            }
            return View(armadoPrenomina);
        }

        // GET: ArmadoPrenominas/Create
        public ActionResult Create()
        {
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente");
            ViewBag.conceptoPreId = new SelectList(db.ConceptosPrenómina, "id", "descripcion");
            ViewBag.tipoPagoId = new SelectList(db.EsquemasPagoes, "id", "descripcion");
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario");
            return View();
        }

        // POST: ArmadoPrenominas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,clienteId,tipoPagoId,conceptoPreId,fechaCreacion,usuarioId")] ArmadoPrenomina armadoPrenomina)
        {
            if (ModelState.IsValid)
            {
                db.ArmadoPrenominas.Add(armadoPrenomina);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", armadoPrenomina.clienteId);
            ViewBag.conceptoPreId = new SelectList(db.ConceptosPrenómina, "id", "descripcion", armadoPrenomina.conceptoPreId);
            ViewBag.tipoPagoId = new SelectList(db.EsquemasPagoes, "id", "descripcion", armadoPrenomina.tipoPagoId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", armadoPrenomina.usuarioId);
            return View(armadoPrenomina);
        }

        // GET: ArmadoPrenominas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArmadoPrenomina armadoPrenomina = db.ArmadoPrenominas.Find(id);
            if (armadoPrenomina == null)
            {
                return HttpNotFound();
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", armadoPrenomina.clienteId);
            ViewBag.conceptoPreId = new SelectList(db.ConceptosPrenómina, "id", "descripcion", armadoPrenomina.conceptoPreId);
            ViewBag.tipoPagoId = new SelectList(db.EsquemasPagoes, "id", "descripcion", armadoPrenomina.tipoPagoId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", armadoPrenomina.usuarioId);
            return View(armadoPrenomina);
        }

        // POST: ArmadoPrenominas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,clienteId,tipoPagoId,conceptoPreId,fechaCreacion,usuarioId")] ArmadoPrenomina armadoPrenomina)
        {
            if (ModelState.IsValid)
            {
                db.Entry(armadoPrenomina).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", armadoPrenomina.clienteId);
            ViewBag.conceptoPreId = new SelectList(db.ConceptosPrenómina, "id", "descripcion", armadoPrenomina.conceptoPreId);
            ViewBag.tipoPagoId = new SelectList(db.EsquemasPagoes, "id", "descripcion", armadoPrenomina.tipoPagoId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", armadoPrenomina.usuarioId);
            return View(armadoPrenomina);
        }

        // GET: ArmadoPrenominas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArmadoPrenomina armadoPrenomina = db.ArmadoPrenominas.Find(id);
            if (armadoPrenomina == null)
            {
                return HttpNotFound();
            }
            return View(armadoPrenomina);
        }

        // POST: ArmadoPrenominas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ArmadoPrenomina armadoPrenomina = db.ArmadoPrenominas.Find(id);
            db.ArmadoPrenominas.Remove(armadoPrenomina);
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
