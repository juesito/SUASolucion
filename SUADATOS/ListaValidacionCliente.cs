//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SUADATOS
{
    using System;
    using System.Collections.Generic;
    
    public partial class ListaValidacionCliente
    {
        public int id { get; set; }
        public int clienteId { get; set; }
        public string validador { get; set; }
        public string emailValidador { get; set; }
        public string autorizador { get; set; }
        public string emailAutorizador { get; set; }
        public string listaEmailAux { get; set; }
        public System.DateTime fechaCreacion { get; set; }
        public int usuarioId { get; set; }
    
        public virtual Cliente Cliente { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
