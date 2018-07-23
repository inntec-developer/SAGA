using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class RequisicionGrallDto
    {
        public Guid Id { get; set; }
        public string VBtra { get; set; }
        public TipoReclutamiento TipoReclutamiento { get; set; }
        public ClaseReclutamiento ClaseReclutamiento { get; set; }
        public decimal SueldoMinimo { get; set; }
        public decimal SueldoMaximo { get; set; }
        public DateTime? fch_Creacion { get; set; }
        public DateTime? fch_Cumplimiento { get; set; }
        public Estatus Estatus { get; set; }
        public Prioridad Prioridad { get; set; }
        public ClienteRequiDto Cliente { get; set; }
        public int Vacantes { get; set; }
        public SolicitanteDto Solicita { get; set; }
        public Int64 Folio { get; set; }
        public int? DiasEnvio { get; set; }
        public bool Confidencial { get; set; }
        public int Postulados { get; set; }
        public int EnProceso { get; set; }
    }

    
}