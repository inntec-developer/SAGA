using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class LoginValidation
    {
        public Guid Id { get; set; }
        public string Clave { get; set; }
    }
}