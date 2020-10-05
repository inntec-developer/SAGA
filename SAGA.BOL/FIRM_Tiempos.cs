using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class FIRM_Tiempos
    {
        [Key]
        public int Id { get; set; }
        public byte DiaSemanaId { get; set; }
        public DiaSemana DiaSemana { get; set; }
        public DateTime Hora { get; set; }
        public int EstatusBitacoraId { get; set; }
        public FIRM_EstatusBitacora EstatusBitacora { get; set; }
        public int ConfigBitacoraId { get; set; }
        public FIRM_ConfigBitacora ConfigBitacora { get; set; }
    }
}
