using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class DocumentosDamsa
    {
        [Key]
        public int Id { get; set; }
        public string documentoDamsa { get; set; }
    }
}
