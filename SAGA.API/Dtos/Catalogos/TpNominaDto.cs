using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class TpNominaDto
    {
        public int Id { get; set; }
        public string tipoDeNomina { get; set; }
        public string clave { get; set; }
        public bool activo { get; set; }
    }
}