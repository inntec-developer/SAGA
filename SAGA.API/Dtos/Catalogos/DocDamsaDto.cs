using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class DocDamsaDto
    {
        public int Id { get; set; }
        public string documentoDamsa { get; set; }
        public bool activo { get; set; }
    }
}