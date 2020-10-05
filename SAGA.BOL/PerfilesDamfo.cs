using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class PerfilesDamfo
    {
        [Key]
        public int Id { get; set; }
        public string Perfil { get; set; }
        public bool Activo { get; set; }
        public Guid EntidadId { get; set; }
        public DateTime fch_Creacion { get; set; }

        public virtual Entidad Entidad { get; set; }
    }
}
