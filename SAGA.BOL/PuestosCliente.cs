using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class PuestosCliente
    {
        [Key]
        public int Id { get; set; }
        public Guid ClienteId { get; set; }
        public Guid PuestosIngresosId { get; set; }

        public Cliente Cliente { get; set; }
        public PuestosIngresos PuestosIngresos { get; set; }

    }
}
