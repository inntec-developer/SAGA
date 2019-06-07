using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class MedicoCandidato
    {
        public Guid Id { get; set; }
        public Guid CandidatoId { get; set; }
        public Guid ClienteId { get; set; }
        public bool Facturado { get; set; }
        public bool Resultado { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }

        public virtual Candidato Candidato { get; set; }
        public virtual Cliente Cliente { get; set; }
    }
}
