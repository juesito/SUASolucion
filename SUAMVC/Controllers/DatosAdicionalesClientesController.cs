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
    public class DatosAdicionalesClientesController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: DatosAdicionalesClientes
        public ActionResult Index(string id)
        {

            //Toma todos los registros de cliente del catalogo Datos adicionales cliente, incluidos en las tablas:Cliente y Usuario
            var datosAdicionalesClientes = db.DatosAdicionalesClientes.Include(d => d.Cliente).Include(d => d.Usuario);
            //Valida que la variable id(esta variable es como la llamo en el icono del index de clientes) sea nula
            if (String.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", "Clientes");
            }
            else
            {
                int idTemp = int.Parse(id);
                Cliente cliente = db.Clientes.Find(idTemp);
                TempData["cliente"] = cliente;
                datosAdicionalesClientes = datosAdicionalesClientes.Where(s => s.clienteId.Equals(idTemp));
            }
            return View(datosAdicionalesClientes.ToList());
        }

        // GET: DatosAdicionalesClientes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DatosAdicionalesCliente datosAdicionalesCliente = db.DatosAdicionalesClientes.Find(id);
            if (datosAdicionalesCliente == null)
            {
                return HttpNotFound();
            }
            return View(datosAdicionalesCliente);
        }

        // GET: DatosAdicionalesClientes/Create
        public ActionResult Create(string clienteId)
        {
            
            int idTemp = int.Parse(clienteId);
            Cliente cliente = db.Clientes.Find(idTemp);
            TempData["cliente"] = cliente;
            return View();
        }

        // POST: DatosAdicionalesClientes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,clienteId,porcentajeComNomina,ivaNomina,porcentajeComIAS,ivaIAS,porcentajeComFlujo,ivaFlujo,costoSocial,conceptoFacturacion,fechaCreacion,usuarioId")] DatosAdicionalesCliente datosAdicionalesCliente, int clienteId)
        {
            if (ModelState.IsValid)
            {

                Usuario usuario = Session["UsuarioData"] as Usuario;
                Cliente cliente = db.Clientes.Find(clienteId);
                TempData["cliente"] = cliente;

                datosAdicionalesCliente.fechaCreacion = DateTime.Now;
                datosAdicionalesCliente.usuarioId = usuario.Id;
                datosAdicionalesCliente.clienteId = cliente.Id;
                datosAdicionalesCliente.Cliente = cliente;
                db.DatosAdicionalesClientes.Add(datosAdicionalesCliente);
                db.SaveChanges();

                
                return RedirectToAction("Index", new { id = clienteId });
            }

            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", datosAdicionalesCliente.clienteId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", datosAdicionalesCliente.usuarioId);
            return View(datosAdicionalesCliente);
        }

        // GET: DatosAdicionalesClientes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Clientes");
            }
            DatosAdicionalesCliente datosAdicionalesCliente = db.DatosAdicionalesClientes.Find(id);
            if (datosAdicionalesCliente == null)
            {
                return HttpNotFound();
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", datosAdicionalesCliente.clienteId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", datosAdicionalesCliente.usuarioId);
            return View(datosAdicionalesCliente);
        }

        // POST: DatosAdicionalesClientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,clienteId,porcentajeComNomina,ivaNomina,porcentajeComIAS,ivaIAS,porcentajeComFlujo,ivaFlujo,costoSocial,conceptoFacturacion,fechaCreacion,usuarioId")] DatosAdicionalesCliente datosAdicionalesCliente)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                datosAdicionalesCliente.fechaCreacion = DateTime.Now;
                datosAdicionalesCliente.usuarioId = usuario.Id;
                db.Entry(datosAdicionalesCliente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new{ id = datosAdicionalesCliente.clienteId});
            }
            ViewBag.clienteId = new SelectList(db.Clientes, "Id", "claveCliente", datosAdicionalesCliente.clienteId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", datosAdicionalesCliente.usuarioId);
            return View(datosAdicionalesCliente);
        }

        // GET: DatosAdicionalesClientes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DatosAdicionalesCliente datosAdicionalesCliente = db.DatosAdicionalesClientes.Find(id);
            if (datosAdicionalesCliente == null)
            {
                return HttpNotFound();
            }
            return View(datosAdicionalesCliente);
        }

        // POST: DatosAdicionalesClientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DatosAdicionalesCliente datosAdicionalesCliente = db.DatosAdicionalesClientes.Find(id);
            db.DatosAdicionalesClientes.Remove(datosAdicionalesCliente);
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
