using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class RequisicionDto
    {
        public Guid Id { get; set; }
        public Int64 Folio { get; set; }
        public DateTime fch_Cumplimiento { get; set; }
        public int PrioridadId { get; set; }
        public int EstatusId { get; set; }
        public bool Confidencial { get; set; }
        public string Usuario { get; set; }
        public virtual List<AsignacionRequi> AsignacionRequi { get; set; }

    }

    public class NewrequiInfo
    {
        public Guid Id { get; set; }
        public Int64 Folio { get; set; }
    }

    public class RequiUNDto
    {
        public string EstadoVacante { get; set; }
        public string UnidadNegocio { get; set; }
    }

    public class SendEmailNuevaRequiDto
    {
        public string Email { get; set; }
        public Int64 Folio { get; set; }
        public string VBtra { get; set; }
    }
}