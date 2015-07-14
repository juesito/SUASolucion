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

        public Asegurado obtenerAseguradoPorNSS(String NSS)
        {
            Asegurado asegurado = db.Asegurados.Where(s => s.numeroAfiliacion.Trim().Equals(NSS)).First();

            return asegurado;
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


        /**
         * Cargamos archivo modificando el nombre del archivo
         * 
         */ 
        public String cargarArchivo(HttpFileCollectionBase files, String destino, String nombreArchivo)
        {

            String path = "";
            String msg = "";

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
                msg = "Se ha cargado el archivo con exito!";
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
            String msg = "";

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
                msg = "Se ha cargado el archivo con exito!";
            }


            return fileName;
        }

    }
}