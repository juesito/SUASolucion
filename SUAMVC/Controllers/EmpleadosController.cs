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
using System.IO;

namespace SUAMVC.Controllers
{
    public class EmpleadosController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Empleados
        public ActionResult Index(string id, string estatus, String controllerDestiny)
        {

            Solicitud solicitud = new Solicitud();
            List<Empleado> empleadosList = new List<Empleado>();

            if (String.IsNullOrEmpty(id))
            {
                empleadosList = (from s in db.SolicitudEmpleadoes
                                 where s.estatus.Equals("A")
                                 orderby s.id
                                 select s.Empleado).ToList();
            }
            else
            {
                int idTemp = int.Parse(id);
                solicitud = db.Solicituds.Find(idTemp);

                ViewBag.solicitud = solicitud;
                ViewBag.controllerDestiny = controllerDestiny;

                empleadosList = (from s in db.SolicitudEmpleadoes
                                 where s.solicitudId.Equals(idTemp)
                                 orderby s.id
                                 select s.Empleado).ToList();
            }


            SolicitudEmpleadoModel solicitudEmpleadoModel = new SolicitudEmpleadoModel();

            solicitudEmpleadoModel.solicitud = solicitud;
            solicitudEmpleadoModel.empleados = empleadosList;

            return View(solicitudEmpleadoModel);
        }

        //agregar empleado
        public ActionResult asignarEmpleado(String[] ids, string solicitudId)
        {
            Empleado empleado = new Empleado();
            Usuario usuario = Session["UsuarioData"] as Usuario;
            int solicitudTempId = int.Parse(solicitudId);
            Solicitud solicitud = db.Solicituds.Find(solicitudTempId);

            ToolsHelper th = new ToolsHelper();

            if (ids != null && ids.Length > 0)
            {
                foreach (String empleadoId in ids)
                {
                    //buscar el empleadoiD en db.Empleados y cambia el estatus a B. con la fecha de baja de la solicitud
                    int empleadoTempId = int.Parse(empleadoId);
                    empleado = db.Empleados.Find(empleadoTempId);

                    //Obtenemos la solicitud antigua
                    Solicitud solicitudEmpleado = obtenerSolicitudActiva(empleado.id);
                    solicitudEmpleado.noTrabajadores = solicitudEmpleado.noTrabajadores - 1;
                    empleado.estatus = "B";
                    empleado.fechaBaja = solicitud.fechaBaja;

                    //Solicitud para modificar el noTrabjadores
                    solicitud.noTrabajadores = solicitud.noTrabajadores + 1;

                    //Creamos el registro en solicitudEmpleados para agregar el empleado a otra solicitud activa
                    crearSolicitudEmpleado(empleado.id, solicitud.id, usuario.Id, "Baja");

                    //empleado.folioEmpleado = solicitud.folioSolicitud.Trim() + "-" + empleado.id.ToString().PadLeft(5, '0');

                    db.Entry(solicitudEmpleado).State = EntityState.Modified;
                    db.Entry(solicitud).State = EntityState.Modified;
                    db.Entry(empleado).State = EntityState.Modified;
                    db.SaveChanges();
                }

            }

            return RedirectToAction("BajaEmpleados", "Empleados", new { id = solicitud.id, clienteId = solicitud.clienteId });
        }

        // GET: Empleados/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DatosEmpleadoModel datosEmpleadoModel = new DatosEmpleadoModel();
            Empleado empleado = db.Empleados.Find(id);

            if (empleado == null)
            {
                return HttpNotFound();
            }
            ToolsHelper th = new ToolsHelper();

            //Obtenemos los tipos de documentos 
            Concepto cv = th.obtenerConceptoPorGrupo("ARCHEMP", "CV");
            Concepto docVarios = th.obtenerConceptoPorGrupo("ARCHEMP", "Documento");
            Concepto contratos = th.obtenerConceptoPorGrupo("ARCHEMP", "Contratos");
            Concepto psicometria = th.obtenerConceptoPorGrupo("ARCHEMP", "Psicometria");
            Concepto confidencial = th.obtenerConceptoPorGrupo("ARCHEMP", "Confidencial");

