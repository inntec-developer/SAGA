using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class Respuestas
    {
        [Key]
        public Guid Id { get; set; }
        public int PreguntaId { get; set; }
        public string Respuesta { get; set; }
        public int Validacion { get; set; }
        public int Orden { get; set; }

        public Preguntas Pregunta { get; set; }

    }
}
