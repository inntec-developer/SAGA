using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class ValidacionCURPRFC
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CandidatosInfoId { get; set; }
        public bool CURP { get; set; }
        public bool RFC { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid UsuarioAlta { get; set; }
        public Guid UsuarioMod { get; set; }

        public CandidatosInfo CandidatosInfo { get; set; }
    }
}
