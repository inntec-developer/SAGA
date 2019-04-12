using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class DireccionClienteDto
    {
        public Guid Id { get; set; }
        public int TipoDireccionId { get; set; }
        public string Calle { get; set; }
        public string NumeroInterior { get; set; }
        public string NumeroExterior { get; set; }
        public int PaisId { get; set; }
        public int EstadoId { get; set; }
        public int MunicipioId { get; set; }
        public int ColoniaId { get; set; }
        public string CodigoPostal { get; set; }
        public bool esPrincipal { get; set; }
        public bool Activo { get; set; }
        public string Referencia { get; set; }
        public Guid EntidadId { get; set; }
        public string Usuario{ get; set; }
    }
}