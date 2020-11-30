using SAGA.API.Utilerias;
using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace SAGA.API.Controllers.SistFirmas
{
    [System.Web.Http.RoutePrefix("api/Firmas")]
    public class NominasController : ApiController
    {
        private SAGADBContext db;
        public NominasController()
        {
            db = new SAGADBContext();
        }
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("uploadFileNomina")]
        public IHttpActionResult UploadFileNomina()
        {
            string fileName = null;
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var postedFile = httpRequest.Files["file"];

                fileName = Path.GetFileName(postedFile.FileName);
                var idx = fileName.LastIndexOf('_') + 1;
                var lon = fileName.Length - idx;
                var dirName = fileName.Substring(idx, lon);

               // string fullPath = "E:\\inetpub\\wwwroot\\sagainn\\Saga\\API.sb\\Utilerias\\files\\SistFirmas\\Nominas\\" + dirName;
                string fullPath = System.Web.Hosting.HostingEnvironment.MapPath("~/utilerias/Files/SistFirmas/Nominas/" + dirName);
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                fileName = fileName.Substring(0, idx - 1);

                fullPath = fullPath + '/' + fileName;

                if (File.Exists(fullPath))
                    File.Delete(fullPath);

                postedFile.SaveAs(fullPath);

                return Ok(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.InternalServerError);
            }

        }
        [HttpGet]
        [Route("getFilesNomina")]
        public IHttpActionResult getFilesNomina(string dirName)
        {
            var dir = "E:\\inetpub\\wwwroot\\sagainn\\Saga\\API.sb\\Utilerias\\files\\SistFirmas\\Nominas\\" + dirName; // "~/utilerias/Files/SistFirmas/Nominas/" + dirName;
            string fullPath = "E:\\inetpub\\wwwroot\\sagainn\\Saga\\API.sb\\Utilerias\\files\\SistFirmas\\Nominas\\" + dirName; // System.Web.Hosting.HostingEnvironment.MapPath(dir);

            if (!Directory.Exists(fullPath))
            {
                return Ok(0);
            }
            else
            {
                DirectoryInfo folderInfo = new DirectoryInfo(fullPath);

                var files = folderInfo.GetFiles(
                        "*.*",
                        SearchOption.AllDirectories).Select(x => new
                        {
                            fullPath = x.FullName,
                            nom = x.Name,
                            ext = x.Extension,
                            size = (long)x.Length / 1024,
                            fc = x.LastWriteTime.ToShortDateString()
                        }).OrderByDescending(o => o.fc);

                return Ok(files);
            }

        }
        [HttpPost]
        [Route("addEventoNomina")]
        public IHttpActionResult AddEventoNomina(FIRM_BitacoraNomina datos)
        {
            try
            {
                FIRM_BitacoraNomina b = new FIRM_BitacoraNomina();
                datos.Fecha = Convert.ToDateTime(datos.Fecha.ToString("dd/MM/yyyy"));
                b = datos;

                b.fch_Creacion = DateTime.Now;
                b.fch_Modificacion = DateTime.Now;

                db.FIRM_BitacoraNomina.Add(b);
                db.SaveChanges();

                return Ok(b.Id);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);

            }
        }
        [HttpPost]
        [Route("updateEventoNomina")]
        public IHttpActionResult UpdateEventoNomina(FIRM_BitacoraNomina datos)
        {
            try
            {
                var d = db.FIRM_BitacoraNomina.Find(datos.Id);
                db.Entry(d).Property(x => x.Porques).IsModified = true;
                db.Entry(d).Property(x => x.fch_Modificacion).IsModified = true;

                d.Porques = datos.Porques;
                d.fch_Modificacion = DateTime.Now;

                db.SaveChanges();
                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);

            }

        }
        [HttpGet]
        [Route("getBitacoraNomina")]
        [Authorize]
        public IHttpActionResult GetBitacoraNomina()
        {
            try
            {
                var estatus = db.FIRM_EstatusNomina.Where(x => x.Activo).Select(e => new
                {
                    e.Id,
                    e.Estatus,
                    e.Observaciones
                });

                var bitacora = db.FIRM_BitacoraNomina.OrderByDescending(o => o.fch_Creacion).Where(x => x.Activo).Select(c => new
                {
                    id = c.Id,
                    c.Fecha,
                    estatus = c.EstatusNomina.Estatus,
                    retardo = c.Retardo ? "RETARDO" : "SIN RETARDO",
                    porques = c.Porques ? "SE ENVIÓ" : "NO SE ENVIÓ",
                    propietario = db.Usuarios.Where(x => x.Id.Equals(c.PropietarioId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()
                }).ToList();

                return Ok(new { estatus, bitacora });
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }

        }
        [HttpGet]
        [Route("getMiBitacoraNomina")]
        [Authorize]
        public IHttpActionResult GetMiBitacoraNomina(Guid propietarioId)
        {
            try
            {
                var estatus = db.FIRM_EstatusNomina.Where(x => x.Activo).Select(e => new
                {
                    e.Id,
                    e.Estatus,
                    e.Observaciones
                });

                var bitacora = db.FIRM_BitacoraNomina.OrderByDescending(o => o.fch_Creacion).Where(x => x.Activo && x.PropietarioId.Equals(propietarioId)).Select(c => new
                {
                    id = c.Id,
                    c.Fecha,
                    estatus = c.EstatusNomina.Estatus,
                    retardo = c.Retardo ? "RETARDO" : "SIN RETARDO",
                    porques = c.Porques ? "SE ENVIÓ" : "NO SE ENVIÓ",
                    propietario = db.Usuarios.Where(x => x.Id.Equals(c.PropietarioId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()
                }).ToList();

                return Ok(new { estatus, bitacora });
            }
            catch
            {
                return Ok(HttpStatusCode.BadRequest);
            }

        }
        [HttpGet]
        [Route("getPorquesNomina")]
        [Authorize]
        public IHttpActionResult GetPorquesNomina(Guid LiderId)
        {
            try
            {
                List<Guid> uids = new List<Guid>();
                GetSub fun = new GetSub();
                if (db.Subordinados.Count(x => x.LiderId.Equals(LiderId)) > 0)
                {
                    var ids = db.Subordinados.Where(x => !x.UsuarioId.Equals(LiderId) && x.LiderId.Equals(LiderId)).Select(u => u.UsuarioId).ToList();

                    uids = fun.RecursividadSub(ids, uids);

                }

                uids.Add(LiderId);
                var result = db.FIRM_Damfo022.OrderByDescending(o => o.fch_Creacion)
                    .Where(x => x.Activo && !x.Estatus).Select(d => new
                    {
                        d.Id,
                        d.Folio,
                        d.Fecha,
                        d.Descripcion,
                        d.Problema,
                        porques = db.FIRM_Porques.Where(x => x.Damfo022Id.Equals(d.Id)).Select(p => p.Porque).ToList(),
                        d.Causa_Raiz,
                        ishikawa = db.FIRM_CausaEfecto.Where(x => x.Damfo022Id.Equals(d.Id)).Select(i => i.Ishikawa.Causa),
                        compromisos = db.FIRM_Compromiso.Where(x => x.Damfo022Id.Equals(d.Id)).ToList(),
                        d.SolucionTmp,
                        d.Solucion,
                        usuarioAlta = db.Usuarios.Where(x => x.Id.Equals(d.UsuarioAlta)).Select(u => u.Clave + " " + u.Nombre + " " + u.ApellidoPaterno + " " + u.ApellidoMaterno),
                        usuarioDepa = db.Usuarios.Where(x => x.Id.Equals(d.UsuarioAlta)).Select(u => u.Departamento.Nombre)
                    }).ToList();

                return Ok(result);

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
    }
}