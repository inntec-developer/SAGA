using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class Ticket
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CandidatoId { get; set; }
        public int ModuloId { get; set; }
        public string Numero { get; set; }
        public int MovimientoId { get; set; }
        public int Estatus { get; set; } //1 espera  2 atendiendo 3 finalizado
        public DateTime fch_Creacion { get; set; }

        public Candidato Candidato { get; set; }
        public ModulosReclutamiento Modulo { get; set; }

    }
}
