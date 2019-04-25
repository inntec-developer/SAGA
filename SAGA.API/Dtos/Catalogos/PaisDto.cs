using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class PaisDto
    {
        public int id { get; set; }
        public string pais { get; set; }
        public bool activo { get; set; }
    }
}