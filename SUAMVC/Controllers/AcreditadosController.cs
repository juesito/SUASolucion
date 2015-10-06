using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;
using System.Text;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using PagedList;
using System.IO;
using System.Web.Helpers;
using SUAMVC.Code52.i18n;
using System.Text.RegularExpressions;
using SUAMVC.Helpers;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;

namespace SUAMVC.Controllers
{
    public class AcreditadosController : Controller
    {
        private suaEntities db = new suaEntities();

        private void setVariables(String plazasId, String patronesId, String clientesId,
            String gruposId, String opcion, String valor, String statusId, String numeroPagina)
        {
            if (!String.IsNullOrEmpty(plazasId))
            {
                ViewBag.pzaId = plazasId;
            }
            if (!String.IsNullOrEmpty(patronesId))
            {
                ViewBag.patId = patronesId;
            }
            if (!String.IsNullOrEmpty(clientesId))
            {
                ViewBag.cteId = clientesId;
            }
            if (!String.IsNullOrEmpty(gruposId))
            {
                ViewBag.gpoId = gruposId;
            }
            if (!String.IsNullOrEmpty(opcion))
            {
                ViewBag.opBuscador = opcion;
            }
            if (!String.IsNullOrEmpty(valor))
            {
                ViewBag.valBuscador = valor;
            }
            if (statusId != null)
            {
                ViewBag.statusId = statusId;
            }
            if (!String.IsNullOrEmpty(numeroPagina))
            {
                ViewData["numeroPagina"] = numeroPagina;
            }

        }
        // GET: Acreditados
        public ActionResult Index(String plazasId, String patronesId, String clientesId, 
            String gruposId, String currentPlaza, String currentPatron, String currentCliente,
            String currentGrupo, String opcion, String valor, String statusId, String numeroPagina, int page = 1)
        {

            Usuario user = Session["UsuarioData"] as Usuario;

            setVariables(plazasId, patronesId, clientesId, gruposId, opcion, valor, statusId, numeroPagina);

            var plazasAsignadas = (from x in db.TopicosUsuarios
                                   where x.usuarioId.Equals(user.Id)
                                   && x.tipo.Equals("P")
                                   select x.topicoId);

            var clientesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("C")
                                     select x.topicoId);

            var patronesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("B")
                                     select x.topicoId);
            
            ViewBag.plazasId = new SelectList((from s in db.Plazas.ToList()
                                               join top in db.TopicosUsuarios on s.id equals top.topicoId
                                               where top.tipo.Trim().Equals("P") && top.usuarioId.Equals(user.Id)
                                               orderby s.descripcion
                                               select new
                                               {
                                                   id = s.id,
                                                   FUllName = s.descripcion
                                               }).Distinct(), "id", "FullName");

            ViewBag.patronesId = new SelectList((from s in db.Patrones.ToList()
                                                 join top in db.TopicosUsuarios on s.Id equals top.topicoId
                                                 where top.tipo.Trim().Equals("B") && top.usuarioId.Equals(user.Id)
                                                 orderby s.registro
                                                 select new
                                                 {
                                                     id = s.Id,
                                                     FullName = s.registro + " - " + s.nombre
                                                 }).Distinct(), "id", "FullName", null);
            
            ViewBag.clientesId = new SelectList((from s in db.Clientes.ToList()
                                                 join top in db.TopicosUsuarios on s.Id equals top.topicoId
                                                 where top.tipo.Trim().Equals("C") && top.usuarioId.Equals(user.Id)
                                                 orderby s.descripcion
                                                 select new
                                                 {
                                                     id = s.Id,
                                                     FUllName = s.claveCliente + " - " + s.descripcion
                                                 }).Distinct(), "id", "FullName");
         
            ViewBag.gruposId = new SelectList((from s in db.Grupos.ToList()
                                               join cli in db.Clientes on s.Id equals cli.Grupo_id
                                               join top in db.TopicosUsuarios on cli.Id equals top.topicoId
                                               where top.tipo.Trim().Equals("C") && top.usuarioId.Equals(user.Id)
                                               orderby s.claveGrupo
                                               select new
                                               {
                                                   id = s.Id,
                                                   FUllName = s.claveGrupo + " - " + s.nombreCorto
                                               }).Distinct(), "id", "FullName");


