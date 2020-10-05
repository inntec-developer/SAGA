using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class EmpleadosSoporte
    {
        [Key]
        public int Id { get; set; }
        public decimal Porcentaje { get; set; }
        public Guid CandidatosInfoId { get; set; }
        public CandidatosInfo candidatosInfo { get; set; }
        public int SoporteFacturacionId { get; set; }
        public SoporteFacturacion SoporteFacturacion { get; set; }
    }
}
