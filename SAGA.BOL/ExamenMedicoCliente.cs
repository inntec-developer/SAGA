using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class ExamenMedicoCliente
    {
        public int Id { get; set; }
        public Guid ClienteId { get; set; }
        public int TipoExamenMedicoId { get; set; }
        public decimal Costo { get; set; }
        public bool Activo { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion{ get; set; }

        public virtual Cliente Cliente { get; set; }
        public virtual TipoExamenMedico TipoExamenMedico { get; set; }
    }
}
