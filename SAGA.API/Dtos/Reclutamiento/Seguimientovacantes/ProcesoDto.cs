using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos.Reclutamiento.Seguimientovacantes
{
    public class ProcesoDto
    {
        public Guid candidatoId { get; set; }
        public int estatusId { get; set; }
        public Guid requisicionId { get; set; }
        public string vacante { get; set; }
        public string nombre { get; set; }
        public string email { get; set; }
    }
}