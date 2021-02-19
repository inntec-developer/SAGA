using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class PeriodoPermisos
    {
        public Guid Id { get; set; }
        public Guid CandidatosInfoId { get; set; }
        public int dias { get; set; }
        public DateTime fchIncio { get; set; }
        public DateTime fchFin { get; set; }
        public string Comentario { get; set; }
        public bool Sueldo { get; set; }
        public int TipoJustificacionId { get; set; }
        public byte Tipo { get; set; } // 1 permiso 2 falta
        public DateTime fchAlta { get; set; }
        public Guid UsuarioAltaId { get; set; }

        public JustificacionTrabajo TipoJustificacion { get; set; }
        public CandidatosInfo CandidatosInfo { get; set; }
        public Usuarios UsuarioAlta { get; set; }
    }
}
