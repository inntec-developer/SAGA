using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class Jornada
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CandidatoInfoId { get; set; }
        public DateTime Fecha { get; set; }
        public byte Dia { get; set; }
        public DateTime Hora { get; set; }
        public byte Tipo { get; set; }

        public CandidatosInfo CandidatoInfo { get; set; }
    }
}
