using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class DiscapacidadDto
    {
        public int Id { get; set; }
        public string tipoDiscapacidad { get; set; }
        public bool activo { get; set; }
    }
}