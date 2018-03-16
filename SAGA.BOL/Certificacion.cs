using System;

namespace SAGA.BOL
{
    public class Certificacion
    {
        public Guid Id { get; set; }
        public string certificacion { get; set; }
        public string AutoridadEmisora { get; set; }
        public string Licencia { get; set; }
        public string UrlCertificacion { get; set; }
        public bool noVence { get; set; }
        public int YearInicioId { get; set; }
        public Year YearInicio { get; set; }
        public int MonthInicioId { get; set; }
        public Month MonthInicio { get; set; }

        public int YearTerminoId { get; set; }
        public Year YearTermino { get; set; }
        public int MonthTerminoId { get; set; }
        public Month MonthTermino { get; set; } 

        public Guid PerfilCandidatoId { get; set; }
        public PerfilCandidato PerfilCandidato { get; set; }

        public Certificacion()
        {
            this.Id = Guid.NewGuid();
        }
    }
}