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
    
    public partial class FamiliaresEmpleado
    {
        public int id { get; set; }
        public int empleadoId { get; set; }
        public int parentescoId { get; set; }
        public string nombre { get; set; }
        public string apellidoMaterno { get; set; }
        public string apellidoPaterno { get; set; }
        public string nombreCompleto { get; set; }
        public string telefonoCelular { get; set; }
        public string telefonoCasa { get; set; }
        public string email { get; set; }
        public System.DateTime fechaCreacion { get; set; }
        public int usuarioId { get; set; }
    
        public virtual Empleado Empleado { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual Concepto Concepto { get; set; }
    }
}
