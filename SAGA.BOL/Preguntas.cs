using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class Preguntas
    {
        [Key]
        public int Id { get; set; }
        public int TipoExamenId { get; set; }
        public string Pregunta { get; set; }
        public int Tipo { get; set; }
        public int Activo { get; set; }

        public TipoExamen TipoExamen { get; set; }


    }
}
