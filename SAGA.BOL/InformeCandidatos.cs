using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class InformeCandidatos
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CandidatoId { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public int tipoMovimientoId { get; set; }
        public Guid UsuarioMod { get; set; }

        public virtual TipoMovimiento TipoMovimiento { get; set; }
    }
}
