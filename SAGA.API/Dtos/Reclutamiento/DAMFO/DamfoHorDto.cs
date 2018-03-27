namespace SAGA.API.Dtos
{
    public class DamfoHorDto
    {
        public string Nombre { get; set; }
        public byte deDiaId { get; set; }
        public byte aDiaId { get; set; }
        public string deHora { get; set; }
        public string aHora { get; set; }
        public byte numeroVacantes { get; set; }
        public string Especificaciones { get; set; }
        public bool Activo { get; set; }
    }
}