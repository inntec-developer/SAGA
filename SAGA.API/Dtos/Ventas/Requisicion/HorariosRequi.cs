using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class HorariosRequi
    {
        public Guid Id { get; set; }
        public byte numeroVacantes { get; set; }
        public string Usuario { get; set; }
        public Guid RequisicionId { get; set; }
        public DateTime? fch_Modificacion { get; set; }
    }
}