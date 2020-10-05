using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class EmpresaBancos
    {
        [Key]
        public int Id { get; set; }
        public Guid ClienteId { get; set; }
        public int BancoId { get; set; }
        public bool Activo { get; set; }

        public Guid UsuarioAlta { get; set; }
        public DateTime fch_Creacion { get; set; }
        public Guid UsuarioMod { get; set; }
        public DateTime fch_Modificacion { get; set; }
    }
}
