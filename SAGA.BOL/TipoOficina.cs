namespace SAGA.BOL
{
    public class TipoOficina
    {  
        [key]
        public int Id { get; set; }
        public string tipoOficina { get; set; }
        public string Icono { get; set; }
    }
}