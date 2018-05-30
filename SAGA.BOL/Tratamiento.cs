using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class Tratamiento
    {
        [key]
        public int Id { get; set; }
        public string tratamiento { get; set; }
    }
}
