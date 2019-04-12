using SAGA.DAL;
using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.API.Dtos;
using SAGA.API.Controllers.Admin;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/Catalogos")]
    public class CatalogosController : ApiController
    {
        private SAGADBContext db;
        public CatalogosController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("getDocDamsa")]
        public IHttpActionResult GetDocDamsa()
        {
            try
            {
                var documentosDamsa = db.DocumentosDamsa.ToList();
                return Ok(documentosDamsa);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpGet]
        [Route("getPrestacionesLey")]
        public IHttpActionResult GetPrestacionesLey()
        {
            try
            {
                var prestacionesLey = db.PrestacionesLey.ToList();
                return Ok(prestacionesLey);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpGet]
        [Route("getPaises")]
        public IHttpActionResult GetPaises()
        {
            CandidatosDto Paises = new CandidatosDto();

            Paises.Paises = (from pais in db.Paises
                             where pais.Id == 42
                             select pais).ToList();

            return Ok(Paises);
        }

        [HttpGet]
        [Route("getDepa")]
        public IHttpActionResult getDepartamento()
        {
            var tu = db.Departamentos.Select(t => new { t.Id, t.Nombre }).OrderBy(x => x.Nombre).ToList();
            return Ok(tu);
        }

        [HttpGet]
        [Route("getTipos")]
        public IHttpActionResult getTiposUsuarios()
        {
            var tu = db.TiposUsuarios.Select(t => new { t.Id, t.Tipo }).ToList();
            return Ok(tu);
        }

        [HttpGet]
        [Route("getGrupos")]
        public IHttpActionResult getGrupos()
        {
            PersonalController obj = new PersonalController();
            List<GruposDtos> data = new List<GruposDtos>();
            var grupos = db.Grupos.Select(g => new
            {
                Id = g.Id,
                Foto = g.Foto,
                Activo = g.Activo,
                Descripcion = g.Descripcion,
                Nombre = g.Nombre,
                UsuarioAlta = g.UsuarioAlta,
                TipoGrupoId = db.TiposUsuarios.Where(x => x.Id.Equals(g.TipoGrupoId)).Select(id => id.Id).FirstOrDefault(),
                TipoGrupo = db.TiposUsuarios.Where(x => x.Id.Equals(g.TipoGrupoId)).Select(n => n.Tipo).FirstOrDefault()
            }).OrderBy(g => g.Nombre).ToList();

            foreach (var g in grupos)
            {
                var aux = obj.GetImage(g.Foto);
                data.Add(new GruposDtos
                {
                    Id = g.Id,
                    Foto = g.Foto,
                    Activo = g.Activo,
                    Descripcion = g.Descripcion,
                    Nombre = g.Nombre,
                    UsuarioAlta = g.UsuarioAlta,
                    TipoGrupoId = g.TipoGrupoId,
                    TipoGrupo = g.TipoGrupo,
                    FotoAux = aux
                });
            }

            obj = null;
            return Ok(data);
        }

        [HttpGet]
        [Route("getRoles")]
        public IHttpActionResult getRoles()
        {
            var roles = db.Roles.Where(x => x.Activo).ToList();
            return Ok(roles);
        }

        [HttpGet]
        [Route("getPrioridades")]
        public IHttpActionResult GetRPioridad()
        {
            var prioridad = db.Prioridades
                .Where(x => x.Activo.Equals(true))
                .ToList();
            return Ok(prioridad);
        }

        [HttpGet]
        [Route("getEstatus")]
        public IHttpActionResult getEstatus(int tipoMov)
        {
            var estatus = db.Estatus
                    .Where(x => x.TipoMovimiento.Equals(tipoMov))
                    .Where(x => x.Activo.Equals(true))
                    .ToList();
            return Ok(estatus);
        }

        [HttpGet]
        [Route("getMotivosLiberacion")]
        public IHttpActionResult GetMotivosLiberacion()
        {
            var motivo = db.MotivosLiberacion.Where(m => m.Activo.Equals(true)).ToList();
            return Ok(motivo);
        }

        [HttpGet]
        [Route("getTiposActividadesRecl")]
        public IHttpActionResult GetTiposActividadesRecl()
        {
            var actividad = db.TipoActividadReclutador
                                .Where(x => x.Activo.Equals(true))
                                .ToList().OrderBy(x => x.Actividad);
            return Ok(actividad);
        }

        [HttpGet]
        [Route("getTipoTelefono")]
        public IHttpActionResult GetTipoTelefono()
        {
            var tipo = db.TiposTelefonos.ToList();
            return Ok(tipo);
        }

        [HttpGet]
        [Route("getTipoDireccion")]
        public IHttpActionResult GetTipoDireccion()
        {
            var tipo = db.TiposDirecciones.ToList();
            return Ok(tipo);
        }

        #region Catalogos para Prospectos Clientes

        [HttpGet]
        [Route("getGiroEmp")]
        public IHttpActionResult GetGiroEmpresa()
        {
            var giro = db.GirosEmpresas.ToList();
            return Ok(giro);
        }

        [HttpGet]
        [Route("getActividadEmp")]
        public IHttpActionResult GetActivadadEmpresa(int GiroId)
        {
            var Actividad = db.ActividadesEmpresas
                .Select(x => new
                {
                    x.Id,
                    x.GiroEmpresaId,
                    x.actividadEmpresa
                })
                .Where(x => x.GiroEmpresaId.Equals(GiroId)).ToList();
            return Ok(Actividad);
        }

        [HttpGet]
        [Route("getTamanioEmp")]
        public IHttpActionResult GetTamanioEmpresa()
        {
            var tamanio = db.TamanoEmpresas.ToList();
            return Ok(tamanio);
        }

        [HttpGet]
        [Route("getTipoEmp")]
        public IHttpActionResult GetTipoempresa()
        {
            var tipo = db.TiposEmpresas.ToList();
            return Ok(tipo);
        }

        [HttpGet]
        [Route("getTipoBase")]
        public IHttpActionResult GetTipoBase()
        {
            var tipo = db.TiposBases.ToList();
            return Ok(tipo);
        }
        #endregion

        #region Localidades
        [HttpGet]
        [Route("getPais")]
        public IHttpActionResult GetPais()
        {
            var pais = db.Paises.Where(x => x.Id.Equals(42)).ToList();
            return Ok(pais);
        }

        [HttpGet]
        [Route("getEstado")]
        public IHttpActionResult GetEstado(int PaisId)
        {
            var estado = db.Estados
                .OrderBy(x => x.estado)
                .Where(x => x.PaisId.Equals(PaisId))
                .Select(x => new
                {
                    x.Id,
                    x.estado
                })
                .ToList();
            return Ok(estado);
        }

        [HttpGet]
        [Route("getMunicipio")]
        public IHttpActionResult GetMunicipo(int EstadoId)
        {
            var municipio = db.Municipios
                .OrderBy(x => x.municipio)
                .Where(x => x.EstadoId.Equals(EstadoId))
                .Select(x => new
                {
                    x.Id,
                    x.municipio
                })
                .ToList();
            return Ok(municipio);
        }

        [HttpGet]
        [Route("getColonia")]
        public IHttpActionResult GetColonias(int MunicipioId)
        {
            var municipio = db.Colonias
                .OrderBy(x => x.colonia)
                .Where(x => x.MunicipioId.Equals(MunicipioId))
                .Select(x => new
                {
                    x.Id,
                    x.colonia,
                    x.CP
                })
                .ToList();
            return Ok(municipio);
        }

        [HttpGet]
        [Route("getInfoCP")]
        public IHttpActionResult GetInfoCP(string CP)
        {
            var info = db.Colonias
                .Where(x => x.CP.Equals(CP))
                .Select(x => new
                {
                    x.Id,
                    x.colonia,
                    x.CP,
                    x.PaisId,
                    x.EstadoId,
                    x.MunicipioId,
                }).ToList().OrderBy(x => x.colonia);
            return Ok(info);
        }
        #endregion
    }
}
