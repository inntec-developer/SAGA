using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class ConfigIncapacidadesDias
    {
        [Key]
        public int Id { get; set; }
        public int Dias { get; set; }
        public decimal Porcentaje { get; set; }
        public int TiposIncapacidadId { get; set; }
        public TiposIncapacidad TiposIncapacidad { get; set; }
        public int ConfigIncapacidadesId { get; set; }
        public ConfigIncapacidades ConfigIncapacidades { get; set; }
    }
}
