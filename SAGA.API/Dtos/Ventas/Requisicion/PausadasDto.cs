using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class PausadasDto
    {
        public Guid Id { get; set; }
        public int dias { get; set; }
        public string email { get; set; }
        public string emailSol { get; set; }
        public string VBtra { get; set; }
        public Int64 Folio { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Aprobacion { get; set; }
        public DateTime fch_Cumplimiento { get; set; }
        public string Cliente { get; set; }
        public string solicitante { get; set; }
        public Guid propietarioId { get; set; }
        public string aprobador { get; set; }
        public Guid aprobadorId { get; set; }
        public string estatus { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public int vacantes { get; set; }
        public int cubiertas { get; set; }
        
    }
}