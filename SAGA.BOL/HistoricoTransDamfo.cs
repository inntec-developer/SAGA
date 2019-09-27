using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class HistoricoTransDamfo
    {
        public int Id { get; set; }
        public Guid DAMFO290Id { get; set; }
        public Guid PropietarioId { get; set; }
        public Guid TransferidoId { get; set; }
        public DateTime fch_Alta { get; set; }
        
        public Entidad Propietario { get; set; }
        public Entidad Transferido { get; set; }
    }
}
