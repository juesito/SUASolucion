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
    
    public partial class Pais
    {
        public Pais()
        {
            this.Estados = new HashSet<Estado>();
            this.Municipios = new HashSet<Municipio>();
        }
    
        public int id { get; set; }
        public string descripcion { get; set; }
        public string naturalez { get; set; }
        public System.DateTime fechaCreacion { get; set; }
        public int usuarioId { get; set; }
    
        public virtual ICollection<Estado> Estados { get; set; }
        public virtual ICollection<Municipio> Municipios { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
