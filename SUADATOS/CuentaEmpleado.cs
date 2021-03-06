//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SUADATOS
{
    using System;
    using System.Collections.Generic;
    
    public partial class CuentaEmpleado
    {
        public CuentaEmpleado()
        {
            this.DetallePrenominas = new HashSet<DetallePrenomina>();
        }
    
        public int id { get; set; }
        public int empleadoId { get; set; }
        public int bancoId { get; set; }
        public string cuenta { get; set; }
        public string cuentaClabe { get; set; }
        public System.DateTime fechaCreacion { get; set; }
        public int usuarioId { get; set; }
    
        public virtual Banco Banco { get; set; }
        public virtual Empleado Empleado { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual ICollection<DetallePrenomina> DetallePrenominas { get; set; }
    }
}
