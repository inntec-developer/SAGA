using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class TipoAccion
    {
        [key]
        public int Id { get; set; }
        public string Clave { get; set; }
        public string Descripcion { get; set; }
        public int Orden { get; set; }
    }
}
