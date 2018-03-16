using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class GiroEmpresa
    {
        [Key]
        public int Id { get; set; }
        public string giroEmpresa { get; set; }
    }
}