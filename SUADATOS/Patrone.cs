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
    
    public partial class Patrone
    {
        public Patrone()
        {
            this.Acreditados = new HashSet<Acreditado>();
            this.Asegurados = new HashSet<Asegurado>();
            this.DetallePagoes = new HashSet<DetallePago>();
            this.Pagos = new HashSet<Pago>();
            this.ReporteConMeses = new HashSet<ReporteConMes>();
            this.ResumenPagoes = new HashSet<ResumenPago>();
            this.SumarizadoClientes = new HashSet<SumarizadoCliente>();
        }
    
        public int Id { get; set; }
        public string registro { get; set; }
        public string rfc { get; set; }
        public string nombre { get; set; }
        public string actividad { get; set; }
        public string domicilio { get; set; }
        public string municipio { get; set; }
        public string codigoPostal { get; set; }
        public string entidad { get; set; }
        public string telefono { get; set; }
        public string remision { get; set; }
        public string zona { get; set; }
        public string delegacion { get; set; }
        public string carEnt { get; set; }
        public int numeroDelegacion { get; set; }
        public string carDel { get; set; }
        public int numSub { get; set; }
        public decimal tipoConvenio { get; set; }
        public string convenio { get; set; }
        public string inicioAfiliacion { get; set; }
        public string patRep { get; set; }
        public string clase { get; set; }
        public string fraccion { get; set; }
        public string STyPS { get; set; }
        public int Plaza_id { get; set; }
        public string direccionArchivo { get; set; }
        public Nullable<int> porcentajeNomina { get; set; }
    
        public virtual ICollection<Acreditado> Acreditados { get; set; }
        public virtual ICollection<Asegurado> Asegurados { get; set; }
        public virtual ICollection<DetallePago> DetallePagoes { get; set; }
        public virtual ICollection<Pago> Pagos { get; set; }
        public virtual Plaza Plaza { get; set; }
        public virtual ICollection<ReporteConMes> ReporteConMeses { get; set; }
        public virtual ICollection<ResumenPago> ResumenPagoes { get; set; }
        public virtual ICollection<SumarizadoCliente> SumarizadoClientes { get; set; }
    }
}
