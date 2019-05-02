using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class TipoLicenciaDto
    {
        public byte Id { get; set; }
        public string tipoLicencia { get; set; }
        public string Descripcion { get; set; }
        public bool activo { get; set; }
    }
}