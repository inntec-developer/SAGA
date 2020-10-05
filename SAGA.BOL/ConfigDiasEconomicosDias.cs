using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class ConfigDiasEconomicosDias
    {
        [Key]
        public int Id { get; set; }
        public int Dias { get; set; } // Días Máximo
        public int ConfigDiasEconomicosId { get; set; }
        public ConfigDiasEconomicos ConfigDiasEconomicos { get; set; }
        public int TiposDiasEconomicosId { get; set; }
        public TiposDiasEconomicos TiposDiasEconomicos { get; set; }
        // 1 - Paternidad 2 - Matrimonio 3- fallecimiento
    }
}
