using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class BiometricosFP
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CandidatosInfoId { get; set; }
        public byte[] FingerPrint { get; set; }
        public bool Activo { get; set; }
        public Guid UsuarioAlta { get; set; }
        public DateTime fch_Creacion { get; set; }
        public Guid UsuarioMod { get; set; }
        public DateTime fch_Modificacion { get; set; }

        public CandidatosInfo CandidatosInfo { get; set; }
    }
}
