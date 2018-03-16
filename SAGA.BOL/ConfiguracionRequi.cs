using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class ConfiguracionRequi
    {
        public int ConfiguracionRequiID { get; set; }
        public int ConfiguracionID { get; set; }
        public int IdentificadorID { get; set; }
        public bool Resumen { get; set; }
        public bool Detalle { get; set; }
    }
}
