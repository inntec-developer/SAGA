using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class CandidatoHorario
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CandidatosInfoId { get; set; }
        public CandidatosInfo CandidatosInfo { get; set; }
        public  Guid HorariosIngresosId { get; set; }
        public HorariosIngresos HorariosIngresos { get; set; }
        public bool Activo { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid UsuarioAlta { get; set; }
        public Guid UsuarioMod { get; set; }

    }
}
