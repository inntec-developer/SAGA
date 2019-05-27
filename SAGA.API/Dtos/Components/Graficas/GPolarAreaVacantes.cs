using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class VacantesReclutador
    {
        public int Vigentes { get; set; }
        public int PorVencer { get; set; }
        public int Vencidas { get; set; }
    }
}