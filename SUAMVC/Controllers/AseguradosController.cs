using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;
using System.Data.OleDb;
using System.Data.Entity.Validation;
using System.Text;
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
    public class AseguradosController : Controller
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

        // GET: Aseguradoes
        public ActionResult Index(String plazasId, String patronesId, String clientesId,
            String gruposId, String currentPlaza, String currentPatron, String currentCliente,
            String currentGrupo, String opcion, String valor, String statusId, String numeroPagina, int page = 1, String sortOrder = null,
            String lastSortOrder = null)
        {


            Usuario user = Session["UsuarioData"] as Usuario;

            setVariables(plazasId, patronesId, clientesId, gruposId, opcion, valor, statusId, numeroPagina);

            var plazasAsignadas = (from x in db.TopicosUsuarios
                                   where x.usuarioId.Equals(user.Id)
                                   && x.tipo.Equals("P")
                                   select x.topicoId);

            var patronesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("B")
                                     select x.topicoId);

            var clientesAsignados = (from x in db.TopicosUsuarios
                                     where x.usuarioId.Equals(user.Id)
                                     && x.tipo.Equals("C")
                                     select x.topicoId);

            var gruposAsignados = (from s in db.Grupos
                                   join cli in db.Clientes on s.Id equals cli.Grupo_id
                                   join top in db.TopicosUsuarios on cli.Id equals top.topicoId
                                   where top.tipo.Trim().Equals("C") && top.usuarioId.Equals(user.Id)
                                   orderby s.claveGrupo
                                   select s.Id);

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

            //Query principal
            var asegurados = from s in db.Asegurados
                             join cli in db.Clientes on s.ClienteId equals cli.Id
                             where plazasAsignadas.Contains(s.Cliente.Plaza_id) &&
                                   clientesAsignados.Contains(s.Cliente.Id) &&
                                   patronesAsignados.Contains(s.PatroneId) &&
                                   gruposAsignados.Contains(s.Cliente.Grupo_id)
                             orderby s.nombreTemporal
                             select s;

            //Comenzamos los filtros
            if (!String.IsNullOrEmpty(plazasId))
            {
                @ViewBag.pzaId = plazasId;
                int idPlaza = int.Parse(plazasId.Trim());
                asegurados = asegurados.Where(s => s.Cliente.Plaza_id.Equals(idPlaza));
            }
            if (!String.IsNullOrEmpty(patronesId))
            {
                @ViewBag.patId = patronesId;
                int idPatron = int.Parse(patronesId.Trim());
                asegurados = asegurados.Where(s => s.PatroneId.Equals(idPatron));
            }

            if (!String.IsNullOrEmpty(clientesId))
            {
                @ViewBag.cteId = clientesId;
                int idCliente = int.Parse(clientesId.Trim());
                asegurados = asegurados.Where(s => s.Cliente.Id.Equals(idCliente));
            }

            if (!String.IsNullOrEmpty(gruposId))
            {
                @ViewBag.gpoId = gruposId;
                int idGrupo = int.Parse(gruposId.Trim());
                asegurados = asegurados.Where(s => s.Cliente.Grupo_id.Equals(idGrupo));
            }

            if (!String.IsNullOrEmpty(opcion))
            {
                switch (opcion)
                {
                    case "1":
                        asegurados = asegurados.Where(s => s.Patrone.registro.Contains(valor));
                        break;
                    case "2":
                        ViewData["numeroPagina"] = "1";
                        asegurados = asegurados.Where(s => s.numeroAfiliacion.Contains(valor));
                        break;
                    case "3":
                        asegurados = asegurados.Where(s => s.CURP.Contains(valor));
                        break;
                    case "4":
                        asegurados = asegurados.Where(s => s.RFC.Contains(valor));
                        break;
                    case "5":
                        asegurados = asegurados.Where(s => s.nombreTemporal.Contains(valor));
                        break;
                    case "6":
                        DateTime valor2 = DateTime.Parse(valor);
                        asegurados = asegurados.Where(s => s.fechaAlta.Year == valor2.Year && s.fechaAlta.Month == valor2.Month && s.fechaAlta.Day == valor2.Day);
                        break;
                    case "7":
                        valor2 = DateTime.Parse(valor);
                        asegurados = asegurados.Where(s => s.fechaBaja.Value.Year == valor2.Year && s.fechaBaja.Value.Month == valor2.Month && s.fechaBaja.Value.Day == valor2.Day);
                        break;
                    case "8":
                        asegurados = asegurados.Where(s => s.salarioImss.ToString().Contains(valor.Trim()));
                        break;
                    case "9":
                        asegurados = asegurados.Where(s => s.Cliente.claveCliente.Contains(valor));
                        break;
                    case "10":
                        asegurados = asegurados.Where(s => s.Cliente.Grupos.nombre.Contains(valor));
                        break;
                    case "11":
                        asegurados = asegurados.Where(s => s.ocupacion.Contains(valor));
                        break;
                    case "12":
                        asegurados = asegurados.Where(s => s.Cliente.Plaza.cveCorta.Contains(valor));
                        break;
                    case "13":
                        asegurados = asegurados.Where(s => s.extranjero.Contains(valor));
                        break;
                }
            }

            if (statusId != null)
            {
                @ViewBag.statusId = statusId;

                if (statusId.Trim().Equals("A"))
                {
                    ViewBag.statusId = statusId;
                    asegurados = asegurados.Where(s => !s.fechaBaja.HasValue);
                }
                else if (statusId.Trim().Equals("B"))
                {
                    ViewBag.statusId = statusId;
                    asegurados = asegurados.Where(s => s.fechaBaja.HasValue);
                }
            }

            ViewBag.activos = asegurados.Where(s => !s.fechaBaja.HasValue).Count();
            ViewBag.registros = asegurados.Count();

            //           asegurados = asegurados.OrderBy(s => s.nombreTemporal);
            //var asegurados2 = asegurados.OrderBy(s => s.nombreTemporal).Take(12).ToList();
            //if (numeroPagina != null)
            //{
            //    numeroPagina = ViewData["numeroPagina"].ToString();
            //    int numeroPag = int.Parse(numeroPagina.Trim());
            //    if (numeroPag != 0)
            //    {
            //        asegurados2 = asegurados.OrderBy(s => s.nombreTemporal).Skip(((numeroPag - 1) * 12)).Take(12).ToList();
            //    }
            //}else{
            //    ViewData["numeroPagina"] = 1;
            //}

            return View(asegurados.ToList());
        }

        // GET: Aseguradoes/Details/5
        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asegurado asegurado = db.Asegurados.Find(id);
            if (asegurado == null)
            {
                return HttpNotFound();
            }

            TempData["idAsegurado"] = id;
            return View(asegurado);
        }

        public ActionResult ViewAttachment(int id, String option, String carga)
        {
            if (carga != null)
            {
                Asegurado asegurado = db.Asegurados.Find(id);
                var movtosTemp = db.Movimientos.Where(x => x.aseguradoId == id
                                 && x.tipo.Equals(option)).OrderByDescending(x => x.fechaTransaccion).ToList();

                Movimiento movto = new Movimiento();
                if (movtosTemp != null && movtosTemp.Count > 0)
                {
                    foreach (var movtosItem in movtosTemp)
                    {
                        movto = movtosItem;
                        break;
                    }//Definimos los valores para la plaza

                    var fileName = "C:\\SUA\\Asegurados\\" + asegurado.numeroAfiliacion + "\\" + option + "\\" + movto.nombreArchivo.Trim();

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
            else
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Aseguradoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Aseguradoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "registroaseguradoal,numeroAfiliacion,CURP,RFC,nombre,salarioImss,salarioInfo,fechaAlta,fechaBaja,tipoTrabajo,semanaJornada,paginaInfo,tipoDescuento,valorDescuento,claveUbicacion,nombreTemporal,fechaDescuento,finDescuento,articulo33,salarioArticulo33,trapeniv,estado,claveMunicipio")] Asegurado asegurado)
        {
            if (ModelState.IsValid)
            {
                db.Asegurados.Add(asegurado);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(asegurado);
        }

        // GET: Aseguradoes/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asegurado asegurado = db.Asegurados.Find(id);
            if (asegurado == null)
            {
                return HttpNotFound();
            }
            return View(asegurado);
        }

        // POST: Aseguradoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "registroaseguradoal,numeroAfiliacion,CURP,RFC,nombre,salarioImss,salarioInfo,fechaAlta,fechaBaja,tipoTrabajo,semanaJornada,paginaInfo,tipoDescuento,valorDescuento,claveUbicacion,nombreTemporal,fechaDescuento,finDescuento,articulo33,salarioArticulo33,trapeniv,estado,claveMunicipio")] Asegurado asegurado)
        {
            if (ModelState.IsValid)
            {
                db.Entry(asegurado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(asegurado);
        }

        // GET: Aseguradoes/Delete/5
        public ActionResult DeleteMov(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovimientosAsegurado asegurado = db.MovimientosAseguradoes.Find(id);
            if (asegurado == null)
            {
                return HttpNotFound();
            }
            return View(asegurado);
        }

        // POST: Aseguradoes/Delete/5
        [HttpPost, ActionName("DeleteMov")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MovimientosAsegurado asegurado = db.MovimientosAseguradoes.Find(id);
            db.MovimientosAseguradoes.Remove(asegurado);
            db.SaveChanges();
            return RedirectToAction("Index");
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

                List<Asegurado> allCust = new List<Asegurado>();

                var asegurados = from s in db.Asegurados
                                 join cli in db.Clientes on s.ClienteId equals cli.Id
                                 where plazasAsignadas.Contains(s.Cliente.Plaza_id) &&
                                       clientesAsignados.Contains(s.Cliente.Id) &&
                                       patronesAsignados.Contains(s.PatroneId)
                                 select s;

                if (!String.IsNullOrEmpty(plazasId))
                {
                    @ViewBag.pzaId = plazasId;
                    int idPlaza = int.Parse(plazasId.Trim());
                    asegurados = asegurados.Where(s => s.Cliente.Plaza_id.Equals(idPlaza));
                }
                if (!String.IsNullOrEmpty(patronesId))
                {
                    @ViewBag.patId = patronesId;
                    int idPatron = int.Parse(patronesId.Trim());
                    asegurados = asegurados.Where(s => s.PatroneId.Equals(idPatron));
                }

                if (!String.IsNullOrEmpty(clientesId))
                {
                    @ViewBag.cteId = clientesId;
                    int idCliente = int.Parse(clientesId.Trim());
                    asegurados = asegurados.Where(s => s.Cliente.Id.Equals(idCliente));
                }

                if (!String.IsNullOrEmpty(gruposId))
                {
                    @ViewBag.gpoId = gruposId;
                    int idGrupo = int.Parse(gruposId.Trim());
                    asegurados = asegurados.Where(s => s.Cliente.Grupo_id.Equals(idGrupo));
                }

                if (!String.IsNullOrEmpty(opcion))
                {
                    @ViewBag.opBuscador = opcion;
                    @ViewBag.valBuscador = valor;
                    TempData["buscador"] = "0";

                    switch (opcion)
                    {
                        case "1":
                            asegurados = asegurados.Where(s => s.Patrone.registro.Contains(valor));
                            break;
                        case "2":
                            asegurados = asegurados.Where(s => s.numeroAfiliacion.Contains(valor));
                            break;
                        case "3":
                            asegurados = asegurados.Where(s => s.CURP.Contains(valor));
                            break;
                        case "4":
                            asegurados = asegurados.Where(s => s.RFC.Contains(valor));
                            break;
                        case "5":
                            asegurados = asegurados.Where(s => s.nombreTemporal.Contains(valor));
                            break;
                        case "6":
                            asegurados = asegurados.Where(s => s.fechaAlta.ToString().Contains(valor));
                            break;
                        case "7":
                            asegurados = asegurados.Where(s => s.fechaBaja.ToString().Contains(valor));
                            break;
                        case "8":
                            asegurados = asegurados.Where(s => s.salarioImss.ToString().Contains(valor.Trim()));
                            break;
                        case "9":
                            asegurados = asegurados.Where(s => s.Cliente.claveCliente.Contains(valor));
                            break;
                        case "10":
                            asegurados = asegurados.Where(s => s.Cliente.Grupos.nombre.Contains(valor));
                            break;
                        case "11":
                            asegurados = asegurados.Where(s => s.ocupacion.Contains(valor));
                            break;
                        case "12":
                            asegurados = asegurados.Where(s => s.Cliente.Plaza.cveCorta.Contains(valor));
                            break;
                        case "13":
                            asegurados = asegurados.Where(s => s.extranjero.Contains(valor));
                            break;
                    }
                }

                if (statusId != null)
                {
                    @ViewBag.statusId = statusId;

                    if (statusId.Trim().Equals("A"))
                    {
                        ViewBag.statusId = statusId;
                        asegurados = asegurados.Where(s => !s.fechaBaja.HasValue);
                    }
                    else if (statusId.Trim().Equals("B"))
                    {
                        ViewBag.statusId = statusId;
                        asegurados = asegurados.Where(s => s.fechaBaja.HasValue);
                    }
                }

                allCust = asegurados.ToList();

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"Asegurados-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
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
        public SheetData crearContenidoHoja(List<Asegurado> asegurados, ExcelHelper eh)
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

            row = eh.addNewCellToRow(index, row, "Salario IMSS", headerColumns[10] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Ubicación", headerColumns[11] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Grupo", headerColumns[12] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Ocupación", headerColumns[13] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Plaza", headerColumns[14] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Extranjero", headerColumns[15] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Fecha Creación", headerColumns[16] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Fecha Modificación", headerColumns[17] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Alta", headerColumns[18] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Baja", headerColumns[19] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);
            
            row = eh.addNewCellToRow(index, row, "Modificación", headerColumns[20] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);
            
            row = eh.addNewCellToRow(index, row, "Permanente", headerColumns[21] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);
            index++;
            //Create the cells that contain the data.
            foreach (Asegurado dp in asegurados)
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

                row = eh.addNewCellToRow(index, row, dp.nombres, headerColumns[i + 6] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.nombreTemporal, headerColumns[i + 7] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                String var1 = String.Format("{0:dd/MM/yyyy}", dp.fechaAlta);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 8] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:dd/MM/yyyy}", dp.fechaBaja);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 9] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.salarioImss);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 10] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Cliente.claveCliente, headerColumns[i + 11] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Cliente.Grupos.nombreCorto, headerColumns[i + 12] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.ocupacion, headerColumns[i + 13] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Cliente.Plaza.cveCorta, headerColumns[i + 14] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.extranjero, headerColumns[i + 15] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:dd/MM/yyyy hh:mm}", dp.fechaCreacion);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 16] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:dd/MM/yyyy hh:mm}", dp.fechaModificacion);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 17] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.alta, headerColumns[i + 18] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.baja, headerColumns[i + 19] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.modificacion, headerColumns[i + 20] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.permanente, headerColumns[i + 21] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                index++;
            }

            return sheetData;
        }

        // GET: Aseguradoes/Delete/5
        public ActionResult DeleteMovs(int idAseguraR, int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovimientosAsegurado asegurado = db.MovimientosAseguradoes.Find(id);
            db.MovimientosAseguradoes.Remove(asegurado);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = idAseguraR });
        }

        public ActionResult ActivaVariable(String buscador, String plazasId, String patronesId, String clientesId,
            String gruposId, String opcion, String valor, String statusId, String numeroPagina)
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
            else
            {
                TempData["buscador"] = "1";
            }
            return RedirectToAction("Index", new { plazasId, patronesId, clientesId, gruposId, opcion, valor, statusId, numeroPagina });
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

