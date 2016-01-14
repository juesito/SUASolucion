using SUADATOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SUAMVC.Helpers
{
    public class LogHelper
    {
        suaEntities db = new suaEntities();

        //Guardamos el log de errores.
        public void saveLog(String campo, String error, String proceso, int usuarioId) {

            Log log = new Log();
            log.fechaEvento = DateTime.Now;
            log.campo = campo;
            log.error = error;
            log.proceso = proceso;
            log.usuarioId = usuarioId;

            db.Logs.Add(log);
            db.SaveChanges();
        }

    }
}