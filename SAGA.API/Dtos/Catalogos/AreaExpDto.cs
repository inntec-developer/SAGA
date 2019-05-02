using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class AreaExpDto
    {
        public int Id { get; set; }
        public string areaExperiencia { get; set; }
        public bool Activo { get; set; }
        public string Icono { get; set; }
    }
}