using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class PeriodoCompensaciones
    {
        public Guid Id { get; set; }
        public Guid CandidatosInfoId { get; set; }
        public DateTime Fecha { get; set; }
        public byte Tipo { get; set; } // prima dominical 2 festivo laborado // dia no calendarizado
        public string Comentario { get; set; }
        public DateTime fchAlta { get; set; }
        public Guid UsuarioAltaId { get; set; }

        public CandidatosInfo CandidatosInfo { get; set; }
        public Usuarios UsuarioAlta { get; set; }
    }
}
