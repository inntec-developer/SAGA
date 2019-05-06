using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class TipoContratoDto
    {
        public int Id { get; set; }
        public string tipoContrato { get; set; }
        public bool periodoPrueba { get; set; }
        public bool activo { get; set; }
    }
}