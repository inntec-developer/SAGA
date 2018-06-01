using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.API.Dtos
{
    public class CreateRequiDto
    {
        public Guid IdDamfo { get; set; }
        public Guid IdAddress { get; set; }
        public string Usuario { get; set; }
    }
}