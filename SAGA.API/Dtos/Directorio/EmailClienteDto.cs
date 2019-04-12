using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class EmailClienteDto
    {
        public Guid Id { get; set; }
        public string email { get; set; }
        public Guid EntidadId { get; set; }
        public string Usuario { get; set; }
        public Guid DireccionId { get; set; }
        public Guid IdDE { get; set; }
    }
}