using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class HacerClienteDto
    {
        public Guid Id { get; set; }
        public string Razonsocial { get; set; }
        public string RFC { get; set; }
        public string Usuario { get; set; }
    }
}