using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class BeneficiosPerfilDto
    {
        public int Id { get; set; }
        public string tipoBeneficio { get; set; }
        public bool activo { get; set; }
    }
}