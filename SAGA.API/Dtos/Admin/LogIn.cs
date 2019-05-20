using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class LogIn
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    public class ReturnLogIn
    {
        public string Token { get; set; }
        public UsuarioDto Usuario { get; set; }
    }
}