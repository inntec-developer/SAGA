using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using SAGA.BOL;
using System.IO;
using SAGA.API.Utilerias;
using SAGA.API.Dtos.SistFirmas;

namespace SAGA.API.Controllers.SistFirmas
{
    [RoutePrefix("api/Firmas")]
    public class FirmasController : ApiController
    {
        private SAGADBContext db;
        public FirmasController()
        {
            db = new SAGADBContext();
        }
        [HttpPost]
        [Route("uploadFile")]
        public IHttpActionResult UploadFile(FirmasDto datos)
        {
            SendEmails obj = new SendEmails();
            try
            {
                //var filename = datos.name + "_" + DateTime.Now.ToString("ddMMyyyyHHMM") + datos.ext;
                var filename = datos.filename;
                string fullPath = "E:\\inetpub\\wwwroot\\sagainn\\Saga\\API.sb\\Utilerias\\files\\SistFirmas\\" + filename;
                    // System.Web.Hosting.HostingEnvironment.MapPath("~/utilerias/Files/SistFirmas/" + filename);
                if (datos.type.Length > 0)
                {
                    datos.file = datos.file.Replace("data:" + datos.type + ";base64,", "");
                }
                byte[] bytes = Convert.FromBase64String(datos.file);
                File.WriteAllBytes(fullPath, bytes);

                bool enviado = obj.EmailSistFirmas(datos, "E:\\inetpub\\wwwroot\\sagainn\\Saga\\API.sb\\Utilerias\\files\\SistFirmas\\", filename);
                if (enviado)
                {
                    return Ok(new { result = HttpStatusCode.OK, path = filename });
                }
                else
                {
                    return Ok(new { result = HttpStatusCode.ExpectationFailed, path = filename });
                }

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
            //var httpRequest = HttpContext.Current.Request;
            //var postedFile = httpRequest.Files["file"];

            //fileName = Path.GetFileName(postedFile.FileName);
            //var idx = fileName.LastIndexOf('_') + 1;
            //var lon = fileName.Length - idx;
            //var id = Convert.ToInt16(fileName.Substring(idx, lon));
            //var estatus = db.FIRM_EstatusBitacora.Where(x => x.Id.Equals(id)).Select(e => e.Estatus).FirstOrDefault(); 
            //string fullPath = System.Web.Hosting.HostingEnvironment.MapPath("~/utilerias/Files/SistFirmas/" + estatus + "_" + DateTime.Now.ToString("ddMMyyyyHHMM") + ".txt");

        }
        [HttpPost]
        [Route("altaDamfo022")]
        public IHttpActionResult AltaDamfo022(Damfo022Dto datos)
        {
            try
            {
                var consecutivo = db.FIRM_Damfo022.Count();
                var folio = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + consecutivo.ToString().PadLeft(4,'0');
                FIRM_Damfo022 damfo = new FIRM_Damfo022();
                damfo = datos.Damfo022;
                damfo.Folio = folio;
                damfo.Activo = true;
                damfo.fch_Creacion = DateTime.Now;
                damfo.fch_Modificacion = DateTime.Now;

                db.FIRM_Damfo022.Add(damfo);
                db.SaveChanges();

                foreach (FIRM_Porques d in datos.Porques)
                {
                    d.Damfo022Id = damfo.Id;
                }
                db.FIRM_Porques.AddRange(datos.Porques);
                db.SaveChanges();
                foreach (FIRM_CausaEfecto ce in datos.CausasEfecto)
                {
                    ce.Damfo022Id = damfo.Id;
                }
                db.FIRM_CausaEfecto.AddRange(datos.CausasEfecto);
                db.SaveChanges();
                foreach (FIRM_Compromiso c in datos.Compromisos)
                {
                    c.Damfo022Id = damfo.Id;
                }
                db.FIRM_Compromiso.AddRange(datos.Compromisos);
                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            } catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
        [HttpPost]
        [Route("cerrarDamfo022")]
        public IHttpActionResult CerrarDamfo022(Damfo022Dto datos)
        {
            try
            {
                var d = db.FIRM_Damfo022.Find(datos.Id);
                db.Entry(d).Property(x => x.Estatus).IsModified = true;
                db.Entry(d).Property(x => x.UsuarioMod).IsModified = true;
                db.Entry(d).Property(x => x.fch_Modificacion).IsModified = true;
                db.Entry(d).Property(x => x.Solucion).IsModified = true;

                d.Estatus = datos.Estatus;
                d.UsuarioMod = datos.UsuarioMod;
                d.fch_Modificacion = DateTime.Now;
                d.Solucion = datos.Solucion;

                db.SaveChanges();

                return Ok(HttpStatusCode.OK);

            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
        [HttpPost]
        [Route("addEvento")]
        public IHttpActionResult AddEvento(BitacoraDto datos)
        {
            try
            {
                string path = datos.Bitacora.FilePath.Length == 0 ? "N/A" : "Files/SistFirmas/" + datos.Bitacora.FilePath;
                FIRM_Bitacora b = new FIRM_Bitacora();
                b = datos.Bitacora;
                b.fch_Creacion = DateTime.Now;
                b.fch_Modificacion = DateTime.Now;
                b.FilePath = path;
                b.FechasEstatusId = db.FIRM_FechasEstatus.Where(x => x.ConfigBitacoraId.Equals(datos.ConfigBitacoraId) && x.EstatusBitacoraId.Equals(datos.EstatusBitacoraId)).Select(F => F.Id).FirstOrDefault();
                db.FIRM_Bitacora.Add(b);

                db.SaveChanges();

                return Ok(b.Id);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);

            }

        }
        [HttpPost]
        [Route("updateEvento")]
        public IHttpActionResult UpdateEvento(FIRM_Bitacora datos)
        {
            try
            {
                var d = db.FIRM_Bitacora.Find(datos.Id);
                db.Entry(d).Property(x => x.Porques).IsModified = true;
                db.Entry(d).Property(x => x.fch_Modificacion).IsModified = true;

                d.Porques = datos.Porques;
                d.fch_Modificacion = DateTime.Now;

                db.SaveChanges();
                return Ok(HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);

            }

        }
        [HttpPost]
        [Route("enviarNotificacion")]
        public IHttpActionResult EnviarNotificacion(FirmasDto datos)
        {
            try
            {
                SendEmails obj = new SendEmails();
                if(obj.EmailSistFirmas(datos, "", ""))
                {
                    return Ok(HttpStatusCode.OK);
                } else
                {
                    return Ok(HttpStatusCode.BadRequest);
                }
            
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);

            }

        }
        [HttpGet]
        [Route("getConfigBitacora")]
        public IHttpActionResult GetConfigBitacora(int sucursalId)
        {
            try
            {

                var bitacora = db.FIRM_ConfigBitacora.Where(x => x.Activo && x.SucursalesId.Equals(sucursalId)).Select(c => new
                {
                    id = c.Id,
                    sucursal = c.Sucursales.Nombre,
                    soporte = c.SoportesNomina.Soporte,
                    registro_pat = c.Sucursales.RegistroPatronal.RP_Clave,
                    clave_nomina = c.TipodeNomina.Clave,
                    tipo_nomina = c.TipodeNomina.tipoDeNomina,
                    destinatario = db.Usuarios.Where(x => x.Id.Equals(c.Destinatario)).Select(e => e.emails.Select(ee => ee.email).FirstOrDefault()),
                    tiempos = db.FIRM_FechasEstatus.Where(x => x.ConfigBitacoraId.Equals(c.Id)).Select(t => new
                    {
                        t.Id,
                        DiaSemanaId = t.WeekDay,
                        t.Hour,
                        t.Fecha,
                        EstatusId = t.EstatusBitacoraId,
                        t.EstatusBitacora.Estatus
                    }).ToList()
                }).ToList();

                var estatus = db.FIRM_EstatusBitacora.Where(x => x.Activo).Select(e => new
                {
                    e.Id,
                    e.Estatus,
                    e.Observaciones
                }).ToList();
                return Ok(new { bitacora, estatus });
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);

            }

        }
        [HttpGet]
        [Route("getMiBitacora")]
        public IHttpActionResult GetMiBitacora(Guid propietarioId)
        {
            try
            {
                var bitacora = db.FIRM_Bitacora.OrderByDescending(o => o.fch_Creacion).Where(x => x.Activo && x.PropietarioId.Equals(propietarioId)).Select(c => new
                {
                    id = c.Id,
                    sucursal = c.FechasEstatus.ConfigBitacora.Sucursales.Nombre,
                    soporte = c.FechasEstatus.ConfigBitacora.SoportesNomina.Soporte,
                    registro_pat = c.FechasEstatus.ConfigBitacora.Sucursales.RegistroPatronal.RP_Clave,
                    clave_nomina = c.FechasEstatus.ConfigBitacora.TipodeNomina.Clave,
                    tipo_nomina = c.FechasEstatus.ConfigBitacora.TipodeNomina.tipoDeNomina,
                    estatus = c.FechasEstatus.EstatusBitacora.Estatus,
                    c.fch_Creacion,
                    hora = c.fch_Creacion,
                    c.FilePath,
                    retardo = c.Retardo ? "RETARDO" : "SIN RETARDO",
                    porques = c.Porques ? "SE ENVIÓ" : "NO SE ENVIÓ"
                }).ToList();

                return Ok(bitacora);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);

            }

        }
        [HttpGet]
        [Route("getIshikawa")]
        public IHttpActionResult GetIshikawa()
        {
            try
            {
                var datos = db.FIRM_Ishikawa.Where(x => x.Activo).ToList();
                return Ok(datos);

            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
        [HttpGet]
        [Route("getPorques")]
        [Authorize]
        public IHttpActionResult GetPorques(Guid LiderId)
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
                    usuarioAlta = db.Usuarios.Where(x => x.Id.Equals(d.UsuarioAlta)).Select(u => u.Clave + " " + u.Nombre + " " + u.ApellidoPaterno + " " + u.ApellidoMaterno).FirstOrDefault(),
                    usuarioDepa = db.Usuarios.Where(x => x.Id.Equals(d.UsuarioAlta)).Select(u => u.Departamento.Nombre).FirstOrDefault()
                }).ToList();

