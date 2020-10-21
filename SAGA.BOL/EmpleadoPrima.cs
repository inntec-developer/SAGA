using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class EmpleadoPrima
    {
        [Key]
        public int Id { get; set; }
        public int ConfigPrimaId { get; set; }
        public ConfigPrima ConfigPrima { get; set; }
        public Guid empleadoId { get; set; }
        public CandidatosInfo Empleado { get; set; }
        public bool Activo { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid UsuarioAlta { get; set; }
        public Guid UsuarioMod { get; set; }
    }
}