            // Obtenemos los documentos cargados para el empleado
            ViewBag.docsCv = db.ArchivoEmpleados.Where(de => de.empleadoId.Equals(empleado.id)
                && de.tipoArchivo.Equals(cv.id)).Count();
            ViewBag.docsVarios = db.ArchivoEmpleados.Where(de => de.empleadoId.Equals(empleado.id)
                && de.tipoArchivo.Equals(docVarios.id)).Count();
            ViewBag.docsContratos = db.ArchivoEmpleados.Where(de => de.empleadoId.Equals(empleado.id)
                && de.tipoArchivo.Equals(contratos.id)).Count();
            ViewBag.docsPsicometricos = db.ArchivoEmpleados.Where(de => de.empleadoId.Equals(empleado.id)
                && de.tipoArchivo.Equals(psicometria.id)).Count();
            ViewBag.docsConfidencial = db.ArchivoEmpleados.Where(de => de.empleadoId.Equals(empleado.id)
                && de.tipoArchivo.Equals(confidencial.id)).Count();

            //Obtenemos la solicitud del empleado
            Solicitud solicitud = obtenerSolicitudActiva(empleado.id);
            DocumentoEmpleado documentosEmpleado = db.DocumentoEmpleadoes.Where(de => de.empleadoId.Equals(empleado.id)).FirstOrDefault();
            SalarialesEmpleado salarialesEmpleado = db.SalarialesEmpleadoes.Where(se => se.empleadoId.Equals(empleado.id)).FirstOrDefault();

            datosEmpleadoModel.solicitud = solicitud;
            datosEmpleadoModel.empleado = empleado;
            datosEmpleadoModel.datosEmpleado = documentosEmpleado;
            datosEmpleadoModel.salarialesEmpleado = salarialesEmpleado;

