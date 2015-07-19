using SUADATOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SUAMVC.Models
{
    public class SolicitudEmpleadoModel
    {

        public Solicitud solicitud { get; set; }
        public List<Empleado> empleados { get; set; }
    }
}