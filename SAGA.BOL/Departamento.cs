using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class Departamento
    {
        [Key]
        public Guid Id { get; set; }
        public int AreaId { get; set; }
        public string Clave { get; set; }
        public string Nombre { get; set; }
        public string Observaciones { get; set; }
        public int  Orden { get; set; }
        public bool Activo { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid UsuarioAlta { get; set; }
        public Guid UsuarioMod { get; set; }

        public virtual Area Area { get; set; }
    }
}
