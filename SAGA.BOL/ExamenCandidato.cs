using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class ExamenCandidato
    {
        [Key]
        public Guid Id { get; set; }
        public int ExamenId { get; set; }
        public Guid CandidatoId { get; set; }
        public Guid RequisicionId { get; set; }
        public int Resultado { get; set; }

        public Examenes Examen { get; set; }
        public Candidato Candidato { get; set; }
        public Requisicion Requisicion { get; set; }
   
    }
}
