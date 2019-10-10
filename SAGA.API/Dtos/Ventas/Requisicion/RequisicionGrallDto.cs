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
        public string TipoReclutamiento { get; set; }
        public int TipoReclutamientoId { get; set; }
        public string ClaseReclutamiento { get; set; }
        public int ClaseReclutamientoId { get; set; }
        public decimal SueldoMinimo { get; set; }
        public decimal SueldoMaximo { get; set; }
        public DateTime? fch_Creacion { get; set; }
        public DateTime? fch_Cumplimiento { get; set; }
        public DateTime? fch_Modificacion { get; set; }
        public string Estatus { get; set; }
        public int EstatusId { get; set; }
        public string Prioridad { get; set; }
        public int PrioridadId { get; set; }
        public string Cliente { get; set; }
        public int Vacantes { get; set; }
        public SolicitanteDto Solicita { get; set; }
        public Int64 Folio { get; set; }
        public int? DiasEnvio { get; set; }
        public bool Confidencial { get; set; }
        public int Postulados { get; set; }
        public int EnProceso { get; set; }
        public int? EstadoId { get; set; }
        public int EstatusOrden { get; set; }
        public string razon { get; set; }
        public string factura { get; set; }
        public string GiroEmpresa { get; set; }
        public string ActividadEmpresa { get; set; }
        public float Porcentaje { get; set; }
        public string Propietario { get; set; }
        public List<string> ComentarioReclutador { get; set; }
        public int diasTrans { get; set; }
    }
}