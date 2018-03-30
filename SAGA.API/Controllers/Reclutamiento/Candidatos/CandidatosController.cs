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
                             //where pais.Id == 42
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

        [HttpGet]
        [Route("getcandidatos")]
        public IHttpActionResult GetCandidatos()
        {

            CandidatosDto Candidatos = new CandidatosDto();

            Candidatos.Candidatos = (from candidatos in db.Candidatos
                                     join perfilcandidato in db.PerfilCandidato on candidatos.Id equals perfilcandidato.CandidatoId
                                     join persona in db.Personas on candidatos.Id equals persona.Id
                                     where perfilcandidato.Id != null
                                     select new CandidatosGralDto
                                     {
                                         Id = candidatos.Id,
                                         Nombre = persona.Nombre,
                                         ApellidoPaterno = persona.ApellidoPaterno,
                                         ApellidoMaterno = persona.ApellidoMaterno,
                                         CP = candidatos.CodigoPostal,
                                         Curp = candidatos.CURP,
                                         Rfc = candidatos.RFC,
                                         Nss = candidatos.NSS
                                      }).ToList();

            return Ok(Candidatos.Candidatos);
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

    }
}
