using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class CandidatoLiberado
    {
        [Key]
        public Guid Id { get; set; }
        public Guid RequisicionId { get; set; }
        public Guid CandidatoId { get; set; }
        public Guid ReclutadorId { get; set; }
        public Guid MotivoLiberacionId { get; set; }
        public string Comentario { get; set; }
        public string fch_Liberacion { get; set; }

        public virtual MotivoLiberacion MotivoLiberacion { get; set; }
        public virtual Candidato Candidato { get; set; }
        public virtual Requisicion Requisicion { get; set; }
        public virtual Usuarios Reclutador { get; set; }
    }
}
