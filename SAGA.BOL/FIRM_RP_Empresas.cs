using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace SAGA.BOL
{
    public class FIRM_RP_Empresas
    {
        [Key]
        public Guid Id { get; set; }
        public Guid EmpresasId { get; set; }
        public Guid FIRM_RPId { get; set; }

        public Empresas Empresas { get; set; }
        public FIRM_RP FIRM_RP { get; set; }
    }
}
