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
    
    public partial class Movimiento
    {
        public int id { get; set; }
        public Nullable<int> aseguradoId { get; set; }
        public Nullable<int> acreditadoId { get; set; }
        public string lote { get; set; }
        public System.DateTime fechaTransaccion { get; set; }
        public string tipo { get; set; }
        public string nombreArchivo { get; set; }
        public Nullable<System.DateTime> fechaCreacion { get; set; }
    
        public virtual Acreditado Acreditado { get; set; }
        public virtual Asegurado Asegurado { get; set; }
    }
}
