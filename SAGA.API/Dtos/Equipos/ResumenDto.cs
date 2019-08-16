using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos.Equipos
{
    public class ResumenDto
    {
       
        public Guid reclutadorId { get; set; }
        public string nombre { get; set; }
        public string clave { get; set; }
        public int tipoUsuario { get; set; }
        public int totalCub { get; set; }
        public int totalPos { get; set; }
        public int totalFal { get; set; }
        public int totalCump { get; set; }
        public List<RequisDtos> requis { get; set; }
        public List<ResumenDto> resumen { get; set; }
    }

    public class RequisDtos
    {
        public Guid requisicionId { get; set; }
        public int vacantes { get; set; }
        public int contratados { get; set; }
        public string vBtra { get; set; }
        public long folio { get; set; }
    }

}