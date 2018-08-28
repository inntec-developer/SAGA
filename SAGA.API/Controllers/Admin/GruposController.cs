using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.DAL;
using SAGA.BOL;
using SAGA.API.Dtos;
using AutoMapper;
using System.Data.Entity;
using System.IO;
using System.Web;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/admin")]
    public class GruposController : ApiController
    {
        private SAGADBContext db;
        public GruposController()
        {
            db = new SAGADBContext();
        }
 
        [HttpPost]
        [Route("addGrupo")]
        public IHttpActionResult AddGrupo(Grupos listJson)
        {
            try
            {
                Grupos grupo = new Grupos();
                grupo.Nombre = listJson.Nombre;
                grupo.Activo = listJson.Activo;
                grupo.Descripcion = listJson.Descripcion;
                grupo.UsuarioAlta = "INNTEC";
                grupo.TipoEntidadId = 4;
                grupo.Foto = listJson.Foto;
                grupo.TipoGrupoId = listJson.TipoGrupoId;
                db.Grupos.Add(grupo);
                db.SaveChanges();

                return Ok(HttpStatusCode.Created);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
            
        }

        [HttpPost]
        [Route("updateGrupo")]
        public IHttpActionResult updateGrupo(Grupos listJson)
        {
            try
            {
                var g = db.Grupos.Find(listJson.Id);

                db.Entry(g).State = EntityState.Modified;
                g.Nombre = listJson.Nombre;
                g.Descripcion = listJson.Descripcion;
                g.UsuarioAlta = "INNTEC";
                g.Activo = listJson.Activo;
                g.Foto = listJson.Foto;
                g.TipoGrupoId = listJson.TipoGrupoId;
                db.SaveChanges();

                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

           
        }

        [HttpPost]
        [Route("deleteGrupo")]
        public IHttpActionResult deleteGrupo(Grupos listJson)
        {
            try
            {
                var r = db.Grupos.Find(listJson.Id);

                db.Entry(r).State = EntityState.Modified;
                r.Activo = false;

                db.SaveChanges();

                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }


        [HttpPost]
        [Route("addUserGroup")]
        public IHttpActionResult AddUserGroup(List<GrupoUsuarios> listJson)
        {
            try
            {
                List<GrupoUsuarios> obj = new List<GrupoUsuarios>();

                foreach (GrupoUsuarios gu in listJson)
                {
                    db.GruposUsuarios.Add(gu);
                    db.SaveChanges();
                }

                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("getGruposRoles")]
        public IHttpActionResult GetGruposRoles()
        {
            var grupos = db.Grupos.Where(x => x.Activo == true).Select(g => new
            {
                Id = g.Id,
                Foto = g.Foto,
                Activo = g.Activo,
                Descripcion = g.Descripcion,
                Nombre = db.Entidad.Where(p => p.Id.Equals(g.Id)).Select(p => p.Nombre).FirstOrDefault(),
                UsuarioAlta = g.UsuarioAlta,
                roles = db.RolEntidades.Where(x => x.EntidadId.Equals(g.Id)).Select(r => new
                {
                    id = r.Id,
                    rol = r.Rol.Rol

                })
            }).OrderBy(o => o.Nombre).ToList();

            //var grupos = db.Grupos.Where(g => g.Activo == true).Select(g => new
            //{
            //    Id = g.Id,
            //    Foto = g.Foto,
            //    Activo = g.Activo,
            //    Descripcion = g.Descripcion,
            //    Nombre = db.Entidad.Where(p => p.Id.Equals(g.Id)).Select(p => p.Nombre).FirstOrDefault(),
            //    UsuarioAlta = g.UsuarioAlta,
            //    roles = db.RolEntidades.Where(x => x.EntidadId.Equals(g.Id)).Select(r => new
            //    {
            //        id = r.Id,
            //        rol = r.Rol.Rol

            //    })
            //}).OrderBy(o => o.Nombre).ToList();


            return Ok(grupos);

        }


    }
}
