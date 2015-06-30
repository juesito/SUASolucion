using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;
using SUAMVC.Helpers;
using SUAMVC.Models;
using System.Data.Entity.Validation;
using System.Text;

namespace SUAMVC.Controllers
{
    public class EmpleadosController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Empleados
        public ActionResult Index(string id)
        {

            var empleados = db.Empleados.Include(e => e.Banco).Include(e => e.EsquemasPago).Include(e => e.EstadoCivil).Include(e => e.Estado).Include(e => e.Municipio).Include(e => e.Pais).Include(e => e.SDI).Include(e => e.Sexo).Include(e => e.Solicitud).Include(e => e.Usuario);
            if (!String.IsNullOrEmpty(id))
            {
                int idTemp = int.Parse(id);
                empleados = empleados.Where(s => s.solicitudId.Equals(idTemp));
            }

            return View(empleados.ToList());
        }

        // GET: Empleados/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empleado empleado = db.Empleados.Find(id);
            if (empleado == null)
            {
                return HttpNotFound();
            }
            return View(empleado);
        }

        // GET: Empleados/Create
        public ActionResult Create(int id)
        {
            Empleado empleado = new Empleado();
            Solicitud solicitud = db.Solicituds.Find(id);

            empleado.Solicitud = solicitud;
            empleado.solicitudId = id;
            ViewBag.bancoId = new SelectList(db.Bancos, "id", "descripcion");
            ViewBag.esquemaPagoId = new SelectList(db.EsquemasPagoes, "id", "descripcion");
            ViewBag.estadoCivilId = new SelectList(db.EstadoCivils, "id", "descripcion");
            ViewBag.estadoNacimientoId = new SelectList(db.Estados, "id", "descripcion");
            ViewBag.municipioNacimientoId = new SelectList(db.Municipios, "id", "descripcion");
            ViewBag.nacionalidadId = new SelectList(db.Paises, "id", "descripcion");
            ViewBag.sdiId = new SelectList(db.SDIs, "id", "descripcion");
            ViewBag.sexoId = new SelectList(db.Sexos, "id", "descripcion");

            return View(empleado);
        }

        // POST: Empleados/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,solicitudId,nss,fechaAltaImss,apellidoMaterno,apellidoPaterno,nombre,nombreCompleto,rfc,homoclave,curp,sexoId,sdiId,esquemaPagoId,salarioReal,categoria,tieneInfonavit,creditoInfonavit,estadoCivilId,fechaNacimiento,nacionalidadId,estadoNacimientoId,municipioNacimientoId,calleNumero,colonia,edoMunicipio,codigoPostal,tramitarTarjeta,bancoId,cuentaBancaria,email,observaciones,usuarioId,fechaCreacion,estatus")] Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;
                empleado.fechaCreacion = DateTime.Now;
                empleado.usuarioId = usuario.Id;
                empleado.nombreCompleto = empleado.nombre + " " + empleado.apellidoPaterno + " " + empleado.apellidoMaterno;
                empleado.estatus = "A";
                db.Empleados.Add(empleado);

                try
                {
                    db.SaveChanges();

                    Solicitud solicitud = db.Solicituds.Find(empleado.solicitudId);
                    solicitud.noTrabajadores = solicitud.noTrabajadores + 1;

                    db.Entry(solicitud).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (var failure in ex.EntityValidationErrors)
                    {
                        sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                        foreach (var error in failure.ValidationErrors)
                        {
                            sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                            sb.AppendLine();
                        }
                    }
                }

                return RedirectToAction("Index", new { id = empleado.solicitudId});
            }

            ViewBag.bancoId = new SelectList(db.Bancos, "id", "descripcion", empleado.bancoId);
            ViewBag.esquemaPagoId = new SelectList(db.EsquemasPagoes, "id", "descripcion", empleado.esquemaPagoId);
            ViewBag.estadoCivilId = new SelectList(db.EstadoCivils, "id", "descripcion", empleado.estadoCivilId);
            ViewBag.estadoNacimientoId = new SelectList(db.Estados, "id", "descripcion", empleado.estadoNacimientoId);
            ViewBag.municipioNacimientoId = new SelectList(db.Municipios, "id", "descripcion", empleado.municipioNacimientoId);
            ViewBag.nacionalidadId = new SelectList(db.Paises, "id", "descripcion", empleado.nacionalidadId);
            ViewBag.sdiId = new SelectList(db.SDIs, "id", "descripcion", empleado.sdiId);
            ViewBag.sexoId = new SelectList(db.Sexos, "id", "descripcion", empleado.sexoId);
            return View(empleado);
        }

        // GET: Empleados/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empleado empleado = db.Empleados.Find(id);
            if (empleado == null)
            {
                return HttpNotFound();
            }
            ViewBag.bancoId = new SelectList(db.Bancos, "id", "descripcion", empleado.bancoId);
            ViewBag.esquemaPagoId = new SelectList(db.EsquemasPagoes, "id", "descripcion", empleado.esquemaPagoId);
            ViewBag.estadoCivilId = new SelectList(db.EstadoCivils, "id", "descripcion", empleado.estadoCivilId);
            ViewBag.estadoNacimientoId = new SelectList(db.Estados, "id", "descripcion", empleado.estadoNacimientoId);
            ViewBag.municipioNacimientoId = new SelectList(db.Municipios, "id", "descripcion", empleado.municipioNacimientoId);
            ViewBag.nacionalidadId = new SelectList(db.Paises, "id", "descripcion", empleado.nacionalidadId);
            ViewBag.sdiId = new SelectList(db.SDIs, "id", "descripcion", empleado.sdiId);
            ViewBag.sexoId = new SelectList(db.Sexos, "id", "descripcion", empleado.sexoId);
            ViewBag.solicitudId = new SelectList(db.Solicituds, "id", "solicita", empleado.solicitudId);
            ViewBag.usuarioId = new SelectList(db.Usuarios, "Id", "nombreUsuario", empleado.usuarioId);
            return View(empleado);
        }

        // POST: Empleados/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,solicitudId,nss,fechaAltaImss,apellidoMaterno,apellidoPaterno,nombre,nombreCompleto,rfc,homoclave,curp,sexoId,sdiId,esquemaPagoId,salarioReal,categoria,tieneInfonavit,creditoInfonavit,estadoCivilId,fechaNacimiento,nacionalidadId,estadoNacimientoId,municipioNacimientoId,calleNumero,colonia,edoMunicipio,codigoPostal,tramitarTarjeta,bancoId,cuentaBancaria,email,observaciones,usuarioId,fechaCreacion,estatus")] Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                db.Entry(empleado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.bancoId = new SelectList(db.Bancos, "id", "descripcion", empleado.bancoId);
            ViewBag.esquemaPagoId = new SelectList(db.EsquemasPagoes, "id", "descripcion", empleado.esquemaPagoId);
            ViewBag.estadoCivilId = new SelectList(db.EstadoCivils, "id", "descripcion", empleado.estadoCivilId);
            ViewBag.estadoNacimientoId = new SelectList(db.Estados, "id", "descripcion", empleado.estadoNacimientoId);
            ViewBag.municipioNacimientoId = new SelectList(db.Municipios, "id", "descripcion", empleado.municipioNacimientoId);
            ViewBag.nacionalidadId = new SelectList(db.Paises, "id", "descripcion", empleado.nacionalidadId);
            ViewBag.sdiId = new SelectList(db.SDIs, "id", "descripcion", empleado.sdiId);
            ViewBag.sexoId = new SelectList(db.Sexos, "id", "descripcion", empleado.sexoId);
            return View(empleado);
        }

        // GET: Empleados/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empleado empleado = db.Empleados.Find(id);
            if (empleado == null)
            {
                return HttpNotFound();
            }
            return View(empleado);
        }

        // POST: Empleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Empleado empleado = db.Empleados.Find(id);
            db.Empleados.Remove(empleado);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult sendEmail()
        {
            Email email = new Email();
            email.EnviarEmail();

            return RedirectToAction("Index");
        }

        public ActionResult CargarEmpleadosPorExcel(int id)
        {
            return View();
        }

        public ActionResult GrabarEmpleadosExcel()
        {

            //ExcelHelper ex = new ExcelHelper();

            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    ExcelHelper ex = new ExcelHelper();
                    //List<PersonalExcelLayout> pel = ex.getPersonalDatos(@"C:\SUA\Layouts\" + file.FileName);
                    LinqToExcelProvider provider = new LinqToExcelProvider(@"C:\SUA\Layouts\" + file.FileName);

                    var query = (from row in provider.GetWorkSheet("datos")
                                 let item = new PersonalExcelLayout
                                 {
                                     nombre = Convert.ToString(row.Field<Object>("Nombre")),
                                     apellidoMaterno = Convert.ToString(row.Field<Object>("ApellidoMaterno")),
                                     apellidoPaterno = Convert.ToString(row.Field<Object>("ApellidoPaterno")),
                                     edad = Convert.ToInt32(row.Field<Object>("Edad"))

                                 }
                                 select item).ToList();



                    foreach (PersonalExcelLayout pelItem in query)
                    {
                        Console.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}", pelItem.nombre, pelItem.apellidoMaterno, pelItem.apellidoPaterno, pelItem.edad.ToString()));
                    }


                }
            }

            return RedirectToAction("Index", "Solicitudes");
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
