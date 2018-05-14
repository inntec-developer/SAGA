using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class Departamento
    {
        [key]
        public Guid Id { get; set; }
        public int AreaId { get; set; }
        public string Clave { get; set; }
        public string Nombre { get; set; }
        public int  Orden { get; set; }

        public virtual Area Area { get; set; }
    }
}
