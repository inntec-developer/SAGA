using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class TpTelefonosDto
    {
        public byte Id { get; set; }
        public string Tipo { get; set; }
        public Boolean Activo { get; set; }
    }
}