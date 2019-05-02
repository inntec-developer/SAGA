using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class PeriodoPagoDto
    {
        public int Id { get; set; }
        public string periodoPago { get; set; }
        public bool activo { get; set; }
    }
}