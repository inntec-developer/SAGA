using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class ArteRequi
    {
        public Guid Id { get; set; }
        public Guid RequisicionId { get; set; }
        public int TitulosArteId { get; set; }
        public string Contenido { get; set; }
        public string BG { get; set; }
        public string Ruta { get; set; }
        public bool Activo { get; set; }

        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid UsuarioAlta { get; set; }
        public Guid UsuarioMod { get; set; }

        public Requisicion Requisicion { get; set; }
        public TitulosArte TitulosArte { get; set; }
    }
}
