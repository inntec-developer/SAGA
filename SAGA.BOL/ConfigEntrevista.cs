﻿using System;
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
        public int EntrevistaId { get; set; }
        public int PreguntaId { get; set; }
        public bool Activo { get; set; }
        public int Orden { get; set; }

        public Entrevista Entrevista { get; set; }
        public Preguntas Pregunta { get; set; }


    }
}
