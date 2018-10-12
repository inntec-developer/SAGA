using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class LiberarCandidatoDto
    {
        public Guid RequisicionId { get; set; }
        public Guid CandidatoId { get; set; }
        public Guid ReclutadorId { get; set; }
        public int MotivoId { get; set; }
        public Guid ProcesoCandidatoId { get; set; }
        public string Comentario { get; set; }
    }
}