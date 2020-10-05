using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class ConfigSuspensionNotasDias
    {
        [Key]
        public int Id { get; set; }
        public byte Dias { get; set; } // número de días máximo
        public byte Retardos { get; set; } // retardos en un mes calendario
        public byte Tipo { get; set; } // 1 - disciplinario 2 - retardo 3 - nota mala 4 - nota buena 5 acta administrativa
        public int ConfigSuspensionNotasId { get; set; }
        public ConfigSuspensionNotas ConfigSuspensionNotas { get; set; }
    }
}
