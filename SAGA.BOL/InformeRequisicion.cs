using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class InformeRequisicion
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CandidatoId { get; set; }
        public Guid RequisicionId { get; set; }
        public Guid ReclutadorId { get; set; }
        public int EstatusId { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime? fch_Modificacion { get; set; }

        public virtual Requisicion Requisicion { get; set; }
        public virtual Estatus Estatus { get; set; }
        public virtual Candidato Candidato { get; set; }
    }
}
