using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public partial class ActividadEmpresa
    {
        public ActividadEmpresa()
        {
        }
        [Key]
        public int Id { get; set; }
        public int GiroEmpresaId { get; set; }
        public string actividadEmpresa{ get; set; }

        public virtual GiroEmpresa GiroEmpresas { get; set; }
    }
}
