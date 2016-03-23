using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using System.Data.OleDb;
using SUAMVC.Helpers;
using SUAMVC.Models;

namespace SUAMVC.Controllers
{
    public class ClientesController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Clientes
        private void setVariables(String plazasId, String opcion, String valor)
        {
            if (!String.IsNullOrEmpty(plazasId))
            {
                ViewBag.pzaId = plazasId;
            }
            if (!String.IsNullOrEmpty(opcion))
            {
                ViewBag.opBuscador = opcion;
            }
            if (!String.IsNullOrEmpty(valor))
            {
                ViewBag.valBuscador = valor;
            }
        }

        // GET: Aseguradoes
        public ActionResult Index(String plazasId, String opcion, String valor, String sortOrder = null, String lastSortOrder = null)
        {

            Usuario user = Session["UsuarioData"] as Usuario;

            setVariables(plazasId, opcion, valor);


            var plazasAsignadas = (from x in db.TopicosUsuarios
                                   where x.usuarioId.Equals(user.Id)
                                   && x.tipo.Equals("P")
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
            
            var clientes = from p in db.Clientes
                           where plazasAsignadas.Contains(p.Plaza_id) &&
                                 clientesAsignados.Contains(p.Id) &&
                                 gruposAsignados.Contains(p.Grupo_id)
                           select p;


            if (!String.IsNullOrEmpty(plazasId))
            {
                @ViewBag.pzaId = plazasId;
                int plazaIdTemp = int.Parse(plazasId);
                clientes = clientes.Where(p => p.Plaza.id.Equals(plazaIdTemp));
            }

            ViewBag.PlazasId = new SelectList((from s in db.Plazas.ToList()
                                               join top in db.TopicosUsuarios on s.id equals top.topicoId
                                               where top.tipo.Trim().Equals("P") && top.usuarioId.Equals(user.Id)
                                               orderby s.descripcion
                                               select new
                                               {
                                                   id = s.id,
                                                   FUllName = s.descripcion
                                               }).Distinct(), "id", "FullName");

            if (!String.IsNullOrEmpty(opcion))
            {

                TempData["buscador"] = "0";
                switch (opcion)
                {
                    case "1":
                        clientes = clientes.Where(s => s.claveCliente.Contains(valor));
                        break;
                    case "2":
                        clientes = clientes.Where(s => s.descripcion.Contains(valor));
                        break;
                }
            }

            SecurityUserModel.llenarPermisos(user.roleId);

            return View(clientes.ToList().OrderBy(p => p.descripcion));
        }

        // GET: Clientes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // GET: Clientes/Create
        public ActionResult Create()
        {
            ViewBag.EmpresaFacturacion_id = new SelectList(db.Empresas);


            ViewBag.Plaza_id = new SelectList((from s in db.Plazas.ToList()
                                               where s.indicador.Equals("U")
                                               orderby s.descripcion
                                               select new
                                               {
                                                   id = s.id,
                                                   descripcion = s.descripcion
                                               }), "id", "descripcion");
            ViewBag.Grupo_id = new SelectList(db.Grupos, "id", "nombreCorto");


            return View();
        }
        // POST: Clientes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,claveCliente,claveSua,rfc,descripcion,nombre,direccionFiscal,contacto,telefono,direccionOficina,email,actividadPrincipal,fechaContratacion,empresaFacturadoraId,tipoClienteId,numeroCuenta,tipoServicioId,Plaza_id,Grupo_id,ejecutivoContadorId,emailContacto")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                Cliente clienteTemp = db.Clientes.Where(p => p.rfc.Equals(cliente.rfc.Trim())).FirstOrDefault();

                if (clienteTemp == null)
                {
                    cliente.claveCliente = cliente.claveCliente.ToUpper();
                    cliente.claveSua = cliente.claveSua.ToUpper();
                    cliente.descripcion = cliente.descripcion.ToUpper();
                    cliente.rfc = cliente.rfc.ToUpper();
                    cliente.nombre = cliente.nombre;
                    cliente.direccionFiscal = cliente.direccionFiscal;
                    cliente.contacto = cliente.contacto;
                    cliente.telefono = cliente.telefono;
                    cliente.direccionOficina = cliente.direccionOficina;
                    cliente.email = cliente.email;
                    cliente.actividadPrincipal = cliente.actividadPrincipal;
                    cliente.fechaContratacion = cliente.fechaContratacion;
                    cliente.empresaFacturadoraId = cliente.empresaFacturadoraId;
                    cliente.tipoClienteId = cliente.tipoClienteId;
                    cliente.numeroCuenta = cliente.numeroCuenta;
                    cliente.tipoServicioId = cliente.tipoServicioId;

                    //Validar esto
                    cliente.ejecutivoContadorId = cliente.ejecutivoContadorId;
                    db.Clientes.Add(cliente);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.Plaza_id = new SelectList((from s in db.Plazas.ToList()
                                               where s.indicador.Equals("U")
                                               orderby s.descripcion
                                               select new
                                               {
                                                   id = s.id,
                                                   descripcion = s.descripcion
                                               }), "id", "descripcion", cliente.Plaza_id);
            ViewBag.Grupo_id = new SelectList(db.Grupos, "id", "nombreCorto", cliente.Grupo_id);
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            ViewBag.Plaza_id = new SelectList((from s in db.Plazas.ToList()
                                               where s.indicador.Equals("U")
                                               orderby s.descripcion
                                               select new
                                               {
                                                   id = s.id,
                                                   descripcion = s.descripcion
                                               }), "id", "descripcion", cliente.Plaza_id);
            ViewBag.Grupo_id = new SelectList(db.Grupos, "id", "nombreCorto", cliente.Grupo_id);
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,claveCliente,claveSua,rfc,descripcion,nombre,direccionFiscal,contacto,telefono,direccionOficina,email,actividadPrincipal,fechaContratacion,empresaFacturadoraId,tipoClienteId,numeroCuenta,tipoServicioId,Plaza_id,Grupo_id,ejecutivoContadorId,emailContacto")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                cliente.claveCliente = cliente.claveCliente.ToUpper();
                cliente.claveSua = cliente.claveSua.ToUpper();
                cliente.descripcion = cliente.descripcion.ToUpper();
                cliente.rfc = cliente.rfc.ToUpper();
                cliente.direccionFiscal = cliente.direccionFiscal;
                cliente.contacto = cliente.contacto;
                cliente.telefono = cliente.telefono;
                cliente.direccionOficina = cliente.direccionOficina;
                cliente.email = cliente.email;
                cliente.actividadPrincipal = cliente.actividadPrincipal;
                cliente.fechaContratacion = cliente.fechaContratacion;
                cliente.empresaFacturadoraId = cliente.empresaFacturadoraId;
                cliente.tipoClienteId = cliente.tipoClienteId;
                cliente.numeroCuenta = cliente.numeroCuenta;
                cliente.tipoServicioId = cliente.tipoServicioId;

                cliente.ejecutivoContadorId = cliente.ejecutivoContadorId;
                db.Entry(cliente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Plaza_id = new SelectList((from s in db.Plazas.ToList()
                                               where s.indicador.Equals("U")
                                               orderby s.descripcion
                                               select new
                                               {
                                                   id = s.id,
                                                   descripcion = s.descripcion
                                               }), "id", "descripcion", cliente.Plaza_id);
            ViewBag.Grupo_id = new SelectList(db.Grupos, "id", "nombreCorto", cliente.Grupo_id);
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cliente cliente = db.Clientes.Find(id);
            db.Clientes.Remove(cliente);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult ActivaVariable(String buscador, String plazasId, String opcion, String valor)
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
            return RedirectToAction("Index", new { plazasId, opcion, valor });
        }


        [HttpGet]
        public void GetExcel(String plazasId, String opcion, String valor)
        {
            Usuario user = Session["UsuarioData"] as Usuario;

            var plazasAsignadas = (from x in db.TopicosUsuarios
                                   where x.usuarioId.Equals(user.Id)
                                   && x.tipo.Equals("P")
                                   select x.topicoId);

            var clientes = from p in db.Clientes
                           where plazasAsignadas.Contains(p.Plaza.id)
                           select p;


            if (!String.IsNullOrEmpty(plazasId))
            {
                int plazaIdTemp = int.Parse(plazasId);
                clientes = clientes.Where(p => p.Plaza.id.Equals(plazaIdTemp));
            }

            if (!String.IsNullOrEmpty(opcion))
            {

                switch (opcion)
                {
                    case "1":
                        clientes = clientes.Where(s => s.claveCliente.Contains(valor));
                        break;
                    case "2":
                        clientes = clientes.Where(s => s.descripcion.Contains(valor));
                        break;
                }
            }


            List<Cliente> allCust = new List<Cliente>();

            allCust = clientes.ToList();

            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"Clientes-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
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

                SheetData sd = crearContenidoHoja4(allCust, eh);//CreateContentRow(); 
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


        string[] headerColumns3 = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG" };
        public SheetData crearContenidoHoja4(List<Cliente> allCust, ExcelHelper eh)
        {
            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();

            row = eh.addNewCellToRow(index, row, "ID Cliente", headerColumns3[0] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Clave SUA", headerColumns3[1] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "RFC", headerColumns3[2] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Nombre", headerColumns3[3] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Dirección Fiscal", headerColumns3[4] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Contacto", headerColumns3[5] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Teléfono", headerColumns3[6] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Dirección Oficina", headerColumns3[7] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "E-mail contacto", headerColumns3[8] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Actividad principal", headerColumns3[9] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Fecha de contratación", headerColumns3[10] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Empresa facturadora", headerColumns3[11] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Ejecutivo", headerColumns3[12] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Plaza", headerColumns3[13] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Grupo", headerColumns3[14] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Tipo de cliente", headerColumns3[15] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Número de cuenta", headerColumns3[16] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Tipo de servicio", headerColumns3[17] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Email envío solicitud", headerColumns3[18] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            index++;
            //Create the cells that contain the data.
            foreach (Cliente dp in allCust)
            {
                int i = 0;

                row = eh.addNewCellToRow(index, row, dp.claveCliente, headerColumns3[i] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.claveSua, headerColumns3[i + 1] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.rfc, headerColumns3[i + 2] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.descripcion, headerColumns3[i + 3] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.direccionFiscal, headerColumns3[i + 4] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.contacto, headerColumns3[i + 5] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.telefono, headerColumns3[i + 6] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.direccionOficina, headerColumns3[i + 7] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.emailContacto, headerColumns3[i + 8] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.actividadPrincipal, headerColumns3[i + 9] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                String var1 = "";
                if(dp.fechaContratacion != null){
                    var1 = dp.fechaContratacion.Value.Date.ToString();
                }
                row = eh.addNewCellToRow(index, row, var1, headerColumns3[i + 10] + index, 2U, CellValues.Date);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.empresaFacturadoraId.ToString(), headerColumns3[i + 11] + index, 2U, CellValues.Number);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.ejecutivoContadorId.ToString(), headerColumns3[i + 12] + index, 2U, CellValues.Number);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Plaza.cveCorta, headerColumns3[i + 13] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.Grupos.nombreCorto, headerColumns3[i + 14] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.tipoClienteId.ToString(), headerColumns3[i + 15] + index, 2U, CellValues.Number);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.numeroCuenta, headerColumns3[i + 16] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.tipoServicioId.ToString(), headerColumns3[i + 17] + index, 2U, CellValues.Number);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.email.ToString(), headerColumns3[i + 18] + index, 2U, CellValues.Number);
                sheetData.AppendChild(row);

                index++;
            }

            return sheetData;
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
