using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class AreaInteres
    {
        [Key]
        public int Id { get; set; }
        public string areaInteres { get; set; }
        public int AreaExperienciaId { get; set; }
        public bool Activo { get; set; }
        public virtual AreaExperiencia AreaExperiencia { get; set; }
    }
}