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
    
    public partial class Acreditado
    {
        public Acreditado()
        {
            this.Movimientos = new HashSet<Movimiento>();
        }
    
        public int id { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string nombre { get; set; }
        public string nombreCompleto { get; set; }
        public string CURP { get; set; }
        public string RFC { get; set; }
        public Nullable<int> clienteId { get; set; }
        public string ocupacion { get; set; }
        public string idGrupo { get; set; }
        public string numeroAfiliacion { get; set; }
        public string numeroCredito { get; set; }
        public System.DateTime fechaAlta { get; set; }
        public Nullable<System.DateTime> fechaBaja { get; set; }
        public Nullable<System.DateTime> fechaInicioDescuento { get; set; }
        public Nullable<System.DateTime> fechaFinDescuento { get; set; }
        public double smdv { get; set; }
        public double sdi { get; set; }
        public decimal sd { get; set; }
        public decimal vsm { get; set; }
        public decimal porcentaje { get; set; }
        public decimal cuotaFija { get; set; }
        public decimal descuentoBimestral { get; set; }
        public decimal descuentoMensual { get; set; }
        public decimal descuentoSemanal { get; set; }
        public decimal descuentoCatorcenal { get; set; }
        public decimal descuentoQuincenal { get; set; }
        public decimal descuentoVeintiochonal { get; set; }
        public decimal descuentoDiario { get; set; }
        public string acuseRetencion { get; set; }
        public int PatroneId { get; set; }
        public int Plaza_id { get; set; }
        public Nullable<System.DateTime> fechaCreacion { get; set; }
        public Nullable<System.DateTime> fechaModificacion { get; set; }
        public string alta { get; set; }
        public string baja { get; set; }
        public string modificacion { get; set; }
        public string permanente { get; set; }
        public Nullable<System.DateTime> fechaUltimoCalculo { get; set; }
    
        public virtual Cliente Cliente { get; set; }
        public virtual Patrone Patrone { get; set; }
        public virtual Plaza Plaza { get; set; }
        public virtual Acreditado Acreditados1 { get; set; }
        public virtual Acreditado Acreditado1 { get; set; }
        public virtual ICollection<Movimiento> Movimientos { get; set; }
    }
}
