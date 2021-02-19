using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class TipoPeriodos
    {
        public byte Id { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public byte Orden { get; set; }
        public string Comentarios { get; set; }
        public int Dias { get; set; }
        public byte Meses { get; set; }
        public DateTime fch_Creacion { get; set; }
        public Guid UsuarioAlta { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid UsuarioMod { get; set; }

    }
}
