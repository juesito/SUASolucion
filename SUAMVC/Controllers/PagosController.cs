using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SUADATOS;
using SUAMVC.Models;
using System.IO;
using System.Data.Entity.Validation;
using System.Text;
using SUAMVC.Helpers;
using System.Web.Helpers;
using Ionic.Zip;


namespace SUAMVC.Controllers
{
    public class PagosController : Controller
    {
        private suaEntities db = new suaEntities();

        // GET: Pagos
        public ActionResult Index(String plazasId, String patronesId, String periodoId, String ejercicioId)
        {
            var pagos = db.Pagos.ToList();

            if (!String.IsNullOrEmpty(plazasId))
            {
                int plazaTempId = int.Parse(plazasId.Trim());
                pagos = pagos.Where(s => s.Patrone.Plaza_id.Equals(plazaTempId)).ToList();
            }
            if (!String.IsNullOrEmpty(patronesId))
            {
                int patronesTempId = int.Parse(patronesId);
                pagos = pagos.Where(s => s.Patrone.Id.Equals(patronesTempId)).ToList();
            }
            if (!String.IsNullOrEmpty(periodoId))
            {
                pagos = pagos.Where(s => s.mes.Trim().Equals(periodoId.Trim())).ToList();
            }
            if (!String.IsNullOrEmpty(ejercicioId))
            {
                pagos = pagos.Where(s => s.anno.Trim().Equals(ejercicioId)).ToList();
            }

            pagos = pagos.OrderBy(p => p.Patrone.registro).ToList();

            return View(pagos.ToList());
        }

        public ActionResult UploadPagos()
        {
            return View();
        }

