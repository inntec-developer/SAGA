using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAGA.BOL
{
    public class Curso
    {
        public Guid Id { get; set; }
        public string curso { get; set; }

        public Guid InstitucionEducativaId { get; set; }
        public virtual InstitucionEducativa InstitucionEducativa { get; set; }

        public int? YearInicioId { get; set; }
        public Year YearInicio { get; set; }
        public int? MonthInicioId { get; set; }
        public Month MonthInicio { get; set; }

        public int? YearTerminoId { get; set; }
        public Year YearTermino { get; set; }
        public int? MonthTerminoId { get; set; }
        public Month MonthTermino { get; set; }

        public int? Horas { get; set; }

        public Guid PerfilCandidatoId { get; set; }
        public PerfilCandidato PerfilCandidato { get; set; }

        public Curso()
        {
            this.Id = Guid.NewGuid();
        }
    }
}