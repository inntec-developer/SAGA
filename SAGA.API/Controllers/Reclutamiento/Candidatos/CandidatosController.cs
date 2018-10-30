using SAGA.DAL;
using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.API.Dtos;
using AutoMapper;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using SAGA.API.Dtos.Reclutamiento.Seguimientovacantes;

namespace SAGA.API.Controllers

{
    [RoutePrefix("api/Candidatos")]
    public class CandidatosController : ApiController
    {

        private SAGADBContext db;

        public CandidatosController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("get")]
        public IHttpActionResult GetPaises()
        {
            CandidatosDto Paises = new CandidatosDto();

            Paises.Paises = (from pais in db.Paises
                             where pais.Id == 42
                             select pais).ToList();

            return Ok(Paises);
        }

        [HttpGet]
        [Route("getestados")]
        public IHttpActionResult GetEstados(int Pais)
        {

            CandidatosDto Estados = new CandidatosDto();

            Estados.Estados = (from estado in db.Estados
                               where estado.PaisId == Pais
                               select estado).ToList();

            return Ok(Estados);
        }

        [HttpGet]
        [Route("getmunicipios")]
        public IHttpActionResult GetMunicipios(int Estado)
        {

            CandidatosDto Municipios = new CandidatosDto();

            Municipios.Municipios = (from municipios in db.Municipios
                                     where municipios.EstadoId == Estado
                                     select municipios).ToList();

            return Ok(Municipios);
        }

        [HttpGet]
        [Route("getcolonias")]
        public IHttpActionResult GetColonias(int Municipio)
        {

            CandidatosDto Colonias = new CandidatosDto();

            Colonias.Colonias = (from colonias in db.Colonias
                                 where colonias.MunicipioId == Municipio
                                 select colonias).ToList();

            return Ok(Colonias);
        }

        [HttpGet]
        [Route("getareasexp")]
        public IHttpActionResult GetAreasExp()
        {

            var areasexp = db.AreasExperiencia.ToList();

            return Ok(areasexp);
        }

        [HttpGet]
        [Route("getperfiles")]
        public IHttpActionResult GetPerfiles()
        {

            var perfil = db.PerfilExperiencia.ToList();

            return Ok(perfil);
        }

        [HttpGet]
        [Route("getgeneros")]
        public IHttpActionResult GetGeneros()
        {

            var genero = db.Generos.ToList();

            return Ok(genero);
        }

        [HttpGet]
        [Route("getdescapacidad")]
        public IHttpActionResult GetDiscapacidad()
        {

            var discapacidad = db.TiposDiscapacidades.ToList();

            return Ok(discapacidad);
        }

        [HttpGet]
        [Route("gettplicencia")]
        public IHttpActionResult GetTpLicencia()
        {

            var tplicencia = db.TiposLicencias.ToList();

            return Ok(tplicencia);
        }

        [HttpGet]
        [Route("getnivelestudio")]
        public IHttpActionResult GetNivelestudio()
        {

            var nvestudio = db.GradosEstudios.ToList();

            return Ok(nvestudio);
        }

        [HttpGet]
        [Route("getidiomas")]
        public IHttpActionResult GetIdiomas()
        {

            var idiomas = db.Idiomas.ToList();

            return Ok(idiomas);
        }

