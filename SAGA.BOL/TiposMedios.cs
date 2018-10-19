using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class TiposMedios
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public int MediosId { get; set; }

        public virtual Medios Medios { get; set; }
    }
}
