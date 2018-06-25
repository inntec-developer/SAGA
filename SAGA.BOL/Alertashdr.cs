using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class Alertashdr
    {
        public Guid Id { get; set; }
        public Guid CandidatoId { get; set; }
        public int FrecuenciaId { get; set; }
        public string CorreoTelefono { get; set; }
        public bool Activo { get; set; }
        public DateTime Fch_UltimaEjecucion { get; set; }
    }

}
