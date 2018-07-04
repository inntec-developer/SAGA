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
                     IdPais = c.Candidato.PaisNacimientoId,
                     IdEstado = c.Candidato.EstadoNacimientoId,
                     IdMunicipio = c.Candidato.MunicipioNacimientoId,
                     nombre = c.Candidato.Nombre,
                     apellidoPaterno = c.Candidato.ApellidoPaterno,
                     apellidoMaterno = c.Candidato.ApellidoMaterno,
                     cp = c.Candidato.CodigoPostal,
                     curp = c.Candidato.CURP,
                     rfc = c.Candidato.RFC,
                     nss = c.Candidato.NSS,
                     Formaciones = c.Formaciones,
                     Experiencias = c.Experiencias,
                     IdAreaExp = c.Experiencias.Select(a => a.AreaId).FirstOrDefault(),
                     IdPerfil = c.AboutMe.Select(p => p.PerfilExperienciaId).FirstOrDefault(),
                     IdGenero = c.Candidato.GeneroId,
                     IdPDiscapacidad = c.Candidato.TipoDiscapacidadId,
                     IdNvEstudios = c.Conocimientos.Select(n => n.NivelId).FirstOrDefault(),
                     Acercademi = c.AboutMe,
                     Salario = c.AboutMe.Select(s => s.SalarioAceptable).FirstOrDefault()

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
                Filtrado = Filtrado
                    .Where(c => c.cp.Equals(Filtros.cp))
                    .ToList();
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

            if (Filtros.IdGenero != null)
            {
                Filtrado = Filtrado
                    .Where(c => c.IdGenero.Equals(Filtros.IdGenero))
                    .ToList();
            }

            if (Filtros.IdPDiscapacidad != null)
            {
                Filtrado = Filtrado
                    .Where(c => c.IdPDiscapacidad.Equals(Filtros.IdPDiscapacidad))
                    .ToList();
            }

            if (Filtros.IdNvEstudios != null)
            {
                Filtrado = Filtrado
                    .Where(c => c.IdNvEstudios.Equals(Filtros.IdNvEstudios))
                    .ToList();
            }

            if (Filtros.IdIdiomas != null)
            {
                Filtrado = Filtrado
                    .Where(c => c.IdIdiomas.Equals(Filtros.IdIdiomas))
                    .ToList();
            }

            //CandidatosDto Candidatos = new CandidatosDto();

            //Candidatos.Candidatos = (from candidatos in db.Candidatos
            //                         join perfilcandidato in db.PerfilCandidato on candidatos.Id equals perfilcandidato.CandidatoId
            //                         join persona in db.Personas on candidatos.Id equals persona.Id
            //                         where perfilcandidato.Id != null
            //                         select new CandidatosGralDto
            //                         {
            //                             Id = candidatos.Id,
            //                             Nombre = persona.Nombre,
            //                             ApellidoPaterno = persona.ApellidoPaterno,
            //                             ApellidoMaterno = persona.ApellidoMaterno,
            //                             CP = candidatos.CodigoPostal,
            //                             Curp = candidatos.CURP,
            //                             Rfc = candidatos.RFC,
            //                             Nss = candidatos.NSS
            //                         }).ToList();

            return Ok(Filtrado);
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
        public IHttpActionResult GetVacantes()
        {
            var vacantes = db.Requisiciones.ToList();

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
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    cdto.Fch_Creacion = DateTime.Now;
                    cdto.Fch_Creacion.ToUniversalTime();
                    db.ProcesoCandidatos.Add(cdto);
                    db.SaveChanges();
                    dbContextTransaction.Commit();

                }
                catch (Exception)
                {
                    dbContextTransaction.Rollback();
                }
            }
            return Ok(cdto);
        }

        [HttpGet]
        [Route("postliberado")]
        public IHttpActionResult LiberarCandidato(int Id)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    ProcesoCandidato ProcesoCandidato = db.ProcesoCandidatos.Find(Id);
                    db.ProcesoCandidatos.Remove(ProcesoCandidato);
                    Save();
                    dbContextTransaction.Commit();
                    return Ok(true);
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    return Ok(ex.Message);
                }
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


    }

}
