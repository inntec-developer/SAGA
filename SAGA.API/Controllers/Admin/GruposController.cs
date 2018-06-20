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
        [Route("UploadImage")]
        public HttpResponseMessage UploadImage()
        {
            string imageName = null;
            var httpRequest = System.Web.HttpContext.Current.Request;
            var postedFile = httpRequest.Files["Image"];
            //imageName = new string(Path.GetFileNameWithoutExtension(postedFile.FileName).Take(10).ToArray()).Replace(" ", "-");
            //imageName = imageName + Path.GetExtension(postedFile.FileName);
            imageName = Path.GetFileName(postedFile.FileName);
            var filePath = HttpContext.Current.Server.MapPath("~/Utilerias/" + imageName);
            postedFile.SaveAs(filePath);

            return Request.CreateResponse(HttpStatusCode.Created);
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
                grupo.TipoEntidadId = 4;
                grupo.Foto = listJson.Foto;
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
                g.Foto = listJson.Foto;              
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
                    db.SaveChanges();
                }

               
                
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
            var grupos = db.Grupos.Where(g => g.Activo == true).Select(g => new
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


            return Ok(grupos);

        }



    }
}
