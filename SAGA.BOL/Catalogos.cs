using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class Catalogos
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int EstructuraId { get; set; }
        public Boolean Activo { get; set; }

        public virtual Estructura Estructura { get; set; }
    }
}
