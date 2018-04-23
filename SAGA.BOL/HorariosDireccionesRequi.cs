using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class HorariosDireccionesRequi
    {
        [key]
        public Guid Id { get; set; }
        public Guid HorariosId { get; set; }
        public Guid DireccionesId { get; set; }
        public Guid RequisicionId { get; set; }
        public byte Vacantes { get; set; }

        public virtual HorarioRequi Horarios { get; set; }
        public virtual Direccion Direcciones { get; set; }
        public virtual Requisicion Requisiciones { get; set; }
        
    }
}
