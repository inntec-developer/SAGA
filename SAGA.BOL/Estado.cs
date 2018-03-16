using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class Estado
    {
        [Key]
        public int Id { get; set; }
        public string estado { get; set; }
        public int PaisId { get; set; }
        public Pais Pais { get; set; }
        public string Clave { get; set; }
        
    }
}