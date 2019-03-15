using AutoMapper;
using SAGA.API.Dtos;
using SAGA.DAL;
using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;

namespace SAGA.API.Controllers.Ventas.DirectorioEmpresarial.Prospectos_Clientes
{
    [RoutePrefix("api/Directorio")]
    public class ClientesProspectosController : ApiController
    {
        
        private SAGADBContext db;
        
        public ClientesProspectosController()
        {
            db = new SAGADBContext();
        }

        [Route("addProspecto")]
        [HttpPost]
        public IHttpActionResult AddProspecto(ProspectoDto prospecto)
        {
            using (DbContextTransaction beginTran = db.Database.BeginTransaction())
            {
                try
                {
                    var cliente = Mapper.Map<ProspectoDto, Cliente>(prospecto);
                    cliente.Nombrecomercial = prospecto.Nombrecomercial;
                    cliente.GiroEmpresaId = prospecto.GiroEmpresaId;
                    cliente.ActividadEmpresaId = prospecto.ActividadEmpresaId;
                    cliente.NumeroEmpleados = prospecto.NumeroEmpleados;
                    cliente.TamanoEmpresaId = prospecto.TamanoEmpresaId;
                    cliente.TipoEmpresaId = prospecto.TipoEmpresaId;
                    cliente.TipoBaseId = prospecto.TipoBaseId;
                    cliente.Clasificacion = prospecto.Clasificacion;
                    cliente.TipoEntidadId = 6;
                    cliente.Activo = true;
                    cliente.esCliente = false;
                    cliente.otraAgencia = false;
                    cliente.direcciones = prospecto.Direcciones;
                    cliente.telefonos = prospecto.Telefonos;
                    cliente.emails = prospecto.Emails;
                    cliente.Contactos = prospecto.Contactos;
                    cliente.Nombre = string.Empty;
                    cliente.ApellidoMaterno = string.Empty;
                    cliente.ApellidoPaterno = string.Empty;
                    cliente.fch_Modificacion = DateTime.Now;
                    cliente.UsuarioAlta = prospecto.Usuario;
                    cliente.UsuarioMod = prospecto.Usuario;
                    db.Clientes.Add(cliente);
                    db.SaveChanges();
                    Guid IdCliente = db.Clientes.OrderByDescending(x => x.fch_Creacion).Take(1).Select(x => x.Id).FirstOrDefault();

                    List<Direccion> direcciones = db.Direcciones.Where(x => x.EntidadId.Equals(IdCliente)).ToList();
                    List<Telefono> telefonos = db.Telefonos.Where(x => x.EntidadId.Equals(IdCliente)).ToList();
                    List<Email> emails = db.Emails.Where(x => x.EntidadId.Equals(IdCliente)).ToList();
                    List<Contacto> contactos = db.Contactos.Where(x => x.ClienteId.Equals(IdCliente)).ToList();

                    foreach (Direccion d in direcciones)
                    {
                        //Recorrer los Telefonos
                        foreach (Telefono t in telefonos)
                        {
                            foreach (var dt in prospecto.DireccionTelefono)
                            {
                                if (d.Calle.Equals(dt.Calle)
                                    && d.NumeroExterior.Equals(dt.NumeroExterior)
                                    && d.NumeroInterior.Equals(dt.NumeroInterior)
                                    && d.CodigoPostal.Equals(dt.CodigoPostal)
                                    && t.telefono.Equals(dt.Telefono)
                                    && t.Extension.Equals(dt.Extension))
                                {
                                    DireccionTelefono de = new DireccionTelefono();
                                    de.DireccionId = d.Id;
                                    de.TelefonoId = t.Id;
                                    db.DireccionesTelefonos.Add(de);
                                    db.SaveChanges();
                                }
                            }
                        }
                        //Recorere los Emails
                        foreach (Email e in emails)
                        {
                            foreach (var em in prospecto.DireccionEmail)
                            {
                                if (d.Calle.Equals(em.Calle)
                                    && d.NumeroExterior.Equals(em.NumeroExterior)
                                    && d.NumeroInterior.Equals(em.NumeroInterior)
                                    && d.CodigoPostal.Equals(em.CodigoPostal)
                                    && e.email.Equals(em.Email))
                                {
                                    DireccionEmail de = new DireccionEmail();
                                    de.DireccionId = d.Id;
                                    de.EmailId = e.Id;
                                    db.DireccionesEmails.Add(de);
                                    db.SaveChanges();
                                }
                            }
                        }
                            
                        //Recorere los Emails
                        foreach (Contacto c in contactos)
                        {
                            foreach (var co in prospecto.DireccionContacto)
                            {
                                if (d.Calle.Equals(co.Calle)
                                    && d.NumeroExterior.Equals(co.NumeroExterior)
                                    && d.NumeroInterior.Equals(co.NumeroInterior)
                                    && d.CodigoPostal.Equals(co.CodigoPostal)
                                    && c.Nombre.Equals(co.Nombre)
                                    && c.ApellidoPaterno.Equals(co.ApellidoPaterno)
                                    && c.ApellidoMaterno.Equals(co.ApellidoMaterno)
                                    && c.Puesto.Equals(co.Puesto))
                                {
                                    DireccionContacto dc = new DireccionContacto();
                                    dc.DireccionId = d.Id;
                                    dc.ContactoId = c.Id;
                                    db.DireccionesContactos.Add(dc);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    beginTran.Commit();
                    return Ok(HttpStatusCode.Accepted);
                }
                catch (Exception ex)
                {
                    beginTran.Rollback();
                    string msg = ex.Message;
                    return Ok(HttpStatusCode.NotModified);
                }
            }
        }
    }
}
