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
        //id: solicitud's id
        // status: is from the solicitudes, this is just to know if the solicitud it was sended
        //clienteId: cliente's id
        //proyectoId: proyecto's id
        public ActionResult Index(String id, String estatus, String controllerDestiny, String clienteId, String proyectoId, String folioId, String status)
        {

            Solicitud solicitud = new Solicitud();
            List<Empleado> empleadosList = new List<Empleado>();

            ViewBag.status = "on";
            if (String.IsNullOrEmpty(status)) {
                ViewBag.status = "off";    
            }

            //empleadosList = (from s in db.SolicitudEmpleadoes
            //                 where s.estatus.Equals("A")
            //                 orderby s.id
            //                 select s.Empleado).ToList();

            if (!String.IsNullOrEmpty(id))
            {
                int idTemp = int.Parse(id);
                solicitud = db.Solicituds.Find(idTemp);

                ViewBag.solicitud = solicitud;
                ViewBag.controllerDestiny = controllerDestiny;

                empleadosList = (from s in db.SolicitudEmpleadoes
                                 where s.solicitudId.Equals(idTemp)
                                 orderby s.Empleado.nombreCompleto
                                 select s.Empleado).ToList();
            }//la solicitud no es nulla?

            if (!String.IsNullOrEmpty(clienteId) && !String.IsNullOrEmpty(proyectoId) && String.IsNullOrEmpty(folioId))
            {

                int clienteIntId = int.Parse(clienteId);
                int proyectoIntId = int.Parse(proyectoId);
                ViewBag.clienteId = clienteId;
                ViewBag.proyectoId = proyectoIntId;

                empleadosList = (from s in db.SolicitudEmpleadoes
                                 where s.Solicitud.clienteId.Equals(clienteIntId)
                                 && s.Solicitud.proyectoId.Equals(proyectoIntId)
                                 orderby s.Empleado.nombreCompleto
                                 select s.Empleado).ToList();

            }//el cliente y el proyecto no son nullos?
            else if (!String.IsNullOrEmpty(clienteId) && !String.IsNullOrEmpty(proyectoId) && !String.IsNullOrEmpty(folioId))
            {
                int clienteIntId = int.Parse(clienteId);
                int proyectoIntId = int.Parse(proyectoId);

                ViewBag.clienteId = clienteId;
                ViewBag.proyectoId = proyectoIntId;
                ViewBag.folioId = folioId;

                empleadosList = (from s in db.SolicitudEmpleadoes
                                 where s.Solicitud.clienteId.Equals(clienteIntId)
                                 && s.Solicitud.proyectoId.Equals(proyectoIntId)
                                 && s.Empleado.folioEmpleado.Trim().Contains(folioId.Trim())
                                 orderby s.Empleado.nombreCompleto
                                 select s.Empleado).ToList();

            }//El folio no es null?
            else
            {
                if (!String.IsNullOrEmpty(folioId))
                {
                ViewBag.folioId = folioId;

                empleadosList = (from s in db.SolicitudEmpleadoes
                                 where s.Empleado.folioEmpleado.Trim().Contains(folioId.Trim())
                                     orderby s.Empleado.nombreCompleto
                                 select s.Empleado).ToList();
            }
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

                    //Solicitud para modificar el noTrabjadores
                    solicitud.noTrabajadores = solicitud.noTrabajadores + 1;

                    //Creamos el registro en solicitudEmpleados para agregar el empleado a otra solicitud activa
                    crearSolicitudEmpleado(empleado.id, solicitud.id, usuario.Id, "Baja");

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
            ViewBag.docsCv = db.ArchivoEmpleadoes.Where(de => de.empleadoId.Equals(empleado.id)
                && de.tipoArchivo.Equals(cv.id)).Count();
            ViewBag.docsVarios = db.ArchivoEmpleadoes.Where(de => de.empleadoId.Equals(empleado.id)
                && de.tipoArchivo.Equals(docVarios.id)).Count();
            ViewBag.docsContratos = db.ArchivoEmpleadoes.Where(de => de.empleadoId.Equals(empleado.id)
                && de.tipoArchivo.Equals(contratos.id)).Count();
            ViewBag.docsPsicometricos = db.ArchivoEmpleadoes.Where(de => de.empleadoId.Equals(empleado.id)
                && de.tipoArchivo.Equals(psicometria.id)).Count();
            ViewBag.docsConfidencial = db.ArchivoEmpleadoes.Where(de => de.empleadoId.Equals(empleado.id)
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
            TempData["solicitudId"] = id;

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
                Solicitud sol = db.Solicituds.Find(solicitudId);



                empleado.fechaCreacion = DateTime.Now;
                empleado.usuarioId = usuario.Id;
                empleado.nombreCompleto = empleado.nombre + " " + empleado.apellidoPaterno + " " + empleado.apellidoMaterno;
                //Ponemos el estatus en Pendiente hasta
                //que se procese la solicitud
                empleado.estatus = "P";
                empleado.rfc = empleado.rfc.Trim();
                empleado.homoclave = empleado.homoclave.Trim();
                empleado.sdiAlternativoId = empleado.sdiId;

                if (!String.IsNullOrEmpty(empleado.nss))
                {
                    Boolean founded = th.verificarEmpleadoPorNSSyCliente(empleado.nss.Trim(), sol.clienteId);

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

                if (!String.IsNullOrEmpty(empleado.foto))
                {
                    empleado.foto = empleado.foto.Trim().ToUpper();
                }
                
                if (!String.IsNullOrEmpty(empleado.nss))
                {
                    empleado.nss = empleado.nss.Trim().ToUpper();
                }
                
                if(!String.IsNullOrEmpty(empleado.apellidoMaterno))
                {
                    empleado.apellidoMaterno = empleado.apellidoMaterno.Trim().ToUpper();
                }
                
                if(!String.IsNullOrEmpty(empleado.apellidoPaterno))
                {
                    empleado.apellidoPaterno = empleado.apellidoPaterno.Trim().ToUpper();
                }
                
                if(!String.IsNullOrEmpty(empleado.nombre))
                {
                    empleado.nombre = empleado.nombre.Trim().ToUpper();
                }
             
                if(!String.IsNullOrEmpty(empleado.nombreCompleto))
                {
                    empleado.nombreCompleto = empleado.nombreCompleto.Trim().ToUpper();
                }
                
                if(!String.IsNullOrEmpty(empleado.rfc))
                {
                    empleado.rfc = empleado.rfc.Trim().ToUpper();
                }

                if (!String.IsNullOrEmpty(empleado.homoclave))
                {
                    empleado.homoclave = empleado.homoclave.Trim().ToUpper();
                }

                if (!String.IsNullOrEmpty(empleado.curp))
                {
                    empleado.curp = empleado.curp.Trim().ToUpper();
                }

                if (!String.IsNullOrEmpty(empleado.categoria))
                {
                    empleado.categoria = empleado.categoria.Trim().ToUpper();
                }

                if (!String.IsNullOrEmpty(empleado.creditoInfonavit))
                {
                    empleado.creditoInfonavit = empleado.creditoInfonavit.Trim().ToUpper();
                }

                if (!String.IsNullOrEmpty(empleado.calleNumero))
                {
                    empleado.calleNumero = empleado.calleNumero.Trim().ToUpper();
                }

                if (!String.IsNullOrEmpty(empleado.colonia))
                {
                    empleado.colonia = empleado.colonia.Trim().ToUpper();
                }

                if (!String.IsNullOrEmpty(empleado.edoMunicipio))
                {
                    empleado.edoMunicipio = empleado.edoMunicipio.Trim().ToUpper();
                }

                if (!String.IsNullOrEmpty(empleado.observaciones))
                {
                    empleado.observaciones = empleado.observaciones.Trim().ToUpper();
                }

                if (!String.IsNullOrEmpty(empleado.estatus))
                {
                    empleado.estatus = empleado.estatus.Trim().ToUpper();
                }
                

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
                    return RedirectToAction("Index", "Solicitudes", new { clienteId = solicitud.clienteId, proyectoId = solicitud.proyectoId });

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
            ViewBag.docsCv = db.ArchivoEmpleadoes.Where(de => de.empleadoId.Equals(empleado.id)
                && de.tipoArchivo.Equals(cv.id)).Count();
            ViewBag.docsVarios = db.ArchivoEmpleadoes.Where(de => de.empleadoId.Equals(empleado.id)
                && de.tipoArchivo.Equals(docVarios.id)).Count();
            ViewBag.docsContratos = db.ArchivoEmpleadoes.Where(de => de.empleadoId.Equals(empleado.id)
                && de.tipoArchivo.Equals(contratos.id)).Count();
            ViewBag.docsPsicometricos = db.ArchivoEmpleadoes.Where(de => de.empleadoId.Equals(empleado.id)
                && de.tipoArchivo.Equals(psicometria.id)).Count();
            ViewBag.docsConfidencial = db.ArchivoEmpleadoes.Where(de => de.empleadoId.Equals(empleado.id)
                && de.tipoArchivo.Equals(confidencial.id)).Count();

            //Obtenemos la solicitud del empleado
            Solicitud solicitud = obtenerSolicitudActiva(empleado.id);
            DocumentoEmpleado documentosEmpleado = db.DocumentoEmpleadoes.Where(de => de.empleadoId.Equals(empleado.id)).FirstOrDefault();
            SalarialesEmpleado salarialesEmpleado = db.SalarialesEmpleadoes.Where(se => se.empleadoId.Equals(empleado.id)).FirstOrDefault();

            ViewBag.estadoNacimientoId = new SelectList(db.Estados, "id", "descripcion", empleado.estadoNacimientoId);
            ViewBag.municipioNacimientoId = new SelectList(db.Municipios, "id", "descripcion", empleado.municipioNacimientoId);
            ViewBag.nacionalidadId = new SelectList(db.Paises, "id", "descripcion", empleado.nacionalidadId);

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

            if(!String.IsNullOrEmpty(empleado.nss))
            {
                empleadoModificado.nss = empleado.nss.Trim().ToUpper();
            }
                
                empleadoModificado.fechaAltaImss = empleado.fechaAltaImss;

            if (!String.IsNullOrEmpty(empleado.apellidoMaterno))
            {
                empleadoModificado.apellidoMaterno = empleado.apellidoMaterno.Trim().ToUpper();
            }

            if (!String.IsNullOrEmpty(empleado.apellidoPaterno))
            {
                empleadoModificado.apellidoPaterno = empleado.apellidoPaterno.Trim().ToUpper();
            }

            if (!String.IsNullOrEmpty(empleado.nombre))
            {
                empleadoModificado.nombre = empleado.nombre.Trim().ToUpper();
            }

            if (!String.IsNullOrEmpty(empleado.rfc))
            {
                empleadoModificado.rfc = empleado.rfc.Trim().ToUpper();
            }

            if (!String.IsNullOrEmpty(empleado.homoclave))
            {
                empleadoModificado.homoclave = empleado.homoclave.Trim().ToUpper();
            }

            if (!String.IsNullOrEmpty(empleado.curp))
            {
                empleadoModificado.curp = empleado.curp.Trim().ToUpper();
            }

                empleadoModificado.sexoId = sexoId;

            if (!String.IsNullOrEmpty(empleado.categoria))
            {
                empleadoModificado.categoria = empleado.categoria.Trim().ToUpper();
            }

                //empleadoModificado.estadoCivilId = empleado.estadoCivilId;
                empleadoModificado.categoria = empleado.categoria;
                empleadoModificado.fechaNacimiento = empleado.fechaNacimiento;
                //empleadoModificado.nacionalidadId = paisId;
                //empleadoModificado.estadoNacimientoId = empleado.estadoNacimientoId;
                //empleadoModificado.municipioNacimientoId = empleado.municipioNacimientoId;
                empleadoModificado.email = empleado.email;

                if (!String.IsNullOrEmpty(empleado.observaciones))
                {
                    empleadoModificado.observaciones = empleado.observaciones.Trim().ToUpper();
                }

                if (!string.IsNullOrEmpty(empleado.Estado.descripcion))
                {
                    empleadoModificado.Estado.descripcion = empleado.Estado.descripcion.Trim().ToUpper();
                }

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

        public ActionResult GrabarEmpleadosExcel(String solicitudId)
        {

            if (!String.IsNullOrEmpty(solicitudId))
            {
                ToolsHelper th = new ToolsHelper();
                int solicitudAct = int.Parse(solicitudId);
                Solicitud solicitud = db.Solicituds.Find(solicitudAct);
                Usuario usuario = Session["UsuarioData"] as Usuario;
                DateTime date = DateTime.Now;

                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        ExcelHelper ex = new ExcelHelper();

                        String fileName = @"C:\SUA\Layouts\" + date.ToString("ddMMyyyyHHmmss") + "-" + file.FileName.Trim();
                        file.SaveAs(fileName.Trim());
                        LinqToExcelProvider provider = new LinqToExcelProvider(fileName.Trim());

                        provider.readExcel("Layout");

                        var query = (from row in provider.GetWorkSheet("Layout")
                                     let item = new PersonalExcelLayout
                                     {
                                         nombre = Convert.ToString(row.Field<Object>("NOMBRE")),
                                         apellidoMaterno = Convert.ToString(row.Field<Object>("AP_MATERNO")),
                                         apellidoPaterno = Convert.ToString(row.Field<Object>("AP_PATERNO")),
                                         RFC = Convert.ToString(row.Field<Object>("RFC")),
                                         homoclave = Convert.ToString(row.Field<Object>("HOMOCLAVE")),
                                         curp = Convert.ToString(row.Field<Object>("CURP")),
                                         nss = Convert.ToString(row.Field<Object>("NSS")),
                                         sexo = Convert.ToString(row.Field<Object>("SEXO")),
                                         fechaNacimiento = Convert.ToString(row.Field<Object>("FECHA_NACIMIENTO")),
                                         fechaAltaImss = Convert.ToString(row.Field<Object>("FECHA")),
                                         creditoInfonavit = Convert.ToString(row.Field<Object>("NUM_CRED_INFONAVIT")),
                                         pais = Convert.ToString(row.Field<Object>("PAIS")),
                                         estado = Convert.ToString(row.Field<Object>("ESTADO_NACIMIENTO")),
                                         municipio = Convert.ToString(row.Field<Object>("MUNICIPIO_NACIMIENTO")),
                                         categoria = Convert.ToString(row.Field<Object>("CATEGORIA")),
                                         calleNumero = Convert.ToString(row.Field<Object>("CALLE_NUMERO")),
                                         colonia = Convert.ToString(row.Field<Object>("COLONIA")),
                                         estadoMunicipio = Convert.ToString(row.Field<Object>("ESTADO_MUNICIPIO")),
                                         codioPostal = Convert.ToString(row.Field<Object>("CODIGO_POSTAL")),
                                         tramitarCuenta = Convert.ToString(row.Field<Object>("TRAMITAR_CUENTA")),
                                         banco = Convert.ToString(row.Field<Object>("BANCO")),
                                         cuentaBanco = Convert.ToString(row.Field<Object>("CUENTA_BANCARIA")),
                                         cuentaClabe = Convert.ToString(row.Field<Object>("CLABE_INTERBANCARIA")),
                                         salarioReal = Convert.ToString(row.Field<Object>("SALARIO_REAL")),
                                         email = Convert.ToString(row.Field<Object>("CORREO_ELECTRONICO")),
                                         observaciones = Convert.ToString(row.Field<Object>("OBSERVACIONES")),

                                     }
                                     select item).ToList();


                        Sexo sexo = new Sexo();
                        EstadoCivil estadoCivil = new EstadoCivil();
                        Pais pais = new Pais();
                        Estado estado = new Estado();
                        Municipio municipio = new Municipio();
                        Banco banco = new Banco();
                        Asegurado asegurado = new Asegurado();
                        Boolean founded = false;

                        foreach (PersonalExcelLayout empleadoL in query)
                        {
                            Empleado empleado = new Empleado();
                            founded = false;
                            
                            if (String.IsNullOrEmpty(empleadoL.nombre) && String.IsNullOrEmpty(empleadoL.apellidoPaterno))
                            {
                                break;
                            }

                            if (!String.IsNullOrEmpty(empleadoL.nss))
                            {
                                empleado.nss = empleadoL.nss.Trim();
                                Empleado empleadoAlterno = th.obtenerEmpleadoPorNSS(empleadoL.nss.Trim());

                                founded = th.verificarEmpleadoPorNSSyCliente(empleadoL.nss.Trim(), solicitud.clienteId);
                                }

                            if (!founded)
                            {

                                asegurado = th.obtenerAseguradoPorNSS(empleado.nss.Trim());

                                if (!(asegurado == null) && !String.IsNullOrEmpty(asegurado.nombre))
                                {
                                    empleado.aseguradoId = asegurado.id;
                                }

                            empleado.nombre = empleadoL.nombre.Trim();
                            empleado.apellidoMaterno = empleadoL.apellidoMaterno.Trim();
                                if (String.IsNullOrEmpty(empleadoL.apellidoMaterno))
                                {
                                empleadoL.apellidoMaterno = " ";
                            }

                            empleado.apellidoPaterno = empleadoL.apellidoPaterno.Trim();
                            empleado.nombreCompleto = empleadoL.nombre.Trim() + " " + empleadoL.apellidoPaterno.Trim() + " " + empleadoL.apellidoMaterno.Trim();

                            empleado.rfc = empleadoL.RFC.Trim();
                            empleado.homoclave = empleadoL.homoclave.Trim();


                            if (!String.IsNullOrEmpty(empleadoL.curp))
                            {
                                if (empleadoL.curp.Trim().Length > 17)
                                {
                                    empleado.curp = empleadoL.curp.Trim().Substring(0, 18);
                                }
                                else
                                {
                                    empleado.curp = empleadoL.curp.Trim();
                                }
                            }
                            if (solicitud.esquemaId != null)
                            {
                                empleado.esquemaPagoId = solicitud.esquemaId;
                            }
                            if (solicitud.sdiId != null)
                            {
                                empleado.sdiId = solicitud.sdiId;
                            }



                            if (!String.IsNullOrEmpty(empleadoL.sexo))
                            {
                                sexo = th.obtenerSexoPorDescripcion(empleadoL.sexo.Trim());
                                if (sexo.descripcion.Trim().Equals("Masculino"))
                                {
                                    empleado.foto = "~/Content/Images/male.png";
                                }
                                else
                                {
                                    empleado.foto = "~/Content/Images/female.png";
                                }
                            }
                            else
                            {
                                sexo = db.Sexos.Find(1);
                            }// el sexo no es null?

                            empleado.sexoId = sexo.id;

                            if (String.IsNullOrEmpty(empleadoL.salarioReal))
                            {
                                empleadoL.salarioReal = "0";
                            }//El salario real es null?

                            empleado.salarioReal = Decimal.Parse(empleadoL.salarioReal);
                            empleado.categoria = empleadoL.categoria.Trim();

                            if (!String.IsNullOrEmpty(empleadoL.fechaAltaImss))
                            {
                                empleado.fechaAltaImss = Convert.ToDateTime(empleadoL.fechaAltaImss.Trim());
                            }// Fecha alta Imms no es null?

                            if (!String.IsNullOrEmpty(empleadoL.fechaNacimiento))
                            {
                                empleado.fechaNacimiento = Convert.ToDateTime(empleadoL.fechaNacimiento.Trim());
                            } // Fecha de nacimiento no es null?

                            if (!String.IsNullOrEmpty(empleadoL.creditoInfonavit))
                            {
                                empleado.creditoInfonavit = empleadoL.creditoInfonavit.Trim();
                                empleado.tieneInfonavit = 1;
                            }
                            else
                            {
                                empleado.tieneInfonavit = 0;
                            }// Tiene infonavit el empleado ?

                            if (!String.IsNullOrEmpty(empleadoL.estadoCivil))
                            {
                                estadoCivil = th.obtenerEstadoCivilPorDescripcion(empleadoL.estadoCivil.Trim());
                            }
                            else
                            {
                                estadoCivil = db.EstadoCivils.Find(1);
                            }
                            empleado.estadoCivilId = estadoCivil.id;

                            if (!String.IsNullOrEmpty(empleadoL.pais))
                            {
                                pais = th.obtenerPaisPorDescripcion(empleadoL.pais.Trim());
                            }
                            else
                            {
                                pais = db.Paises.FirstOrDefault();
                            } //Pais de nacimiento es null?
                            empleado.nacionalidadId = pais.id;
                            if (!String.IsNullOrEmpty(empleadoL.estado))
                            {
                               estado = th.obtenerEstadoPorDescripcion(empleadoL.estado.Trim());
                            }
                                else
                                {
                                    estado = db.Estados.Find(1);
                                } // Estado de nacimiento no es null?
                                empleado.estadoNacimientoId = estado.id;

                            if (pais.descripcion.ToLower().Trim().Equals("mexico"))
                            {
                                if (!String.IsNullOrEmpty(empleadoL.municipio))
                                {
                                    municipio = th.obtenerMunicipioPorDescripcion(empleadoL.municipio.Trim());
                                }
                                else
                                {
                                    municipio = db.Municipios.Find(2);
                                } // municipio de nacimiento no es null?
                                empleado.municipioNacimientoId = municipio.id;
                            }

                            if (!String.IsNullOrEmpty(empleadoL.calleNumero))
                            {
                                empleado.calleNumero = empleadoL.calleNumero.Trim();
                            }
                            else
                            {
                                empleado.calleNumero = "No especificado";
                            } //calle y numero no son null?

                            if (!String.IsNullOrEmpty(empleadoL.colonia))
                            {
                                empleado.colonia = empleadoL.colonia.Trim();
                            }
                            else
                            {
                                empleado.colonia = "No especificado";
                            } // colonia no es null?

                            if (!String.IsNullOrEmpty(empleadoL.estadoMunicipio))
                            {
                                empleado.edoMunicipio = empleadoL.estadoMunicipio.Trim();
                            }
                            else
                            {
                                empleado.edoMunicipio = "No especificado";
                            } // Municipio no es null?

                            if (!String.IsNullOrEmpty(empleadoL.codioPostal))
                            {
                                empleado.codigoPostal = empleadoL.codioPostal.Trim();
                            }//codigo postal no es null?

                            if (!String.IsNullOrEmpty(empleadoL.cuentaBanco))
                            {
                                empleado.cuentaBancaria = empleadoL.cuentaBanco.Trim();
                            }//cuenta banco no es null?

                            if (!String.IsNullOrEmpty(empleadoL.cuentaClabe))
                            {
                                empleado.cuentaClabe = empleadoL.cuentaClabe.Trim();
                            } // cuenta clabe no es null?

                            if (!String.IsNullOrEmpty(empleadoL.email))
                            {
                                empleado.email = empleadoL.email.Trim();
                            }//email no es null?

                            if (!String.IsNullOrEmpty(empleadoL.tramitarCuenta))
                            {
                                if (empleadoL.tramitarCuenta.Equals("Si"))
                                {
                                    empleado.tramitarTarjeta = 1;
                                }
                                else
                                {
                                    empleado.tramitarTarjeta = 0;
                                }
                            }
                            else
                            {
                                empleado.tramitarTarjeta = 0;
                            }//tramitar cuenta no es null?

                            if (!String.IsNullOrEmpty(empleadoL.banco))
                            {
                                banco = th.obtenerBancoPorDescripcion(empleadoL.banco.Trim());
                            }
                            else
                            {
                                banco = db.Bancos.Find(1);
                            }// banco no es null?
                            empleado.bancoId = banco.id;

                            if (!String.IsNullOrEmpty(empleadoL.observaciones))
                            {
                                empleado.observaciones = empleadoL.observaciones.Trim();
                            } // observaciones no es null


                            empleado.usuarioId = usuario.Id;
                                //Ponemos en pendiente el empleado hasta que se 
                                //procese
                                empleado.estatus = "P";


                            try
                            {
                                if (!founded)
                                {
                                    empleado.fechaCreacion = DateTime.Now;
                                    db.Empleados.Add(empleado);
                                }
                                else
                                {
                                    empleado.fechaModificacion = DateTime.Now;
                                }


                                db.SaveChanges();
                                crearSolicitudEmpleado(empleado.id, solicitud.id, usuario.Id, "Alta");

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
                        }//Se encontro ya el nss y cliente?
                    }

                }
                return RedirectToAction("Index", "Solicitudes", new { clienteId = solicitud.clienteId, proyectoId = solicitud.proyectoId });
            }

            return RedirectToAction("CargarEmpleadosPorExcel", "Empleados", new { id = solicitudId });
        }

        //BAJA EMPLEADOS
        // GET: BajaEmpleados
        public ActionResult BajaEmpleados(int id, string clienteId)
        {

            List<Empleado> listEmpleados = new List<Empleado>();
            int clienteTempId = int.Parse(clienteId);
            Solicitud solicitud = db.Solicituds.Find(id);


            ViewBag.solicitudId = id;

            var empleadosIds = (from s in db.SolicitudEmpleadoes
                                        where s.estatus.Equals("A")
                                        select s.Empleado.id).ToList();


            List<Empleado> empleadosList = (from s in db.SolicitudEmpleadoes
                                            where s.Solicitud.clienteId.Equals(clienteTempId) 
                                            && !empleadosIds.Contains(s.Empleado.id)
                                            && s.Solicitud.plazaId.Equals(solicitud.plazaId)
                                            && s.Empleado.estatus.Equals("A")
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
        public ActionResult ModificarEmpleado(string solicitudId)
        {

            List<Empleado> listEmpleados = new List<Empleado>();

            if (!String.IsNullOrEmpty(solicitudId))
            {
                int solicitudIdTemp = int.Parse(solicitudId);
                Solicitud solicitud = db.Solicituds.Find(solicitudIdTemp);
                int proyectoId = solicitud.proyectoId;
                int clienteTempId = solicitud.clienteId;

                TempData["solicitudId"] = solicitudIdTemp;
                //ViewBag.solicitudId = solicitudIdTemp;

                //Filtramos solo empleados de solicitudes de alta
                List<Empleado> empleadosList = (from s in db.SolicitudEmpleadoes
                                                join e in db.Empleados on s.empleadoId equals e.id
                                                where s.Solicitud.clienteId.Equals(clienteTempId) //Empleados del mismo cliente
                                                && !s.Solicitud.id.Equals(solicitud.id)
                                                && e.estatus.Equals("A") && s.Solicitud.proyectoId.Equals(proyectoId) //Clientes del mismo proyecto
                                                && !e.EsquemasPago.descripcion.Equals("IAS")  //Esquema de Pago diferente a IAS
                                                && s.estatus.Equals("A")  //Solicitud Activa
                                                orderby s.Empleado.nombreCompleto
                                                select s.Empleado).ToList();

                foreach (Empleado emp in empleadosList)
                {
                    listEmpleados.Add(emp);

                }
                db.Entry(solicitud).State = EntityState.Modified;
                db.SaveChanges();
            }
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


        [HttpPost]
        public ActionResult asignarEmpleadoParaModificar(String[] ids, String solicitudId)
        {
            TempData["solicitudId"] = solicitudId;

            if (ids != null && !String.IsNullOrEmpty(solicitudId) )
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

                        empleado.sdiAlternativoId = solicitud.sdiId;
                        empleado.fechaModificacion = DateTime.Now;

                        //Solicitud para modificar el noTrabjadores
                        solicitud.noTrabajadores = solicitud.noTrabajadores + 1;

                        //Creamos el registro en solicitudEmpleados para agregar el empleado a otra solicitud activa
                        crearSolicitudEmpleado(empleado.id, solicitud.id, usuario.Id, "Baja");

                        db.Entry(solicitud).State = EntityState.Modified;
                        db.Entry(empleado).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }

        }
            //return RedirectToAction("BajaEmpleados", "Empleados", new { id = solicitud.id, clienteId = solicitud.clienteId });
            return RedirectToAction("SolicitudEmpleado", "SolicitudesModificacion", new { solicitudId = solicitudId });
        }

                
        public ActionResult ModificarSalario()
        {

            var empleadoId = Request["EmpleadoId"] as string;
            var sdiId = Request["Sdi"] as string;
            var solicitudId = Request["SolicitudId"] as string;
            TempData["solicitudId"] = solicitudId;
            Empleado empleado = new Empleado();

            if (!String.IsNullOrEmpty(empleadoId) && !String.IsNullOrEmpty(sdiId))
            {
                int empleadoTempId = int.Parse(empleadoId);
                int sdiTempId = int.Parse(sdiId);

                empleado = db.Empleados.Find(empleadoTempId);
                //Asignamos el esquema alternativo
                empleado.sdiAlternativoId = sdiTempId;
                empleado.fechaModificacion = DateTime.Now;

                db.Entry(empleado).State = EntityState.Modified;
                try
                {
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


            return Json(new { employee = empleado }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult SoloParaRefrescar(String solicitudId)
        {

            var sol = TempData["solicitudId"];
            int solId = int.Parse(sol.ToString());
            return RedirectToAction("ModificarEmpleado", "Empleados", new { solicitudId = solId });
        }

        public ActionResult extraExcel()
        {

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

        public ActionResult desasignarDeSolicitudEmpleado(String empleadoId, String solicitudId, String sourceController) {

            if (!String.IsNullOrEmpty(empleadoId) && !String.IsNullOrEmpty(solicitudId))
            {

                int solicitudInt = int.Parse(solicitudId);
                int empleadoInt = int.Parse(empleadoId);

                SolicitudEmpleado solicitudEmpleado = new SolicitudEmpleado();

                //Regresamos el salario diario alternativo al original
                Empleado empleado = db.Empleados.Where(e => e.id.Equals(empleadoInt)).FirstOrDefault();
                empleado.sdiAlternativoId = empleado.sdiId;

                db.Entry(empleado).State = EntityState.Modified;

                //Rompemos el link entre la solicitud y el empleado
                solicitudEmpleado = db.SolicitudEmpleadoes.Where(s => s.empleadoId.Equals(empleadoInt) &&
                        s.solicitudId.Equals(solicitudInt)).FirstOrDefault();

                solicitudEmpleado.estatus = "B";
                db.Entry(solicitudEmpleado).State = EntityState.Modified;

                //Le reducimos un trabajador a la solicitud
                Solicitud solicitud = db.Solicituds.Where(so => so.id.Equals(solicitudInt)).FirstOrDefault();
                solicitud.noTrabajadores = solicitud.noTrabajadores - 1;

                //Salvamos los cambios
                db.SaveChanges();
                
            }

            return RedirectToAction("SolicitudEmpleado", sourceController.Trim(), new {solicitudId = solicitudId });
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
