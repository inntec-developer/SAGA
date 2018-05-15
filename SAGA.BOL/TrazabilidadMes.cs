using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class TrazabilidadMes
    {
        [key]
        public Guid Id { get; set; }
        public int TipoMovimientoId { get; set; }
        public int Folio { get; set; }
        public DateTime fch_Creacion { get; set; }
        public string UsuarioAlta { get; set; }
        public Guid UsuarioId { get; set; }

        public TipoMovimiento TipoMovimiento { get; set; }
        public Usuarios Usuario { get; set; }
    }
}
