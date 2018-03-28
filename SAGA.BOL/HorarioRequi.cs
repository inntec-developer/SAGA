﻿using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class HorarioRequi
    {
        public HorarioRequi()
        {
            this.Id = Guid.NewGuid();
        }
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public byte deDiaId { get; set; }
        public byte aDiaId { get; set; }
        public string deHora { get; set; }
        public string aHora { get; set; }
        public byte numeroVacantes { get; set; }
        public string Especificaciones { get; set; }
        public bool Activo { get; set; }
        public Guid RequisicionId { get; set; }

        public virtual Requisicion Requisicion { get; set; }
        public virtual DiaSemana deDia { get; set; }
        public virtual DiaSemana aDia { get; set; }
    }
}
