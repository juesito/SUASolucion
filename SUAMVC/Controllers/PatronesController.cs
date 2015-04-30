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
    public class PatronesController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Patrones
        public ActionResult Index(String plazaId)
        {
            var patrones = from p in db.Patrones
                           select p;

            if (!String.IsNullOrEmpty(plazaId))
            {
                int plazaIdTemp = int.Parse(plazaId);
                patrones = patrones.Where(p => p.Plaza.id == plazaIdTemp);
            }
            else {
                patrones = db.Patrones.Include(p => p.Plaza);
            }

            patrones.OrderBy(p => p.nombre);
            ViewBag.PlazaId = new SelectList(db.Plazas, "id", "descripcion");
            
            return View(patrones.ToList());
        }

        // GET: Patrones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patrone patrone = db.Patrones.Find(id);
            if (patrone == null)
            {
                return HttpNotFound();
            }
            return View(patrone);
        }

        // GET: Patrones/Create
        public ActionResult Create()
        {
            ViewBag.Plaza_id = new SelectList(db.Plazas, "id", "descripcion");
            return View();
        }

        // POST: Patrones/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,rfc,nombre,actividad,domicilio,municipio,codigoPostal,entidad,telefono,remision,zona,delegacion,carEnt,numeroDelegacion,carDel,numSub,tipoConvenio,convenio,inicioAfiliacion,patRep,clase,fraccion,STyPS,Plaza_id,registro, direccionArchivo")] Patrone patrone)
        {
            if (ModelState.IsValid)
            {
                db.Patrones.Add(patrone);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Plaza_id = new SelectList(db.Plazas, "id", "descripcion", patrone.Plaza_id);
            return View(patrone);
        }

        // GET: Patrones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patrone patrone = db.Patrones.Find(id);
            if (patrone == null)
            {
                return HttpNotFound();
            }
            ViewBag.Plaza_id = new SelectList(db.Plazas, "id", "descripcion", patrone.Plaza_id);
            return View(patrone);
        }

        // POST: Patrones/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,rfc,nombre,actividad,domicilio,municipio,codigoPostal,entidad,telefono,remision,zona,delegacion,carEnt,numeroDelegacion,carDel,numSub,tipoConvenio,convenio,inicioAfiliacion,patRep,clase,fraccion,STyPS,Plaza_id,registro, direccionArchivo")] Patrone patrone)
        {
            if (ModelState.IsValid)
            {
                db.Entry(patrone).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Plaza_id = new SelectList(db.Plazas, "id", "descripcion", patrone.Plaza_id);
            return View(patrone);
        }

        // GET: Patrones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patrone patrone = db.Patrones.Find(id);
            if (patrone == null)
            {
                return HttpNotFound();
            }
            return View(patrone);
        }

        // POST: Patrones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Patrone patrone = db.Patrones.Find(id);
            db.Patrones.Remove(patrone);
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
