using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class CostosDamfo290
    {
        [Key]
        public Guid Id { get; set; }
        public int TipoCostosId { get; set; }
        public decimal Costo { get; set; }
        public Guid DAMFO290Id { get; set; }

        public TipoCostos TipoCostos { get; set; }
        public DAMFO_290 DAMFO290 { get; set;  }
    }
}
