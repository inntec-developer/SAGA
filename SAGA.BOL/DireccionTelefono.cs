using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class DireccionTelefono
    {
        public Guid Id { get; set; }
        public Guid DireccionId { get; set; }
        public Guid TelefonoId { get; set; }

        public virtual Telefono Telefono { get; set; }
        public virtual Direccion Direccion { get; set; }
    }
}
