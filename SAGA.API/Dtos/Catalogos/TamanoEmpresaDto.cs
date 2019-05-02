using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class TamanoEmpresaDto
    {
        public int Id { get; set; }
        public string tamanoEmpresa { get; set; }
        public bool activo { get; set; }
    }
}