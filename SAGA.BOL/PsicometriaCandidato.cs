using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class PsicometriaCandidato
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CandidatoId { get; set; }
        public Guid RequisicionId { get; set; }
        public Guid RequiClaveId { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Resultado { get; set; }
        public string Resultado { get; set; }
        public Guid UsuarioId { get; set; }

        public Candidato Candidato { get; set; }
        public Requisicion Requisicion { get; set; }
        public RequiClaves RequiClave { get; set; }
        public Usuarios Usuario { get; set; }



    }
}
