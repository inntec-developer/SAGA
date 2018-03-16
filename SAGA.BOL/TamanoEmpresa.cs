using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public partial class TamanoEmpresa
    {
        public TamanoEmpresa()
        {

        }
        [Key]
        public int Id { get; set; }
        public string tamanoEmpresa { get; set; }
    }
}