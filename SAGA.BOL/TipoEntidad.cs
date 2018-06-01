using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class TipoEntidad
    {
        [key]
        public int Id { get; set; }
        public string tipoEntidad { get; set; }
        public bool Activo { get; set; } = true;
    }
}
