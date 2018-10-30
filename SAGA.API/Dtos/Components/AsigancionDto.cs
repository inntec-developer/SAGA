using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class AsigancionDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Email { get; set; }
        public int TipoEntidadId { get; set; }
    }

    public class AsignacionGrupsDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public List<UsuariosDto> Usuarios { get; set; }
    }

    public class UsuariosDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Usuario { get; set; }
        public string Email { get; set; }
        public string TipoUsuario { get; set; }
    }
}