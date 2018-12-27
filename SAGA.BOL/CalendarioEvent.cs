using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class CalendarioEvent
    {
        [Key]
        public Guid Id { get; set; }
        public Guid EntidadId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool AllDay { get; set; }
        public string backgroundColor { get; set; } 
        public string borderColor { get; set; }
        public DateTime fch_Creacion { get; set; }
        public int TipoActividadId { get; set; }
        public bool Activo { get; set; }

        public Entidad Entidad { get; set; }
        public TipoActividadReclutador TipoActividad { get; set; }
    }
}