            var acreditados = from s in db.Acreditados
                              join cli in db.Clientes on s.clienteId equals cli.Id
                              where plazasAsignadas.Contains(s.Cliente.Plaza_id) &&
                                    clientesAsignados.Contains(s.Cliente.Id) &&
                                    patronesAsignados.Contains(s.PatroneId)
                              orderby s.nombreCompleto
                             select s;

            if (!String.IsNullOrEmpty(plazasId))
            {
                int idPlaza = int.Parse(plazasId.Trim());
                acreditados = acreditados.Where(s => s.Cliente.Plaza_id.Equals(idPlaza));
            }
            if (!String.IsNullOrEmpty(patronesId))
            {
                int idPatron = int.Parse(patronesId.Trim());
                acreditados = acreditados.Where(s => s.PatroneId.Equals(idPatron));
            }

            if (!String.IsNullOrEmpty(clientesId))
            {
                int idCliente = int.Parse(clientesId.Trim());
                acreditados = acreditados.Where(s => s.Cliente.Id.Equals(idCliente));
            }

            if (!String.IsNullOrEmpty(gruposId))
            {
                int idGrupo = int.Parse(gruposId.Trim());
                acreditados = acreditados.Where(s => s.Cliente.Grupo_id.Equals(idGrupo));
            }

            if (!String.IsNullOrEmpty(opcion))
            {
                //TempData["buscador"] = "0";
                switch (opcion)
                {
                    case "1":
                        acreditados = acreditados.Where(s => s.Patrone.registro.Contains(valor));
                        break;
                    case "2":
                        acreditados = acreditados.Where(s => s.numeroAfiliacion.Contains(valor));
                        break;
                    case "3":
                        acreditados = acreditados.Where(s => s.CURP.Contains(valor));
                        break;
                    case "4":
                        acreditados = acreditados.Where(s => s.RFC.Contains(valor));
                        break;
                    case "5":
                        acreditados = acreditados.Where(s => s.nombreCompleto.Contains(valor));
                        break;
                    case "6":
                        acreditados = acreditados.Where(s => s.fechaAlta.ToString().Contains(valor));
                        break;
                    case "7":
                        acreditados = acreditados.Where(s => s.fechaBaja.ToString().Contains(valor));
                        break;
                    case "8":
                        acreditados = acreditados.Where(s => s.sdi.ToString().Contains(valor));
                        break;
                    case "9":
                        acreditados = acreditados.Where(s => s.Cliente.claveCliente.Contains(valor));
                        break;
                    case "10":
                        acreditados = acreditados.Where(s => s.Cliente.Grupos.nombre.Contains(valor));
                        break;
                    case "11":
                        acreditados = acreditados.Where(s => s.ocupacion.Contains(valor));
                        break;
                    case "12":
                        acreditados = acreditados.Where(s => s.Cliente.Plaza.cveCorta.Contains(valor));
                        break;
                }
            }

            if (statusId != null)
            {
                @ViewBag.statusId = statusId;

                if (statusId.Trim().Equals("A"))
                {
                    acreditados = acreditados.Where(s => !s.fechaBaja.HasValue);
                }
                else if (statusId.Trim().Equals("B"))
                {
                    acreditados = acreditados.Where(s => s.fechaBaja.HasValue);
                }
            }

            ViewBag.activos = acreditados.Where(s => !s.fechaBaja.HasValue).Count();
            ViewBag.registros = acreditados.Count();

            //var acreditados2 = acreditados.OrderBy(s => s.nombreCompleto).Take(12).ToList();
            //if (numeroPagina != null)
            //{
            //    ViewData["numeroPagina"] = numeroPagina;
            //    int numeroPag = int.Parse(numeroPagina.Trim());
            //    if (numeroPag != 0)
            //    {
            //        acreditados2 = acreditados.OrderBy(s => s.nombreCompleto).Skip(((numeroPag - 1) * 12)).Take(12).ToList();
            //    }
            //}
            //else
            //{
            //    ViewData["numeroPagina"] = 1;
            //}

