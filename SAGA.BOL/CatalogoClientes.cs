using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class CatalogoClientes
    {
        public Guid Id { get; set; }
        public int CatalogosId { get; set; }
        public Guid ClienteId { get; set; }
        public bool Activo { get; set; }
        public string Observaciones { get; set; }
        public DateTime fchAlta { get; set; }
        public Guid UsuarioAlta { get; set; }
        public DateTime fchModificacion { get; set; }
        public Guid UsuarioMod { get; set; }

        public Catalogos Catalogos { get; set; }
        public Cliente Cliente { get; set; }
    }
}
