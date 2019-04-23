using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class PonderacionRequisiciones
    {
        public Guid Id { get; set; }
        public int Ponderacion { get; set; }
        public Guid RequisicionId { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }

        public virtual Requisicion Requisicion { get; set; }
    }
}
