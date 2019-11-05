using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos.SistTickets
{
    public class LoginDto
    {
        public Guid Id { get; set; }
        public string userId { get; set; }
        public string username { get; set; }
        public string pass { get; set; }
        public string nombre { get; set; }
    }
}