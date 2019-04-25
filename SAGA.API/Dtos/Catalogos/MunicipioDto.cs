using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class MunicipioDto
    {
        public int Id { get; set; }
        public string municipio { get; set; }
       // public int EstadoId { get; set; }
        public string Estado { get; set; }
        public bool Activo { get; set; }
    }
}