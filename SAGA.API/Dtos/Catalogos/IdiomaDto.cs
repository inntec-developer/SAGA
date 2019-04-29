using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class IdiomaDto
    {
        public int Id { get; set; }
        public string idioma { get; set; }
        public Boolean activo { get; set; }
    }
}