using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class DiasSemanaDto
    {
        public byte Id { get; set; }
        public string diaSemana { get; set; }
        public int tipo { get; set; }
        public bool activo { get; set; }
    }
}