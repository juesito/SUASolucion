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
    
    public partial class SalarialesEmpleado
    {
        public int id { get; set; }
        public int empleadoId { get; set; }
        public Nullable<decimal> salarioMensual { get; set; }
        public Nullable<decimal> salarioHrsExtra { get; set; }
        public Nullable<int> descuentos { get; set; }
        public Nullable<decimal> montoInfonavit { get; set; }
        public string creditoFonacot { get; set; }
        public Nullable<decimal> importeFonacot { get; set; }
        public Nullable<int> fonacotDescuentos { get; set; }
        public string numeroPrestamo { get; set; }
        public Nullable<decimal> importePrestamo { get; set; }
        public Nullable<decimal> prestamoDescuentos { get; set; }
        public Nullable<decimal> porcientoPension { get; set; }
        public Nullable<decimal> importePension { get; set; }
        public Nullable<int> periodoId { get; set; }
        public System.DateTime fechaCreacion { get; set; }
        public int usuarioId { get; set; }
    
        public virtual Concepto Concepto { get; set; }
        public virtual Empleado Empleado { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
