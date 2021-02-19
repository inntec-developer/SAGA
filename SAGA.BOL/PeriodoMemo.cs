using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class PeriodoMemo
    {
        public Guid Id { get; set; }
        public Guid CandidatosInfoId { get; set; }
        public DateTime fchIncio { get; set; }
        public DateTime fchFin { get; set; }
        public string Comentario { get; set; }
        public byte Retardos { get; set; }
        public byte Faltas { get; set; }
        public DateTime fchAlta { get; set; }
        public Guid UsuarioAltaId { get; set; }

        public CandidatosInfo CandidatosInfo { get; set; }
        public Usuarios UsuarioAlta { get; set; }
    }
}
