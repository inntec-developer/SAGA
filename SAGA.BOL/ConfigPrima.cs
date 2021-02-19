using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class ConfigPrima
    {
        [Key]
        public int Id { get; set; }
        public byte Horas { get; set; }
        public decimal porcentaje { get; set; }
        public string Observaciones { get; set; }
        public bool Activo { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid UsuarioAlta { get; set; }
        public Guid UsuarioMod { get; set; }

        public Guid ClienteId { get; set; }
        public Cliente Cliente { get; set; }

    }
}
