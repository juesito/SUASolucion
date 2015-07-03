using SUADATOS;
using System;
using System.Collections.Generic;
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
        public Concepto obtenerConceptoPorGrupo(String grupoId, String value){
            Concepto concepto = db.Conceptos.Where(s => s.grupo.ToLower().Trim().Equals(grupoId.ToLower().Trim())
                && s.descripcion.ToLower().Trim().Equals(value.ToLower().Trim())).First();

            return concepto;
        }

        public Asegurado obtenerAseguradoPorNSS(String NSS) {
            Asegurado asegurado = db.Asegurados.Where(s => s.numeroAfiliacion.Trim().Equals(NSS)).First();

            return asegurado;
        }

        public Acreditado obtenerAcreditadoPorNSS(String NSS)
        {
            Acreditado acreditado = db.Acreditados.Where(s => s.numeroAfiliacion.Trim().Equals(NSS)).First();

            return acreditado;
        }

    }
}