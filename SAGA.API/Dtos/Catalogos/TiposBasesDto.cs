using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class TiposBasesDto
    {
        public int Id { get; set; }
        public string tipoBase { get; set; }
        public bool activo { get; set; }
    }
}