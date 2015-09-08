using SUADATOS;
using SUAMVC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using SUAMVC.Helpers;
using System.Data.SqlClient;

namespace SUAMVC.Controllers
{
    public class RepCostoSocialController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: RepCostoSocials
        public ActionResult Index(String fechaAlta, String fechaBaja, String patronesId, String clientesId, String gruposId)
        {
            Usuario user = Session["UsuarioData"] as Usuario;
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

            if (fechaAlta != null && fechaBaja != null && !fechaAlta.Trim().Equals("") && !fechaBaja.Trim().Equals(""))
            {
                LlenaReporte(fechaAlta, fechaBaja, clientesId, patronesId, user.Id, gruposId);
            }
            return View();
        }


        public ActionResult LlenaReporte(String fecIni, String fecFin, String idCliente, String idPatron, int userId, String idGrupo)
        {
            int result = db.Database.ExecuteSqlCommand("sp_LimpiaReporte @usuarioId", new SqlParameter("@usuarioId", userId));
            
            DateTime fec1 = DateTime.Parse(fecIni);
            DateTime fec2 = DateTime.Parse(fecFin);
            TimeSpan dias = fec2 - fec1;
            int diasPeriodo = dias.Days + 1;

            ParametrosHelper parameterHelper = new ParametrosHelper();

            Parametro smdfParameter = parameterHelper.getParameterByKey("SMDF");
            Parametro sinfonParameter = parameterHelper.getParameterByKey("SINFON");
            Decimal smdf = Decimal.Parse(smdfParameter.valorMoneda.ToString());
            Decimal sinfon = Decimal.Parse(sinfonParameter.valorMoneda.ToString());

            //Preparamos la consulta
            var resAsegura = from b in db.Asegurados
                              where b.fechaAlta <= fec2
                              && ((b.fechaBaja >= fec1 && b.fechaBaja <= fec2) || b.fechaBaja.Equals(null))
//                              && b.ClienteId == clienteId
                              select b;

            if (idCliente != null && !idCliente.Trim().Equals("")) 
            {
                int clienteId = int.Parse(idCliente.Trim());
                resAsegura = resAsegura.Where(s => s.Cliente.Id.Equals(clienteId));
            }
            if (idPatron != null && !idPatron.Trim().Equals(""))
            {
                int patronId = int.Parse(idPatron.Trim());
                resAsegura = resAsegura.Where(s => s.PatroneId.Equals(patronId));
            }
            if (idGrupo != null && !idGrupo.Trim().Equals(""))
            {
                int grupoId = int.Parse(idGrupo.Trim());
                resAsegura = resAsegura.Where(s => s.Cliente.Grupo_id.Equals(grupoId));
            }
            if (resAsegura != null)
            {
                foreach (var aseg in resAsegura.ToList())
                {
                    //if (aseg.numeroAfiliacion.Equals("19148987480") || aseg.numeroAfiliacion.Equals("27149191176"))
                    //{
                    //    Decimal salarioIMSS2 = Decimal.Parse("0.0");
                    //}
                    Decimal salarioIMSS = Decimal.Parse("0.0");
                    if (!aseg.fechaBaja.Equals(null))
                    {
                        var movTemp2 = (from s in db.MovimientosAseguradoes
                                        where s.aseguradoId.Equals(aseg.id) &&
                                             (s.CatalogoMovimiento.tipo.Equals("01") ||
                                              s.CatalogoMovimiento.tipo.Equals("07") || s.CatalogoMovimiento.tipo.Equals("08") ||
                                              s.CatalogoMovimiento.tipo.Equals("13"))
                                              && s.fechaInicio <= fec2
                                        orderby s.fechaInicio descending
                                        select s).ToList();

                        MovimientosAsegurado movto = new MovimientosAsegurado();
                        if (movTemp2 != null && movTemp2.Count() > 0)
                        {
                            foreach (var movItem in movTemp2)
                            {
                                movto = movItem;
                                break;
                            }
                            salarioIMSS = Decimal.Parse(movto.sdi.ToString());
                        }
                    }
                    else
                    {
                        salarioIMSS = Decimal.Parse(aseg.salarioImss.ToString());
                    }
                    RepCostoSocial reporte = new RepCostoSocial();
                    int diasCotizados = diasPeriodo;
                    if (aseg.fechaAlta >= fec1)
                    {
                        dias = fec2 - aseg.fechaAlta;
                        diasCotizados = dias.Days + 1;
                    }
                    reporte.numeroAfiliacion = aseg.numeroAfiliacion;
                    reporte.nombre = aseg.nombreTemporal;
                    reporte.fechaAlta = aseg.fechaAlta;
                    reporte.diasCotizados = diasCotizados;
                    reporte.salarioIMSS = Double.Parse(salarioIMSS.ToString());
                    reporte.ubicacion = aseg.Cliente.claveCliente;
                    reporte.grupo = aseg.Cliente.Grupos.claveGrupo;
                    reporte.registroPatronal = aseg.Patrone.registro;
                    reporte.nombreRegPatronal = aseg.Patrone.nombre;
                    reporte.porcNomina = aseg.Patrone.porcentajeNomina;
                    //reporte.usuarioId = userId;

                    if (!aseg.paginaInfo.Trim().Equals(""))
                    {
                        reporte.numeroCredito = aseg.paginaInfo;
                        DateTime date = DateTime.Now;

                        Decimal valueToCalculate = Decimal.Parse(aseg.valorDescuento.ToString());

                        Decimal newValue = Decimal.Parse("0.0");
                        if (aseg.tipoDescuento.Trim().Equals("2"))
                        {
                            // Descuento tipo cuota fija
                            newValue = valueToCalculate * 2;
                        }
                        else if (aseg.tipoDescuento.Trim().Equals("3"))
                        {
                            // Descuento tipo VSM
                            newValue = valueToCalculate * smdf * 2;
                            newValue = newValue + sinfon;
                            newValue = Math.Round(newValue, 3);
                        }

                        reporte.descuentoMensual = Math.Round(newValue / 2, 3);
                        Decimal newValue2 = Decimal.Parse(reporte.descuentoMensual.ToString()) * Decimal.Parse((7 / 30.4).ToString());
                        newValue2 = Math.Round(newValue2, 3);
                        reporte.descuentoSemanal = newValue2;

                        newValue2 = Decimal.Parse(reporte.descuentoMensual.ToString()) * Decimal.Parse((14 / 30.4).ToString());
                        newValue2 = Math.Round(newValue2, 3);
                        reporte.descuentoCatorcenal = newValue2;
                        reporte.descuentoQuincenal = Math.Round(newValue / 4, 3);
                        reporte.descuentoVeintiochonal = Math.Round(Decimal.Parse(reporte.descuentoMensual.ToString()) * Decimal.Parse((28 / 30.4).ToString()), 3);
                        reporte.descuentoDiario = Math.Round(newValue / Decimal.Parse("60.1"), 3);

                    }

                    var movimientos = (from b in db.MovimientosAseguradoes
                                      where b.aseguradoId.Equals(aseg.id)
                                      && b.fechaInicio >= fec1
                                      && b.fechaInicio <= fec2
                                      && (b.CatalogoMovimiento.tipo.Equals("02") ||
                                           b.CatalogoMovimiento.tipo.Equals("07") || b.CatalogoMovimiento.tipo.Equals("08") ||
                                           b.CatalogoMovimiento.tipo.Equals("13"))
                                      orderby (b.fechaInicio)
                                      select b).ToList();
                    if (movimientos != null && movimientos.Count() > 0)
                    {
                        foreach (var movs in movimientos)
                        {
                            if (aseg.fechaAlta >= fec1)
                            {
                                dias = movs.fechaInicio - aseg.fechaAlta;
                                diasCotizados = dias.Days + 1;
                            }
                            else
                            {
                                dias = movs.fechaInicio - fec1;
                                diasCotizados = dias.Days + 1;
                            }
                            if (movs.movimientoId == 2)  // Baja
                            {
                                reporte.fechaBaja = movs.fechaInicio;
                            }
                            else  // Reingreso
                            {
                                salarioIMSS = Decimal.Parse(movs.sdi.ToString());
                                reporte.fechaAlta = movs.fechaInicio;
                            }
                            reporte.diasCotizados = diasCotizados;
                            reporte.salarioIMSS = Double.Parse(salarioIMSS.ToString());
                            Decimal var1 = Decimal.Parse("0.2040");
                            Decimal cuotaFija = smdf * diasCotizados * var1;
                            Decimal excedentePatronal = 0;
                            Decimal excedenteObrero = 0; ;
                            if (salarioIMSS > (smdf * 3))
                            {
                                var1 = Decimal.Parse("0.0110");
                                excedentePatronal = (salarioIMSS - (smdf * 3)) * diasCotizados * var1;
                                var1 = Decimal.Parse("0.0040");
                                excedenteObrero = (salarioIMSS - (smdf * 3)) * diasCotizados * var1;
                            }
                            var1 = Decimal.Parse("0.0070");
                            Decimal prestDineroPatron = salarioIMSS * diasCotizados * var1;
                            var1 = Decimal.Parse("0.0025");
                            Decimal prestDineroObrero = salarioIMSS * diasCotizados * var1;
                            var1 = Decimal.Parse("0.0105");
                            Decimal prestEspeciePatron = salarioIMSS * diasCotizados * var1;
                            var1 = Decimal.Parse("0.00375");
                            Decimal prestEspecieObrero = salarioIMSS * diasCotizados * var1;
                            var1 = Decimal.Parse("0.0");
                            var primaRT = (from s in db.PrimaRTs
                                            where s.registroPatronal.Equals(aseg.PatroneId) 
                                            select s).ToList();
                            PrimaRT primaRTVal = new PrimaRT();
                            if (primaRT != null && primaRT.Count() > 0)
                            {
                                foreach (var rtItem in primaRT)
                                {
                                    primaRTVal = rtItem;
                                    break;
                                }
                                var1 = primaRTVal.primaRT1;
                            }
                            Decimal riesgoTrabajo = salarioIMSS * diasCotizados * var1 / 100;
                            var1 = Decimal.Parse("0.0175");
                            Decimal invalidezVidaPatron = salarioIMSS * diasCotizados * var1;
                            var1 = Decimal.Parse("0.00625");
                            Decimal invalidezVidaObrero = salarioIMSS * diasCotizados * var1;
                            var1 = Decimal.Parse("0.0100");
                            Decimal guarderías = salarioIMSS * diasCotizados * var1;
                            reporte.IMSS = cuotaFija + excedentePatronal + excedenteObrero + prestDineroPatron + prestDineroObrero +
                                           prestEspeciePatron + prestEspecieObrero + riesgoTrabajo + invalidezVidaPatron +
                                           invalidezVidaObrero + guarderías;

                            var1 = Decimal.Parse("0.0200");
                            Decimal retiro = salarioIMSS * diasCotizados * var1;
                            var1 = Decimal.Parse("0.0315");
                            Decimal cesantiaVejezPatron = salarioIMSS * diasCotizados * var1;
                            var1 = Decimal.Parse("0.01125");
                            Decimal cesantiaVejezObrero = salarioIMSS * diasCotizados * var1;
                            reporte.RCV = retiro + cesantiaVejezPatron + cesantiaVejezObrero;

                            var1 = Decimal.Parse("0.0500");
                            reporte.Infonavit = salarioIMSS * diasCotizados * var1;

                            reporte.totalCosto = reporte.IMSS + reporte.RCV + reporte.Infonavit;

                            var1 = Decimal.Parse("0.0300");
                            reporte.porcCotizado = salarioIMSS * diasCotizados * var1;
                            if (diasCotizados == 0)
                            {
                                reporte.impuestoSNomina = 0;
                            }else
                            {
                                reporte.impuestoSNomina = reporte.porcCotizado / diasCotizados;
                            }

                            reporte.totalCostoSocial = reporte.totalCosto + reporte.porcCotizado;

                            db.RepCostoSocials.Add(reporte);
                            db.SaveChanges();

                        }
                    }
                    else
                    {
                        reporte.diasCotizados = diasCotizados;
                        reporte.salarioIMSS = Double.Parse(salarioIMSS.ToString());
                        Decimal var1 = Decimal.Parse("0.2040");
                        Decimal cuotaFija = smdf * diasCotizados * var1;
                        Decimal excedentePatronal = 0;
                        Decimal excedenteObrero = 0; ;
                        if (salarioIMSS > (smdf * 3))
                        {
                            var1 = Decimal.Parse("0.0110");
                            excedentePatronal = (salarioIMSS - (smdf * 3)) * diasCotizados * var1;
                            var1 = Decimal.Parse("0.0040");
                            excedenteObrero = (salarioIMSS - (smdf * 3)) * diasCotizados * var1;
                        }
                        var1 = Decimal.Parse("0.0070");
                        Decimal prestDineroPatron = salarioIMSS * diasCotizados * var1;
                        var1 = Decimal.Parse("0.0025");
                        Decimal prestDineroObrero = salarioIMSS * diasCotizados * var1;
                        var1 = Decimal.Parse("0.0105");
                        Decimal prestEspeciePatron = salarioIMSS * diasCotizados * var1;
                        var1 = Decimal.Parse("0.00375");
                        Decimal prestEspecieObrero = salarioIMSS * diasCotizados * var1;
                        var1 = Decimal.Parse("0.0");
                        var primaRT = (from s in db.PrimaRTs
                                       where s.registroPatronal.Equals(aseg.PatroneId)
                                       select s).ToList();
                        PrimaRT primaRTVal = new PrimaRT();
                        if (primaRT != null && primaRT.Count() > 0)
                        {
                            foreach (var rtItem in primaRT)
                            {
                                primaRTVal = rtItem;
                                break;
                            }
                            var1 = primaRTVal.primaRT1;
                        }
                        Decimal riesgoTrabajo = salarioIMSS * diasCotizados * var1 / 100;
                        var1 = Decimal.Parse("0.0175");
                        Decimal invalidezVidaPatron = salarioIMSS * diasCotizados * var1;
                        var1 = Decimal.Parse("0.00625");
                        Decimal invalidezVidaObrero = salarioIMSS * diasCotizados * var1;
                        var1 = Decimal.Parse("0.0100");
                        Decimal guarderías = salarioIMSS * diasCotizados * var1;
                        reporte.IMSS = cuotaFija + excedentePatronal + excedenteObrero + prestDineroPatron + prestDineroObrero + 
                                       prestEspeciePatron + prestEspecieObrero + riesgoTrabajo + invalidezVidaPatron + 
                                       invalidezVidaObrero + guarderías;

                        var1 = Decimal.Parse("0.0200");
                        Decimal retiro = salarioIMSS * diasCotizados * var1;
                        var1 = Decimal.Parse("0.0315");
                        Decimal cesantiaVejezPatron = salarioIMSS * diasCotizados * var1;
                        var1 = Decimal.Parse("0.01125");
                        Decimal cesantiaVejezObrero = salarioIMSS * diasCotizados * var1;
                        reporte.RCV = retiro + cesantiaVejezPatron + cesantiaVejezObrero;

                        var1 = Decimal.Parse("0.0500");
                        reporte.Infonavit = salarioIMSS * diasCotizados * var1;

                        reporte.totalCosto = reporte.IMSS + reporte.RCV + reporte.Infonavit;

                        var1 = Decimal.Parse("0.0300");
                        reporte.porcCotizado = salarioIMSS * diasCotizados * var1;
                        if (diasCotizados == 0)
                        {
                            reporte.impuestoSNomina = 0;
                        }
                        else
                        {
                            reporte.impuestoSNomina = reporte.porcCotizado / diasCotizados;
                        }

                        reporte.totalCostoSocial = reporte.totalCosto + reporte.porcCotizado;

                        db.RepCostoSocials.Add(reporte);
                        db.SaveChanges();
                    }
                }
            }


            FileStream fileStream = null;
            MemoryStream mem = new MemoryStream();
            try
            {

            var queryReporte = from b in db.RepCostoSocials
                               select b;
            List<RepCostoSocial> allCust = new List<RepCostoSocial>();
            allCust = queryReporte.ToList();

                DateTime date = DateTime.Now;
                String path = @"C:\\SUA\\Exceles\\";
                String fileName = @"CostoSocial-" + date.ToString("ddMMyyyyHHmm") + ".xlsx";
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
            return RedirectToAction("Index");
        }


        string[] headerColumns = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG" };
        public SheetData crearContenidoHoja(List<RepCostoSocial> allCust, ExcelHelper eh)
        {
            SheetData sheetData = new SheetData();
            int index = 1;

            //Creamos el Header
            Row row = new Row();
            row = eh.addNewCellToRow(index, row, "NSS", headerColumns[0] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Días cotizados", headerColumns[1] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Salario IMSS", headerColumns[2] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Nombre", headerColumns[3] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Fecha Alta", headerColumns[4] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Fecha Baja", headerColumns[5] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Ubicación", headerColumns[6] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Id Grupo", headerColumns[7] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Reg. Patronal", headerColumns[8] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Nombre Reg. Patronal", headerColumns[9] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "IMSS", headerColumns[10] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "RCV", headerColumns[11] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Infonavit", headerColumns[12] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Total Costo", headerColumns[13] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Impuesto Sobre Nómina", headerColumns[14] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "% Impuesto.", headerColumns[15] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "% Cotizado", headerColumns[16] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Total Costo Social", headerColumns[17] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Número de crédito", headerColumns[18] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Desc. Infonavit Mensual", headerColumns[19] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Desc. Infonavit Veintioch.", headerColumns[20] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Desc. Infonavit Quincenal", headerColumns[21] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Desc. Infonavit Catorcenal", headerColumns[22] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Desc. Infonavit Semanal", headerColumns[23] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            row = eh.addNewCellToRow(index, row, "Desc. Infonavit Diario", headerColumns[24] + index, 5U, CellValues.String);
            sheetData.AppendChild(row);

            index++;
            //Create the cells that contain the data.
            foreach (RepCostoSocial dp in allCust)
            {
                int i = 0;

                row = eh.addNewCellToRow(index, row, dp.numeroAfiliacion, headerColumns[i] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.diasCotizados.ToString(), headerColumns[i + 1] + index, 2U, CellValues.Number);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.salarioIMSS.ToString(), headerColumns[i + 2] + index, 2U, CellValues.Number);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.nombre, headerColumns[i + 3] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                String var1 = String.Format("{0:dd/MM/yyyy}", dp.fechaAlta);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 4] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:dd/MM/yyyy}", dp.fechaBaja);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 5] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.ubicacion, headerColumns[i + 6] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.grupo, headerColumns[i + 7] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.registroPatronal, headerColumns[i + 8] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.nombreRegPatronal, headerColumns[i + 9] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.IMSS);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 10] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.RCV);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 11] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.Infonavit);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 12] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.totalCosto);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 13] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.impuestoSNomina);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 14] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.porcNomina);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 15] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.porcCotizado);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 16] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.totalCostoSocial);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 17] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                row = eh.addNewCellToRow(index, row, dp.numeroCredito, headerColumns[i + 18] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.descuentoMensual);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 19] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.descuentoVeintiochonal);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 20] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.descuentoQuincenal);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 21] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.descuentoCatorcenal);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 22] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.descuentoSemanal);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 23] + index, 2U, CellValues.String);
                sheetData.AppendChild(row);

                var1 = String.Format("{0:###,###,##0.00}", dp.descuentoDiario);
                row = eh.addNewCellToRow(index, row, var1, headerColumns[i + 24] + index, 2U, CellValues.String);
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
