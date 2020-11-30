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
        public Guid empresasId { get; set; }
        public int puestoId { get; set; }

        public Empresas Empresas { get; set; }
        public Puesto Puesto { get; set; }

    }
}