            return View(acreditados.ToList());
        }

        public ActionResult UploadFile(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acreditado acreditado = db.Acreditados.Find(id);
            if (acreditado == null)
            {
                return HttpNotFound();
            }

            return View(acreditado);
        }

        [HttpPost]
        public ActionResult UploadPDFFile([Bind(Include = "id")] Acreditado acreditado, String answer)
        {
            if (acreditado.id > 0)
            {
                acreditado = db.Acreditados.Find(acreditado.id);
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        String path = "C:\\SUA\\Acreditados\\" + acreditado.numeroAfiliacion.Trim() + "\\" + answer + "\\"; //Path.Combine("C:\\SUA\\", uploadModel.subFolder);
                        if (!System.IO.File.Exists(path))
                        {
                            System.IO.Directory.CreateDirectory(path);
                        }

                        //var fileName = Path.GetFileName(file.FileName);
                        var fileName = answer + "-" + acreditado.numeroAfiliacion.Trim() + ".pdf";
                        var pathFinal = Path.Combine(path, fileName);
                        file.SaveAs(pathFinal);
                        //Move();

                        ViewBag.dbUploaded = true;

                        //Validamos la acción realizada
                        if (answer.Equals("Alta"))
                        {
                            acreditado.alta = "S";
                        }
                        else if (answer.Equals("Baja"))
                        {
                            acreditado.baja = "S";
                        }
                        else if (answer.Equals("Modificacion"))
                        {
                            acreditado.modificacion = "S";
                        }
                        else
                        {
                            acreditado.permanente = "S";
                        }

                        db.Acreditados.Add(acreditado);
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult ViewAttachment(int id, String option, String carga)
        {
            if (carga != null)
            {
                Acreditado acreditado = db.Acreditados.Find(id);
                var movtosTemp = db.Movimientos.Where(x => x.acreditadoId == id
                                 && x.tipo.Equals(option)).OrderByDescending(x => x.fechaTransaccion).ToList(); 

                Movimiento movto = new Movimiento();
                if (movtosTemp != null)
                {
                    foreach (var movtosItem in movtosTemp)
                    {
                        movto = movtosItem;
                        break;
                    }//Definimos los valores para la plaza
                }

                var fileName = "C:\\SUA\\Acreditados\\" + acreditado.numeroAfiliacion + "\\" + option + "\\" + movto.nombreArchivo.Trim();

                if (System.IO.File.Exists(fileName))
                {
                    FileStream fs = new FileStream(fileName, FileMode.Open);

                    return File(fs, "application/pdf");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Acreditados/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acreditado acreditado = db.Acreditados.Find(id);
            if (acreditado == null)
            {
                return HttpNotFound();
            }
            return View(acreditado);
        }

        // GET: Acreditados/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Acreditados/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "registroPatronal,apellidoPaterno,apellidoMaterno,nombre,nombreCompleto,CURP,RFC,ubicacion,ocupacion,idGrupo,numeroAfiliacion,numeroCredito,fechaAlta,fechaBaja,fechaInicioDescuento,fechaFinDescuento,smdv,sdi,sd,vsm,porcentaje,cuotaFija,descuentoBimestral,descuentoMensual,descuentoSemanal,descuentoCatorcenal,descuentoQuincenal,descuentoVeintiochonal,descuentoDiario,idPlaza,acuseRetencion")] Acreditado acreditado)
        {
            if (ModelState.IsValid)
            {
                db.Acreditados.Add(acreditado);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(acreditado);
        }

        // GET: Acreditados/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acreditado acreditado = db.Acreditados.Find(id);
            if (acreditado == null)
            {
                return HttpNotFound();
            }
            return View(acreditado);
        }

        // POST: Acreditados/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "registroPatronal,apellidoPaterno,apellidoMaterno,nombre,nombreCompleto,CURP,RFC,ubicacion,ocupacion,idGrupo,numeroAfiliacion,numeroCredito,fechaAlta,fechaBaja,fechaInicioDescuento,fechaFinDescuento,smdv,sdi,sd,vsm,porcentaje,cuotaFija,descuentoBimestral,descuentoMensual,descuentoSemanal,descuentoCatorcenal,descuentoQuincenal,descuentoVeintiochonal,descuentoDiario,idPlaza,acuseRetencion")] Acreditado acreditado)
        {
            if (ModelState.IsValid)
            {
                db.Entry(acreditado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(acreditado);
        }

        // GET: Acreditados/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acreditado acreditado = db.Acreditados.Find(id);
            if (acreditado == null)
            {
                return HttpNotFound();
            }
            return View(acreditado);
        }

        // POST: Acreditados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Acreditado acreditado = db.Acreditados.Find(id);
            db.Acreditados.Remove(acreditado);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UploadData()
        {
            ViewBag.patronesId = new SelectList(db.Patrones, "registro", "nombre");
            ViewBag.clientesId = new SelectList(db.Clientes, "id", "descripcion");
            return View();
        }

        [HttpGet]
        public void GetExcel(String plazasId, String patronesId, String clientesId, 
            String gruposId, String opcion, String valor, String statusId)
        {

            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {
                Usuario user = Session["UsuarioData"] as Usuario;
            var plazasAsignadas = (from x in db.TopicosUsuarios
                                   where x.usuarioId.Equals(user.Id)
                                   && x.tipo.Equals("P")
                                   select x.topicoId);

            var clientesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("C")
                                     select x.topicoId);

            var patronesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("B")
                                     select x.topicoId);

            List<Acreditado> allCust = new List<Acreditado>();

            var acreditados = from s in db.Acreditados
                              join cli in db.Clientes on s.clienteId equals cli.Id
                             where plazasAsignadas.Contains(s.Cliente.Plaza_id) &&
                                   clientesAsignados.Contains(s.Cliente.Id) &&
                                   patronesAsignados.Contains(s.PatroneId)
                             select s;

            if (!String.IsNullOrEmpty(plazasId))
            {
                @ViewBag.pzaId = plazasId;
                int idPlaza = int.Parse(plazasId.Trim());
                acreditados = acreditados.Where(s => s.Cliente.Plaza_id.Equals(idPlaza));
            }
            if (!String.IsNullOrEmpty(patronesId))
            {
                @ViewBag.patId = patronesId;
                int idPatron = int.Parse(patronesId.Trim());
                acreditados = acreditados.Where(s => s.PatroneId.Equals(idPatron));
            }

            if (!String.IsNullOrEmpty(clientesId))
            {
                @ViewBag.cteId = clientesId;
                int idCliente = int.Parse(clientesId.Trim());
                acreditados = acreditados.Where(s => s.Cliente.Id.Equals(idCliente));
            }

            if (!String.IsNullOrEmpty(gruposId))
            {
                @ViewBag.gpoId = gruposId;
                int idGrupo = int.Parse(gruposId.Trim());
                acreditados = acreditados.Where(s => s.Cliente.Grupo_id.Equals(idGrupo));
            }

            if (!String.IsNullOrEmpty(opcion))
            {
                @ViewBag.opBuscador = opcion;
                @ViewBag.valBuscador = valor;
                TempData["buscador"] = "0";
                switch (opcion)
                {
                    case "1":
                        acreditados = acreditados.Where(s => s.Patrone.registro.Contains(valor));
                        break;
                    case "2":
                        acreditados = acreditados.Where(s => s.numeroAfiliacion.Contains(valor));
                        break;
                    case "3":
                        acreditados = acreditados.Where(s => s.CURP.Contains(valor));
                        break;
                    case "4":
                        acreditados = acreditados.Where(s => s.RFC.Contains(valor));
                        break;
                    case "5":
                        acreditados = acreditados.Where(s => s.nombreCompleto.Contains(valor));
                        break;
                    case "6":
                        acreditados = acreditados.Where(s => s.fechaAlta.ToString().Contains(valor));
                        break;
                    case "7":
                        acreditados = acreditados.Where(s => s.fechaBaja.ToString().Contains(valor));
                        break;
                    case "8":
                        acreditados = acreditados.Where(s => s.sdi.ToString().Contains(valor));
                        break;
                    case "9":
                        acreditados = acreditados.Where(s => s.Cliente.claveCliente.Contains(valor));
                        break;
                    case "10":
                        acreditados = acreditados.Where(s => s.Cliente.Grupos.nombre.Contains(valor));
                        break;
                    case "11":
                        acreditados = acreditados.Where(s => s.ocupacion.Contains(valor));
                        break;
                    case "12":
                        acreditados = acreditados.Where(s => s.Cliente.Plaza.cveCorta.Contains(valor));
                        break;
                }
            }

            if (statusId != null)
            {
                @ViewBag.statusId = statusId;

                if (statusId.Trim().Equals("A"))
                {
                    ViewBag.statusId = statusId;
                    acreditados = acreditados.Where(s => !s.fechaBaja.HasValue);
                }
                else if (statusId.Trim().Equals("B"))
                {
                    ViewBag.statusId = statusId;
                    acreditados = acreditados.Where(s => s.fechaBaja.HasValue);
                }
            }

            allCust = acreditados.ToList();


                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"Acreditados-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
                String fullName = path + fileName;

                ExcelHelper eh = new ExcelHelper();
                //Creamos el objeto del workbook
                SpreadsheetDocument xl = SpreadsheetDocument.Create(fullName, SpreadsheetDocumentType.Workbook);

                WorkbookPart wbp = xl.AddWorkbookPart();
                WorksheetPart wsp = wbp.AddNewPart<WorksheetPart>();
                Workbook wb = new Workbook();
                FileVersion fv = new FileVersion();
                fv.ApplicationName = "Microsoft Office Excel";

                Worksheet ws = new Worksheet();
                WorkbookStylesPart wbsp = wbp.AddNewPart<WorkbookStylesPart>();
                // add styles to sheet
                wbsp.Stylesheet = eh.CreateStylesheet();
                wbsp.Stylesheet.Save();

                SheetData sd = crearContenidoHoja(allCust, eh);//CreateContentRow(); 
                ws.Append(sd);
                wsp.Worksheet = ws;
                wsp.Worksheet.Save();

                Sheets sheets = new Sheets();
                Sheet sheet = new Sheet();
                sheet.Name = "Sheet1";
                sheet.SheetId = 1;
                sheet.Id = wbp.GetIdOfPart(wsp);

                sheets.Append(sheet);
                wb.Append(fv);
                wb.Append(sheets);

                xl.WorkbookPart.Workbook = wb;
                xl.WorkbookPart.Workbook.Save();
                xl.Close();

                fileStream = new FileStream(fullName, FileMode.Open);
                fileStream.Position = 0;
                mem = new MemoryStream();
                fileStream.CopyTo(mem);

                mem.Position = 0;
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                ToolsHelper th = new ToolsHelper();
                Response.ContentType = th.getMimeType(fullName);
                Response.BinaryWrite(mem.ToArray());

                Response.End();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Flush();
                    fileStream.Close();
                }
                mem.Flush();
                mem.Close();
            }

        }


        string[] headerColumns = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG" };
        public SheetData crearContenidoHoja(List<Acreditado> acreditados, ExcelHelper eh)
        {
            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();
            row = eh.addNewCellToRow(index, row, "Registro", headerColumns[0] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Número Afiliación", headerColumns[1] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "CURP", headerColumns[2] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "RFC", headerColumns[3] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Apellido Paterno", headerColumns[4] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Apellido Materno", headerColumns[5] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Nombre", headerColumns[6] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Nombre Completo", headerColumns[7] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Fecha Alta", headerColumns[8] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Fecha Baja", headerColumns[9] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Ubicación", headerColumns[10] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Grupo", headerColumns[11] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Ocupación", headerColumns[12] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Plaza", headerColumns[13] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Número de Crédito", headerColumns[14] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Inicio descuento", headerColumns[15] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Fin descuento", headerColumns[16] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SMDV", headerColumns[17] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SDI", headerColumns[18] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "SD", headerColumns[19] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "VSM", headerColumns[20] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Porcentaje", headerColumns[21] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Cuota Fija", headerColumns[22] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Descuento Bimestral", headerColumns[23] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Decuento Mensual", headerColumns[24] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Descuento Veintiochonal", headerColumns[25] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Descuento Quincenal", headerColumns[26] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Descuento Catorcenal", headerColumns[27] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Descuento Semanal", headerColumns[28] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Descuento Diario", headerColumns[29] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            index++;
            //Create the cells that contain the data.
            foreach (Acreditado dp in acreditados)
            {
                int i = 0;

                row = eh.addNewCellToRow(index, row, dp.Patrone.registro, headerColumns[i] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.numeroAfiliacion, headerColumns[i + 1] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.CURP, headerColumns[i + 2] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.RFC, headerColumns[i + 3] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.apellidoPaterno, headerColumns[i + 4] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.apellidoMaterno, headerColumns[i + 5] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.nombre, headerColumns[i + 6] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.nombreCompleto, headerColumns[i + 7] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                String var1 = String.Format("{0:dd/MM/yyyy}", dp.fechaAlta);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 8] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:dd/MM/yyyy}", dp.fechaBaja);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 9] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Cliente.claveCliente, headerColumns[i + 10] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Cliente.Grupos.nombreCorto, headerColumns[i + 11] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.ocupacion, headerColumns[i + 12] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Cliente.Plaza.cveCorta, headerColumns[i + 13] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.numeroCredito, headerColumns[i + 14] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:dd/MM/yyyy}", dp.fechaInicioDescuento);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 15] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:dd/MM/yyyy}", dp.fechaFinDescuento);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 16] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.smdv);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 17] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.sdi);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 18] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.sd);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 19] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.0000}", dp.vsm);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 20] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,##0}", dp.porcentaje);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 21] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.cuotaFija);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 22] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.descuentoBimestral);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 23] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.descuentoMensual);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 24] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.descuentoVeintiochonal);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 25] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.descuentoQuincenal);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 26] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.descuentoCatorcenal);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 27] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.descuentoSemanal);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 28] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.descuentoDiario);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 29] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                index++;
            }

            return sheetData;
        }

        public ActionResult ActivaVariable(String buscador, String plazasId, String patronesId, String clientesId,
            String gruposId, String opcion, String valor, String statusId)
        {
            if (buscador != null)
            {
                if (!buscador.Equals("1"))
                {
                    TempData["buscador"] = "1";
                }
                else
                {
                    TempData["buscador"] = "0";
                }
            }
            else {
                TempData["buscador"] = "1";
            }
            return RedirectToAction("Index", new { plazasId, patronesId, clientesId, gruposId, opcion, valor, statusId });
        }

        [HttpGet]
        public ActionResult Avanza(String plazasId, String patronesId, String clientesId,
            String gruposId, String opcion, String valor, String statusId, String numeroPagina)
        {
            int numeroPag = int.Parse(numeroPagina.Trim());
            numeroPag = numeroPag + 1;
            numeroPagina = numeroPag.ToString();
            ViewData["numeroPagina"] = numeroPagina;
            return RedirectToAction("Index", new { plazasId, patronesId, clientesId, gruposId, opcion, valor, statusId, numeroPagina });
        }


        [HttpGet]
        public ActionResult Retrocede(String plazasId, String patronesId, String clientesId,
            String gruposId, String opcion, String valor, String statusId, String numeroPagina)
        {
            int numeroPag = int.Parse(numeroPagina.Trim());
            if (numeroPag != 1)
            {
                numeroPag = numeroPag - 1;
                numeroPagina = numeroPag.ToString();
                ViewData["numeroPagina"] = numeroPagina;
            }
            return RedirectToAction("Index", new { plazasId, patronesId, clientesId, gruposId, opcion, valor, statusId, numeroPagina });
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
