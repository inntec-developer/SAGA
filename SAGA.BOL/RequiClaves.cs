using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class RequiClaves
    {
        [Key]
        public Guid Id { get; set; }
        public Guid RequisicionId { get; set; }
        public string Clave { get; set; }
        public int Activo { get; set; }
        public DateTime fch_Creacion { get; set; }
        public Guid UsuarioId { get; set; }

        public Requisicion Requisicion { get; set; }
        public Usuarios Usuario { get; set; }

    }
}
