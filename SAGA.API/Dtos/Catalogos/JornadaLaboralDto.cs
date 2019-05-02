using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class JornadaLaboralDto
    {
        public int Id { get; set; }
        public string Jornada { get; set; }
        public int Orden { get; set; }
        public bool VariosHorarios { get; set; }
        public bool activo { get; set; }
    }
}