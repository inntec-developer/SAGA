using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class PerfilDamfoRel
    {
        [Key]
        public Guid Id { get; set; }
        public Guid damfo_290Id { get; set; }
        public Guid ActividadesPerfilesId { get; set; }

        public virtual DAMFO_290 DAMFO_290 { get; set; }
        public virtual ActividadesPerfiles PerfilesDamfo { get; set; }

    }
}
