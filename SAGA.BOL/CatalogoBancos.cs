using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class CatalogoBancos
    {
        [Key]
        public int Id { get; set; }
        public int Clave { get; set; }
        public string Nombre { get; set; }
        public string RazonSocial { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid UsuarioAlta { get; set; }
        public Guid UsuarioMod { get; set; }
    }
}
