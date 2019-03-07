using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class EstatusRequiDto
    {
        public Guid RequisicionId { get; set; }
        public int diasTotal { get; set; }
        public int EstatusId { get; set; }
        public string Estatus { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Double diasTrans { get; set; }
    }
}