using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class GrupoEmpleados
    {
        [Key]
        public int Id { get; set; }
        public int GrupoId { get; set; }
        public Guid EmpleadoId { get; set; }
        public bool Activo { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid UsuarioAlta { get; set; }
        public Guid UsuarioMod { get; set; }
        public virtual CandidatosInfo Empleado { get; set; }
        public virtual Grupos Grupo { get; set; }
    }
}
