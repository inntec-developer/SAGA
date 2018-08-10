using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class DpTpDiscapacidad
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CandidatoId { get; set; }
        public int tipoDiscapacidadId { get; set; }

        public virtual TipoDiscapacidad tipoDiscapacidad { get; set; }
        public virtual Candidato candidato { get; set; }

        public DpTpDiscapacidad()
        {
            this.Id = Guid.NewGuid();
        }

        public DpTpDiscapacidad(Guid candidatoId, int tpdiscapacidad)
        {
            CandidatoId = candidatoId;
            tipoDiscapacidadId = tpdiscapacidad;
            this.Id = Guid.NewGuid();
        }
    }

}