                return Ok(result);

            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
        [HttpGet]
        [Route("getHistorial")]
        public IHttpActionResult GetHistorial()
        {
            try
            {
                var result = db.FIRM_Damfo022.OrderByDescending(o => o.fch_Creacion).Where(x => x.Activo && x.Estatus).Select(d => new
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
                    usuarioAlta = db.Usuarios.Where(x => x.Id.Equals(d.UsuarioAlta)).Select(u => u.Clave + " " + u.Nombre + " " + u.ApellidoPaterno + " " + u.ApellidoMaterno).FirstOrDefault(),
                    usuarioDepa = db.Usuarios.Where(x => x.Id.Equals(d.UsuarioAlta)).Select(u => u.Departamento.Nombre).FirstOrDefault()
                }).ToList();

                var totalIshikawa = db.FIRM_Ishikawa.Where(x => x.Activo).Select(res => new
                {
                    res.Id,
                    causa = res.Causa.Substring(0, res.Causa.IndexOf("/") >= 0 ? res.Causa.IndexOf("/") - 1 : res.Causa.Length),
                    total = db.FIRM_CausaEfecto.Where(x => x.Damfo022.Activo && x.Damfo022.Estatus && x.IshikawaId.Equals(res.Id)).Count()

                }).ToList();

                return Ok(new { result, totalIshikawa });

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
    }
}