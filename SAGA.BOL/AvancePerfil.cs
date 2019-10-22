using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class AvancePerfil
    {
        public Guid Id { get; set; }
        public Guid PerfilCandidatoId { get; set; }
        public int Avance { get; set; }

    }
}
