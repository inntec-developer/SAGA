using System;
using System.Collections.Generic;
using System.Linq;
using SAGA.DAL;
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
    public class ConfigBitacoraController : ApiController
    {
        private SAGADBContext db;
        public Guid auxID = new Guid("00000000-0000-0000-0000-000000000000");
        public ConfigBitacoraController()
        {
            db = new SAGADBContext();
        }
        [HttpPost]
        [Route("addBitacora")]
        [Authorize]
        public IHttpActionResult AddBitacora(BitacoraDto datos)
        {
            try
            {
                if ( datos.Configuracion.Count() > 0)
                {
                    foreach (var r in datos.Configuracion)
                    {
                        var emp = db.FIRM_ConfigBitacora.
                            Where(x => x.EmpresasId.Equals(r.Empresa.EmpresasId)
                            && x.TipodeNominaId.Equals(r.Empresa.TipodeNominaId)
                            && x.SoportesNominaId.Equals(r.Empresa.SoportesNominaId)).Select(e => e.Id).FirstOrDefault();
                        if (emp != 0)
                        {
                            var fe = db.FIRM_FechasEstatus.Where(x => x.ConfigBitacoraId.Equals(emp));
                            db.FIRM_FechasEstatus.RemoveRange(fe);

                            var eemp = db.FIRM_ConfigBitacora.Find(emp);
                            db.FIRM_ConfigBitacora.Remove(eemp);

                            db.SaveChanges();

                        }
                        FIRM_ConfigBitacora bit = new FIRM_ConfigBitacora();

                        bit = r.Empresa;
                        bit.fch_Creacion = DateTime.Now;
                        bit.fch_Modificacion = DateTime.Now;

                        db.FIRM_ConfigBitacora.Add(bit);
                        db.SaveChanges();

                        var apt = db.FIRM_FechasEstatus.Where(x => x.ConfigBitacoraId.Equals(bit.Id));
                        db.FIRM_FechasEstatus.RemoveRange(apt);

                        foreach (FIRM_FechasEstatus fe in r.FechasEstatus)
                        {
                            fe.ConfigBitacoraId = bit.Id;
                        }

                        db.FIRM_FechasEstatus.AddRange(r.FechasEstatus);
                        db.SaveChanges();

                        var ee = db.FIRM_EstatusEmails.Where(x => x.ConfigBitacoraId.Equals(bit.Id));
                        db.FIRM_EstatusEmails.RemoveRange(ee);

                        foreach(FIRM_EstatusEmails esem in r.EstatusEmails)
                        {
                            esem.ConfigBitacoraId = bit.Id;
                        }

                        db.FIRM_EstatusEmails.AddRange(r.EstatusEmails);
                        db.SaveChanges();
                    }
                    return Ok(HttpStatusCode.OK);

                }
                else if (datos.Bitacora != null)
                {
                    FIRM_ConfigBitacora bit = new FIRM_ConfigBitacora();

                    bit = datos.ConfigBitacora;
                    bit.fch_Creacion = DateTime.Now;
                    bit.fch_Modificacion = DateTime.Now;

                    db.FIRM_ConfigBitacora.Add(bit);
                    db.SaveChanges();

                    foreach (FIRM_FechasEstatus fe in datos.FechasEstatus)
                    {
                        fe.ConfigBitacoraId = bit.Id;
                    }

                    db.FIRM_FechasEstatus.AddRange(datos.FechasEstatus);
                    db.SaveChanges();

                    foreach (FIRM_EstatusEmails esem in datos.EstatusEmails)
                    {
                        esem.ConfigBitacoraId = bit.Id;
                    }

                    db.FIRM_EstatusEmails.AddRange(datos.EstatusEmails);
                    db.SaveChanges();

                    return Ok(HttpStatusCode.OK);
                }

                return Ok(HttpStatusCode.Continue);
            }
            catch(Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }

        }
       
        [HttpGet]
        [Route("getEstatusBitacora")]
        [Authorize]
        public IHttpActionResult GetEstatusBitacora()
        {
            try
            {
                var estatus = db.FIRM_EstatusBitacora.Where(x => x.Activo).Select(e => new
                {
                    e.Id,
                    e.Estatus
                });

                return Ok(estatus);

            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }

        }
        [HttpGet]
        [Route("getEmpresas")]
        [Authorize]
        public IHttpActionResult GetEmpresas()
        {
            try
            {
                var clientes = db.Empresas.Where(x => x.Activo).Select(c => new
                {
                    id = c.Id,
                    clave = c.Clave,
                    sucursal = c.Observaciones.Equals("SIN REGISTRO") ? ""  : c.Observaciones,
                    empresa = c.Nombre,
                    claveemp = c.Clave.Equals("S/R") ? "" : c.Clave,
                    registro_pat = db.FIRM_RPEmpresas.Where(x => x.EmpresasId.Equals(c.Id)).Select(rp => rp.FIRM_RP.RP_Clave + " " + rp.FIRM_RP.RP_Base).FirstOrDefault()
                }).ToList();
                return Ok(clientes);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);

            }

        }
        [HttpGet]
        [Route("getSoporteNominas")]
        [Authorize]
        public IHttpActionResult GetSoporteNominas()
        {
            try
            {
                var soportes = db.FIRM_SoportesNomina.Where(x => x.Activo).Select(c => new
                {
                    c.Id,
                    c.Soporte
                }).ToList();
                return Ok(soportes);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);

            }

        }
        [HttpGet]
        [Route("getTiposNomina")]
        [Authorize]
        public IHttpActionResult GetTiposNomina()
        {
            try
            {
                var tipos = db.TiposNominas.Where(x => x.Tipo.Equals(2)).Select(c => new
                {
                    c.Id,
                    c.Clave,
                    c.tipoDeNomina
                }).ToList();
                return Ok(tipos);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);

            }

        }
        [HttpGet]
        [Route("getUsuarios")]
        [Authorize]
        public IHttpActionResult GetUsuarios()
        {
            try
            {
                var users = db.Usuarios.Where(x => x.Activo && x.Departamento.Nombre.ToLower().Equals("nominas"))
                    .Select(c => new
                {
                    c.Id,
                    c.Clave,
                    nombreCompleto = c.Nombre + " " + c.ApellidoPaterno + " " + c.ApellidoMaterno,
                    email = db.Emails.Where(x => x.EntidadId.Equals(c.Id)).Select(e => e.email).FirstOrDefault()
                }).ToList();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
    }
}
