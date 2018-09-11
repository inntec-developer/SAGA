﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class ComentarioVacante
    {
        [Key]
        public Guid Id { get; set; }
        public Guid RespuestaId { get; set; }
        public string Comentario { get; set; }
        public Guid RequisicionId { get; set; }
        public string UsuarioAlta { get; set; }
        public Guid ReclutadorId { get; set; }
        public DateTime fch_Creacion { get; set; }

        public virtual Usuarios Reclutador { get; set; }

        public ComentarioVacante()
        {
            this.Id = Guid.NewGuid();
        }
    }
}
