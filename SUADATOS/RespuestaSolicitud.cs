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
    
    public partial class RespuestaSolicitud
    {
        public int id { get; set; }
        public int solicitudId { get; set; }
        public int departamentoId { get; set; }
        public int estatusId { get; set; }
        public string observaciones { get; set; }
        public int usuarioId { get; set; }
        public System.DateTime fechaCreacion { get; set; }
    
        public virtual Concepto Concepto { get; set; }
        public virtual Departamento Departamento { get; set; }
        public virtual Solicitud Solicitud { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
