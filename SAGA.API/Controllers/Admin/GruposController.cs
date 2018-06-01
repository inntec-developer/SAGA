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

namespace SAGA.API.Controllers.Admin
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
        public IHttpActionResult AddGrupo(GruposDtos listJson)
        {
            string msj = "Agregó";
          

            try
            {
                var grupo = Mapper.Map<GruposDtos, Grupos>(listJson);
                grupo.Nombre = listJson.Nombre;
                grupo.Activo = listJson.Activo;
                grupo.Descripcion = listJson.Descripcion;
                grupo.UsuarioAlta = "INNTEC";

                db.Grupos.Add(grupo);
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                msj = ex.Message;

            }
            return Ok(msj);
        }

        [HttpPost]
        [Route("updateGrupo")]
        public IHttpActionResult updateGrupo(Grupos listJson)
        {
            string msj = "Actualizó";
            try
            {
                var g = db.Grupos.Find(listJson.Id);

                db.Entry(g).State = EntityState.Modified;
                g.Nombre = listJson.Nombre;
                g.Descripcion = listJson.Descripcion;
                g.UsuarioAlta = "INNTEC";
                g.Activo = listJson.Activo;
                              
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = ex.Message;
            }

            return Ok(msj);
        }

        [HttpPost]
        [Route("deleteGrupo")]
        public IHttpActionResult deleteGrupo(Grupos listJson)
        {
            string msj = "Borró";
            try
            {
                var r = db.Grupos.Find(listJson.Id);

                db.Entry(r).State = EntityState.Modified;
                r.Activo = false;

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = ex.Message;
            }

            return Ok(msj);
        }


        [HttpPost]
        [Route("addUserGroup")]
        public IHttpActionResult AddUserGroup(List<GrupoUsuarios> listJson)
        {
            string msj = "Agrego";

            try
            {
                List<GrupoUsuarios> obj = new List<GrupoUsuarios>();

                foreach (GrupoUsuarios gu in listJson)
                {
                    db.GruposUsuarios.Add(gu);
                }

               
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                msj = ex.Message;
            }
            return Ok(msj);
        }

        [HttpGet]
        [Route("getGruposRoles")]
        public IHttpActionResult GetGruposRoles()
        {


            var grupos = db.Grupos.Select(g => new
            {
                Id = g.Id,
                Activo = g.Activo,
                Descripcion = g.Descripcion,
                Nombre = db.Entidad.Where(p => p.Id.Equals(g.Id)).Select(p => p.Nombre),
                UsuarioAlta = g.UsuarioAlta,
                roles = db.Privilegios.Where(p => p.EntidadId.Equals(g.Id)).Select(gr => new
                {
                    rol = db.Roles.Where(x => x.Id.Equals(gr.RolId)).Select(x => x.Rol)
                })
            }).ToList();


            return Ok(grupos);

        }



    }
}
