using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class DireccionContacto
    {
        public Guid Id { get; set; }
        public Guid DireccionId { get; set; }
        public Guid ContactoId { get; set; }

        public virtual Contacto Contacto { get; set; }
        public virtual Direccion Direccion { get; set; }
    }
}
