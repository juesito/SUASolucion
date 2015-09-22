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
    
    public partial class DetallePrenomina
    {
        public int id { get; set; }
        public int solicitudId { get; set; }
        public int empleadoId { get; set; }
        public int diasLaborados { get; set; }
        public Nullable<int> totalSyS { get; set; }
        public Nullable<int> totalIAS { get; set; }
        public Nullable<int> horasExtra { get; set; }
        public decimal ingresos { get; set; }
        public decimal gratificacion { get; set; }
        public Nullable<decimal> primaVacacional { get; set; }
        public Nullable<decimal> aguinaldo { get; set; }
        public Nullable<decimal> descuentoInfonavit { get; set; }
        public Nullable<decimal> descuentoFonacot { get; set; }
        public Nullable<decimal> descuentoPension { get; set; }
        public Nullable<decimal> reembolso { get; set; }
        public Nullable<decimal> otrosDescuentos { get; set; }
        public Nullable<decimal> netoPagar { get; set; }
        public Nullable<int> cuentaId { get; set; }
        public System.DateTime fechaCreacion { get; set; }
        public Nullable<int> usuarioId { get; set; }
        public Nullable<decimal> vacaciones { get; set; }
        public Nullable<decimal> premioPuntualidad { get; set; }
        public Nullable<decimal> premioAsistencia { get; set; }
        public Nullable<decimal> uniforme { get; set; }
        public Nullable<decimal> compensacion { get; set; }
        public Nullable<decimal> cajaAhorro { get; set; }
        public Nullable<decimal> viaticos { get; set; }
        public Nullable<decimal> comida { get; set; }
    
        public virtual CuentaEmpleado CuentaEmpleado { get; set; }
        public virtual Empleado Empleado { get; set; }
        public virtual SolicitudPrenomina SolicitudPrenomina { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
