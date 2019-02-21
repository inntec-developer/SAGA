using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class UnidadNegocio
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }

        public ICollection<OficinaReclutamiento> OficinasReclutamiento { get; set; }
    }
}
