using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAGA.BOL;
using SAGA.DAL;

namespace SAGA.API.Dtos
{
    public class PersonasDtos
    {
        public Guid Id { get; set; }
        public Guid EntidadId { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string tipoUsuario { get; set; }
        public byte TipoUsuarioId { get; set; }
        public string Usuario { get; set; } //alias
        public List<Email> Email { get; set; }
        public Guid DepartamentoId { get; set; }
        public string Clave { get; set; }
        public string UsuarioAlta { get; set; }
        public string Password { get; set; }
        public string Departamento { get; set; }
        public bool Activo { get; set; }
        public int TipoEntidadID { get; set; }
        public string TipoEntidad { get; set; }
        public string Foto { get; set; }
        public String FotoAux { get; set; }
        public string Descripcion { get; set; }
        public ICollection<Grupos> Grupos { get; set; }
        public Guid liderId { get; set; }

    }
}