using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class RequisicionDeleteDto
    {
        public Guid Id { get; set; }
        public string UsuarioMod { get; set; }
    }
}