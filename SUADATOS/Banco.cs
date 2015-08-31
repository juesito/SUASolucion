//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SUADATOS
{
    using System;
    using System.Collections.Generic;
    
    public partial class Banco
    {
        public Banco()
        {
            this.CuentaEmpleadoes = new HashSet<CuentaEmpleado>();
            this.Empleados = new HashSet<Empleado>();
            this.Pagos = new HashSet<Pago>();
        }
    
        public int id { get; set; }
        public string descripcion { get; set; }
        public System.DateTime fechaCreacion { get; set; }
        public int usuarioId { get; set; }
    
        public virtual Usuario Usuario { get; set; }
        public virtual ICollection<CuentaEmpleado> CuentaEmpleadoes { get; set; }
        public virtual ICollection<Empleado> Empleados { get; set; }
        public virtual ICollection<Pago> Pagos { get; set; }
    }
}
