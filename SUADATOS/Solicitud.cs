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
    
    public partial class Solicitud
    {
        public Solicitud()
        {
            this.Empleados = new HashSet<Empleado>();
        }
    
        public int id { get; set; }
        public string folioSolicitud { get; set; }
        public int clienteId { get; set; }
        public int residenciaId { get; set; }
        public System.DateTime fechaSolicitud { get; set; }
        public int esquemaId { get; set; }
        public int sdiId { get; set; }
        public int contratoId { get; set; }
        public System.DateTime fechaInicial { get; set; }
        public System.DateTime fechaFinal { get; set; }
        public int tipoPersonalId { get; set; }
        public string solicita { get; set; }
        public string valida { get; set; }
        public string autoriza { get; set; }
        public int noTrabajadores { get; set; }
        public string observaciones { get; set; }
        public string estatusSolicitud { get; set; }
        public string estatusNomina { get; set; }
        public string estatusAfiliado { get; set; }
        public string estatusJuridico { get; set; }
        public string estatusTarjeta { get; set; }
        public int usuarioId { get; set; }
        public int proyectoId { get; set; }
        public Nullable<System.DateTime> fechaEnvio { get; set; }
    
        public virtual Cliente Cliente { get; set; }
        public virtual ICollection<Empleado> Empleados { get; set; }
        public virtual EsquemasPago EsquemasPago { get; set; }
        public virtual Proyecto Proyecto { get; set; }
        public virtual Residencia Residencia { get; set; }
        public virtual SDI SDI { get; set; }
        public virtual TipoContrato TipoContrato { get; set; }
        public virtual TipoPersonal TipoPersonal { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