        /**
         * Se cargan los pagos por periodo y patron via carga masiva desde el SUA.mdb
         */
        [HttpPost]
        public ActionResult Upload(String patronesId, String periodoId, String ejercicioId, String usuarioId)
        {
            if (!String.IsNullOrEmpty(patronesId) && !String.IsNullOrEmpty(periodoId) && !String.IsNullOrEmpty(ejercicioId)
                && !String.IsNullOrEmpty(usuarioId))
            {
                int userId = int.Parse(usuarioId.Trim());
                String periodo = ejercicioId.Trim() + periodoId.Trim();
                int mes = int.Parse(periodoId.Trim());

                int patronTemp = int.Parse(patronesId);
                Patrone patron = db.Patrones.Find(patronTemp);
                String path = this.UploadFile(patron.direccionArchivo);

                bool esBimestral = ((mes % 2) == 0);

                if (!path.Equals(""))
                {
                    Boolean existe = false;
                    SUAHelper suaHelper = new SUAHelper(path);

                    String sSQL = "SELECT * FROM RESUMEN" +
                               "  WHERE Reg_Patr = '" + patron.registro + "'" +
                               "    AND Mes_Ano = '" + periodo + "'" +
                               "   ORDER BY Reg_Patr";

                    DataTable dt2 = suaHelper.ejecutarSQL(sSQL);

                    foreach (DataRow rows in dt2.Rows)
                    {
                        Pago pago = new Pago();
                        pago = db.Pagos.Where(p => p.patronId.Equals(patron.Id) && p.mes.Trim().Equals(periodoId.Trim()) && p.anno.Trim().Equals(ejercicioId.Trim())).FirstOrDefault();
                        Boolean actualizar = false;

                        if (pago != null)
                        {
                            actualizar = true;
                        }
                        else
                        {
                            pago = new Pago();
                            pago.mes = periodoId;
                            pago.anno = ejercicioId;
                        }

                        pago.imss = Decimal.Parse(rows["CTA_FIJ"].ToString()) + Decimal.Parse(rows["CTA_EXC"].ToString()) +
                                    Decimal.Parse(rows["PRE_DIN"].ToString()) + Decimal.Parse(rows["PRE_ESP"].ToString()) +
                                    Decimal.Parse(rows["RIE_TRA"].ToString()) + Decimal.Parse(rows["INV_VID"].ToString()) +
                                    Decimal.Parse(rows["GUA_DER"].ToString());

                        pago.rcv = Decimal.Parse(rows["RET_SAR"].ToString()) + Decimal.Parse(rows["CEN_VEJPat"].ToString()) +
                                   Decimal.Parse(rows["Cen_VEJObr"].ToString());

                        pago.infonavit = Decimal.Parse(rows["VIV_SIN"].ToString()) + Decimal.Parse(rows["VIV_CON"].ToString()) +
                                         Decimal.Parse(rows["AMO_INF"].ToString());

                        pago.total = pago.imss + pago.rcv + pago.infonavit;

                        pago.recargos = Decimal.Parse(rows["REC_IMS"].ToString()) + Decimal.Parse(rows["REC_SAR"].ToString()) +
                                        Decimal.Parse(rows["REC_VIV"].ToString());

                        pago.actualizaciones = Decimal.Parse(rows["ACT_IMS"].ToString()) + Decimal.Parse(rows["ACT_SAR"].ToString()) +
                                               Decimal.Parse(rows["ACT_VIV"].ToString());

                        pago.granTotal = pago.recargos + pago.actualizaciones;

                        sSQL = "SELECT COUNT(*) FROM RELTRA" +
                           "  WHERE Reg_Pat = '" + patron.registro + "'" +
                           "    AND Periodo = '" + periodo + "'";

                        DataTable dt3 = suaHelper.ejecutarSQL(sSQL);

                        foreach (DataRow rows1 in dt3.Rows)
                        {
                            pago.nt = int.Parse(rows1[0].ToString());
                        }

                        if (pago.nt == 0)
                        {
                            break;
                        }

                        pago.patronId = patron.Id;
                        pago.Patrone = patron;
                        pago.fechaCreacion = DateTime.Now;
                        pago.usuarioId = userId;

 //                       Guardamos el pago.
                        if (actualizar)
                        {
                            db.Entry(pago).State = EntityState.Modified;
                        }
                        else
                        {
                            db.Pagos.Add(pago);
                        }
                        db.SaveChanges();
                        existe = true;

                        if (existe)
                        {

                            //Preparamos el query del resúmen
                            sSQL = "SELECT * FROM RELTRA " +
                                   "  WHERE Reg_Pat = '" + patron.registro + "'" +
                                   "    AND Periodo = '" + periodo + "'" +
                                   "   ORDER BY Reg_Pat";

                            DataTable dt4 = suaHelper.ejecutarSQL(sSQL);

                            foreach (DataRow row2 in dt4.Rows)
                            {
                                Boolean actualizarDetalle = false;

                                DetallePago detallePago = new DetallePago();
                                String nss = row2["Num_Afi"].ToString().Trim();

                                Asegurado asegurado = db.Asegurados.Where(a => a.numeroAfiliacion.Equals(nss.Trim())).FirstOrDefault();

                                detallePago = db.DetallePagos.Where(dp => dp.pagoId.Equals(pago.id) && dp.aseguradoId.Equals(asegurado.id)).FirstOrDefault();

                                if (detallePago != null)
                                {
                                    actualizarDetalle = true;
                                }
                                else
                                {
                                    detallePago = new DetallePago();
                                    detallePago.pagoId = pago.id;
                                    detallePago.Pago = pago;
                                    detallePago.aseguradoId = asegurado.id;
                                    detallePago.Asegurado = asegurado;
                                    detallePago.patronId = patron.Id;
                                    detallePago.Patrone = patron;
                                }
                                detallePago.diasCotizados = int.Parse(row2["dia_cot"].ToString().Trim());
                                detallePago.sdi = decimal.Parse(row2["sal_dia"].ToString().Trim());

                                if (String.IsNullOrEmpty(row2["Dia_Inc"].ToString()))
                                {
                                    detallePago.diasIncapacidad = 0;
                                }
                                else
                                {
                                    detallePago.diasIncapacidad = int.Parse(row2["Dia_Inc"].ToString().Trim());
                                }

                                if (String.IsNullOrEmpty(row2["Dia_Aus"].ToString()))
                                {
                                    detallePago.diasAusentismo = 0;
                                }
                                else
                                {
                                    detallePago.diasAusentismo = int.Parse(row2["Dia_Aus"].ToString().Trim());
                                }

                                if (String.IsNullOrEmpty(row2["CF"].ToString()))
                                {
                                    detallePago.cuotaFija = 0;
                                }
                                else
                                {
                                    detallePago.cuotaFija = decimal.Parse(row2["CF"].ToString().Trim());
                                }

                                if (String.IsNullOrEmpty(row2["EXPA"].ToString()))
                                {
                                    detallePago.expa = 0;
                                }
                                else
                                {
                                    detallePago.expa = decimal.Parse(row2["EXPA"].ToString().Trim());
                                }

                                if (String.IsNullOrEmpty(row2["EXO"].ToString()))
                                {
                                    detallePago.exO = 0;
                                }
                                else
                                {
                                    detallePago.exO = decimal.Parse(row2["EXO"].ToString().Trim());
                                }

                                if (String.IsNullOrEmpty(row2["PDP"].ToString()))
                                {
                                    detallePago.pdp = 0;
                                }
                                else
                                {
                                    detallePago.pdp = decimal.Parse(row2["PDP"].ToString().Trim());
                                }

                                if (String.IsNullOrEmpty(row2["PDO"].ToString()))
                                {
                                    detallePago.pdo = 0;
                                }
                                else
                                {
                                    detallePago.pdo = decimal.Parse(row2["PDO"].ToString().Trim());
                                }

                                if (String.IsNullOrEmpty(row2["GMPP"].ToString()))
                                {
                                    detallePago.gmpp = 0;
                                }
                                else
                                {
                                    detallePago.gmpp = decimal.Parse(row2["GMPP"].ToString().Trim());
                                }

                                if (String.IsNullOrEmpty(row2["GMPO"].ToString()))
                                {
                                    detallePago.gmpo = 0;
                                }
                                else
                                {
                                    detallePago.gmpo = decimal.Parse(row2["GMPO"].ToString().Trim());
                                }

                                if (String.IsNullOrEmpty(row2["RT"].ToString()))
                                {
                                    detallePago.rt = 0;
                                }
                                else
                                {
                                    detallePago.rt = decimal.Parse(row2["RT"].ToString().Trim());
                                }

                                if (String.IsNullOrEmpty(row2["IVP"].ToString()))
                                {
                                    detallePago.ivp = 0;
                                }
                                else
                                {
                                    detallePago.ivp = decimal.Parse(row2["IVP"].ToString().Trim());
                                }

                                if (String.IsNullOrEmpty(row2["IVO"].ToString()))
                                {
                                    detallePago.ivo = 0;
                                }
                                else
                                {
                                    detallePago.ivo = decimal.Parse(row2["IVO"].ToString().Trim());
                                }

                                if (String.IsNullOrEmpty(row2["GPS"].ToString()))
                                {
                                    detallePago.gps = 0;
                                }
                                else
                                {
                                    detallePago.gps = decimal.Parse(row2["GPS"].ToString().Trim());
                                }

                                detallePago.patronal = detallePago.cuotaFija + detallePago.expa + detallePago.pdp + detallePago.gmpp + detallePago.rt + detallePago.ivp + detallePago.gps;
                                detallePago.obrera = detallePago.exO + detallePago.pdo + detallePago.gmpo + detallePago.ivo;
                                detallePago.imss = detallePago.patronal + detallePago.obrera;

                                if (esBimestral)
                                {

                                    // Se guardan los datos bimestrales.
                                    sSQL = "SELECT * FROM RELTRABIM " +
                                           "  WHERE Reg_Pat = '" + patron.registro + "'" +
                                           "    AND Periodo = '" + periodo + "'" +
                                           "    AND Num_Afi = '" + asegurado.numeroAfiliacion.Trim() + "'" +
                                           "   ORDER BY Reg_Pat";

                                    DataTable dt5 = suaHelper.ejecutarSQL(sSQL);

                                    foreach (DataRow row5 in dt5.Rows)
                                    {

                                        if (String.IsNullOrEmpty(row5["Retiro"].ToString()))
                                        {
                                            detallePago.retiro = 0;
                                        }
                                        else
                                        {
                                            detallePago.retiro = decimal.Parse(row5["Retiro"].ToString().Trim());
                                        }
                                        if (String.IsNullOrEmpty(row5["CyVP"].ToString()))
                                        {
                                            detallePago.patronalBimestral = 0;
                                        }
                                        else
                                        {
                                            detallePago.patronalBimestral = decimal.Parse(row5["CyVP"].ToString().Trim());
                                        }
                                        if (String.IsNullOrEmpty(row5["CyVO"].ToString()))
                                        {
                                            detallePago.obreraBimestral = 0;
                                        }
                                        else
                                        {
                                            detallePago.obreraBimestral = decimal.Parse(row5["CyVO"].ToString().Trim());
                                        }

                                        detallePago.rcv = detallePago.retiro + detallePago.patronal + detallePago.obrera;

                                        if (String.IsNullOrEmpty(row5["Aportasc"].ToString()))
                                        {
                                            detallePago.aportacionsc = 0;
                                        }
                                        else
                                        {
                                            detallePago.aportacionsc = decimal.Parse(row5["Aportasc"].ToString().Trim());
                                        }
                                        if (String.IsNullOrEmpty(row5["Aportacc"].ToString()))
                                        {
                                            detallePago.aportacioncc = 0;
                                        }
                                        else
                                        {
                                            detallePago.aportacioncc = decimal.Parse(row5["Aportacc"].ToString().Trim());
                                        }
                                        if (String.IsNullOrEmpty(row5["Amortiza"].ToString()))
                                        {
                                            detallePago.amortizacion = 0;
                                        }
                                        else
                                        {
                                            detallePago.amortizacion = decimal.Parse(row5["Amortiza"].ToString().Trim());
                                        }

                                        detallePago.infonavit = detallePago.aportacionsc + detallePago.aportacioncc + detallePago.amortizacion;

                                    }
                                } //El periodo es bimestre

                                detallePago.usuarioId = userId;
                                detallePago.fechaCreacion = DateTime.Now;

                                try
                                {
                                    if (actualizarDetalle)
                                    {
                                        db.Entry(detallePago).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        db.DetallePagos.Add(detallePago);
                                    }

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
                            }

                        }
                    }
//                    path = path + "\\SUA.mdb";
//                    System.IO.File.Delete(path);
                }
            }
            return RedirectToAction("UploadPagos");
        }

        public String UploadFile(String subFolder)
        {
            String path = "";
            ParametrosHelper parameterHelper = new ParametrosHelper();
            Parametro rutaParameter = parameterHelper.getParameterByKey("SUARUTA");

            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {

                    if (!String.IsNullOrEmpty(subFolder))
                    {
                        path = Path.Combine(rutaParameter.valorString.Trim(), subFolder.Trim());
                        if (!System.IO.File.Exists(path))
                        {
                            System.IO.Directory.CreateDirectory(path);
                        }
                    }
                    else
                    {
                        path = rutaParameter.valorString.Trim();
                    }

                    var fileName = Path.GetFileName(file.FileName);
                    //var path = Path.Combine(Server.MapPath("~/App_LocalResources/"), fileName);
                    var pathFinal = Path.Combine(path, fileName);
                    file.SaveAs(pathFinal);
                    ZipFile zip = ZipFile.Read(pathFinal);
                    zip.ExtractAll(path, ExtractExistingFileAction.OverwriteSilently);
                    ViewBag.dbUploaded = true;
                    TempData["error"] = false;
                    TempData["viewMessage"] = "Se ha realizado la actualización con exito!";
                }
            }

            return path;
        }

