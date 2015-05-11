using SUADATOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SUAMVC.Models
{
    public class SecurityUserModel
    {
        private suaEntities db = new suaEntities();

        public Boolean tienesPermiso(int modulo) {
            Boolean esPermitido = false;

            return esPermitido;
        }
    }
}