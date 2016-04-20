using SUADATOS;
using System;
using System.Collections.Generic;

namespace SUAMVC.Models
{
    public class ArmadoPrenominaModel
    {
        public EsquemasPago tipoPago { get; set; }
        public List<ConceptosPrenómina> conceptos { get; set; }
        public List<ConceptosPrenómina> conceptosPorTipo { get; set; }
     
    }
}