        [HttpGet]
        public ActionResult ResumenPagos(int id)
        {
            PagosResumenModel resumenPagoModel = new PagosResumenModel();

            Pago pago = db.Pagos.Where(p => p.id.Equals(id)).FirstOrDefault();

            List<DetallePago> detallePago = db.DetallePagos.Where(r => r.pagoId.Equals(id)).ToList();

            resumenPagoModel.pago = pago;
            resumenPagoModel.detalle = detallePago;

            return View(resumenPagoModel);
        }

        public ActionResult actualizarPagos([Bind(Include = "id,bancoId,fechaDeposito")]Pago pago, String bancoId)
        {

            if (pago != null)
            {
                int bancoTempId = int.Parse(bancoId);
                Pago pagoTemp = db.Pagos.Find(pago.id);
                pagoTemp.fechaDeposito = pago.fechaDeposito;
                pagoTemp.bancoId = bancoTempId;
                db.Entry(pagoTemp).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");

        }

        public ActionResult UploadComprobantes(String id, String comprobanteId)
        {

            @ViewBag.id = id;
            @ViewBag.comprobanteId = comprobanteId;

            return View();
        }

        public ActionResult GuardarComprobantes(String id, String comprobanteId)
        {

            if (!String.IsNullOrEmpty(id))
            {
                ToolsHelper th = new ToolsHelper();
                ParametrosHelper ph = new ParametrosHelper();

                Parametro parametro = ph.getParameterByKey("COMPRUTA");
                String destino = parametro.valorString.Trim() + comprobanteId.Trim();

                String fileName = th.cargarArchivo(Request.Files, destino);

                int idTemp = int.Parse(id.Trim());

                Pago pago = db.Pagos.Find(idTemp);
                if (comprobanteId.Trim().Equals("CL"))
                {
                    pago.comprobantePago = fileName.Trim();
                }
                else
                    if (comprobanteId.Trim().Equals("RL"))
                    {
                        pago.resumenLiquidacion = fileName.Trim();
                    }
                    else
                    {
                        pago.cedulaAutodeterminacion = fileName.Trim();
                    }

                db.Entry(pago).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult VerComprobante(String fileNameString)
        {
            if (!String.IsNullOrEmpty(fileNameString))
            {

                ParametrosHelper ph = new ParametrosHelper();

                Parametro parametro = ph.getParameterByKey("COMPRUTA");
                Parametro rutaParameter = ph.getParameterByKey("SUARUTA");
                var fileName = rutaParameter.valorString.Trim() + parametro.valorString.Trim() + fileNameString.Trim();

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

        [HttpGet]
        public void ExcelDetalle(int id)
        {

            Pago pago = db.Pagos.Where(p => p.id.Equals(id)).FirstOrDefault();

            List<DetallePago> detallePago = db.DetallePagos.Where(r => r.pagoId.Equals(id)).ToList();

            List<DetallePago> allCust = new List<DetallePago>();

            allCust = detallePago;

            WebGrid grid = new WebGrid(source: allCust, canPage: false, canSort: false);

            List<WebGridColumn> gridColumns = new List<WebGridColumn>();

            gridColumns.Add(grid.Column("Pago.Patrone.registro", "Patrón", null, null, true));
            gridColumns.Add(grid.Column("Pago.mes", "Periodo", null, null, true));
            gridColumns.Add(grid.Column("Pago.anno", "Ejercicio", null, null, true));
            gridColumns.Add(grid.Column("Pago.fechaDeposito", "Fecha depósito", null, null, true));
            gridColumns.Add(grid.Column("Pago.imss", "IMSS", null, null, true));
            gridColumns.Add(grid.Column("Pago.rcv", "RCV", null, null, true));
            gridColumns.Add(grid.Column("Pago.infonavit", "Infonavit", null, null, true));
            gridColumns.Add(grid.Column("Pago.total", "Total", null, null, true));
            gridColumns.Add(grid.Column("Asegurado.numeroAfiliacion", "NSS", null, null, true));
            gridColumns.Add(grid.Column("Asegurado.nombreTemporal", "Nombre", null, null, true));
            gridColumns.Add(grid.Column("diasCotizados", "Dias", null, null, true));
            gridColumns.Add(grid.Column("sdi", "S.D.I.", null, null, true));
            gridColumns.Add(grid.Column("diasIncapacidad", "Inc.", null, null, true));
            gridColumns.Add(grid.Column("diasAusentismo", "Aus.", null, null, true));
            gridColumns.Add(grid.Column("cuotaFija", "C.F.", null, null, true));
            gridColumns.Add(grid.Column("expa", "Ex.P", null, null, true));
            gridColumns.Add(grid.Column("exo", "Ex. O.", null, null, true));
            gridColumns.Add(grid.Column("Asegurado.Cliente.claveCliente", "Ubicación", null, null, true));
            gridColumns.Add(grid.Column("PDP", "PDP", null, null, true));
            gridColumns.Add(grid.Column("GMPP", "GMP. Patron", null, null, true));
            gridColumns.Add(grid.Column("GMPO", "GMP. Obrero", null, null, true));
            gridColumns.Add(grid.Column("rt", "R.T.", null, null, true));
            gridColumns.Add(grid.Column("ivp", "I.V.P", null, null, true));
            gridColumns.Add(grid.Column("ivo", "I.V.O", null, null, true));
            gridColumns.Add(grid.Column("gps", "G.P.S.", null, null, true));
            gridColumns.Add(grid.Column("patronal", "Patronal", null, null, true));
            gridColumns.Add(grid.Column("obrera", "Obrera", null, null, true));
            gridColumns.Add(grid.Column("imss", "IMSS", null, null, true));
            gridColumns.Add(grid.Column("retiro", "Retiro", null, null, true));
            gridColumns.Add(grid.Column("patronalBimestral", "Patronal Bim", null, null, true));
            gridColumns.Add(grid.Column("obreraBimestral", "Obrera Bim", null, null, true));
            gridColumns.Add(grid.Column("rcv", "R.C.V.", null, null, true));
            gridColumns.Add(grid.Column("aportacionsc", "Aportacion SC", null, null, true));
            gridColumns.Add(grid.Column("aportacioncc", "Aportacion CC", null, null, true));
            gridColumns.Add(grid.Column("amortizacion", "Amortizacion", null, null, true));

            string gridData = grid.GetHtml(
                columns: grid.Columns(gridColumns.ToArray())
                    ).ToString();

            Response.ClearContent();
            DateTime date = DateTime.Now;
            String fileName = "DetallePagos-" + date.ToString("ddMMyyyyHHmm") + ".xls";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();
        }



        [HttpGet]
        public void GetExcel(String plazasId, String patronesId, String periodoId, String ejercicioId)
        {
            var pagos = db.Pagos.ToList();

            if (!String.IsNullOrEmpty(plazasId))
            {
                int plazaTempId = int.Parse(plazasId.Trim());
                pagos = pagos.Where(s => s.Patrone.Plaza_id.Equals(plazaTempId)).ToList();
            }
            if (!String.IsNullOrEmpty(patronesId))
            {
                int patronesTempId = int.Parse(patronesId);
                pagos = pagos.Where(s => s.Patrone.Id.Equals(patronesTempId)).ToList();
            }
            if (!String.IsNullOrEmpty(periodoId))
            {
                pagos = pagos.Where(s => s.mes.Trim().Equals(periodoId.Trim())).ToList();
            }
            if (!String.IsNullOrEmpty(ejercicioId))
            {
                pagos = pagos.Where(s => s.anno.Trim().Equals(ejercicioId)).ToList();
            }

            pagos = pagos.OrderBy(p => p.Patrone.registro).ToList();
            List<Pago> allCust = new List<Pago>();

            allCust = pagos.ToList();

            WebGrid grid = new WebGrid(source: allCust, canPage: false, canSort: false);

            List<WebGridColumn> gridColumns = new List<WebGridColumn>();

            gridColumns.Add(grid.Column("Patrone.registro", "Reg. Patronal", null, null, true));
            gridColumns.Add(grid.Column("Patrone.nombre", "ID. Empresa", null, null, true));
            gridColumns.Add(grid.Column("mes", "Mes", null, null, true));
            gridColumns.Add(grid.Column("anno", "Año", null, null, true));
            gridColumns.Add(grid.Column("imss", "IMSS", null, null, true));
            gridColumns.Add(grid.Column("rcv", "RCV", null, null, true));
            gridColumns.Add(grid.Column("infonavit", "Infonavit", null, null, true));
            gridColumns.Add(grid.Column("total", "Total", null, null, true));
            gridColumns.Add(grid.Column("recargos", "Recargos", null, null, true));
            gridColumns.Add(grid.Column("actualizaciones", "Actualizaciones", null, null, true));
            gridColumns.Add(grid.Column("granTotal", "Gran Total", null, null, true));
            gridColumns.Add(grid.Column("fechaDeposito", "F.Deposito", format: (item) => item.fechaDeposito != null ? String.Format("{0:yyyy-MM-dd}", item.fechaDeposito) : String.Empty));
            gridColumns.Add(grid.Column("bancoId", "Banco", format: (item) => item.bancoid != null ? String.Format("{0,11:S}", item.Banco.descripcion) : String.Empty));
            gridColumns.Add(grid.Column("nt", "NT", null, null, true));
            gridColumns.Add(grid.Column("Patrone.Plaza.Descripcion", "Localidad SUA", null, null, true));

            string gridData = grid.GetHtml(
                columns: grid.Columns(gridColumns.ToArray())
                    ).ToString();

            Response.ClearContent();
            DateTime date = DateTime.Now;
            String fileName = "Pagos-" + date.ToString("ddMMyyyyHHmm") + ".xls";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/excel";
            Response.Write(gridData);
            Response.End();
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
