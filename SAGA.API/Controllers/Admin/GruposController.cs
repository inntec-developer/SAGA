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

       

    }
}
