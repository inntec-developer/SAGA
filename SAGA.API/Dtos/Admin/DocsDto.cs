using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class DocsDto
    {
        public Guid id { get; set; }
        public int documentoId { get; set; }
        public string descripcion { get; set; }
        public Guid candidatoId { get; set; }
        public Guid usuarioId { get; set; }
        public string ruta { get; set; }
    }
}