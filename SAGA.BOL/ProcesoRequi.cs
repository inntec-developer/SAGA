using System;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class ProcesoRequi
    {
        [Key]
        public Guid Id { get; set; }
        public string Proceso { get; set; }
        public Guid RequisicionId { get; set; }
		public int Orden { get; set; }
        public string UsuarioAlta { get; set; }
        public DateTime fch_Creacion { get; set; }
        public string UsuarioMod { get; set; }
        public DateTime? fch_Modificacion { get; set; }

        public virtual Requisicion Requisicion { get; set; }

        public ProcesoRequi()
        {
            this.Id = Guid.NewGuid();
        }
    }
}