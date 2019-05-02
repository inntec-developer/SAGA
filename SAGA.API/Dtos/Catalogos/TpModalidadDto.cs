using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class TpModalidadDto
    {
        public int Id { get; set; }
        public string Modalidad { get; set; }
        public int Orden { get; set; }
        public bool activo { get; set; }
    }
}