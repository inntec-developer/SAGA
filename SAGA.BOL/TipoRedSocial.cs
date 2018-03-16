using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class TipoRedSocial
    {
        [Key]
        public byte Id { get; set; }
        public string tipoRedSocial {get; set;}
    }
}