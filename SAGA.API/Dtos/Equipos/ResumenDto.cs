using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos.Equipos
{
    public class ResumenDto
    {
        public Guid requisicionId { get; set; }
        public Guid reclutadorId { get; set; }
        public int vacantes { get; set; }
        public string nombre { get; set; }
        public int contratados { get; set; }
        public string vBtra { get; set; }
        public long folio { get; set; }
    }
}