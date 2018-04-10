using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class ConfiguracionRequi
    {
        public int id { get; set; }
        public Guid IdRequi { get; set; }
        public int IdEstructura { get; set; }
        public string Campo { get; set; }
        public int R_D { get; set; }
        public bool Resumen { get; set; }
        public bool Detalle { get; set; }
    }
}
