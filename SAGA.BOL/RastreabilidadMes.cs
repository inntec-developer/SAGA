using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class RastreabilidadMes
    {
        [Key]
        public Guid Id { get; set; }
        public Guid TrazabilidadMesId { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public string UsuarioMod { get; set; }
        public int TipoAccionId { get; set; }
        public string Descripcion { get; set; }

        public TrazabilidadMes TrazabilidadMes { get; set; }
        public TipoAccion TipoAccion { get; set; }
    }
}
