using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class Costos
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
        public DateTime Fch_Creacion { get; set; }
        public DateTime Fch_Modificacion { get; set; }
        public Guid UsuarioCreacion { get; set; }
        public Guid UsuarioModificacion { get; set; }
    }
}
