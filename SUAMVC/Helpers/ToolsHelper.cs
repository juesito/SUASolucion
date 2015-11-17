using SUADATOS;
using SUAMVC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SUAMVC.Helpers
{
    public class ToolsHelper
    {
        suaEntities db = new suaEntities();

        //<summary>
        //Obtenemos los conceptos por grupo y valor.
        //
        public Concepto obtenerConceptoPorGrupo(String grupoId, String value)
        {
            Concepto concepto = db.Conceptos.Where(s => s.grupo.ToLower().Trim().Equals(grupoId.ToLower().Trim())
                && s.descripcion.ToLower().Trim().Contains(value.ToLower().Trim())).FirstOrDefault();

            return concepto;
        }

        public Asegurado obtenerAseguradoPorNSS(String NSS)
        {
            Asegurado asegurado = db.Asegurados.Where(s => s.numeroAfiliacion.Trim().Equals(NSS)).FirstOrDefault();

            return asegurado;
        }

        public Empleado obtenerEmpleadoPorNSS(String NSS)
        {
            Empleado empleado = db.Empleados.Where(s => s.nss.Trim().Equals(NSS.Trim())).FirstOrDefault();

            return empleado;
        }

        public Acreditado obtenerAcreditadoPorNSS(String NSS)
        {
            var acreditadoTemp = db.Acreditados.Where(s => s.numeroAfiliacion.Trim().Equals(NSS)).FirstOrDefault();

            Acreditado acreditado = new Acreditado();
            if (acreditadoTemp != null)
            {
                acreditado = acreditadoTemp as Acreditado;
            }
            return acreditado;
        }

        public Sexo obtenerSexoPorDescripcion(String desc) {
            Sexo sexo = db.Sexos.Where(s => s.descripcion.ToLower().Trim().Equals(desc.ToLower().Trim())).FirstOrDefault();

            return sexo;
        
        }

        public EstadoCivil obtenerEstadoCivilPorDescripcion(String desc)
        {
            EstadoCivil estadoCivil = db.EstadoCivils.Where(s => s.descripcion.ToLower().Trim().Equals(desc.ToLower().Trim())).FirstOrDefault();

            return estadoCivil;

        }
        public Pais obtenerPaisPorDescripcion(String desc)
        {
            Pais pais = db.Paises.Where(s => s.descripcion.ToLower().Trim().Equals(desc.ToLower().Trim())).FirstOrDefault();

            return pais;

        }
        public Estado obtenerEstadoPorDescripcion(String desc)
        {
            Estado estado = db.Estados.Where(s => s.descripcion.ToLower().Trim().Equals(desc.ToLower().Trim())).FirstOrDefault();

            return estado;

        }
        public Municipio obtenerMunicipioPorDescripcion(String desc)
        {
            Municipio municipio = db.Municipios.Where(s => s.descripcion.ToLower().Trim().Equals(desc.ToLower().Trim())).FirstOrDefault();

            return municipio;

        }

        public Banco obtenerBancoPorDescripcion(String desc)
        {
            Banco banco = db.Bancos.Where(s => s.descripcion.Trim().ToLower().Equals(desc.Trim().ToLower())).FirstOrDefault();

            return banco;

        }

        public Boolean verificarEmpleadoPorNSSyCliente(String nss, int clienteId)
        {
            int existe = db.SolicitudEmpleadoes.Where(s => s.Empleado.nss.Trim().ToLower().Equals(nss.Trim().ToLower())
                                                 && s.Solicitud.clienteId.Equals(clienteId) 
                ).Count();

            return (existe > 0);

        }

        /**
         * Cargamos archivo modificando el nombre del archivo
         * 
         */
        public String cargarArchivo(HttpFileCollectionBase files, String destino, String nombreArchivo)
        {

            String path = "";

            ParametrosHelper parameterHelper = new ParametrosHelper();
            Parametro rutaParameter = parameterHelper.getParameterByKey("SUARUTA");

            var file = files[0];
            var fileName = "";

            if (file != null && file.ContentLength > 0)
            {

                if (!destino.Equals(""))
                {
                    path = Path.Combine(rutaParameter.valorString.Trim(), destino);
                    if (!System.IO.File.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }
                }
                else
                {
                    path = rutaParameter.valorString.Trim();
                }

                fileName = Path.GetFileName(file.FileName);
                var pathFinal = Path.Combine(path, nombreArchivo);
                file.SaveAs(pathFinal);
            }


            return fileName;
        }

        /**
         * Cargamos archivo sin modificar el nombre del archivo
         * 
         */
        public String cargarArchivo(HttpFileCollectionBase files, String destino)
        {

            String path = "";

            ParametrosHelper parameterHelper = new ParametrosHelper();
            Parametro rutaParameter = parameterHelper.getParameterByKey("SUARUTA");

            var file = files[0];
            var fileName = "";
            if (file != null && file.ContentLength > 0)
            {

                if (!destino.Equals(""))
                {
                    path = Path.Combine(rutaParameter.valorString.Trim(), destino);
                    if (!System.IO.File.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }
                }
                else
                {
                    path = rutaParameter.valorString.Trim();
                }

                fileName = Path.GetFileName(file.FileName);
                var pathFinal = Path.Combine(path, fileName);
                file.SaveAs(pathFinal);
            }


            return fileName;
        }

        public String getMimeType(String path)
        {
            String mimeType = "";

            String extension = Path.GetExtension(path);

            if (extension.Trim().Equals(".pdf"))
            {
                mimeType = "application/pdf";
            }
            else if (extension.Trim().Equals(".doc"))
            {
                mimeType = "application/msword";
            }
            else if (extension.Trim().Equals(".docx"))
            {
                mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            }
            else if (extension.Trim().Equals(".xls"))
            {
                mimeType = "application/vnd.ms-excel";
            }
            else if (extension.Trim().Equals(".xlsx"))
            {
                mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }
            else if (extension.Trim().Equals(".ppt"))
            {
                mimeType = "application/vnd.ms-powerpoint";
            }
            else if (extension.Trim().Equals(".pptx"))
            {
                mimeType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
            }
            else if (extension.Trim().Equals(".ppsx"))
            {
                mimeType = "application/vnd.openxmlformats-officedocument.presentationml.slideshow";
            }

            return mimeType;
        }

        public void BorrarArchivo(String fileName) {

            if (System.IO.File.Exists(fileName))
            {
                try
                {
                    System.IO.File.Delete(fileName);
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
            }
        }

        /**
         * Regresa el último movimiento del empleado
         * 
         */
       public MovimientosAsegurado obtenerUltimoMovto(String nss, String clienteId)
        {
           int cliente = int.Parse(clienteId);
            MovimientosAsegurado movimiento = db.MovimientosAseguradoes.Where(s => s.Asegurado.numeroAfiliacion.Trim().Equals(nss.Trim()) && s.Asegurado.Cliente.Id.Equals(cliente)).OrderByDescending(s => s.fechaInicio).FirstOrDefault();

            return movimiento;
        }

    }
}