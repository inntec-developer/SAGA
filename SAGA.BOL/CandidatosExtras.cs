using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class CandidatosExtras
    {
        [Key]
        public Guid Id { get; set; }

        public Guid CandidatoInfoId { get; set; }
        public virtual CandidatosInfo CandidatoInfo { get; set; }

        public string Conyuge { get; set; }
        public string NomPadre { get; set; }
        public string NomMadre { get; set; }
        public string NomBeneficiario { get; set; }
        public string Nacionalidad { get; set; }

        public int GradoEstudioId { get; set; }
        public virtual GradoEstudio GradosEstudio { get; set; }

        public string Observaciones { get; set; }


    }
}
