using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class SoportePuestos
    {
        [Key]
        public int Id { get; set; }
        public int puestoId { get; set; }
        public Puesto Puesto { get; set; }
        public int SoporteFacturacionId { get; set; }
        public SoporteFacturacion SoporteFacturacion { get; set; }
    }
}
