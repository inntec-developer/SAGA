using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class Gafetes
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CandidatoId { get; set; }
        public string Clave { get; set; }
        public string Codigo { get; set; }
        public DateTime fch_Ingreso { get; set; }
        public Guid UsuarioId { get; set; }
        public bool Activo { get; set; }

        public Usuarios Usuario { get; set; }

    }
}
