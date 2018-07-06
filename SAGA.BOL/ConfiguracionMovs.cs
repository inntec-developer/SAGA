using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class ConfiguracionMovs
    {
        [Key]
        public int Id { get; set; }
        public int EstructuraId { get; set; }
        public bool esPublicable { get; set; }
        public bool esEditable { get; set; }
        public string nuevaEtiqueta { get; set; }
        public string nuevoValor { get; set; }

        public Estructura Estructura { get; set; }
    }
}
