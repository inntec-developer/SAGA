using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class DiasFestivos
    {
        [Key]
        public Guid Id { get; set; }
        public string Clave { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public int Anio { get; set; }
        public int MesNum { get; set; }
        public string MesNombre { get; set; }
        public int DiaSemanaNum { get; set; }
        public string DiaSemanaNombre { get; set; }
        public string Comentario { get; set; }
        public int Tipo { get; set; }
        public bool Activo { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid UsuarioAlta { get; set; }
        public Guid UsuarioMod { get; set; }

    }
}
