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
        public string Reclutador { get; set; }
        public int Estatus { get; set; }
        public int TpContrato { get; set; }
        public DateTime Fch_Creacion { get; set; }
        public DateTime? Fch_Modificacion { get; set; }
    }
}
