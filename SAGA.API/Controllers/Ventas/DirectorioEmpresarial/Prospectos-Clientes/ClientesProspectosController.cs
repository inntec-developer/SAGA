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

        [Route("hacerCliente")]
        [HttpPost]
        public IHttpActionResult HacerCliente(HacerClienteDto cliente)
        {
            try
            {
                var prospecto = db.Clientes.Find(cliente.Id);
                db.Entry(prospecto).State = EntityState.Modified;
                prospecto.RazonSocial = cliente.Razonsocial;
                prospecto.RFC = cliente.RFC;
                prospecto.esCliente = true;
                prospecto.UsuarioMod = cliente.Usuario;
                prospecto.fch_Modificacion = DateTime.Now;
                db.SaveChanges();


                return Ok(HttpStatusCode.OK);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotAcceptable);
            }
        }

        [Route("getCliente")]
        [HttpGet]
        public IHttpActionResult GetCliente(Guid ClienteId)
        {
            try
            {
                var cliente = db.Clientes
                    .Where(x => x.Id.Equals(ClienteId))
                    .Select(x => new
                    {
                        Id = x.Id,
                        RazonSocial = x.RazonSocial != null ? x.RazonSocial : "",
                        RFC = x.RFC != null ? x.RFC : "",
                        NombreComercial = x.Nombrecomercial,
                        GiroEmpresa = x.GiroEmpresas,
                        ActividadEmpresa = x.ActividadEmpresas,
                        TamanoEmpresa = x.TamanoEmpresas,
                        TipoEmpresa = x.TipoEmpresas,
                        TipoBase = x.TipoBases,
                        esCliente = x.esCliente,
                        NumeroEmpleados = x.NumeroEmpleados,
                        Direcciones = x.direcciones.Select(d => new
                        {
                            Id = d.Id,
                            Activo = d.Activo,
                            Calle = d.Calle,
                            CodigoPostal = d.CodigoPostal,
                            Colonia = d.Colonia.colonia,
                            ColoniaId = d.ColoniaId,
                            esPrincipal = d.esPrincipal,
                            Estado = d.Estado.estado,
                            EstadoId = d.EstadoId,
                            Municipio = d.Municipio.municipio,
                            MunicipioId = d.MunicipioId,
                            NumeroInterior = d.NumeroInterior,
                            NumeroExterior = d.NumeroExterior,
                            Pais = d.Pais.pais,
                            PaisId = d.PaisId,
                            Referencia = d.Referencia != null ? d.Referencia : "",
                            TipoDireccion = d.TipoDireccion.tipoDireccion,
                            TipoDireccionId = d.TipoDireccionId

                        }).ToList(),
                        Telefonos = db.Telefonos
                                    .Where(t => t.EntidadId.Equals(ClienteId))
                                    .Select(t => new
                                    {
                                        Id = t.Id,
                                        IdDT = db.DireccionesTelefonos.Where(dt => dt.TelefonoId.Equals(t.Id)).Select(dt => dt.Id).FirstOrDefault(),
                                        Direccion = db.DireccionesTelefonos
                                                    .Where(dt => dt.TelefonoId.Equals(t.Id))
                                                    .Select(dt => new {Calle =  dt.Direccion.Calle + " No. " + dt.Direccion.NumeroExterior + " C.P. " + dt.Direccion.CodigoPostal })
                                                    .FirstOrDefault(),
                                        DireccionId = db.DireccionesTelefonos.Where(dt => dt.TelefonoId.Equals(t.Id)).Select(dt => dt.DireccionId).FirstOrDefault(),
                                        ClavePais = t.ClavePais,
                                        ClaveLada = t.ClaveLada,
                                        Extencion = t.Extension,
                                        Telefono = t.telefono,
                                        TipoTelefono = t.TipoTelefono,
                                        Activo = t.Activo,
                                        esPrincipal = t.esPrincipal
                                    })
                                    .ToList(),
                        Correos = db.Emails
                                    .Where(e => e.EntidadId.Equals(ClienteId))
                                    .Select(e => new {
                                       Id = e.Id,
                                       IdDE = db.DireccionesEmails.Where(de => de.EmailId.Equals(e.Id)).Select(de => de.Id).FirstOrDefault(),
                                       Direccion = db.DireccionesEmails
                                                    .Where(de => de.EmailId.Equals(e.Id))
                                                    .Select(de => new { Calle = de.Direccion.Calle + " No. " + de.Direccion.NumeroExterior + " C.P. " + de.Direccion.CodigoPostal })
                                                    .FirstOrDefault(),
                                       DireccionId = db.DireccionesEmails.Where(de => de.EmailId.Equals(e.Id)).Select(de => de.DireccionId).FirstOrDefault(),
                                       Email = e.email
                                    })
                                    .ToList() ,
                        Contactos = db.Contactos
                                    .Where(c => c.ClienteId.Equals(ClienteId))
                                    .Select(c => new
                                    {
                                        Id = c.Id,
                                        IdDE = db.DireccionesContactos.Where(dc => dc.ContactoId.Equals(c.Id)).Select(de => de.Id).FirstOrDefault(),
                                        Direccion = db.DireccionesContactos
                                                    .Where(dc => dc.ContactoId.Equals(c.Id))
                                                    .Select(dc => new { Calle = dc.Direccion.Calle + " No. " + dc.Direccion.NumeroExterior + " C.P. " + dc.Direccion.CodigoPostal })
                                                    .FirstOrDefault(),
                                        DireccionId = db.DireccionesContactos.Where(dc => dc.ContactoId.Equals(c.Id)).Select(de => de.DireccionId).FirstOrDefault(),
                                        Nombre = c.Nombre,
                                        ApellidoPaterno = c.ApellidoPaterno,
                                        ApellidoMaterno = c.ApellidoMaterno != null ? c.ApellidoMaterno : "",
                                        Puesto = c.Puesto,
                                        Telefonos = x.telefonos.Select(ct => new
                                        {
                                            Id = ct.Id,
                                            ClavePais = ct.ClavePais,
                                            ClaveLada = ct.ClaveLada,
                                            Extencion = ct.Extension,
                                            Telefono = ct.telefono,
                                            TipoTelefono = ct.TipoTelefono,
                                        }).ToList(),
                                        Emails = x.emails.Select(ce => new {
                                            Id = ce.Id,
                                            Email = ce.email,
                                        }).ToList(),
                                    })
                                    .ToList()
                    }).FirstOrDefault();
                    
                return Ok(cliente);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
    }
}