        [HttpPost]
        [Route("getcandidatos")]
        public IHttpActionResult GetCandidatos(FiltrosDto Filtros)
        {
            // Generamos el objeto que contendra los datos para el filtrado.
            List<FiltrosDto> Filtrado = new List<FiltrosDto>();
            Filtrado = db.PerfilCandidato
                .Where(c => c.Id != null)
                 .Select(c => new FiltrosDto {
                     IdCandidato = c.CandidatoId,
                     Estado = db.Direcciones.Where(cp => cp.EntidadId.Equals(c.CandidatoId)).Select(d => d.Estado.estado).FirstOrDefault(),
                     Municipio = db.Direcciones.Where(cp => cp.EntidadId.Equals(c.CandidatoId)).Select(d => d.Municipio.municipio).FirstOrDefault(),
                     IdPais = db.Direcciones.Where(cp => cp.EntidadId.Equals(c.CandidatoId)).Select(d => d.PaisId).FirstOrDefault(),
                     IdEstado = db.Direcciones.Where(cp => cp.EntidadId.Equals(c.CandidatoId)).Select(d => d.EstadoId).FirstOrDefault(),
                     IdMunicipio = db.Direcciones.Where(cp => cp.EntidadId.Equals(c.CandidatoId)).Select(d => d.MunicipioId).FirstOrDefault(),
                     nombre = c.Candidato.Nombre,
                     apellidoPaterno = c.Candidato.ApellidoPaterno,
                     apellidoMaterno = c.Candidato.ApellidoMaterno,
                     cp = db.Direcciones.Where(cp => cp.EntidadId.Equals(c.CandidatoId)).Select(d => d.CodigoPostal).FirstOrDefault(),
                     curp = c.Candidato.CURP,
                     fechaNacimiento = c.Candidato.FechaNacimiento,
                     rfc = c.Candidato.RFC,
                     nss = c.Candidato.NSS,
                     Formaciones = c.Formaciones,
                     Experiencias = c.Experiencias,
                     IdAreaExp = c.AboutMe.Select(a => a.AreaExperienciaId).FirstOrDefault(),
                     IdPerfil = c.AboutMe.Select(p => p.PerfilExperienciaId).FirstOrDefault(),
                     IdGenero = c.Candidato.GeneroId,
                     IdPDiscapacidad = c.Candidato.TipoDiscapacidadId,
                     IdTipoLicencia = c.Candidato.TipoLicenciaId,
                     Acercademi = c.AboutMe,
                     Salario = c.AboutMe.Select(s => s.SalarioAceptable).FirstOrDefault(),
                     Idiomas = c.Idiomas,
                     Reubicacion = c.Candidato.puedeRehubicarse,
                     TpVehiculo = c.Candidato.tieneVehiculoPropio

                 }).ToList();

            var FiltradoExp = Filtrado.Select(c => c.Experiencias).ToList();

            // Revisamos cada filtro que se envio para armar de nuevo la consulta.
            if (Filtros.IdPais > 0)
            {
                Filtrado = Filtrado
                    .Where(c => c.IdPais.Equals(Filtros.IdPais))
                    .ToList();
            }
            if (Filtros.IdEstado > 0)
            {
                Filtrado = Filtrado
                    .Where(c => c.IdEstado.Equals(Filtros.IdEstado))
                    .ToList();
            }
            if (Filtros.IdMunicipio > 0)
            {
                Filtrado = Filtrado
                    .Where(c => c.IdMunicipio.Equals(Filtros.IdMunicipio))
                    .ToList();
            }
            if (Filtros.cp != null)
            {
                List<FiltrosDto> fl = new List<FiltrosDto>();
                foreach (FiltrosDto x in Filtrado)
                {
                    if(x.cp != null)
                    {
                        if (x.cp.Contains(Filtros.cp))
                        {
                            fl.Add(x);
                        }
                    }
                }
                Filtrado = fl;
            }

            if (Filtros.IdAreaExp != null)
            {
                Filtrado = Filtrado
                    .Where(c => c.IdAreaExp.Equals(Filtros.IdAreaExp))
                    .ToList();
            }

            if (Filtros.IdPerfil != null)
            {
                Filtrado = Filtrado
                    .Where(c => c.IdPerfil.Equals(Filtros.IdPerfil))
                    .ToList();
            }

            if (Filtros.Salario != null)
            {
                Filtrado = Filtrado
                    .Where(c => c.Salario.Equals(Filtros.Salario))
                    .ToList();
            }

            if (Filtros.IdGenero != null)
            {
                Filtrado = Filtrado
                    .Where(c => c.IdGenero.Equals(Filtros.IdGenero))
                    .ToList();
            }

            if(Filtros.Reubicacion)
            {
                Filtrado = Filtrado
                    .Where(c => c.Reubicacion == true)
                    .ToList();
            }

            if (Filtros.IdPDiscapacidad != null)
            {
                Filtrado = Filtrado
                    .Where(c => c.IdPDiscapacidad.Equals(Filtros.IdPDiscapacidad))
                    .ToList();
            }

            if (Filtros.IdTipoLicencia != null)
            {
                Filtrado = Filtrado
                    .Where(c => c.IdTipoLicencia.Equals(Filtros.IdTipoLicencia))
                    .ToList();
            }

            if (Filtros.TpVehiculo)
            {
                Filtrado = Filtrado
                    .Where(c => c.TpVehiculo == true)
                    .ToList();
            }

            if (Filtros.IdNvEstudios != null)
            {
                Filtrado = Filtrado
                    .Where(c => c.Formaciones.Select(x => x.GradoEstudioId).FirstOrDefault().Equals(Filtros.IdNvEstudios))
                    .ToList();
            }

            if (Filtros.IdIdiomas != null)
            {
                List<FiltrosDto> fl = new List<FiltrosDto>();
                foreach (FiltrosDto x in Filtrado)
                {
                    var I = x.Idiomas.Where(i => i.IdiomaId.Equals(Filtros.IdIdiomas)).FirstOrDefault();
                    if (I != null)
                    {
                        fl.Add(x);
                    }
                }
                Filtrado = fl;
            }

            if(Filtros.Edad > 0)
            {
                foreach(FiltrosDto x in Filtrado)
                {
                    int edad = DateTime.Today.AddTicks(-Convert.ToDateTime(x.fechaNacimiento).Ticks).Year - 1;
                    if(edad == Filtros.Edad)
                    {
                        Filtrado = Filtrado
                            .Where(c => c.fechaNacimiento.Equals(x.fechaNacimiento))
                            .ToList();
                        break;
                    }
                }
            }
            var candidatos = Filtrado.Select(x => new
            {
                candidatoId = x.IdCandidato,
                nombre = x.nombre + " " + x.apellidoPaterno + " " + x.apellidoMaterno,
                AreaExp = x.Acercademi.Select(a => a.AreaExperiencia).FirstOrDefault() != null ? x.Acercademi.Select(a => a.AreaExperiencia.areaExperiencia).FirstOrDefault() : "S/D",
                AreaInt = x.Acercademi.Select(a => a.AreaInteres).FirstOrDefault() != null ? x.Acercademi.Select(a => a.AreaInteres.areaInteres).FirstOrDefault() : "S/D",
                edad = x.fechaNacimiento,
                curp = x.curp,
                rfc = x.rfc != null ? x.rfc : "S/D",
                sueldoMinimo = x.Acercademi.Select(a => a.SalarioAceptable.ToString()).FirstOrDefault() != null ? x.Acercademi.Select(a => a.SalarioAceptable).FirstOrDefault() : 0,
                localidad = x.Estado + " / " + x.Municipio,
                estatus = db.ProcesoCandidatos.Where(p => p.CandidatoId.Equals(x.IdCandidato)).Count() > 0 ? 
                          db.ProcesoCandidatos.Where(p => p.CandidatoId.Equals(x.IdCandidato)).OrderByDescending(p => p.Fch_Modificacion).Select(p => p.Estatus.Descripcion).FirstOrDefault() : 
                          "DISPONIBLE"
            }).ToList();

            return Ok(candidatos);
         }



