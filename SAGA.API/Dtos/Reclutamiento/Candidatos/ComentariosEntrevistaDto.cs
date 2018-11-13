using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class ComentariosEntrevistaDto
    {
        public int Id { get; set; }
        public string Comentario { get; set; }
        public Guid CandidatoId { get; set; }
        public Guid RequisicionId { get; set; }
        public string Usuario { get; set; }
        public Guid UsuarioId { get; set; }
        public int MotivoId { get; set; }
        public Guid RespuestaId { get; set; }
        public int estatusId { get; set; }
       
    }
}