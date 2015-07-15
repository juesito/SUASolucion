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
    
    public partial class documentosEmpleado
    {
        public int id { get; set; }
        public int empleadoId { get; set; }
        public string actividades { get; set; }
        public string domicilioOficina { get; set; }
        public System.DateTime fechaAntiguedad { get; set; }
        public Nullable<decimal> salarioVSM { get; set; }
        public int jornadaLaboralId { get; set; }
        public Nullable<int> diasDescanso { get; set; }
        public decimal salarioNominal { get; set; }
        public Nullable<int> diasVacaciones { get; set; }
        public Nullable<int> diasAguinaldo { get; set; }
        public string otros { get; set; }
        public string telefono { get; set; }
        public string tipoSangre { get; set; }
        public System.DateTime fechaCreacion { get; set; }
        public int usuarioId { get; set; }
        public int usuaNvarioId { get; set; }
    
        public virtual Concepto Concepto { get; set; }
        public virtual Empleado Empleado { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual UsuaNvario UsuaNvario { get; set; }
    }
}
