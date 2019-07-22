using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class VertionSistem
    {
        public int Id { get; set; }
        public string Version { get; set; }
        public string Descripcion { get; set; }
        public DateTime fch_Creacion { get; set; }
        public bool Liberada { get; set; }
    }
}
