using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class FIRM_FechasEstatus
    {
        [Key]
        public Guid Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int WeekDay { get; set; }
        public DateTime Hour { get; set; }
        public DateTime Fecha { get; set; }
        public int ConfigBitacoraId { get; set; }
        public FIRM_ConfigBitacora ConfigBitacora { get; set; }
        public int EstatusBitacoraId { get; set; }
        public FIRM_EstatusBitacora EstatusBitacora { get; set; }
    }
}
