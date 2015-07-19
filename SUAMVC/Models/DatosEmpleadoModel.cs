using SUADATOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SUAMVC.Models
{
    public class DatosEmpleadoModel
    {
        public Empleado empleado { get; set; }
        public DocumentosEmpleado datosEmpleado { get; set; }
        public SalarialesEmpleado salarialesEmpleado { get; set; }
    }
}