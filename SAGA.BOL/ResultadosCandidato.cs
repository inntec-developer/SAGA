using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class ResultadosCandidato
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CandidatoId { get; set; }
        public int PreguntaId { get; set; }
        public string Value { get; set; }
        public Guid RespuestaId { get; set; }

        public Candidato Candidato { get; set; }
        public Preguntas Pregunta { get; set; }
        public Respuestas Respuesta { get; set; }

    }
}
