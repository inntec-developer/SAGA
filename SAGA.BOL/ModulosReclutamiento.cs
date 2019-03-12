using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class ModulosReclutamiento
    {
        public int Id { get; set; }
        public Guid SucursalId { get; set; }
        public string Modulo { get; set; }
        public int TipoModulo { get; set; }
        public string Descripcion { get; set; }

        public OficinaReclutamiento Sucursal { get; set; }
    }
}
