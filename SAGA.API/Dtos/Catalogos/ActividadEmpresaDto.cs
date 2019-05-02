using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class ActividadEmpresaDto
    {
        public int Id { get; set; }
        public string GiroEmpresa { get; set; }
        public string actividadEmpresa { get; set; }
        public bool activo { get; set; }
    }
}