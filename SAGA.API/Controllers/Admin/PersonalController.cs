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
using System.Web;
using System.IO;
using System.Drawing;
using SAGA.API.Utilerias;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/admin")]
    public class PersonalController : ApiController
    {
        private SAGADBContext db;
        public PersonalController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("get")]
        public IHttpActionResult getDtosPersonal()
        {
            List<PersonasDtos> dts = new List<PersonasDtos>();

            //dts = db.Usuarios.Select(c => new PersonasDtos
            //{
            //    Id = c.Id,
            //    Clave = c.Clave,
            //    nombre = db.Personas.Where(p => p.Id.Equals(c.Id)).Select(x => x.Nombre).FirstOrDefault(),
            //    apellidoPaterno = db.Personas.Where(p => p.Id.Equals(c.Id)).Select(x => x.ApellidoPaterno).FirstOrDefault(),
            //    apellidoMaterno = db.Personas.Where(p => p.Id.Equals(c.Id)).Select(x => x.ApellidoMaterno).FirstOrDefault(),
            //    tipoUsuario = c.TipoUsuario.Tipo,
            //    Usuario = c.Usuario,
            //    Departamento = c.Departamento.Nombre,
            //    Email = db.Emails.Where(x => x.PersonaId.Equals(c.Id)).Select(e => new{
            //        Email = e.email,
            //        Ie.Id }).ToList()

            //}).ToList();

            var persona = db.Usuarios.Select(u => new
            {
                EntidadId = u.Id,
                Foto = u.Foto,
                Clave = u.Clave,
                nombre = u.Nombre,
                apellidoPaterno = u.ApellidoPaterno,
                apellidoMaterno = u.ApellidoMaterno,
                tipoUsuario = u.TipoUsuario.Tipo,
                tipoUsuarioId = u.TipoUsuario.Id,
                Usuario = u.Usuario,
                Departamento = u.Departamento.Nombre,
                DepartamentoId = u.Departamento.Id,
                Email = db.Emails.Where(x => x.EntidadId.Equals(u.Id)).Select(e => new {
                    email = e.email
                }),
                grupos = db.GruposUsuarios.Where(gu => gu.EntidadId.Equals(u.Id)).Select(g => new
                {
                    Id = g.GrupoId,
                    Nombre = db.Grupos.Where(x => x.Id.Equals(g.GrupoId)).Select(x => x.Nombre)
                }),
              
                activo = u.Activo

            }).OrderBy(o => o.nombre).ToList();

            return Ok(persona);

        }

        [HttpGet]
        [Route("getEntidades2")]
        public IHttpActionResult GetEntidades2()
        {
            List<PersonasDtos> data = new List<PersonasDtos>();

            var persona = db.Entidad.Where(x => x.TipoEntidadId.Equals(1) || x.TipoEntidadId.Equals(4)).Select(u => new
            {
                EntidadId = u.Id,
                userActivo = db.Usuarios.Where(x => x.Id.Equals(u.Id)).Select(c => c.Activo).FirstOrDefault(),
                grupoActivo = db.Grupos.Where(x => x.Id.Equals(u.Id)).Select(c => c.Activo).FirstOrDefault(),
                Foto = String.IsNullOrEmpty(u.Foto) ? "utilerias/img/user/default.jpg" : u.Foto,
                Clave = db.Usuarios.Where(x => x.Id.Equals(u.Id)).Select(c => c.Clave).FirstOrDefault(),
                nombre = u.Nombre,
                apellidoPaterno = string.IsNullOrEmpty(u.ApellidoPaterno) ? "" : u.ApellidoPaterno,
                apellidoMaterno = string.IsNullOrEmpty(u.ApellidoMaterno) ? "" : u.ApellidoMaterno,
                Usuario = db.Usuarios.Where(x => x.Id.Equals(u.Id)).Select(c => c.Usuario).FirstOrDefault(),
                Descripcion = db.Grupos.Where(x => x.Id.Equals(u.Id)).Select(x => string.IsNullOrEmpty(x.Descripcion) ? "" : x.Descripcion).FirstOrDefault(),
                Departamento = db.Usuarios.Where(x => x.Id.Equals(u.Id)).Select(c => c.Departamento.Nombre).FirstOrDefault(),
                Emails = db.Emails.Where(x => x.EntidadId.Equals(u.Id)).Select(e => new {
                    email = e.email
                }),
                grupos = db.GruposUsuarios.Where(gu => gu.EntidadId.Equals(u.Id)).Select(g => new
                {
                    Id = g.GrupoId,
                    Grupo = g.Grupo.Nombre,
                    Rol = db.RolEntidades.Where(x => x.EntidadId.Equals(g.GrupoId)).Select(r => new
                    {
                        id = r.RolId,
                        rol = r.Rol.Rol

                    })

                }),
                roles = db.RolEntidades.Where(x => x.EntidadId.Equals(u.Id)).Select(r => new
                {
                    id = r.RolId,
                    rol = r.Rol.Rol

                })

            }).Where(a => a.grupoActivo || a.userActivo).OrderBy(o => o.nombre).ToList();


            //var mocos = persona.Where(u => u.userActivo).ToList();
            //var mocos2 = persona.Where(g => g.grupoActivo).ToList();

            //var mocos3 = mocos.Union(mocos2);
            return Ok(persona);

        }

        [HttpGet]
        [Route("getEntidades")]
        public IHttpActionResult GetEntidades()
        {
            List<PersonasDtos> data = new List<PersonasDtos>();

            var persona = db.Entidad.Where(x => x.TipoEntidadId.Equals(1) || x.TipoEntidadId.Equals(4)).Select(u => new
            {
                EntidadId = u.Id,
                Foto = String.IsNullOrEmpty(u.Foto) ? "utilerias/img/user/default.jpg" : u.Foto,
                Clave = db.Usuarios.Where(x => x.Id.Equals(u.Id)).Select(c => c.Clave).FirstOrDefault(),
                userActivo = db.Usuarios.Where(x => x.Id.Equals(u.Id)).Select(c => c.Activo).FirstOrDefault(),
                grupoActivo = db.Grupos.Where(x => x.Id.Equals(u.Id)).Select(c => c.Activo).FirstOrDefault(),
                nombre = u.Nombre,
                apellidoPaterno = string.IsNullOrEmpty(u.ApellidoPaterno) ? "" : u.ApellidoPaterno,
                apellidoMaterno = string.IsNullOrEmpty(u.ApellidoMaterno) ? "" : u.ApellidoMaterno,
                Usuario = db.Usuarios.Where(x => x.Id.Equals(u.Id)).Select(c => c.Usuario).FirstOrDefault(),
                Descripcion = db.Grupos.Where(x => x.Id.Equals(u.Id)).Select(x => string.IsNullOrEmpty(x.Descripcion) ? "" : x.Descripcion).FirstOrDefault(),
                Departamento = db.Usuarios.Where(x => x.Id.Equals(u.Id)).Select(c => c.Departamento.Nombre).FirstOrDefault(),
                Emails = db.Emails.Where(x => x.EntidadId.Equals(u.Id)).Select(e => new {
                    email = e.email
                }),
                grupos = db.GruposUsuarios.Where(gu => gu.EntidadId.Equals(u.Id)).Select(g => new
                {
                    Id = g.GrupoId,
                    Grupo = db.Grupos.Where(x => x.Id.Equals(g.GrupoId)).Select(x => x.Nombre).FirstOrDefault(),

                }),
                roles = db.RolEntidades.Where(x => x.EntidadId.Equals(u.Id)).Select(r => new
                {
                    id = r.RolId,
                    rol = r.Rol.Rol

                })

            }).Where(a => a.grupoActivo || a.userActivo).OrderBy(o => o.nombre).ToList();

            //foreach (var g in persona)
            //{
            //    var aux = GetImage(g.Foto);
            //    data.Add(new PersonasDtos()
            //    {
            //        EntidadId = g.EntidadId,
            //        Foto = g.Foto,
            //        Clave = g.Clave,
            //        nombre = g.nombre,
            //        apellidoPaterno = g.apellidoPaterno,
            //        apellidoMaterno = g.apellidoMaterno,
            //        Usuario = g.Usuario,
            //        Descripcion = g.Descripcion,
            //        FotoAux = aux,
            //        Email = g.Emails.Select(x => new Email {
            //               email = x.email.ToString()
            //       }).ToList()
            //    });
            //}


            return Ok(persona);

        }

        [HttpGet]
        [Route("getUsuarioByDepa")]
        public IHttpActionResult getDtosByDepa(Guid id)
        {
            List<PersonasDtos> dts = new List<PersonasDtos>();

            var persona = db.Usuarios.Where(a => a.DepartamentoId.Equals(id)).Select(u => new
            {
                Id = u.Id,
                Clave = u.Clave,
                nombre = db.Entidad.Where(p => p.Id.Equals(u.Id)).Select(p => p.Nombre),
                apellidoPaterno = db.Entidad.Where(p => p.Id.Equals(u.Id)).Select(p => p.ApellidoPaterno),
                apellidoMaterno = db.Entidad.Where(p => p.Id.Equals(u.Id)).Select(p => p.ApellidoMaterno),
                tipoUsuario = u.TipoUsuario.Tipo,
                Usuario = u.Usuario,
                Departamento = u.Departamento.Nombre,
                Email = db.Emails.Where(x => x.EntidadId.Equals(u.Id)).Select(e => new {
                    email = e.email
                }),
                activo = u.Activo

            }).ToList();

            return Ok(persona);
        }

        [HttpGet]
        [Route("getUsuarioByGrupo")]
        public IHttpActionResult GetDtosByGrupo(Guid id)
        {
            List<PersonasDtos> data = new List<PersonasDtos>();
            try
            {
                var persona = db.GruposUsuarios.Where(x => x.GrupoId.Equals(id) & x.Grupo.Activo).Select(u => new
                {
                    EntidadId = u.EntidadId,
                    Foto = db.Entidad.Where(x => x.Id.Equals(u.EntidadId)).Select(f => String.IsNullOrEmpty(f.Foto) ? "utilerias/img/user/default.jpg" : f.Foto).FirstOrDefault(),
                    TipoEntidadId = db.Entidad.Where(x => x.Id.Equals(u.EntidadId)).Select(n => n.TipoEntidadId).FirstOrDefault(),
                    nombre = db.Entidad.Where(x => x.Id.Equals(u.EntidadId)).Select(n => n.Nombre).FirstOrDefault(),
                    apellidoPaterno = db.Entidad.Where(x => x.Id.Equals(u.EntidadId)).Select(n => string.IsNullOrEmpty(n.ApellidoPaterno) ? "" : n.ApellidoPaterno).FirstOrDefault(),
                    apellidoMaterno = db.Entidad.Where(x => x.Id.Equals(u.EntidadId)).Select(n => string.IsNullOrEmpty(n.ApellidoMaterno) ? "" : n.ApellidoMaterno).FirstOrDefault(),
                    Usuario = db.Usuarios.Where(x => x.Id.Equals(u.EntidadId)).Select(c => string.IsNullOrEmpty(c.Usuario) ? "" : c.Usuario).FirstOrDefault(),
                    Departamento = db.Usuarios.Where(x => x.Id.Equals(u.EntidadId)).Select(c => c.Departamento.Nombre).FirstOrDefault(),
                    grupos = db.GruposUsuarios.Where(gu => gu.EntidadId.Equals(u.EntidadId)).Select(g => new
                    {
                        Id = g.GrupoId,
                        Grupo = db.Entidad.Where(x => x.Id.Equals(g.GrupoId)).Select(x => x.Nombre).FirstOrDefault()
                    })

                }).OrderBy(o => o.TipoEntidadId).ToList();

                //foreach (var g in persona)
                //{
                //    var aux = GetImage(g.Foto);
                //    data.Add(new PersonasDtos()
                //    {
                //        EntidadId = g.EntidadId,
                //        Foto = g.Foto,
                //        TipoEntidadID = g.TipoEntidadId,
                //        nombre = g.nombre,
                //        apellidoPaterno = g.apellidoPaterno,
                //        apellidoMaterno = g.apellidoMaterno,
                //        Usuario = g.Usuario,
                //        FotoAux = aux
                //    });
                //}

                return Ok(persona);
            }
            catch( Exception ex)
            {
                return Ok(ex);
            }

        }

        [HttpGet]
        [Route("getEntidadesByRol")]
        public IHttpActionResult GetDtosByRol(int id)
        {
            List<PersonasDtos> data = new List<PersonasDtos>();
            try
            {
                var persona = db.RolEntidades.Where(x => x.RolId.Equals(id) & x.Rol.Activo).Select(u => new
                {
                    EntidadId = u.EntidadId,
                    Foto = db.Entidad.Where(x => x.Id.Equals(u.EntidadId)).Select(f => String.IsNullOrEmpty(f.Foto) ? "utilerias/img/user/default.jpg" : f.Foto).FirstOrDefault(),
                    TipoEntidadId = db.Entidad.Where(x => x.Id.Equals(u.EntidadId)).Select(n => n.TipoEntidadId).FirstOrDefault(),
                    nombre = db.Entidad.Where(x => x.Id.Equals(u.EntidadId)).Select(n => n.Nombre).FirstOrDefault(),
                    apellidoPaterno = db.Entidad.Where(x => x.Id.Equals(u.EntidadId)).Select(n => string.IsNullOrEmpty(n.ApellidoPaterno) ? "" : n.ApellidoPaterno).FirstOrDefault(),
                    apellidoMaterno = db.Entidad.Where(x => x.Id.Equals(u.EntidadId)).Select(n => string.IsNullOrEmpty(n.ApellidoMaterno) ? "" : n.ApellidoMaterno).FirstOrDefault(),
                    Usuario = db.Usuarios.Where(x => x.Id.Equals(u.EntidadId)).Select(c => string.IsNullOrEmpty(c.Usuario) ? "" : c.Usuario).FirstOrDefault(),
                    Departamento = db.Usuarios.Where(x => x.Id.Equals(u.EntidadId)).Select(c => c.Departamento.Nombre).FirstOrDefault(),
                    Descripcion = db.GruposUsuarios.Where(x => x.GrupoId.Equals(u.EntidadId)).Select(d => d.Grupo.Descripcion).FirstOrDefault(),
                    grupos = db.GruposUsuarios.Where(gu => gu.EntidadId.Equals(u.EntidadId)).Select(g => new
                    {
                        Id = g.GrupoId,
                        Grupo = db.Entidad.Where(x => x.Id.Equals(g.GrupoId)).Select(x => x.Nombre).FirstOrDefault()
                    })

                }).OrderBy(o => o.TipoEntidadId).ToList();

                //foreach (var g in persona)
                //{
                //    var aux = GetImage(g.Foto);
                //    data.Add(new PersonasDtos()
                //    {
                //        EntidadId = g.EntidadId,
                //        Foto = g.Foto,
                //        TipoEntidadID = g.TipoEntidadId,
                //        nombre = g.nombre,
                //        apellidoPaterno = g.apellidoPaterno,
                //        apellidoMaterno = g.apellidoMaterno,
                //        Usuario = g.Usuario,
                //        FotoAux = aux
                //    });
                //}

                return Ok(persona);
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }

        }


        [HttpPost]
        [Route("addUsuario")]
        public IHttpActionResult AddUsuario(PersonasDtos listJson)
        {
            try
            {
                //var usuario = Mapper.Map<PersonasDtos, Usuarios>(listJson);
                var usuario = new Usuarios();

                usuario.Clave = listJson.Clave;
                usuario.Usuario = listJson.Usuario;
                usuario.Nombre = listJson.nombre;
                usuario.ApellidoPaterno = listJson.apellidoPaterno;
                usuario.ApellidoMaterno = listJson.apellidoMaterno;
                usuario.emails = listJson.Email;
                usuario.DepartamentoId = listJson.DepartamentoId;
                usuario.UsuarioAlta = "INNTEC";
                usuario.TipoUsuarioId = 0;
                usuario.Password = listJson.Password;
                usuario.TipoEntidadId = 1;
                usuario.Foto = listJson.Foto;

                db.Usuarios.Add(usuario);
                db.SaveChanges();

                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpPost]
        [Route("sendEmailRegister")]
        public IHttpActionResult SendEmailRegister(PersonasDtos Dtos)
        {
            try
            {
                SendEmails obj = new SendEmails();
                obj.SendEmailRegistro(Dtos);
              
                

                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("udActivo")]
        public IHttpActionResult UdActivo(Guid id, bool v)
        {
            try
            {
                var usuario = db.Usuarios.Find(id);

                db.Usuarios.Attach(usuario);
                usuario.Activo = v;

                db.SaveChanges();

                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpPost]
        [Route("updateUsuario")]
        public IHttpActionResult UpdateUsuario(PersonasDtos listJson)
        {
            try
            {
                var usuario = db.Usuarios.Find(listJson.EntidadId);
                db.Entry(usuario).State = EntityState.Modified;
                usuario.Clave = listJson.Clave;
                usuario.Usuario = listJson.Usuario;
                usuario.Nombre = listJson.nombre;
                usuario.ApellidoPaterno = listJson.apellidoPaterno;
                usuario.ApellidoMaterno = listJson.apellidoMaterno;
                usuario.DepartamentoId = listJson.DepartamentoId;
                usuario.UsuarioAlta = "INNTEC";
                usuario.TipoUsuarioId = listJson.TipoUsuarioId;
                usuario.Foto = listJson.Foto;
                db.SaveChanges();

                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpPost]
        [Route("UploadImage")]
        public IHttpActionResult UploadImage()
        {
            string imageName = null;

            try
            {
                var httpRequest = HttpContext.Current.Request;
                var postedFile = httpRequest.Files["image"];
                var id = Guid.Parse(Path.GetFileNameWithoutExtension(postedFile.FileName).ToString());
                //var id = new string(Path.GetFileNameWithoutExtension(postedFile.FileName).Take(10).ToArray()).Replace(" ", "-");

                //imageName = imageName + Path.GetExtension(postedFile.FileName);
                imageName = Path.GetFileName(postedFile.FileName);

                var path = "~/utilerias/img/user/" + imageName;

                string fullPath = System.Web.Hosting.HostingEnvironment.MapPath(path);

                if (File.Exists(fullPath))
                    File.Delete(fullPath);

                postedFile.SaveAs(fullPath);

                ActualizarFoto(id, imageName);

                return Ok(HttpStatusCode.Created); //201

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.InternalServerError);
            }

        }

        [HttpGet]
        [Route("getImage")]
        public IHttpActionResult GetImage2(string ruta)
        {
           string fullPath;

            try
            {
                fullPath = System.Web.Hosting.HostingEnvironment.MapPath("~/utilerias/img/user/" + ruta);
            }
            catch
            {
                fullPath = System.Web.Hosting.HostingEnvironment.MapPath("~/utilerias/img/user/default.jpg");

            }

           

            Bitmap bmp = new Bitmap(fullPath);
            FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            byte[] bimage = new byte[fs.Length];
            fs.Read(bimage, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            
            return Ok(Convert.ToBase64String(bimage));

        }


        public string GetImage(string ruta)
        {
            List<byte[]> aux = new List<byte[]>();
            string fullPath;
            try
            {
                fullPath = System.Web.Hosting.HostingEnvironment.MapPath("~/" + ruta);
            }
            catch
            {
                fullPath = System.Web.Hosting.HostingEnvironment.MapPath("~/utilerias/img/user/default.jpg" );

            }

           // Bitmap bmp = new Bitmap(fullPath);
            FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.ReadWrite);
            byte[] bimage = new byte[fs.Length];
            fs.Read(bimage, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            fs = null;

            return ("data:image/jpeg;base64," + Convert.ToBase64String(bimage));
        }

        public Boolean ActualizarFoto(Guid id, string imageName)
        {
            
           // var entidad = db.Entidad.Where(x => x.Id.Equals(id)).FirstOrDefault();
            var entidad = db.Entidad.Find(id);

            db.Entry(entidad).State = EntityState.Modified;
            entidad.Foto = "utilerias/img/user/" + imageName;

            db.SaveChanges();

            return true;
        }


        [HttpPost]
        [Route("deleteUserGroup")]
        public IHttpActionResult DeleteUserGroup(GrupoUsuarios indices)
        {
            try
            {
               var idGU =  db.GruposUsuarios
                    .Where(x => x.EntidadId.Equals(indices.EntidadId))
                    .Where(x => x.GrupoId.Equals(indices.GrupoId))
                    .Select(x => x.Id).FirstOrDefault();
       
                var dts = db.GruposUsuarios.Find(idGU);

                db.Entry(dts).State = EntityState.Deleted;

                db.SaveChanges();

                return Ok(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpGet]
        [Route("validarEmail")]
        public IHttpActionResult ValidarEmail(string e)
        {
            var Data =
                   (from users in db.Usuarios
                    join email in db.Emails on users.Id equals email.EntidadId
                    where email.email == e & users.TipoEntidadId.Equals(1)
                    select new
                    {
                        id = users.Id,
                        nombre = users.Nombre + " " + users.ApellidoPaterno,
                        usuario = users.Usuario,
                        activo = users.Activo,
                        email = email.email,
                        foto = users.Foto,
                        clave = users.Clave
                    }).ToList();

            if (Data.Count() > 0)
            {
                if (Data.Select(x => x.activo).FirstOrDefault())
                {
                    return Ok(HttpStatusCode.Found);
                }
                else
                    return Ok(HttpStatusCode.Found); // 302 encontro pero no esta activo
            }
            else
            {
                return Ok(HttpStatusCode.NotFound); // 404 no esta registrado
            }
        }

        [HttpGet]
        [Route("validarDAL")]
        public IHttpActionResult ValidarDAL(string dal)
        {
            var Data =
                   (from users in db.Usuarios
                    where users.Clave == dal & users.TipoEntidadId.Equals(1)
                    select new
                    {
                        id = users.Id,
                        nombre = users.Nombre + " " + users.ApellidoPaterno,
                        usuario = users.Usuario,
                        activo = users.Activo,
                        foto = users.Foto,
                        clave = users.Clave
                    }).ToList();

            if (Data.Count() > 0)
            {
                if (Data.Select(x => x.activo).FirstOrDefault())
                {
                    return Ok(HttpStatusCode.Found); //302 ya esta
                }
                else
                    return Ok(HttpStatusCode.Found); // 302 encontro pero no esta activo
            }
            else
            {
                return Ok(HttpStatusCode.NotFound); // 404 no esta registrado
            }
        }

        [HttpGet]
        [Route("setUsers")]
        public IHttpActionResult SetUsers(string p, string e)
        {
            PrivilegiosController obj = new PrivilegiosController();
            UsuarioDto userData = new UsuarioDto();

            var Data =
                   (from users in db.Usuarios
                    join email in db.Emails on users.Id equals email.EntidadId
                    where users.Password == p & email.email == e & users.TipoEntidadId == 1
                    select new
                    {
                        id = users.Id,
                        nombre = users.Nombre + " " + users.ApellidoPaterno,
                        usuario = users.Usuario,
                        activo = users.Activo,
                        email = email.email,
                        foto = users.Foto,
                        clave = users.Clave,
                        tipousuario = users.TipoUsuarioId
                    }).ToList();

            if(Data.Count() > 0)
            {
                if (Data.Select(x => x.activo).FirstOrDefault() == true)
                {
                    userData.Id = Data.Select(x => x.id).FirstOrDefault();
                    userData.Nombre = Data.Select(x => x.nombre).FirstOrDefault();
                    userData.Usuario = Data.Select(x => x.usuario).FirstOrDefault();
                    userData.Privilegios = obj.GetPrivilegios(userData.Id);
                    userData.Email = Data.Select(x => x.email).FirstOrDefault();
                    userData.Foto = Data.Select(x => x.foto).FirstOrDefault();
                    userData.Clave = Data.Select(x => x.clave).FirstOrDefault();

                    return Ok(userData);
                }
                else if(Data.Select(x => x.activo).FirstOrDefault() == false && db.Roles.ToList().Count() == 0)
                {
                    userData.Id = Data.Select(x => x.id).FirstOrDefault();
                    userData.Nombre = Data.Select(x => x.nombre).FirstOrDefault();
                    userData.Usuario = Data.Select(x => x.usuario).FirstOrDefault();
                    userData.Privilegios = obj.GetPrivilegiosDios();
                    userData.Email = Data.Select(x => x.email).FirstOrDefault();
                    userData.Foto = Data.Select(x => x.foto).FirstOrDefault();
                    userData.Clave = Data.Select(x => x.clave).FirstOrDefault();

                    return Ok(userData);
                }
                else
                {
                    return Ok(HttpStatusCode.NotAcceptable);
                }
                    
            }
            else
            {
                return Ok(HttpStatusCode.NotFound);
            }
        }

    }
}
