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
        public int ExamenId { get; set; }
        public string Pregunta { get; set; }
        public int Tipo { get; set; } // Abierta 1 - Opcion multiple 2 - puntaje 3
        public int Activo { get; set; }
        public int Orden { get; set; }

        public Examenes Examen { get; set; }


    }
}
