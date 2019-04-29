using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class DepartamentosDto
    {
        public Guid Id { get; set; }
        public string nombre { get; set; }
        public string Area { get; set; }
        public string clave { get; set; }
        public int orden { get; set; }
    }
}