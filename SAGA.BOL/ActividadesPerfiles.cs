using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class ActividadesPerfiles
    {
        [Key]
        public Guid Id { get; set; }
        public string Actividades { get; set; }
        public bool Activo { get; set; }
        public int PerfilesDamfoId { get; set; }
        public Guid UsuarioAlta { get; set; }
        public DateTime fch_Creacion { get; set; }
        public Guid UsuarioMod { get; set; }
        public DateTime? fch_Modificacion { get; set; }

        public virtual PerfilesDamfo PerfilesDamfo { get; set; }
    }
}
