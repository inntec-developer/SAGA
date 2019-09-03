using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class ConfigEntrevista
    {
        [Key]
        public int Id { get; set; }
        public int examenId { get; set; }
        public int numPreguntas { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid usuarioId { get; set; }
        public bool Activo { get; set; }

        public Examenes Examen { get; set; }
        public Usuarios Usuario { get; set; }
    }
}
