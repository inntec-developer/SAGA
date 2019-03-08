using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAGA.BOL
{
    public class AspNetUsers
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public Nullable<System.DateTime> LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string UserName { get; set; }
        public Guid? IdPersona { get; set; }
        public Guid? claveConfirmacionEmail { get; set; }
        public string Clave { get; set; }
        public DateTime? RegistroClave { get; set; }
        public string Pasword { get; set; }
        public bool? SesionEs { get; set; }
        public DateTime? FechaDesactiva { get; set; }
        public int? Activo { get; set; }
        public DateTime? UltimoInicio { get; set; }
        public DateTime? FechaEnvioEmail { get; set; }
    }
}
