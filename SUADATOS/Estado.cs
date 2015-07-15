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
    
    public partial class Estado
    {
        public Estado()
        {
            this.Empleados = new HashSet<Empleado>();
            this.Municipios = new HashSet<Municipio>();
        }
    
        public int id { get; set; }
        public int paisId { get; set; }
        public string entidad { get; set; }
        public string descripcion { get; set; }
        public System.DateTime fechaCreacion { get; set; }
        public int usuarioId { get; set; }
        public int usuaNvarioId { get; set; }
    
        public virtual ICollection<Empleado> Empleados { get; set; }
        public virtual Pais Pais { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual ICollection<Municipio> Municipios { get; set; }
        public virtual UsuaNvario UsuaNvario { get; set; }
    }
}
