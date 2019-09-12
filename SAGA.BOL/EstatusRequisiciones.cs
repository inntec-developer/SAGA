using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class EstatusRequisiciones
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PropietarioId { get; set; }
        public Guid RequisicionId { get; set; }
        public int EstatusId { get; set; }
        public DateTime? fch_Modificacion { get; set; }
        public string UsuarioMod { get; set; }

        public virtual Requisicion Requisicion { get; set; }
        public virtual Estatus Estatus { get; set; }
        public virtual Usuarios Propietario { get; set; }

    }
}
