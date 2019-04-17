using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class AsignarVacanteReclutador
    {
        public Guid Id { get; set; }
        public DateTime fch_Cumplimiento { get; set; }
        public string Aprobador { get; set; }
        public Guid AprobadorId { get; set; }
        public int? DiasEnvio { get; set; }
        public string Usuario { get; set; }
        public virtual List<AsignacionRequi> AsignacionRequi { get; set; }
    }

    public class PublicarRedesSocialesDto
    {
        public Guid Id { get; set; }
        public Guid RequisicionId { get; set; }
        public string Oficio { get; set; }
    }
}