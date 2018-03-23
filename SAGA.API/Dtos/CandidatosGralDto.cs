using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class CandidatosGralDto
    {
        public Guid Id { get; set; }
        public string Candidato { get; set; }
        public string CP { get; set; }
        public string Curp { get; set; }
        public string Rfc { get; set; }
        public string Nss { get; set; }
    }
}