using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
   public class Estructura
    {
        public int Id { get; set; }
        public int IdPadre { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Orden { get; set; }
        public bool Menu { get; set; }
        public bool Confidencial { get; set; }
        public bool Inclusivo { get; set; }
        public bool Activo { get; set; }
        public string Icono { get; set; }
        public string Accion { get; set; }
        public int TipoEstructuraId { get; set; }
        public int AmbitoId { get; set; }
        public int TipoMovimientoId { get; set; }

        public virtual TipoEstructura TipoEstructura { get; set; }
        public virtual Ambito Ambito { get; set; }
    }
}