            return View(datosEmpleadoModel);
        }

        // GET: Empleados/Create
        public ActionResult Create(int id)
        {
            Empleado empleado = new Empleado();
            Solicitud solicitud = db.Solicituds.Find(id);

            empleado.fechaNacimiento = DateTime.Now;
            empleado.tramitarTarjeta = 0;
            empleado.tieneInfonavit = 1;
            empleado.esquemaPagoId = solicitud.esquemaId;
            ViewBag.solicitudId = id;

            ViewBag.bancoId = new SelectList(db.Bancos, "id", "descripcion", empleado.bancoId);
            ViewBag.municipioNacimientoId = new SelectList(db.Municipios, "id", "descripcion", empleado.municipioNacimientoId);
            ViewBag.nacionalidadId = new SelectList(db.Paises, "id", "descripcion", empleado.nacionalidadId);
            ViewBag.estadoNacimientoId = new SelectList(db.Estados, "id", "descripcion", empleado.estadoNacimientoId);
            ViewBag.estadoCivilId = new SelectList(db.EstadoCivils, "id", "descripcion", empleado.estadoCivilId);
            ViewBag.sexoId = new SelectList(db.Sexos, "id", "descripcion");

            return View(empleado);
        }

        // POST: Empleados/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,nss,fechaAltaImss,apellidoMaterno,apellidoPaterno,nombre,nombreCompleto,rfc,homoclave,curp,sexoId,sdiId,esquemaPagoId,salarioReal,categoria,tieneInfonavit,creditoInfonavit,estadoCivilId,fechaNacimiento,nacionalidadId,estadoNacimientoId,municipioNacimientoId,calleNumero,colonia,edoMunicipio,codigoPostal,tramitarTarjeta,bancoId,cuentaBancaria,email,observaciones,usuarioId,fechaCreacion,estatus")] Empleado empleado, int solicitudId)
        {
            if (ModelState.IsValid)
            {
                ToolsHelper th = new ToolsHelper();
                Usuario usuario = Session["UsuarioData"] as Usuario;

                empleado.fechaCreacion = DateTime.Now;
                empleado.usuarioId = usuario.Id;
                empleado.nombreCompleto = empleado.nombre + " " + empleado.apellidoPaterno + " " + empleado.apellidoMaterno;
                empleado.estatus = "A";
                empleado.rfc = empleado.rfc.Trim();
                empleado.homoclave = empleado.homoclave.Trim();

                if (!String.IsNullOrEmpty(empleado.nss))
                {
                    Asegurado asegurado = th.obtenerAseguradoPorNSS(empleado.nss.Trim());

                    if (!(asegurado == null) && !String.IsNullOrEmpty(asegurado.nombre))
                    {
                        empleado.aseguradoId = asegurado.id;
                    }
                }

                //Obtenemos el sexo del empleado
                empleado.Sexo = db.Sexos.Find(empleado.sexoId);
                if (empleado.Sexo.descripcion.ToLower().Trim().Contains("femenino") ||
                        empleado.Sexo.descripcion.ToLower().Trim().Contains("mujer"))
                {
                    empleado.foto = "~/Content/Images/girl.png";
                }
                else
                {
                    empleado.foto = "~/Content/Images/male.png";
                }

                empleado.foto = empleado.foto.Trim();
                db.Empleados.Add(empleado);


                try
                {
                    db.SaveChanges();
                    crearSolicitudEmpleado(empleado.id, solicitudId, usuario.Id, "Alta");

                    //Obtenemos la solicitud par modificar el noTrabjadores
                    //a su vez con ella obtener el folio de Solicitud para generar el folioEmpleado
                    Solicitud solicitud = obtenerSolicitudActiva(empleado.id);
                    solicitud.noTrabajadores = solicitud.noTrabajadores + 1;

                    empleado.folioEmpleado = solicitud.folioSolicitud.Trim() + "-" + empleado.id.ToString().PadLeft(5, '0');

                    //Preparamos las entidades para guardar
                    db.Entry(empleado).State = EntityState.Modified;
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

                return RedirectToAction("Index", "Solicitudes", new { id = solicitudId });
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
            DatosEmpleadoModel datosEmpleadoModel = new DatosEmpleadoModel();
            Empleado empleado = db.Empleados.Find(id);

            if (empleado == null)
            {
                return HttpNotFound();
            }
            ToolsHelper th = new ToolsHelper();

            //Obtenemos los tipos de documentos 
            Concepto cv = th.obtenerConceptoPorGrupo("ARCHEMP", "CV");
            Concepto docVarios = th.obtenerConceptoPorGrupo("ARCHEMP", "Documento");
            Concepto contratos = th.obtenerConceptoPorGrupo("ARCHEMP", "Contratos");
            Concepto psicometria = th.obtenerConceptoPorGrupo("ARCHEMP", "Psicometria");
            Concepto confidencial = th.obtenerConceptoPorGrupo("ARCHEMP", "Confidencial");

            // Obtenemos los documentos cargados para el empleado
            ViewBag.docsCv = db.ArchivoEmpleados.Where(de => de.empleadoId.Equals(empleado.id)
                && de.tipoArchivo.Equals(cv.id)).Count();
            ViewBag.docsVarios = db.ArchivoEmpleados.Where(de => de.empleadoId.Equals(empleado.id)
                && de.tipoArchivo.Equals(docVarios.id)).Count();
            ViewBag.docsContratos = db.ArchivoEmpleados.Where(de => de.empleadoId.Equals(empleado.id)
                && de.tipoArchivo.Equals(contratos.id)).Count();
            ViewBag.docsPsicometricos = db.ArchivoEmpleados.Where(de => de.empleadoId.Equals(empleado.id)
                && de.tipoArchivo.Equals(psicometria.id)).Count();
            ViewBag.docsConfidencial = db.ArchivoEmpleados.Where(de => de.empleadoId.Equals(empleado.id)
                && de.tipoArchivo.Equals(confidencial.id)).Count();

            //Obtenemos la solicitud del empleado
            Solicitud solicitud = obtenerSolicitudActiva(empleado.id);
            DocumentoEmpleado documentosEmpleado = db.DocumentoEmpleadoes.Where(de => de.empleadoId.Equals(empleado.id)).FirstOrDefault();
            SalarialesEmpleado salarialesEmpleado = db.SalarialesEmpleadoes.Where(se => se.empleadoId.Equals(empleado.id)).FirstOrDefault();

            datosEmpleadoModel.solicitud = solicitud;
            datosEmpleadoModel.empleado = empleado;
            datosEmpleadoModel.datosEmpleado = documentosEmpleado;
            datosEmpleadoModel.salarialesEmpleado = salarialesEmpleado;

            return View(datosEmpleadoModel);
        }

        // POST: Empleados/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,nss,fechaAltaImss,apellidoMaterno,apellidoPaterno,nombre,rfc,homoclave,curp,categoria, fechaNacimiento,email,observaciones")] Empleado empleado, int sexoId)
        {
            if (ModelState.IsValid)
            {
                Empleado empleadoModificado = db.Empleados.Find(empleado.id);

                empleadoModificado.nss = empleado.nss;
                empleadoModificado.fechaAltaImss = empleado.fechaAltaImss;
                empleadoModificado.apellidoMaterno = empleado.apellidoMaterno;
                empleadoModificado.apellidoPaterno = empleado.apellidoPaterno;
                empleadoModificado.nombre = empleado.nombre;
                empleadoModificado.rfc = empleado.rfc;
                empleadoModificado.homoclave = empleado.homoclave;
                empleadoModificado.curp = empleado.curp;
                empleadoModificado.sexoId = sexoId;
                //empleadoModificado.estadoCivilId = empleado.estadoCivilId;
                empleadoModificado.categoria = empleado.categoria;
                empleadoModificado.fechaNacimiento = empleado.fechaNacimiento;
                //empleadoModificado.nacionalidadId = paisId;
                //empleadoModificado.estadoNacimientoId = empleado.estadoNacimientoId;
                //empleadoModificado.municipioNacimientoId = empleado.municipioNacimientoId;
                empleadoModificado.email = empleado.email;
                empleadoModificado.observaciones = empleado.observaciones;

                try
                {
                    db.Entry(empleadoModificado).State = EntityState.Modified;
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
                return RedirectToAction("Edit", "Empleados", new { id = empleado.id });
            }
            return View(empleado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GuardarDocumentos([Bind(Include = "id,calleNumero,colonia,edoMunicipio,codigoPostal")] Empleado empleado,
            [Bind(Include = "actividades, domicilioOficina, fechaAntiguedad, salarioVSM, diasDescanso, salarioNominal,diasVacaciones,diasAguinaldo,otros,telefono,tipoSangre")] DocumentoEmpleado datosEmpleado, int jornadaLaboralId)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;
                Empleado empleadoModificado = db.Empleados.Find(empleado.id);
                empleadoModificado.calleNumero = empleado.calleNumero;
                empleadoModificado.colonia = empleado.colonia;
                empleadoModificado.edoMunicipio = empleado.edoMunicipio;
                empleadoModificado.codigoPostal = empleado.codigoPostal;

                DocumentoEmpleado documentoEmpleadoModificado = db.DocumentoEmpleadoes.Where(de => de.empleadoId.Equals(empleado.id)).FirstOrDefault();


                //Hay que buscar en internet como se puede hacer esto porque se ve que esta jodido
                if (documentoEmpleadoModificado != null)
                {
                    documentoEmpleadoModificado.empleadoId = empleado.id;
                    documentoEmpleadoModificado.actividades = datosEmpleado.actividades;
                    documentoEmpleadoModificado.domicilioOficina = datosEmpleado.domicilioOficina;
                    documentoEmpleadoModificado.fechaAntiguedad = datosEmpleado.fechaAntiguedad;
                    documentoEmpleadoModificado.salarioVSM = datosEmpleado.salarioVSM;
                    documentoEmpleadoModificado.jornadaLaboralId = jornadaLaboralId;
                    documentoEmpleadoModificado.diasDescanso = datosEmpleado.diasDescanso;
                    documentoEmpleadoModificado.salarioNominal = datosEmpleado.salarioNominal;
                    documentoEmpleadoModificado.diasVacaciones = datosEmpleado.diasVacaciones;
                    documentoEmpleadoModificado.diasAguinaldo = datosEmpleado.diasAguinaldo;
                    documentoEmpleadoModificado.otros = datosEmpleado.otros;
                    documentoEmpleadoModificado.tipoSangre = datosEmpleado.tipoSangre;

                    db.Entry(documentoEmpleadoModificado).State = EntityState.Modified;
                }
                else
                {
                    documentoEmpleadoModificado = new DocumentoEmpleado();
                    documentoEmpleadoModificado.empleadoId = empleado.id;
                    documentoEmpleadoModificado.actividades = datosEmpleado.actividades;
                    documentoEmpleadoModificado.domicilioOficina = datosEmpleado.domicilioOficina;
                    documentoEmpleadoModificado.fechaAntiguedad = datosEmpleado.fechaAntiguedad;
                    documentoEmpleadoModificado.salarioVSM = datosEmpleado.salarioVSM;
                    documentoEmpleadoModificado.jornadaLaboralId = jornadaLaboralId;
                    documentoEmpleadoModificado.diasDescanso = datosEmpleado.diasDescanso;
                    documentoEmpleadoModificado.salarioNominal = datosEmpleado.salarioNominal;
                    documentoEmpleadoModificado.diasVacaciones = datosEmpleado.diasVacaciones;
                    documentoEmpleadoModificado.diasAguinaldo = datosEmpleado.diasAguinaldo;
                    documentoEmpleadoModificado.otros = datosEmpleado.otros;
                    documentoEmpleadoModificado.tipoSangre = datosEmpleado.tipoSangre;
                    documentoEmpleadoModificado.fechaCreacion = DateTime.Now;
                    documentoEmpleadoModificado.usuarioId = usuario.Id;

                    db.DocumentoEmpleadoes.Add(documentoEmpleadoModificado);
                }

                db.Entry(empleadoModificado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit", "Empleados", new { id = empleado.id });
            }
            return RedirectToAction("Edit", "Empleados", new { id = empleado.id });
        }

        public ActionResult GuardarSalariales([Bind(Include = "id,salarioReal,creditoInfonavit")] Empleado empleado,
            [Bind(Include = "salarioMensual, salarioHrsExtra, montoInfonavit,importeFonacot,porcientoPension, periodoId, importePension")] SalarialesEmpleado salarialesEmpleado, int bancoId)
        {
            if (ModelState.IsValid)
            {
                Usuario usuario = Session["UsuarioData"] as Usuario;
                Empleado empleadoModificado = db.Empleados.Find(empleado.id);
                empleadoModificado.salarioReal = empleado.salarioReal;
                empleadoModificado.bancoId = bancoId;
                empleadoModificado.creditoInfonavit = empleado.creditoInfonavit;

                SalarialesEmpleado documentoEmpleadoModificado = db.SalarialesEmpleadoes.Where(de => de.empleadoId.Equals(empleado.id)).FirstOrDefault();

                if (documentoEmpleadoModificado != null)
                {
                    documentoEmpleadoModificado.empleadoId = empleado.id;
                    documentoEmpleadoModificado.salarioMensual = salarialesEmpleado.salarioMensual;
                    documentoEmpleadoModificado.salarioHrsExtra = salarialesEmpleado.salarioHrsExtra;
                    documentoEmpleadoModificado.montoInfonavit = salarialesEmpleado.montoInfonavit;
                    documentoEmpleadoModificado.importeFonacot = salarialesEmpleado.importeFonacot;
                    documentoEmpleadoModificado.porcientoPension = salarialesEmpleado.porcientoPension;
                    documentoEmpleadoModificado.periodoId = salarialesEmpleado.periodoId;
                    documentoEmpleadoModificado.importePension = salarialesEmpleado.importePension;

                    db.Entry(documentoEmpleadoModificado).State = EntityState.Modified;
                }
                else
                {
                    documentoEmpleadoModificado = new SalarialesEmpleado();
                    documentoEmpleadoModificado.empleadoId = empleado.id;
                    documentoEmpleadoModificado.salarioMensual = salarialesEmpleado.salarioMensual;
                    documentoEmpleadoModificado.salarioHrsExtra = salarialesEmpleado.salarioHrsExtra;
                    documentoEmpleadoModificado.montoInfonavit = salarialesEmpleado.montoInfonavit;
                    documentoEmpleadoModificado.importeFonacot = salarialesEmpleado.importeFonacot;
                    documentoEmpleadoModificado.porcientoPension = salarialesEmpleado.porcientoPension;
                    documentoEmpleadoModificado.periodoId = salarialesEmpleado.periodoId;
                    documentoEmpleadoModificado.importePension = salarialesEmpleado.importePension;
                    documentoEmpleadoModificado.fechaCreacion = DateTime.Now;
                    documentoEmpleadoModificado.usuarioId = usuario.Id;

                    db.SalarialesEmpleadoes.Add(documentoEmpleadoModificado);
                }

                db.Entry(empleadoModificado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit", "Empleados", new { id = empleado.id });
            }
            return RedirectToAction("Edit", "Empleados", new { id = empleado.id });
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

            Solicitud solicitud = obtenerSolicitudActiva(id);
            solicitud.noTrabajadores = solicitud.noTrabajadores - 1;

            SolicitudEmpleado solEmp = db.SolicitudEmpleadoes.Where(se => se.solicitudId.Equals(solicitud.id)
                && se.empleadoId.Equals(id)).FirstOrDefault();

            db.SolicitudEmpleadoes.Remove(solEmp);
            db.Entry(solicitud).State = EntityState.Modified;
            db.Empleados.Remove(empleado);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /*
         * Nos vamos a la vista para subir la foto
         * 
         */
        [HttpGet]
        public ActionResult SubirFoto(int empleadoId)
        {

            Empleado empleado = db.Empleados.Find(empleadoId);

            return View(empleado);
        }

        /*
         * Guardamos la foto del empleado segun su Id
         */
        [HttpPost]
        public ActionResult GuardarFoto(int id)
        {
            ParametrosHelper ph = new ParametrosHelper();
            Parametro parametro = ph.getParameterByKey("IMGFOLDER");
            ToolsHelper th = new ToolsHelper();

            HttpFileCollectionBase files = Request.Files;
            String destino = parametro.valorString.Trim() + id + "\\";
            String name = th.cargarArchivo(files, destino);

            Empleado empleado = db.Empleados.Find(id);

            if (empleado.foto.Contains(destino))
            {
                th.BorrarArchivo(empleado.foto);
            }

            empleado.foto = destino.Trim() + name.Trim();

            db.Entry(empleado).State = EntityState.Modified;
            try
            {
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
            return RedirectToAction("Edit", "Empleados", new { id = id });

        }


        public ActionResult CargarEmpleadosPorExcel(int id)
        {
            ViewBag.solicitud = db.Solicituds.Find(id);
            
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

                    provider.readExcel("datos");

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

                    Empleado empleadoL = new Empleado();
                    Empleado empleado = new Empleado();
                        Usuario usuario = new Usuario();
                        Solicitud solicitud = new Solicitud();

                            if (!String.IsNullOrEmpty(empleadoL.observaciones))
                            {
                                empleado.observaciones = empleadoL.observaciones.Trim();
                            } // observaciones no es null


                            empleado.usuarioId = usuario.Id;
                            empleado.estatus = "A";


                            try
                            {
                                if (!false)
                                {
                                    empleado.fechaCreacion = DateTime.Now;
                                    db.Empleados.Add(empleado);
                                }
                                else
                                {
                                    empleado.fechaCreacion = DateTime.Now;
                                }

                                db.SaveChanges();
                                crearSolicitudEmpleado(empleado.id, solicitud.id, usuario.Id, solicitud.Concepto5.descripcion);

                                //Obtenemos la solicitud par modificar el noTrabjadores
                                //a su vez con ella obtener el folio de Solicitud para generar el folioEmpleado
                                solicitud.noTrabajadores = solicitud.noTrabajadores + 1;

                                empleado.folioEmpleado = solicitud.folioSolicitud.Trim() + "-" + empleado.id.ToString().PadLeft(5, '0');

                                //Preparamos las entidades para guardar
                                db.Entry(empleado).State = EntityState.Modified;
                                db.Entry(solicitud).State = EntityState.Modified;
                                db.SaveChanges();

                            }
                            catch (DbEntityValidationException exm)
                            {
                                StringBuilder sb = new StringBuilder();

                                foreach (var failure in exm.EntityValidationErrors)
                                {
                                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                                    foreach (var error in failure.ValidationErrors)
                                    {
                                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                                        sb.AppendLine();
                                    }
                                }
                            }

                        }
                    }

            return RedirectToAction("Index", "Solicitudes");
        }

        //BAJA EMPLEADOS
        // GET: BajaEmpleados
        public ActionResult BajaEmpleados(int id, string clienteId)
        {

            List<Empleado> listEmpleados = new List<Empleado>();
            int clienteTempId = int.Parse(clienteId);
            Solicitud solicitud = db.Solicituds.Find(id);

            ViewBag.solicitudId = id;

            List<Empleado> empleadosList = (from s in db.SolicitudEmpleadoes
                                            join e in db.Empleados on s.empleadoId equals e.id
                                            where e.estatus.Equals("A")
                                            orderby s.id
                                            select s.Empleado).ToList();

            foreach (Empleado emp in empleadosList)
            {
                emp.fechaBaja = solicitud.fechaBaja;
                listEmpleados.Add(emp);
            }

            return View(listEmpleados);
        }

        //Modificar Empleado
        // GET: ModificarEmpleado
        public ActionResult ModificarEmpleado(int id, string clienteId, string solicitudId)
        {

            List<Empleado> listEmpleados = new List<Empleado>();
            int clienteTempId = int.Parse(clienteId);
            Solicitud solicitud = db.Solicituds.Find(id);


            ViewBag.solicitudId = id;

                List<Empleado> empleadosList = (from s in db.SolicitudEmpleadoes
                                                join e in db.Empleados on s.empleadoId equals e.id
                                                where s.Solicitud.clienteId.Equals(clienteTempId)
                                            && e.estatus.Equals("A")
                                                orderby s.id
                                                select s.Empleado).ToList();

          
           
            

                foreach (Empleado emp in empleadosList)
                {
                    emp.fechaCreacion = DateTime.Parse(solicitud.fechaSolicitud.ToString());
                    listEmpleados.Add(emp);

                }
                db.Entry(solicitud).State = EntityState.Modified;
                db.SaveChanges();
            return View(listEmpleados);
        }

        public ActionResult validarNss(String nss)
        {
            ToolsHelper th = new ToolsHelper();
            Empleado empleado = th.obtenerEmpleadoPorNSS(nss.Trim());



            if (empleado == null)
            {
                ViewBag.editMode = true;
                return Json("");
            }
            else
            {
                ViewBag.editMode = false;
                // Json(new { ok = true, newurl = Url.Action("Create") });
                return RedirectToAction("Create", "Empleados", empleado);
            }
        }

        public Solicitud obtenerSolicitudActiva(int empleadoId)
        {

            Solicitud solicitud = (from s in db.SolicitudEmpleadoes
                                   where s.empleadoId.Equals(empleadoId)
                                   && s.estatus.Equals("A")
                                   select s.Solicitud).FirstOrDefault();
            return solicitud;
        }

        /*
         * Se crea el registro para relacionar empleados con solicitud
         */
        public void crearSolicitudEmpleado(int empleadoId, int solicitudId, int usuarioId, String tipo)
        {
            ToolsHelper th = new ToolsHelper();
            Concepto concepto = th.obtenerConceptoPorGrupo("SOLCON", tipo);

            var solicitudesEmpleado = db.SolicitudEmpleadoes.Where(se => se.empleadoId.Equals(empleadoId)
                && se.estatus.Equals("A")).ToList();

            foreach (SolicitudEmpleado solist in solicitudesEmpleado)
            {
                solist.estatus = "B";
                db.Entry(solist).State = EntityState.Modified;
                db.SaveChanges();
            }

            //Creamos y guardamos el registro para el amarre de la soicitud
            SolicitudEmpleado solicitudEmpleado = new SolicitudEmpleado();
            solicitudEmpleado.empleadoId = empleadoId;
            solicitudEmpleado.solicitudId = solicitudId;
            solicitudEmpleado.estatus = "A";
            solicitudEmpleado.tipoId = concepto.id;
            solicitudEmpleado.fechaCreacion = DateTime.Now;
            solicitudEmpleado.usuarioId = usuarioId;

            db.SolicitudEmpleadoes.Add(solicitudEmpleado);
            db.SaveChanges();

        }

        /**
         * Agregar foto de empleado.
         */
        public ActionResult MyFoto(String foto)
        {

            FileInfo f = new FileInfo(foto);

            if (f.Exists)
            {
                FileStream s = f.Open(FileMode.Open, FileAccess.Read);

                return File(s, "image/*");
            }
            else
            {
                f = new FileInfo("~/Content/Images/camera.png");
                FileStream s = f.Open(FileMode.Open, FileAccess.Read);
                return File(s, "image/*");
            }


        }

        public ActionResult ModificarSalario()
        {

            var empleadoId = Request["EmpleadoId"];
            var sdiId = Request["Sdi"];
            if (empleadoId != null && sdiId != null)
            {
                int empleadoTempId = int.Parse(empleadoId);
                int sdiTempId = int.Parse(sdiId);

                Empleado empleado = db.Empleados.Find(empleadoTempId);
                empleado.sdiId = sdiTempId;

                db.Entry(empleado).State = EntityState.Modified;
                db.SaveChanges();

            }


            return Json(Response.Output);

        }

        public ActionResult extraExcel() {

            ExcelHelper eh = new ExcelHelper();
            eh.makeExcel(@"C:\\hello.xlsx");

            return RedirectToAction("Index", "Empleados");

        }

        [HttpGet]
        public ActionResult obtenerCategorias(String term)
        {
            List<Concepto> catalogos = new List<Concepto>();
            catalogos = db.Conceptos.Where(m => m.grupo.Equals("CATEGORIA") &&
                m.descripcion.Trim().ToLower().StartsWith(term.Trim().ToLower())).ToList();
            var data = catalogos.Select(p => p.descripcion).Distinct();
            return Json(data, JsonRequestBehavior.AllowGet);
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
