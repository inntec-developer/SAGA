using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class CalendarioCandidato
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UbicacionId { get; set; }
        public Guid CandidatoId { get; set; }
        public DateTime Fecha { get; set; }
        public int Estatus { get; set; }
        public int Folio { get; set; }
        public Guid RequisicionId { get; set; }

    }
}
