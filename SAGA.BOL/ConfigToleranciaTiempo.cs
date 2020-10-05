using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class ConfigToleranciaTiempo
    {
        [Key]
        public int Id { get; set; }
        public DateTime Tiempo { get; set; }
        public byte Tipo { get; set; } // 1 entrada 2 salida 3 estancia
        public int ConfigToleranciaId { get; set; }
        public ConfigTolerancia ConfigTolerancia { get; set; }
    }
}
