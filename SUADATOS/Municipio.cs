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
    
    public partial class Municipio
    {
        public Municipio()
        {
            this.Empleados = new HashSet<Empleado>();
        }
    
        public int id { get; set; }
        public int paisId { get; set; }
        public int estadoId { get; set; }
        public string entidad { get; set; }
        public string municipio1 { get; set; }
        public string descripcion { get; set; }
        public string tipo { get; set; }
        public string nomTipo { get; set; }
        public string region { get; set; }
        public string gradoMarginacion { get; set; }
        public System.DateTime fechaCreacion { get; set; }
        public int usuarioId { get; set; }
        public string municipio11 { get; set; }
        public int usuaNvarioId { get; set; }
    
        public virtual ICollection<Empleado> Empleados { get; set; }
        public virtual Estado Estado { get; set; }
        public virtual Pais Pais { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual UsuaNvario UsuaNvario { get; set; }
    }
}
