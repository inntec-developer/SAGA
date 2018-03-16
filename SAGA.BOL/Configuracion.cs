using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class Configuracion
    {
        public int ConfiguracionID { get; set; }
        public string NombreGeneral { get; set; }
        public string NombreDetalle { get; set; }
        public bool Activo { get; set; }
    }
}
