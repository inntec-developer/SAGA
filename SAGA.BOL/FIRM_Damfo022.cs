using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class FIRM_Damfo022
    {
        [Key]
        public Guid Id { get; set; }
        public string Folio { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
        public string Problema { get; set; }
        public string Causa_Raiz { get; set; }
        public string SolucionTmp { get; set; }
        public string Solucion { get; set; }
        public bool Estatus { get; set; }
        public bool Activo { get; set; }
        public DateTime fch_Creacion { get; set; }
        public DateTime fch_Modificacion { get; set; }
        public Guid UsuarioAlta { get; set; }
        public Guid UsuarioMod { get; set; }

    }
}
