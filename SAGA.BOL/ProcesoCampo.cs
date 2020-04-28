using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class ProcesoCampo
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CandidatoId { get; set; }
        public Guid RequisicionId { get; set; }
        public Guid ReclutadorId { get; set; }
        public Guid UsuarioId { get; set; }
        public DateTime Fch_Creacion { get; set; }

        public virtual Candidato Candidato { get; set; }
        public virtual Requisicion Requisicion { get; set; }
        public virtual Usuarios Usuario { get; set; }
    }
}
