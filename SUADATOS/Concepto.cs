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
    
    public partial class Concepto
    {
        public Concepto()
        {
            this.ArchivoEmpleadoes = new HashSet<ArchivoEmpleado>();
            this.DocumentoEmpleadoes = new HashSet<DocumentoEmpleado>();
            this.RespuestaSolicituds = new HashSet<RespuestaSolicitud>();
            this.Solicituds = new HashSet<Solicitud>();
            this.Solicituds1 = new HashSet<Solicitud>();
            this.Solicituds2 = new HashSet<Solicitud>();
            this.Solicituds3 = new HashSet<Solicitud>();
            this.Solicituds4 = new HashSet<Solicitud>();
            this.Solicituds5 = new HashSet<Solicitud>();
        }
    
        public int id { get; set; }
        public string grupo { get; set; }
        public string descripcion { get; set; }
        public System.DateTime fechaCreacion { get; set; }
        public int usuarioId { get; set; }
    
        public virtual ICollection<ArchivoEmpleado> ArchivoEmpleadoes { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual ICollection<DocumentoEmpleado> DocumentoEmpleadoes { get; set; }
        public virtual ICollection<RespuestaSolicitud> RespuestaSolicituds { get; set; }
        public virtual ICollection<Solicitud> Solicituds { get; set; }
        public virtual ICollection<Solicitud> Solicituds1 { get; set; }
        public virtual ICollection<Solicitud> Solicituds2 { get; set; }
        public virtual ICollection<Solicitud> Solicituds3 { get; set; }
        public virtual ICollection<Solicitud> Solicituds4 { get; set; }
        public virtual ICollection<Solicitud> Solicituds5 { get; set; }
    }
}
