using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class Transferencias
    {
        [Key]
        public int Id { get; set; }
        public Guid antId { get; set; }
        public Guid actId { get; set; }
        public Guid requisicionId { get; set; }
        public int tipoTransferenciaId { get; set; }
        public DateTime fch_Modificacion { get; set; }

        public virtual TiposTransferencias TipoTransferencia { get; set; }

    }
}
