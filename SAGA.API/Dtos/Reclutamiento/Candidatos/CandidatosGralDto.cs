using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class CandidatosGralDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string CP { get; set; }
        public string Curp { get; set; }
        public string Rfc { get; set; }
        public string Nss { get; set; }
        public string Credencial { get; set; }
        public string Foto { get; set; }

        //Melina
        public List<Email> Email { get; set; }
        public int PaisNacimientoId { get; set; }
        public int EstadoNacimientoId { get; set; }
        public int MunicipioNacimientoId { get; set; }
        public List<Telefono> Telefono { get; set; }
        public DateTime FechaNac { get; set; }
        public byte GeneroId { get; set; }
        public byte OpcionRegistro { get; set; }
        public Guid requisicionId { get; set; }
        public Guid reclutadorId { get; set; }
        public Guid reclutadorCampoId { get; set; }
        public Guid horarioId { get; set; }
        public int tipoMediosId { get; set; }

        //
        public string Acercademi { get; set; }
        public string Puesto { get; set; }
        public decimal SalarioAceptable { get; set; }
        public decimal SalarioDeseado { get; set; }
        public string Areaexp { get; set; }
        public string Areainteres { get; set; }
        public string Experiencia { get; set; }
        public string Sitioweb { get; set; }
    }
}