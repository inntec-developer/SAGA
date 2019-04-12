using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class EstadoDto
    {
        public int Id { get; set; }
        public string estado { get; set; }
        public string Clave { get; set; }
        public string Pais { get; set; }
    }
}