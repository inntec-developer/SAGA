using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class FacturacionPuro
    {
        [Key]
        public Guid Id { get; set; }
        public Guid RequisicionId { get; set; }
        public float Porcentaje { get; set; }
        public Decimal Monto { get; set; }
        public float PerContratado { get; set; }
        public Decimal MontoContratado { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }

        public Requisicion Requisicion { get; set; }
    }
}
