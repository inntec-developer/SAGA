using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class ProcesoCandidato
    {
        [Key]
        public int Id { get; set; }
        public Guid CandidatoId { get; set; }
        public Guid RequisicionId { get; set; }
        public long Folio { get; set; }
        public string Reclutador { get; set; }
        public Guid ReclutadorId { get; set; }
        public int EstatusId { get; set; }
        public int TpContrato { get; set; }
        public DateTime Fch_Creacion { get; set; }
        public DateTime? Fch_Modificacion { get; set; }
        
        public virtual Estatus Estatus { get; set; }
        public virtual Candidato Candidato { get; set; }
        public virtual Requisicion Requisicion { get; set; }
    }
}
