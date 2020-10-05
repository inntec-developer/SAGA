using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class GrupoVacaciones
    {
        [Key]
        public int Id { get; set; }
        public int ConfigVacacionesId { get; set; }
        public ConfigVacaciones ConfigVacaciones { get; set; }
        public int GruposId { get; set; }
        public Grupos Grupos { get; set; }
        public bool Activo { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid UsuarioAlta { get; set; }
        public Guid UsuarioMod { get; set; }
    }
}
