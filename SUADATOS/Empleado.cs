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
    
    public partial class Empleado
    {
        public Empleado()
        {
            this.ArchivosEmpleados = new HashSet<ArchivosEmpleado>();
            this.CuentaEmpleadoes = new HashSet<CuentaEmpleado>();
            this.DocumentosEmpleadoes = new HashSet<DocumentosEmpleado>();
            this.FamiliaresEmpleadoes = new HashSet<FamiliaresEmpleado>();
            this.SalarialesEmpleadoes = new HashSet<SalarialesEmpleado>();
            this.SolicitudEmpleadoes = new HashSet<SolicitudEmpleado>();
        }
    
        public int id { get; set; }
        public int solicitudId { get; set; }
        public string folioEmpleado { get; set; }
        public string nss { get; set; }
        public Nullable<System.DateTime> fechaAltaImss { get; set; }
        public string apellidoMaterno { get; set; }
        public string apellidoPaterno { get; set; }
        public string nombre { get; set; }
        public string nombreCompleto { get; set; }
        public string rfc { get; set; }
        public string homoclave { get; set; }
        public string curp { get; set; }
        public int sexoId { get; set; }
        public Nullable<int> sdiId { get; set; }
        public Nullable<int> esquemaPagoId { get; set; }
        public decimal salarioReal { get; set; }
        public string categoria { get; set; }
        public int tieneInfonavit { get; set; }
        public string creditoInfonavit { get; set; }
        public int estadoCivilId { get; set; }
        public System.DateTime fechaNacimiento { get; set; }
        public int nacionalidadId { get; set; }
        public Nullable<int> estadoNacimientoId { get; set; }
        public Nullable<int> municipioNacimientoId { get; set; }
        public string calleNumero { get; set; }
        public string colonia { get; set; }
        public string edoMunicipio { get; set; }
        public string codigoPostal { get; set; }
        public int tramitarTarjeta { get; set; }
        public int bancoId { get; set; }
        public string cuentaBancaria { get; set; }
        public string cuentaClabe { get; set; }
        public string email { get; set; }
        public string observaciones { get; set; }
        public int usuarioId { get; set; }
        public System.DateTime fechaCreacion { get; set; }
        public string estatus { get; set; }
        public Nullable<System.DateTime> fechaBaja { get; set; }
        public Nullable<int> aseguradoId { get; set; }
        public string foto { get; set; }
    
        public virtual Acreditado Acreditado { get; set; }
        public virtual ICollection<ArchivosEmpleado> ArchivosEmpleados { get; set; }
        public virtual Banco Banco { get; set; }
        public virtual ICollection<CuentaEmpleado> CuentaEmpleadoes { get; set; }
        public virtual ICollection<DocumentosEmpleado> DocumentosEmpleadoes { get; set; }
        public virtual EsquemasPago EsquemasPago { get; set; }
        public virtual EstadoCivil EstadoCivil { get; set; }
        public virtual Estado Estado { get; set; }
        public virtual Municipio Municipio { get; set; }
        public virtual Pais Pais { get; set; }
        public virtual SDI SDI { get; set; }
        public virtual Sexo Sexo { get; set; }
        public virtual Solicitud Solicitud { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual ICollection<FamiliaresEmpleado> FamiliaresEmpleadoes { get; set; }
        public virtual ICollection<SalarialesEmpleado> SalarialesEmpleadoes { get; set; }
        public virtual ICollection<SolicitudEmpleado> SolicitudEmpleadoes { get; set; }
    }
}
