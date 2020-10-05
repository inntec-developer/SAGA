using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class ConfigVacacionesDias
    {
        [Key]
        public int Id { get; set; }
        public int Dias { get; set; }
        public int TiempoAntiguedadId { get; set; }
        public TiempoAntiguedad TiempoAntiguedad { get; set; }
        public int ConfigVacacionesId { get; set; }
        public ConfigVacaciones ConfigVacaciones { get; set; }
    }
}
