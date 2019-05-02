using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class TiposPiscoDto
    {
        public int Id { get; set; }
        public string tipoPsicometria { get; set; }
        public string descripcion { get; set; }
        public bool activo { get; set; }
    }
}