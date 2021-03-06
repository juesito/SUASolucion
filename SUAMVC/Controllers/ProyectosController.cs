﻿using System;
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
    public class ProyectosController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Proyectos
        public ActionResult Index(String clienteId)
        {
            
            if (!String.IsNullOrEmpty(clienteId)) { 
                var proyectos = db.Proyectos.Include(p => p.Cliente);
                int clienteTempId = int.Parse(clienteId.Trim());
                proyectos = proyectos.Where(p => p.clienteId.Equals(clienteTempId)).OrderBy(p => p.fechaCreacion);

                ViewBag.clienteId = clienteTempId;

                return View(proyectos.ToList());
            }

            List<Proyecto> list = new List<Proyecto>();
            return View(list);
        }

        // GET: Proyectos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proyecto proyecto = db.Proyectos.Find(id);
            if (proyecto == null)
            {
                return HttpNotFound();
            }
            return View(proyecto);
        }

        // GET: Proyectos/Create
        public ActionResult Create(int clienteId)
        {
            ViewBag.clienteId = clienteId;
            return View();
        }

        // POST: Proyectos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,clienteId,descripcion,fechaCreacion,usuarioId")] Proyecto proyecto)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                proyecto.fechaCreacion = DateTime.Now;
                proyecto.usuarioId = usuario.Id;
                db.Proyectos.Add(proyecto);
                db.SaveChanges();
                return RedirectToAction("Index", new { clienteId = proyecto.clienteId});
            }

            return View(proyecto);
        }

        // GET: Proyectos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proyecto proyecto = db.Proyectos.Find(id);
            if (proyecto == null)
            {
                return HttpNotFound();
            }
            return View(proyecto);
        }

        // POST: Proyectos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,clienteId,descripcion,fechaCreacion,usuarioId")] Proyecto proyecto)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;

                proyecto.fechaCreacion = DateTime.Now;
                proyecto.usuarioId = usuario.Id;
                proyecto.descripcion = proyecto.descripcion.Trim();
                db.Entry(proyecto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { clienteId = proyecto.id});
            }
            return View(proyecto);
        }

        // GET: Proyectos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proyecto proyecto = db.Proyectos.Find(id);
            if (proyecto == null)
            {
                return HttpNotFound();
            }
            return View(proyecto);
        }

        // POST: Proyectos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Proyecto proyecto = db.Proyectos.Find(id);
            db.Proyectos.Remove(proyecto);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ObtenerProyectosPorClienteId(int clienteId)
        {
            List<Proyecto> proyectos = new List<Proyecto>();
            proyectos = db.Proyectos.Where(m => m.clienteId == clienteId).OrderBy(m => m.descripcion).ToList();
            SelectList proyecto = new SelectList(proyectos, "Id", "descripcion", 0);
            return Json(proyecto);
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
