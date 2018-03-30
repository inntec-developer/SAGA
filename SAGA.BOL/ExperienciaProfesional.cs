using System;

namespace SAGA.BOL
{
    public class ExperienciaProfesional
    {
        public Guid Id { get; set; }
        public string Empresa { get; set; }
        public int GiroEmpresaId { get; set; }
        public GiroEmpresa GiroEmpresa { get; set; }
        public string CargoAsignado { get; set; }
        public int AreaId { get; set; }
        public virtual Area Area { get; set; }
        public int YearInicioId { get; set; }
        public Year YearInicio { get; set; }
        public int MonthInicioId { get; set; }
        public Month MonthInicio { get; set; }
        public int YearTerminoId { get; set; }
        public Year YearTermino { get; set; }
        public int MonthTerminoId { get; set; }
        public Month MonthTermino { get; set; }
        public decimal Salario { get; set; }
        public bool TrabajoActual { get; set; }
        public string Descripcion { get; set; }
        public Guid PerfilCandidatoId { get; set; }
        public PerfilCandidato PerfilCandidato { get; set; }

        public ExperienciaProfesional()
        {
            this.Id = Guid.NewGuid();
        }
    }
}