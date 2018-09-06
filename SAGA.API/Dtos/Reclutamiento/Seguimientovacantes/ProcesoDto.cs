using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos.Reclutamiento.Seguimientovacantes
{
    public class ProcesoDto
    {
        public Guid CandidatoId { get; set; }
        public int EstatusId { get; set; }
    }
}