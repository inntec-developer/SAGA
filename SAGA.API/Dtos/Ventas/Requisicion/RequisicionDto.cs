using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class RequisicionDto
    {
        public int Folio { get; set; }
        public DateTime fch_Cumplimiento { get; set; }
        public int PrioridadId { get; set; }
        public int EstatusId { get; set; }
        public bool Confidencial { get; set; }
    }
}