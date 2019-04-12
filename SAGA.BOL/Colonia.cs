using System.ComponentModel.DataAnnotations;

namespace SAGA.BOL
{
    public class Colonia
    {
        [Key]
        public int Id { get; set; }
        public string colonia { get; set; }
        public string CP { get; set; }
        public string TipoColonia { get; set; }
        public int MunicipioId { get; set; }
        public virtual Municipio Municipio { get; set; }
        public int EstadoId { get; set; }
        public virtual Estado Estado { get; set; }
        public int PaisId { get; set; }
        public virtual Pais Pais { get; set; }
        public bool Activo { get; set; }

    }
}