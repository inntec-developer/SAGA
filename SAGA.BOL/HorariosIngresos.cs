using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class HorariosIngresos
    {
        [Key]
        public Guid Id { get; set; }
        public string Clave { get; set; }
        public string Nombre { get; set; }
        public string Especificaciones { get; set; }
        public byte TurnosHorariosId { get; set; }
        public byte HorasTotales { get; set; }
        public byte HorasComida { get; set; }
        public byte HorasDescanso { get; set; }
        public bool Activo { get; set; }
        public Guid UsuarioAlta { get; set; }
        public DateTime fch_Creacion { get; set; }
        public Guid UsuarioMod { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid ClienteId { get; set; }

        public Cliente Cliente { get; set; }
        public virtual TurnosHorarios TurnosHorarios { get; set; }

    }
}
