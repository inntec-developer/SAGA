using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class DireccionEmail
    {
        public Guid Id { get; set; }
        public Guid DireccionId { get; set; }
        public Guid EmailId { get; set; }

        public virtual Email Email { get; set; }
        public virtual Direccion Direccion { get; set; }
    }
}
