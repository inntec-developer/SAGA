using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public partial class TipoEmpresa
    {
        public TipoEmpresa(){}
        [Key]
        public int Id { get; set; }
        public string tipoEmpresa { get; set; }
    }
}