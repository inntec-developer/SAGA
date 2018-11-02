using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class ComentariosRequisicionesDto
    {
        public Guid Id { get; set; }
        public Guid RequisicionId { get; set; }
        public string Comentario { get; set; }
        public Guid RespuestaId { get; set; }
        public string UsuarioAlta { get; set; }
        public Guid ReclutadorId { get; set; }
        public int MotivoId { get; set; }
    }
}