        [HttpGet]
        [Route("getMisCandidatos")]
        public IHttpActionResult GetMisCandidatos(Guid Id)
        {
            var aux = db.ProcesoCandidatos.Where(x => x.ReclutadorId.Equals(Id)).Select(c => c.CandidatoId).Distinct().ToList();

            //var candidatos = db.ProcesoCandidatos.Where(x => x.ReclutadorId.Equals(Id) && aux.Contains(x.CandidatoId)).OrderByDescending(f => f.Fch_Creacion)
            //    .Select(x => new
            //{
            //    candidatoId = x.CandidatoId,
            //    nombre = x.Candidato.Nombre + " " + x.Candidato.ApellidoPaterno + " " + x.Candidato.ApellidoMaterno,
            //    AreaExp = db.PerfilCandidato.Where(p => p.CandidatoId.Equals(x.CandidatoId)).Select(p => p.AboutMe.Select(a => a.AreaExperiencia.areaExperiencia).FirstOrDefault()).FirstOrDefault() != null ?
            //              db.PerfilCandidato.Where(p => p.CandidatoId.Equals(x.CandidatoId)).Select(p => p.AboutMe.Select(a => a.AreaExperiencia.areaExperiencia).FirstOrDefault()).FirstOrDefault() : "",
            //    AreaInt = db.PerfilCandidato.Where(p => p.CandidatoId.Equals(x.CandidatoId)).Select(p => p.AboutMe.Select(a => a.AreaInteres.areaInteres).FirstOrDefault()).FirstOrDefault() != null ?
            //              db.PerfilCandidato.Where(p => p.CandidatoId.Equals(x.CandidatoId)).Select(p => p.AboutMe.Select(a => a.AreaInteres.areaInteres).FirstOrDefault()).FirstOrDefault() : "",
            //    Edad = x.Candidato.FechaNacimiento,
            //    curp = x.Candidato.CURP,
            //    rfc = x.Candidato.RFC != null ? x.Candidato.RFC : "",
            //    sueldoMinimo = db.PerfilCandidato.Where(p => p.CandidatoId.Equals(x.CandidatoId)).Select(p => p.AboutMe.Select(a => a.SalarioAceptable.ToString()).FirstOrDefault()).FirstOrDefault() != null ?
            //                   db.PerfilCandidato.Where(p => p.CandidatoId.Equals(x.CandidatoId)).Select(p => p.AboutMe.Select(a => a.SalarioAceptable).FirstOrDefault()).FirstOrDefault() : 0,
            //    localidad = x.Candidato.direcciones.Select(d => d.Estado.estado).FirstOrDefault() + " / " + x.Candidato.direcciones.Select(d => d.Municipio.municipio).FirstOrDefault(),
            //    folio = x.Folio,
            //    vBtra = x.Requisicion.VBtra,
            //    estatus = x.Estatus.Descripcion.First(),
            //    estatusId = x.EstatusId
            //}).ToList();

            var candidatos = db.PerfilCandidato.Where(x => aux.Contains(x.CandidatoId))
           .Select(p => new
           {
               candidatoId = p.CandidatoId,
               nombre = p.Candidato.Nombre + " " + p.Candidato.ApellidoPaterno + " " + p.Candidato.ApellidoMaterno,
               AreaExp = p.AboutMe.Select(a => a.AreaExperiencia.areaExperiencia).FirstOrDefault() != null ? p.AboutMe.Select(a => a.AreaExperiencia.areaExperiencia).FirstOrDefault() : "",
               AreaInt = p.AboutMe.Select(a => a.AreaInteres.areaInteres).FirstOrDefault() != null ? p.AboutMe.Select(a => a.AreaInteres.areaInteres).FirstOrDefault() : "",
               Edad = p.Candidato.FechaNacimiento,
               curp = p.Candidato.CURP,
               rfc = p.Candidato.RFC != null ? p.Candidato.RFC : "",
               sueldoMinimo = p.AboutMe.Select(a => a.SalarioAceptable.ToString()).FirstOrDefault() != null ? p.AboutMe.Select(a => a.SalarioAceptable).FirstOrDefault() : 0,
               localidad = p.Candidato.direcciones.Select(d => d.Estado.estado).FirstOrDefault() + " / " + p.Candidato.direcciones.Select(d => d.Municipio.municipio).FirstOrDefault(),
               folio = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(p.CandidatoId)).OrderByDescending(f => f.Fch_Modificacion).Select(r => r.Folio).FirstOrDefault(),
               vBtra = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(p.CandidatoId)).OrderByDescending(f => f.Fch_Modificacion).Select(r => r.Requisicion.VBtra).FirstOrDefault(),
               estatus = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(p.CandidatoId)).OrderByDescending(f => f.Fch_Modificacion).Select(r => r.Estatus.Descripcion).FirstOrDefault(),
               estatusId = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(p.CandidatoId)).OrderByDescending(f => f.Fch_Modificacion).Select(r => r.EstatusId).FirstOrDefault(),
           }).ToList();


            return Ok(candidatos);
        }

        [HttpGet]
        [Route("getCandidatoPalabraClave")]
        public IHttpActionResult GetCandidatoPalabraClave(string palabraClave)
        {
            try
            {
                palabraClave = palabraClave.ToLower().Trim();
                var Aboutme = db.AcercaDeMi.Where(a => a.AcercaDeMi.ToLower().Contains(palabraClave)).Select(a => a.PerfilCandidatoId).ToList();
                var Descripcion = db.ExperienciasProfesionales.Where(e => e.CargoAsignado.ToLower().Contains(palabraClave) || e.Descripcion.Contains(palabraClave))
                    .Select(e => e.PerfilCandidatoId).ToList();
                var filtros = Aboutme.Union(Descripcion).ToList().Distinct();

                var encontrados = db.PerfilCandidato
                .Where(x => filtros.Contains(x.Id))
                 .Select(x => new
                 {
                     candidatoId = x.CandidatoId,
                     nombre = x.Candidato.Nombre + " " + x.Candidato.ApellidoPaterno + " " + x.Candidato.ApellidoMaterno,
                     AreaExp = x.AboutMe.Select(a => a.AreaExperiencia).FirstOrDefault() != null ? x.AboutMe.Select(a => a.AreaExperiencia.areaExperiencia).FirstOrDefault() : "",
                     AreaInt = x.AboutMe.Select(a => a.AreaInteres).FirstOrDefault() != null ? x.AboutMe.Select(a => a.AreaInteres.areaInteres).FirstOrDefault() : "",
                     edad = x.Candidato.FechaNacimiento,
                     curp = x.Candidato.CURP,
                     rfc = x.Candidato.RFC != null ? x.Candidato.RFC : "",
                     sueldoMinimo = x.AboutMe.Select(a => a.SalarioAceptable.ToString()).FirstOrDefault() != null ? x.AboutMe.Select(a => a.SalarioAceptable).FirstOrDefault() : 0,
                     localidad = db.Direcciones.Where(cp => cp.EntidadId.Equals(x.CandidatoId)).Select(d => d.Estado.estado).FirstOrDefault() + " / " + db.Direcciones.Where(cp => cp.EntidadId.Equals(x.CandidatoId)).Select(d => d.Municipio.municipio).FirstOrDefault(),
                 }).ToList();
                
                return Ok(encontrados);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;

                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("getcandidatoid")]
        public IHttpActionResult GetCandidatoid(Guid Id)
        {

            var Candidato = db.PerfilCandidato
                .Where(x => x.CandidatoId.Equals(Id))
                .ToList();

            return Ok(Candidato);
        }

        [HttpGet]
        [Route("getestatuscandidato")]
        public IHttpActionResult GetEstatusCandidato(Guid Id)
        {

            var Estatus = db.ProcesoCandidatos
                .Where(x => x.CandidatoId.Equals(Id))
                .ToList();

            return Ok(Estatus);
        }

        [HttpGet]
        [Route("getpostulaciones")]
        public IHttpActionResult GetPostulaciones(Guid IdCandidato)
        {
            //var Postulaciones = db.Postulaciones
            //    .Where(p => p.CandidatoId == IdCandidato)
            //    .ToList();

            var postulacion = (from ps in db.Postulaciones
                               join st in db.StatusPostulaciones on ps.StatusId equals st.Id
                               join rq in db.Requisiciones on ps.RequisicionId equals rq.Id
                               where (ps.CandidatoId == IdCandidato)
                               select new
                               {
                                   st.Status,
                                   rq.VBtra
                               }).ToList();

            return Ok(postulacion);
        }

        [HttpGet]
        [Route("getvacantes")]
        public IHttpActionResult GetVacantes(Guid IdUsuario)
        {
            var Grupos = db.GruposUsuarios // Obtenemos los Ids de las celulas o grupos a los que pertenece.
                .Where(g => g.EntidadId.Equals(IdUsuario))
                .Select(g => g.GrupoId)
                .ToList();

            var RequisicionesGrupos = db.AsignacionRequis
                .Where(r => Grupos.Contains(r.GrpUsrId))
                .Select(r => r.RequisicionId)
                .ToList();

            var RequisicionesInd = db.AsignacionRequis
                .Where(r => r.GrpUsrId.Equals(IdUsuario))
                .Select(r => r.RequisicionId)
                .ToList();

            var vacantes = db.Requisiciones
                .Where(v => RequisicionesGrupos.Contains(v.Id) || RequisicionesInd.Contains(v.Id))
                .ToList();

            return Ok(vacantes);
        }

        [HttpGet]
        [Route("getvacantesdtl")]
        public IHttpActionResult GetVacantesdtl(Guid IdVacante)
        {
            var vacantesdtl = db.Requisiciones
                .Where(v => v.Id.Equals(IdVacante))
                .ToList();

            return Ok(vacantesdtl);
        }

        [HttpPost]
        [Route("postapartado")]
        public IHttpActionResult ApartarCandidato(ProcesoCandidato cdto)
        {
            try
            {
                if(db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(cdto.CandidatoId)).Count() == 0)
                {
                    cdto.Fch_Creacion = DateTime.Now;
                    cdto.Fch_Creacion.ToUniversalTime();
                    db.ProcesoCandidatos.Add(cdto);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {

            }
             return Ok(cdto);
        }

        [HttpGet]
        [Route("postliberado")]
        public IHttpActionResult LiberarCandidato(int Id)
        {
            try
            {
                ProcesoCandidato ProcesoCandidato = db.ProcesoCandidatos.Find(Id);
                db.ProcesoCandidatos.Remove(ProcesoCandidato);
                Save();
                return Ok(true);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        private void Save()
        {
            bool saveFailed;
            do
            {
                saveFailed = false;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;
                    // Update the values of the entity that failed to save from the store
                    ex.Entries.Single().Reload();
                }

            } while (saveFailed);
        }

        [HttpGet]
        [Route("getAreasRecl")]
        public IHttpActionResult GetAreasRecl()
        {
            try
            {
                var areas = db.Departamentos.Where(x => x.AreaId.Equals(16)).Select(a => new
                {
                    Id = a.Id,
                    Nombre = a.Nombre
                }).ToList();

                return Ok(areas);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost]
        [Route("updateFuenteRecl")]
        public IHttpActionResult UpdateFuenteReclutamiento(ProcesoDto datos)
        {
            try
            {
                var pc = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(datos.candidatoId) && x.RequisicionId.Equals(datos.requisicionId)).Select(d => d.Id).FirstOrDefault();

                var ppc = db.ProcesoCandidatos.Find(pc);

                db.Entry(ppc).State = System.Data.Entity.EntityState.Modified;
                ppc.TipoMediosId = datos.tipoMediosId;

                db.SaveChanges();

                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpPost]
        [Route("updateContratados")]
        public IHttpActionResult UpdateContratados(ProcesoDto datos)
        {
            var aux = new Guid("00000000-0000-0000-0000-000000000000");
            ContratadosInfo obj = new ContratadosInfo();
            try
            {
                var cc = db.ContratadosInfo.Where(x => x.EntidadId.Equals(datos.candidatoId)).Select(c => c.Id).FirstOrDefault();

                if(cc == aux)
                {
                    obj.EntidadId = datos.candidatoId;
                    obj.CURP = datos.curp;
                    obj.RFC = datos.rfc;
                    obj.NSS = datos.nss;
                    obj.Nombre = datos.nombreCandidato;
                    obj.ApellidoPaterno = datos.apellidoPaterno;
                    obj.ApellidoMaterno = datos.apellidoMaterno;
                    obj.PaisNacimientoId = datos.paisNacimientoId;
                    obj.EstadoNacimientoId = datos.estadoNacimientoId;
                    obj.MunicipioNacimientoId = datos.estadoNacimientoId;
                    obj.GeneroId = datos.generoId;
                    obj.ReclutadorId = datos.ReclutadorId;
                    obj.fch_Creacion = DateTime.Now;
                    obj.fch_Creacion.ToUniversalTime();
                    obj.fch_Modificacion = DateTime.Now;
                    obj.fch_Modificacion.ToUniversalTime();

                    db.SaveChanges();

                }
                else
                {
                    var ccc = db.ContratadosInfo.Find(cc);

                    db.Entry(ccc).State = System.Data.Entity.EntityState.Modified;
                    ccc.Nombre = datos.nombreCandidato;
                    ccc.ApellidoPaterno = datos.apellidoPaterno;
                    ccc.ApellidoMaterno = datos.apellidoMaterno;
                    ccc.Nombre = datos.nombreCandidato;
                    ccc.FechaNacimiento = datos.fechaNacimiento;
                    ccc.CURP = datos.curp;
                    ccc.ReclutadorId = datos.ReclutadorId;
                    ccc.fch_Modificacion = DateTime.Now;
                    ccc.fch_Modificacion.ToUniversalTime();

                    db.SaveChanges();

                }

                var pc = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(datos.candidatoId) && x.RequisicionId.Equals(datos.requisicionId)).Select(d => d.Id).FirstOrDefault();

                var ppc = db.ProcesoCandidatos.Find(pc);

                db.Entry(ppc).State = System.Data.Entity.EntityState.Modified;
                ppc.TipoMediosId = datos.tipoMediosId;
                ppc.DepartamentoId = datos.departamentoId;

                db.SaveChanges();

                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }





    }

}
