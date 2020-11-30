using SAGA.API.Dtos;
using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using SAGA.API.Utilerias;

namespace SAGA.API.Controllers.Component
{
    [RoutePrefix("api/Candidatos")]
    public class InfoCandidatoController : ApiController
    {
        private SAGADBContext db;

        public InfoCandidatoController()
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

            var areasexp = db.AreasInteres.Where(a => a.Id != 0).ToList().OrderBy(a => a.areaInteres);

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

        [Route("getInfoCandidato")]
        [HttpGet]
        public IHttpActionResult _GetInfoCandidato(Guid Id)
        {
            try
            {
                string fecha = "07/12/1990";
                var mocos = db.MiCVUpload.Where(x => x.CandidatoId.Equals(Id)).Count();
                var infoCanditato = db.PerfilCandidato.Where(p => p.CandidatoId.Equals(Id)).Select(p => new
                {
                    Id = p.CandidatoId,
                    Nombre = p.Candidato.Nombre + " " + p.Candidato.ApellidoPaterno + " " + p.Candidato.ApellidoMaterno,
                    Foto = p.Candidato.ImgProfileUrl,
                    Genero = p.Candidato.Genero.genero,
                    FechaNacimiento = p.Candidato.FechaNacimiento,
                    Candidato = new { p.Candidato.FechaNacimiento,
                        p.Candidato.estadoNacimiento.estado,
                        p.Candidato.paisNacimiento.pais,
                        p.Candidato.esDiscapacitado,
                        p.Candidato.TipoDiscapacidad.tipoDiscapacidad,
                        p.Candidato.OtraDiscapacidad,
                        p.Candidato.tieneLicenciaConducir,
                        p.Candidato.TipoLicencia.tipoLicencia,
                        p.Candidato.TipoLicencia.Descripcion,
                        p.Candidato.tieneVehiculoPropio,
                        p.Candidato.puedeViajar,
                        p.Candidato.puedeRehubicarse,
                        p.Candidato.CURP,
                        p.Candidato.RFC,
                        p.Candidato.NSS,
                    },
                    email = p.Candidato.emails.FirstOrDefault().email,
                    AboutMe = p.AboutMe,
                    Cursos = p.Cursos,
                    Conocimientos = p.Conocimientos,
                    Idiomas = p.Idiomas,
                    Formaciones = p.Formaciones,
                    Experiencias = p.Experiencias,
                    Certificaciones = p.Certificaciones,
                    Direccion = p.Candidato.direcciones.Select(dd => new {
                        dd.Municipio.municipio,
                        dd.Estado.estado,
                       dd.Pais.pais,
                        dd.Colonia.TipoColonia,
                        dd.Colonia.colonia,
                        dd.Calle,
                        dd.NumeroExterior,
                        dd.NumeroInterior,
                        dd.CodigoPostal
                    }).FirstOrDefault(),
                    Telefono = p.Candidato.telefonos.Select(tt => new
                    {
                        tt.TipoTelefono.Tipo,
                        tt.telefono
                    }),
                    Estatus = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion).Where(e => e.CandidatoId.Equals(p.CandidatoId)).Select(PC => new {
                        id = PC.Id,
                        EstatusId = PC.EstatusId,
                        descripcion = PC.Estatus.Descripcion,
                        requisicionId = PC.RequisicionId,
                        reclutador = db.Usuarios.Where(x => x.Id.Equals(PC.ReclutadorId)).Select(RR => RR.Nombre + " " + RR.ApellidoPaterno + " " + RR.ApellidoMaterno).FirstOrDefault(),
                        reclutadorId = PC.ReclutadorId
                    }).FirstOrDefault(),
                    RedSocial = db.RedesSociales.Where(r => r.EntidadId.Equals(p.CandidatoId)).Select(r => r.redSocial).ToList(),
                    propietarioId = db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(p.CandidatoId) && x.EstatusId.Equals(24)).Count() > 0 ? db.ProcesoCandidatos.Where(x => x.CandidatoId.Equals(p.CandidatoId)).OrderByDescending(o => o.Fch_Modificacion).Select(id => id.Requisicion.PropietarioId).FirstOrDefault() : new Guid("00000000-0000-0000-0000-000000000000"),
                    URLCv = db.MiCVUpload.Where(x => x.CandidatoId.Equals(p.CandidatoId)).Count() > 0 ? db.MiCVUpload.OrderByDescending(x => x.Id).Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(x => x.UrlCV).FirstOrDefault().Trim() : "",
                    Edad =0
            }).FirstOrDefault();

                return Ok(infoCanditato);
            }
            catch (Exception ex)
            {
                APISAGALog obj = new APISAGALog();
                obj.WriteError(ex.Message);
                obj.WriteError(ex.InnerException.Message);
                return Ok(StatusCode(HttpStatusCode.NotFound));
            }
        }

        [Route("getPostulaciones")]
        [HttpGet]
        //[Authorize]
        public IHttpActionResult GetPostulaciones(Guid Id)
        {
            try
            {
                var postulaciones = db.Postulaciones
                    .Where(p => p.CandidatoId.Equals(Id) && p.Requisicion.Activo.Equals(true))
                    .Select(p => new
                    {
                        Folio = p.Requisicion.Folio,
                        vBtra = p.Requisicion.VBtra,
                        Estatus = p.Status.Status,
                        EstatusId = p.StatusId,
                        fecha = p.fch_Postulacion
                    }).OrderByDescending(p => p.Folio).ToList();
                return Ok(postulaciones);
            }
            catch (Exception ex)
            {
                string msd = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

    }
}
