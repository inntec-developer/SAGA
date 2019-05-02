using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class AreaInteresDto
    {
        public int Id { get; set; }
        public string areaInteres { get; set; }
        public string AreaExperiencia { get; set; }
        public bool Activo { get; set; }
    }
}