using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class RelacionClientesSistemas
    {
        public Guid Id { get; set; }
        public string Clave_Empresa { get; set; }
        public string Clave_Razon { get; set; }
        public string Clave_Unica { get; set; }
        public string Usuario { get; set; }
    }
}
