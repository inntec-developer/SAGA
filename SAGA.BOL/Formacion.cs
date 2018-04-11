using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAGA.BOL
{
    public class Formacion
    {

        public Formacion()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public Guid InstitucionEducativaId { get; set; }
        public int GradoEstudioId { get; set; }
        public int EstadoEstudioId { get; set; }
        public int? DocumentoValidadorId { get; set; }
        public Guid CarreraId { get; set; }
        public int? YearInicioId { get; set; }
        public int? MonthInicioId { get; set; }
        public int? YearTerminoId { get; set; }
        public int? MonthTerminoId { get; set; }

        public virtual InstitucionEducativa InstitucionEducativa { get; set; }
        public virtual GradoEstudio GradosEstudio { get; set; }
        public virtual EstadoEstudio EstadoEstudio { get; set; }
        public virtual DocumentoValidador DocumentoValidador { get; set; }
        public virtual Carrera Carrera { get; set; }
        public virtual Year YearInicio { get; set; }
        public virtual Month MonthInicio { get; set; }
        public virtual Year YearTermino { get; set; }
        public virtual Month MonthTermino { get; set; }
        public Guid PerfilCandidatoId { get; set; }
        public virtual PerfilCandidato PerfilCandidato { get; set; }

    }
}