using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos.Admin
{
    public class ArteDto
    {
        public Guid requisicionId { get; set; }
        public string arte { get; set; }
        public Guid usuarioId { get; set; }

    }
}