using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos.Examenes
{
    public class EntrevistaDto
    {
        public List<PreguntaDto> Preguntas { get; set; }
        public string Nombre { get; set; }
        public Guid usuarioId { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public string Descripcion { get; set; }
    }

}