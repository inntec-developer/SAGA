using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos.SistFirmas
{
    public class FirmasDto
    {
        public string file { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string ext { get; set; }
        public string cliente { get; set; }
        public string estatus { get; set; }
        public string email_envio { get; set; }
        public string subject { get; set; }
    }
}