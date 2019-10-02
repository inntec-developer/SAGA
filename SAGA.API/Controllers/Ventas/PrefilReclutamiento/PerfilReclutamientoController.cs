using SAGA.API.Dtos;
using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers.Ventas.PrefilReclutamiento
{
    [RoutePrefix("api/PerfilReclutamiento")]
    //[Authorize]
    public class PerfilReclutamientoController : ApiController
    {
        private SAGADBContext db;

        public PerfilReclutamientoController()
        {
            db = new SAGADBContext();
        }

        #region Get Informacion
        [HttpGet]
        [Route("getCliente")]
        public IHttpActionResult GetClientes(string busqueda)
        {
            try
            {
                var clientes = db.Clientes
                    .Where(c => c.RazonSocial.Contains(busqueda) || c.Nombrecomercial.Contains(busqueda) || c.RFC.Contains(busqueda))
                    .Where(c => c.Activo.Equals(true) && c.esCliente.Equals(true))
                    .Select(c => new
                    {
                        c.Id,
                        c.RazonSocial,
                        c.Nombrecomercial,
                        c.RFC,
                        c.GiroEmpresas.giroEmpresa,
                        c.ActividadEmpresas.actividadEmpresa
                    })
                    .ToList();

                return Ok(clientes);
            }
            catch (Exception ex)
            {
                string mesg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("getClienteId")]
        public IHttpActionResult GetClienteId(Guid PerfilId)
        {
            try
            {
                var clientes = db.DAMFO290
                    .Where(d => d.Id.Equals(PerfilId))
                    .Select(d => new
                    {
                        Id = d.Id,
                        ClienteId = d.ClienteId,
                        RazonSocial = d.Cliente.RazonSocial,
                        Nombrecomercial = d.Cliente.Nombrecomercial,
                        RFC = d.Cliente.RFC,
                        GiroEmpresa = d.Cliente.GiroEmpresas.giroEmpresa,
                        ActividadEmpresa = d.Cliente.ActividadEmpresas.actividadEmpresa,
                        Tipo = d.TipoReclutamientoId,
                        Clase = d.ClaseReclutamientoId,
                    })
                    .FirstOrDefault();

                return Ok(clientes);
            }
            catch (Exception ex)
            {
                string mesg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("getInfoCliente")]
        public IHttpActionResult GetInformacionCliente(Guid ClienteId)
        {
            try
            {
                var info = db.Clientes.
                    Where(c => c.Id.Equals(ClienteId))
                    .Select(c => new
                    {
                        direcciones = c.direcciones.Select(d => new
                        {
                            id = d.Id,
                            tipoDireccion = d.TipoDireccion.tipoDireccion,
                            pais = d.Pais.pais,
                            estado = d.Estado.estado,
                            municipio = d.Municipio.municipio,
                            colonia = d.Colonia.colonia,
                            calle = d.Calle,
                            numeroExterior = d.NumeroExterior,
                            numeroInterior = d.NumeroInterior,
                            codigoPostal = d.CodigoPostal,
                            activo = d.Activo,
                            esPrincipal = d.esPrincipal,
                        }).ToList(),
                        telefonos = db.Telefonos
                                    .Where(t => t.EntidadId == ClienteId)
                                    .Select(t => new {
                                        Calle = db.DireccionesTelefonos
                                            .Where(dt => dt.TelefonoId.Equals(t.Id)).FirstOrDefault() != null ?
                                            db.DireccionesTelefonos
                                            .Where(dt => dt.TelefonoId.Equals(t.Id))
                                            .Select(dt => dt.Direccion.Calle + " No. " + dt.Direccion.NumeroExterior + " C.P. " + dt.Direccion.CodigoPostal)
                                            .FirstOrDefault() : "Sin Registro",
                                        tipo = t.TipoTelefono.Tipo,
                                        clavePais = t.ClavePais,
                                        claveLada = t.ClaveLada,
                                        telefono = t.telefono,
                                        extension = t.Extension,
                                        activo = t.Activo,
                                        esPrincipal = t.esPrincipal
                                    }).ToList(),
                        contactos = db.Contactos
                                    .Where(cn => cn.ClienteId == ClienteId)
                                    .Select(cn => new
                                    {
                                        Calle = db.DireccionesContactos
                                                    .Where(dc => dc.ContactoId.Equals(cn.Id)).FirstOrDefault() != null ? db.DireccionesContactos
                                                    .Where(dc => dc.ContactoId.Equals(cn.Id))
                                                    .Select(dc => dc.Direccion.Calle + " No. " + dc.Direccion.NumeroExterior + " C.P. " + dc.Direccion.CodigoPostal)
                                                    .FirstOrDefault() : "Sin Registro",
                                        nombre = cn.Nombre,
                                        apellidoPaterno = cn.ApellidoPaterno,
                                        apellidoMaterno = cn.ApellidoMaterno,
                                        puesto = cn.Puesto,
                                        infoAdicional = cn.InfoAdicional,
                                        telefonos = db.Telefonos
                                            .Where(t => t.EntidadId == cn.Id)
                                            .Select(t => new {
                                                tipo = t.TipoTelefono.Tipo,
                                                clavePais = t.ClavePais,
                                                claveLada = t.ClaveLada,
                                                telefono = t.telefono,
                                                extension = t.Extension
                                            })
                                            .ToList(),
                                        Email = db.Emails
                                            .Where(e => e.EntidadId == cn.Id)
                                            .Select(e => new { email = e.email })
                                            .ToList(),
                                    }).ToList(),


                    }).FirstOrDefault();

                return Ok(info);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("getInfoPerfil")]
        public IHttpActionResult GetInfoPerfil(Guid PerfilId)
        {
            try
            {
                var perfil = db.DAMFO290
                .Where(x => x.Id.Equals(PerfilId))
                .Select(x => new
                {
                    x.NombrePerfil,
                    x.GeneroId,
                    x.EdadMinima,
                    x.EdadMaxima,
                    x.EstadoCivilId,
                    escolaridades = x.escolardadesPerfil
                        .Select(e => new
                        {
                            e.Id,
                            e.EscolaridadId,
                            nivelId = e.EstadoEstudioId,
                            Escolaridad = e.Escolaridad.gradoEstudio,
                            Nivel = e.EstadoEstudio.estadoEstudio
                        }).ToList(),
                    x.AreaId,
                    x.ContratoInicialId,
                    x.TiempoContratoId,
                    aptitudes = x.aptitudesPerfil
                        .Select(a => new
                        {
                            Id = a.AptitudId
                        }).ToList(),
                    x.Experiencia,
                    x.SueldoMinimo,
                    x.SueldoMaximo,
                    x.DiaCorteId,
                    x.TipoNominaId,
                    x.DiaPagoId,
                    x.PeriodoPagoId
                })
                .FirstOrDefault();
                return Ok(perfil);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("getAnexosPerfil")]
        public IHttpActionResult GetAnexosPerfil(Guid PerfilId)
        {
            try
            {
                var anexos = db.DAMFO290
                    .Where(p => p.Id.Equals(PerfilId))
                    .Select(p => new
                    {
                        Beneficios = p.beneficiosPerfil.Select(b => new
                        {
                            id = b.Id,
                            tipoBeneficio = b.TipoBeneficio.tipoBeneficio,
                            cantidad = b.Cantidad,
                            observaciones = b.Observaciones,
                            tipoBeneficioId = b.TipoBeneficioId,
                        }).ToList(),
                        Horarios = p.horariosPerfil.Select(h => new {
                            id = h.Id,
                            horario = h.Nombre,
                            deDia = h.deDia.diaSemana,
                            aDia = h.aDia.diaSemana,
                            deDiaId = h.deDiaId,
                            aDiaId = h.aDiaId,
                            deHora = h.deHora,
                            aHora = h.aHora,
                            vacantes = h.numeroVacantes,
                            especificaciones = h.Especificaciones,
                            activo = h.Activo,
                        }).ToList(),
                        Actividades = p.actividadesPerfil.Select(a => new
                        {
                            id = a.Id,
                            actividad = a.Actividades
                        }).ToList(),
                        Observaciones = p.observacionesPerfil.Select(o => new
                        {
                            id = o.Id,
                            observacion = o.Observaciones
                        }).ToList(),
                        PsicometriasD = p.psicometriasDamsa.Select(d => new
                        {
                            id = d.Id,
                            psicometriaId = d.PsicometriaId,
                            psicometria = d.Psicometria.tipoPsicometria,
                            descripcion = d.Psicometria.descripcion
                        }).ToList(),
                        PsicometriasC = p.psicometriasCliente.Select(d => new
                        {
                            id = d.Id,
                            psicometria = d.Psicometria,
                            descripcion = d.Descripcion
                        }).ToList(),
                        Documentos = p.documentosCliente.Select(dc => new
                        {
                            id = dc.Id,
                            documento = dc.Documento
                        }).ToList(),
                        Procesos = p.procesoPerfil
                        .OrderBy(pro => pro.Orden)
                        .Select(pro => new
                        {
                            id = pro.Id,
                            proceso = pro.Proceso,
                        }).ToList(),
                        Prestaciones = p.prestacionesCliente.Select(pre => new
                        {
                            id = pre.Id,
                            prestacion = pre.Prestamo
                        }).ToList(),
                        Cardinales = p.competenciasCardinalPerfil.Select(cc => new
                        {
                            Id = cc.Id,
                            Nivel = cc.Nivel,
                            Competencia = cc.Competencia.competenciaCardinal,
                            CompetenciaId = cc.CompetenciaId
                        }).ToList(),
                        Areas = p.competenciasAreaPerfil.Select(cc => new
                        {
                            Id = cc.Id,
                            Nivel = cc.Nivel,
                            Competencia = cc.Competencia.competenciaArea,
                            CompetenciaId = cc.CompetenciaId
                        }).ToList(),
                        Gerenciales = p.competetenciasGerencialPerfil.Select(cc => new
                        {
                            Id = cc.Id,
                            Nivel = cc.Nivel,
                            Competencia = cc.Competencia.competenciaGerencial,
                            CompetenciaId = cc.CompetenciaId
                        }).ToList(),
                        Arte = @"https://apisb.damsa.com.mx/utilerias/" + "img/ArteRequi/BG/" + p.Arte + ".jpg",
                    })
                    .FirstOrDefault();
                return Ok(anexos);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        #endregion

        #region Escolaridadades
        [HttpPost]
        [Route("addEscolaridad")]
        public IHttpActionResult AddEscolaridades(EscoPerfilDto esco)
        {
            try
            {
                var escolaridad = new EscolaridadesPerfil();
                escolaridad.EscolaridadId = esco.EscolaridadId;
                escolaridad.EstadoEstudioId = esco.EstadoEstudioId;
                escolaridad.DAMFO290Id = esco.Damfo290Id;
                escolaridad.UsuarioAlta = esco.Usuario;
                db.EscolaridadesPerfil.Add(escolaridad);
                db.SaveChanges();
                var escoID = db.EscolaridadesPerfil
                    .Where(e => e.DAMFO290Id.Equals(esco.Damfo290Id))
                    .OrderByDescending(e => e.fch_Creacion)
                    .Select(e => e.Id)
                    .Take(1)
                    .FirstOrDefault();
                return Ok(escoID);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("editEscolaridad")]
        public IHttpActionResult EditEscolaridades(EscoPerfilDto esco)
        {
            try
            {
                var escolaridad = db.EscolaridadesPerfil.Find(esco.Id);
                escolaridad.EscolaridadId = esco.EscolaridadId;
                escolaridad.EstadoEstudioId = esco.EstadoEstudioId;
                escolaridad.UsuarioMod = esco.Usuario;
                escolaridad.fch_Modificacion = DateTime.Now;
                db.Entry(escolaridad).State = EntityState.Modified;
                db.SaveChanges();
                return Ok(HttpStatusCode.Accepted);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("deleteEscolaridad")]
        public IHttpActionResult DeleteEscolaridades(EscoPerfilDto esco)
        {
            try
            {
                var escolaridad = new EscolaridadesPerfil();
                escolaridad.Id = esco.Id;
                db.Entry(escolaridad).State = EntityState.Deleted;
                db.SaveChanges();
                return Ok(HttpStatusCode.Accepted);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
        #endregion
        #region Beneficios
        [HttpPost]
        [Route("crudBeneficios")]
        public IHttpActionResult CrudBeneficios(BenPerfilDto Beneficio)
        {
            try
            {
                switch (Beneficio.Action)
                {
                    case "create":
                        var existes = db.BeneficiosPerfil
                            .Where(e => e.DAMFO290Id.Equals(Beneficio.DAMFO290Id) && e.TipoBeneficioId == Beneficio.TipoBeneficioId).Count();
                        if (existes == 0)
                        {
                            var x = new BeneficiosPerfil();
                            x.TipoBeneficioId = Beneficio.TipoBeneficioId;
                            x.Cantidad = Beneficio.Cantidad;
                            x.Observaciones = Beneficio.Observaciones.ToUpper();
                            x.UsuarioAlta = Beneficio.Usuario;
                            x.DAMFO290Id = Beneficio.DAMFO290Id;
                            db.BeneficiosPerfil.Add(x);
                            db.SaveChanges();
                            var BeneficioId = db.BeneficiosPerfil
                                .Where(e => e.DAMFO290Id.Equals(Beneficio.DAMFO290Id))
                                .OrderByDescending(e => e.fch_Creacion)
                                .Select(e => e.Id)
                                .Take(1)
                                .FirstOrDefault();
                            return Ok(BeneficioId);
                        }
                        else
                        {
                            return Ok(HttpStatusCode.Ambiguous);
                        }
                        
                    case "update":
                        var upexiste = db.BeneficiosPerfil
                            .Where(e => e.DAMFO290Id.Equals(Beneficio.DAMFO290Id) && e.TipoBeneficioId == Beneficio.TipoBeneficioId && e.Id != Beneficio.Id).Count();
                        if (upexiste == 0)
                        {
                            var u = db.BeneficiosPerfil.Find(Beneficio.Id);
                            db.Entry(u).State = EntityState.Modified;
                            u.TipoBeneficioId = Beneficio.TipoBeneficioId;
                            u.Cantidad = Beneficio.Cantidad;
                            u.Observaciones = Beneficio.Observaciones.ToUpper();
                            u.UsuarioMod = Beneficio.Usuario;
                            u.fch_Modificacion = DateTime.Now;
                            db.SaveChanges();
                            return Ok(HttpStatusCode.OK);
                        }
                        else
                        {
                            return Ok(HttpStatusCode.Ambiguous);
                        }

                    case "delete":
                        var d = db.BeneficiosPerfil.Find(Beneficio.Id);
                        db.Entry(d).State = EntityState.Deleted;
                        db.SaveChanges();
                        return Ok(HttpStatusCode.OK);
                    default:
                        return Ok(HttpStatusCode.NotAcceptable);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
        #endregion
        #region Horarios
        [HttpPost]
        [Route("crudHorarios")]
        public IHttpActionResult CrudHorarios(HrsPerfilDto hrs)
        {
            try
            {
                switch (hrs.Action)
                {
                    case "create":

                        var existes = db.HorariosPerfiles
                            .Where(e => e.DAMFO290Id.Equals(hrs.DAMFO290Id) && e.Nombre == hrs.Nombre).Count();
                        if (existes == 0)
                        {
                            var c = new HorarioPerfil();
                            c.Nombre = hrs.Nombre.ToUpper().Trim();
                            c.deDiaId = hrs.deDiaId;
                            c.aDiaId = hrs.aDiaId;
                            c.deHora = hrs.deHora;
                            c.aHora = hrs.aHora;
                            c.numeroVacantes = hrs.numeroVacantes;
                            c.Especificaciones = hrs.Especificaciones.ToUpper().Trim();
                            c.Activo = hrs.Activo;
                            c.DAMFO290Id = hrs.DAMFO290Id;
                            c.UsuarioAlta = hrs.Usuario;
                            db.HorariosPerfiles.Add(c);
                            db.SaveChanges();
                            var horarioId = db.HorariosPerfiles
                                .Where(e => e.DAMFO290Id.Equals(hrs.DAMFO290Id))
                                .OrderByDescending(e => e.fch_Creacion)
                                .Select(e => e.Id)
                                .Take(1)
                                .FirstOrDefault();
                            return Ok(horarioId);
                        }
                        else
                        {
                            return Ok(HttpStatusCode.Ambiguous);
                        }

                    case "update":
                        var upexiste = db.HorariosPerfiles
                            .Where(e => e.DAMFO290Id.Equals(hrs.DAMFO290Id) && e.Nombre == hrs.Nombre && e.Id != hrs.Id).Count();
                        if (upexiste == 0)
                        {
                            var u = db.HorariosPerfiles.Find(hrs.Id);
                            db.Entry(u).State = EntityState.Modified;
                            u.Nombre = hrs.Nombre.ToUpper().Trim();
                            u.deDiaId = hrs.deDiaId;
                            u.aDiaId = hrs.aDiaId;
                            u.deHora = hrs.deHora;
                            u.aHora = hrs.aHora;
                            u.numeroVacantes = hrs.numeroVacantes;
                            u.Especificaciones = hrs.Especificaciones.ToUpper().Trim();
                            u.Activo = hrs.Activo;
                            u.DAMFO290Id = hrs.DAMFO290Id;
                            u.UsuarioMod = hrs.Usuario;
                            u.fch_Modificacion = DateTime.Now;
                            db.SaveChanges();
                            return Ok(u);
                        }
                        else
                        {
                            return Ok(HttpStatusCode.Ambiguous);
                        }
                    case "delete":
                        var d = db.HorariosPerfiles.Find(hrs.Id);
                        db.Entry(d).State = EntityState.Deleted;
                        db.SaveChanges();
                        return Ok(HttpStatusCode.OK);
                    default:
                        return Ok(HttpStatusCode.NotAcceptable);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
        #endregion
        #region Actividades
        [HttpPost]
        [Route("crudActividades")]
        public IHttpActionResult CurdActividades(ActPerfilDto act)
        {
            try
            {
                switch (act.Action)
                {
                    case "create":
                        var c = new ActividadesPerfil();
                        c.Actividades = act.Actividades.ToUpper().Trim();
                        c.UsuarioAlta = act.Usuario;
                        c.DAMFO290Id = act.DAMFO290Id;
                        db.ActividadesPerfil.Add(c);
                        db.SaveChanges();
                        var actividadId = db.ActividadesPerfil
                            .Where(e => e.DAMFO290Id.Equals(act.DAMFO290Id))
                            .OrderByDescending(e => e.fch_Creacion)
                            .Select(e => e.Id)
                            .Take(1)
                            .FirstOrDefault();
                        return Ok(actividadId);
                    case "update":
                        var u = db.ActividadesPerfil.Find(act.Id);
                        db.Entry(u).State = EntityState.Modified;
                        u.Actividades = act.Actividades.ToUpper().Trim();
                        u.DAMFO290Id = act.DAMFO290Id;
                        u.UsuarioMod = act.Usuario;
                        u.fch_Modificacion = DateTime.Now;
                        db.SaveChanges();
                        return Ok(u);
                    case "delete":
                        var d = db.ActividadesPerfil.Find(act.Id);
                        db.Entry(d).State = EntityState.Deleted;
                        db.SaveChanges();
                        return Ok(HttpStatusCode.OK);
                    default:
                        return Ok(HttpStatusCode.NotAcceptable);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
        #endregion
        #region Observaciones
        [HttpPost]
        [Route("crudObservaciones")]
        public IHttpActionResult CurdObservaciones(ObsPerfilDto obs)
        {
            try
            {
                switch (obs.Action)
                {
                    case "create":
                        var c = new ObservacionesPerfil();
                        c.Observaciones = obs.Observaciones.ToUpper().Trim();
                        c.UsuarioAlta = obs.Usuario;
                        c.DAMFO290Id = obs.DAMFO290Id;
                        db.ObservacionesPerfil.Add(c);
                        db.SaveChanges();
                        var actividadId = db.ObservacionesPerfil
                            .Where(e => e.DAMFO290Id.Equals(obs.DAMFO290Id))
                            .OrderByDescending(e => e.fch_Creacion)
                            .Select(e => e.Id)
                            .Take(1)
                            .FirstOrDefault();
                        return Ok(actividadId);
                    case "update":
                        var u = db.ObservacionesPerfil.Find(obs.Id);
                        db.Entry(u).State = EntityState.Modified;
                        u.Observaciones = obs.Observaciones.ToUpper().Trim();
                        u.DAMFO290Id = obs.DAMFO290Id;
                        u.UsuarioMod = obs.Usuario;
                        u.fch_Modificacion = DateTime.Now;
                        db.SaveChanges();
                        return Ok(u);
                    case "delete":
                        var d = db.ObservacionesPerfil.Find(obs.Id);
                        db.Entry(d).State = EntityState.Deleted;
                        db.SaveChanges();
                        return Ok(HttpStatusCode.OK);
                    default:
                        return Ok(HttpStatusCode.NotAcceptable);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
        #endregion
        #region PsicometriasDamsa
        [HttpPost]
        [Route("crudPsicometriasDamsa")]
        public IHttpActionResult CrudPsicometriasDamsa(PstDamsaDto pst)
        {
            try
            {
                switch (pst.Action)
                {
                    case "create":
                        var existes = db.PsicometriasDamsa
                           .Where(p => p.DAMFO290Id.Equals(pst.DAMFO290Id) && p.PsicometriaId == pst.PsicometriaId).Count();
                        if (existes == 0)
                        {
                            var c = new PsicometriasDamsa();
                            c.PsicometriaId = pst.PsicometriaId;
                            c.DAMFO290Id = pst.DAMFO290Id;
                            c.UsuarioAlta = pst.Usuario;
                            db.PsicometriasDamsa.Add(c);
                            db.SaveChanges();
                            var psicometriaId = db.PsicometriasDamsa
                                .Where(e => e.DAMFO290Id.Equals(pst.DAMFO290Id))
                                .OrderByDescending(e => e.fch_Creacion)
                                .Select(e => e.Id)
                                .Take(1)
                                .FirstOrDefault();
                            return Ok(psicometriaId);
                        }
                        else
                        {
                            return Ok(HttpStatusCode.Ambiguous);
                        }
                    case "update":
                        var upexiste = db.PsicometriasDamsa
                            .Where(e => e.DAMFO290Id.Equals(pst.DAMFO290Id) && e.PsicometriaId == pst.PsicometriaId && e.Id != pst.Id).Count();
                        if (upexiste == 0)
                        {
                            var u = db.PsicometriasDamsa.Find(pst.Id);
                            db.Entry(u).State = EntityState.Modified;
                            u.PsicometriaId = pst.PsicometriaId;
                            u.DAMFO290Id = pst.DAMFO290Id;
                            u.UsuarioMod = pst.Usuario;
                            u.fch_Modificacion = DateTime.Now;
                            db.SaveChanges();
                            return Ok(u);
                        }
                        else
                        {
                            return Ok(HttpStatusCode.Ambiguous);
                        }
                    case "delete":
                        var d = db.PsicometriasDamsa.Find(pst.Id);
                        db.Entry(d).State = EntityState.Deleted;
                        db.SaveChanges();
                        return Ok(HttpStatusCode.OK);
                    default:
                        return Ok(HttpStatusCode.NotAcceptable);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
        #endregion
        #region PsicometriasCliente
        [HttpPost]
        [Route("crudPsicometriasCliente")]
        public IHttpActionResult CrudPsicometriasCliente(PstClienteDto pst)
        {
            try
            {
                switch (pst.Action)
                {
                    case "create":
                        var existes = db.PsicometriasCliente
                           .Where(p => p.DAMFO290Id.Equals(pst.DAMFO290Id) && p.Psicometria == pst.Psicometria).Count();
                        if (existes == 0)
                        {
                            var c = new PsicometriasCliente();
                            c.Psicometria = pst.Psicometria;
                            c.Descripcion = pst.Descripcion;
                            c.DAMFO290Id = pst.DAMFO290Id;
                            c.UsuarioAlta = pst.Usuario;
                            db.PsicometriasCliente.Add(c);
                            db.SaveChanges();
                            var psicometriaId = db.PsicometriasDamsa
                                .Where(e => e.DAMFO290Id.Equals(pst.DAMFO290Id))
                                .OrderByDescending(e => e.fch_Creacion)
                                .Select(e => e.Id)
                                .Take(1)
                                .FirstOrDefault();
                            return Ok(psicometriaId);
                        }
                        else
                        {
                            return Ok(HttpStatusCode.Ambiguous);
                        }
                    case "update":
                        var upexiste = db.PsicometriasCliente
                            .Where(e => e.DAMFO290Id.Equals(pst.DAMFO290Id) && e.Psicometria == pst.Psicometria && e.Id != pst.Id).Count();
                        if (upexiste == 0)
                        {
                            var u = db.PsicometriasCliente.Find(pst.Id);
                            db.Entry(u).State = EntityState.Modified;
                            u.Psicometria = pst.Psicometria.ToUpper().Trim();
                            u.Descripcion = pst.Descripcion.ToUpper().Trim();
                            u.DAMFO290Id = pst.DAMFO290Id;
                            u.UsuarioMod = pst.Usuario;
                            u.fch_Modificacion = DateTime.Now;
                            db.SaveChanges();
                            return Ok(u);
                        }
                        else
                        {
                            return Ok(HttpStatusCode.Ambiguous);
                        }
                    case "delete":
                        var d = db.PsicometriasCliente.Find(pst.Id);
                        db.Entry(d).State = EntityState.Deleted;
                        db.SaveChanges();
                        return Ok(HttpStatusCode.OK);
                    default:
                        return Ok(HttpStatusCode.NotAcceptable);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
        #endregion
        #region Documentos Cliente
        [HttpPost]
        [Route("crudDocumento")]
        public IHttpActionResult CrudDocumento(DocClienteDto doc)
        {
            try
            {
                switch (doc.Action)
                {
                    case "create":
                        var c = new DocumentosCliente();
                        c.Documento = doc.Documento.ToUpper().Trim();
                        c.UsuarioAlta = doc.Usuario;
                        c.DAMFO290Id = doc.DAMFO290Id;
                        db.DocumentosClientes.Add(c);
                        db.SaveChanges();
                        var documentoId = db.DocumentosClientes
                            .Where(e => e.DAMFO290Id.Equals(doc.DAMFO290Id))
                            .OrderByDescending(e => e.fch_Creacion)
                            .Select(e => e.Id)
                            .Take(1)
                            .FirstOrDefault();
                        return Ok(documentoId);
                    case "update":
                        var u = db.DocumentosClientes.Find(doc.Id);
                        db.Entry(u).State = EntityState.Modified;
                        u.Documento = doc.Documento.ToUpper().Trim();
                        u.DAMFO290Id = doc.DAMFO290Id;
                        u.UsuarioMod = doc.Usuario;
                        u.fch_Modificacion = DateTime.Now;
                        db.SaveChanges();
                        return Ok(u);
                    case "delete":
                        var d = db.DocumentosClientes.Find(doc.Id);
                        db.Entry(d).State = EntityState.Deleted;
                        db.SaveChanges();
                        return Ok(HttpStatusCode.OK);
                    default:
                        return Ok(HttpStatusCode.NotAcceptable);
                }
                
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
        #endregion
        #region Proceso
        [HttpPost]
        [Route("crudProceso")]
        public IHttpActionResult CrudProceso(ProcesoPerfilDto pro)
        {
            try
            {
                switch (pro.Action)
                {
                    case "create":
                        var cout = db.ProcesosPerfil.OrderByDescending(x => x.Orden).Where(p => p.DAMFO290Id.Equals(pro.DAMFO290Id)).Select(x => x.Orden).FirstOrDefault();
                        var c = new ProcesoPerfil();
                        c.Proceso = pro.Proceso.ToUpper().Trim();
                        c.Orden = cout + 1;
                        c.UsuarioAlta = pro.Usuario;
                        c.DAMFO290Id = pro.DAMFO290Id;
                        db.ProcesosPerfil.Add(c);
                        db.SaveChanges();
                        var procesoId = db.ProcesosPerfil
                            .Where(e => e.DAMFO290Id.Equals(pro.DAMFO290Id))
                            .OrderByDescending(e => e.fch_Creacion)
                            .Select(e => e.Id)
                            .Take(1)
                            .FirstOrDefault();
                        return Ok(procesoId);
                    case "update":
                        var u = db.ProcesosPerfil.Find(pro.Id);
                        db.Entry(u).State = EntityState.Modified;
                        u.Proceso = pro.Proceso.ToUpper().Trim();
                        u.DAMFO290Id = pro.DAMFO290Id;
                        u.UsuarioMod = pro.Usuario;
                        u.fch_Modificacion = DateTime.Now;
                        db.SaveChanges();
                        return Ok(u);
                    case "delete":
                        var d = db.ProcesosPerfil.Find(pro.Id);
                        db.Entry(d).State = EntityState.Deleted;
                        db.SaveChanges();
                        return Ok(HttpStatusCode.OK);
                    default:
                        return Ok(HttpStatusCode.NotAcceptable);
                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
        #endregion
        #region Prestaiocnes Cliente
        [HttpPost]
        [Route("crudPrestacion")]
        public IHttpActionResult CudPrestacion(PrestacionPerfilDto pre)
        {
            try
            {
                switch (pre.Action)
                {
                    case "create":
                        var c = new PrestacionesClientePerfil();
                        c.Prestamo = pre.Prestacion.ToUpper().Trim();
                        c.DAMFO290Id = pre.DAMFO290Id;
                        c.UsuarioAlta = pre.Usuario;
                        db.PrestacionesClientePerfil.Add(c);
                        db.SaveChanges();
                        var prestacionId = db.PrestacionesClientePerfil
                            .Where(e => e.DAMFO290Id.Equals(pre.DAMFO290Id))
                            .OrderByDescending(e => e.fch_Creacion)
                            .Select(e => e.Id)
                            .Take(1)
                            .FirstOrDefault();
                        return Ok(prestacionId);
                    case "update":
                        var u = db.PrestacionesClientePerfil.Find(pre.Id);
                        db.Entry(u).State = EntityState.Modified;
                        u.Prestamo = pre.Prestacion.ToUpper().Trim();
                        u.DAMFO290Id = pre.DAMFO290Id;
                        u.UsuarioMod = pre.Usuario;
                        u.fch_Modificacion = DateTime.Now;
                        db.SaveChanges();
                        return Ok(u);
                    case "delete":
                        var d = db.PrestacionesClientePerfil.Find(pre.Id);
                        db.Entry(d).State = EntityState.Deleted;
                        db.SaveChanges();
                        return Ok(HttpStatusCode.OK);
                    default:
                        return Ok(HttpStatusCode.NotAcceptable);
                }
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
        #endregion
        #region Competencias
            #region Cardinales
            [HttpPost]
            [Route("crudCompCardinal")]
            public IHttpActionResult CrudComCardinal(CompeteciasPerfilDto comp)
            {
                try
                {
                    switch (comp.Action)
                    {
                        case "create":
                            var addexist = db.CompetenciaCardinalPerfil
                               .Where(p => p.DAMFO290Id.Equals(comp.DAMFO290Id) && p.CompetenciaId == comp.CompetenciaId).Count();
                            if(addexist == 0)
                            {
                                var c = new CompetenciaCardinalPerfil();
                                c.CompetenciaId = comp.CompetenciaId;
                                c.Nivel = comp.Nivel;
                                c.DAMFO290Id = comp.DAMFO290Id;
                                c.UsuarioAlta = comp.Usuario;
                                db.CompetenciaCardinalPerfil.Add(c);
                                db.SaveChanges();
                                var cardinalId = db.CompetenciaCardinalPerfil
                                    .Where(ca => ca.DAMFO290Id.Equals(comp.DAMFO290Id))
                                    .OrderByDescending(ca => ca.fch_Creacion)
                                    .Select(ca => ca.Id)
                                    .FirstOrDefault();
                                return Ok(cardinalId);
                            }
                            else
                            {
                                return Ok(HttpStatusCode.Ambiguous);
                            }
                        
                        case "update":
                            var upexiste = db.CompetenciaCardinalPerfil
                                .Where(e => e.DAMFO290Id.Equals(comp.DAMFO290Id) && e.CompetenciaId == comp.CompetenciaId && e.Id != comp.Id).Count();
                            if (upexiste == 0)
                            {
                                var u = db.CompetenciaCardinalPerfil.Find(comp.Id);
                                db.Entry(u).State = EntityState.Modified;
                                u.CompetenciaId = comp.CompetenciaId;
                                u.Nivel = comp.Nivel;
                                u.DAMFO290Id = comp.DAMFO290Id;
                                u.UsuarioMod = comp.Usuario;
                                u.fch_Modificacion = DateTime.Now;
                                db.SaveChanges();
                                return Ok(u);
                            }
                            else
                            {
                                return Ok(HttpStatusCode.Ambiguous);
                            }
                        case "delete":
                            var d = db.CompetenciaCardinalPerfil.Find(comp.Id);
                            db.Entry(d).State = EntityState.Deleted;
                            db.SaveChanges();
                            return Ok(HttpStatusCode.OK);
                        default:
                            return Ok(HttpStatusCode.NotAcceptable);
                    }
                }
                catch(Exception ex)
                {
                    string msg = ex.Message;
                    return Ok(HttpStatusCode.NotFound);
                }
            }
            #endregion
            #region Area
            [HttpPost]
            [Route("crudCompArea")]
            public IHttpActionResult CrudComArea(CompeteciasPerfilDto comp)
            {
                try
                {
                    switch (comp.Action)
                    {
                        case "create":
                            var addexist = db.CompetenciaAreaPerfil
                               .Where(p => p.DAMFO290Id.Equals(comp.DAMFO290Id) && p.CompetenciaId == comp.CompetenciaId).Count();
                            if (addexist == 0)
                            {
                                var c = new CompetenciaAreaPerfil();
                                c.CompetenciaId = comp.CompetenciaId;
                                c.Nivel = comp.Nivel;
                                c.DAMFO290Id = comp.DAMFO290Id;
                                c.UsuarioAlta = comp.Usuario;
                            db.CompetenciaAreaPerfil.Add(c);
                                db.SaveChanges();
                                var cardinalId = db.CompetenciaAreaPerfil
                                    .Where(ca => ca.DAMFO290Id.Equals(comp.DAMFO290Id))
                                    .OrderByDescending(ca => ca.fch_Creacion)
                                    .Select(ca => ca.Id)
                                    .FirstOrDefault();
                                return Ok(cardinalId);
                            }
                            else
                            {
                                return Ok(HttpStatusCode.Ambiguous);
                            }

                        case "update":
                            var upexiste = db.CompetenciaAreaPerfil
                                .Where(e => e.DAMFO290Id.Equals(comp.DAMFO290Id) && e.CompetenciaId == comp.CompetenciaId && e.Id != comp.Id).Count();
                            if (upexiste == 0)
                            {
                                var u = db.CompetenciaAreaPerfil.Find(comp.Id);
                                db.Entry(u).State = EntityState.Modified;
                                u.CompetenciaId = comp.CompetenciaId;
                                u.Nivel = comp.Nivel;
                                u.DAMFO290Id = comp.DAMFO290Id;
                                u.UsuarioMod = comp.Usuario;
                                u.fch_Modificacion = DateTime.Now;
                                db.SaveChanges();
                                return Ok(u);
                            }
                            else
                            {
                                return Ok(HttpStatusCode.Ambiguous);
                            }
                        case "delete":
                            var d = db.CompetenciaAreaPerfil.Find(comp.Id);
                            db.Entry(d).State = EntityState.Deleted;
                            db.SaveChanges();
                            return Ok(HttpStatusCode.OK);
                        default:
                            return Ok(HttpStatusCode.NotAcceptable);
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    return Ok(HttpStatusCode.NotFound);
                }
            }
            #endregion
            #region Gerenciales
            [HttpPost]
            [Route("crudCompGerencial")]
            public IHttpActionResult CrudComGerencial(CompeteciasPerfilDto comp)
            {
                try
                {
                    switch (comp.Action)
                    {
                        case "create":
                            var addexist = db.CompetenciaGerencialPerfil
                               .Where(p => p.DAMFO290Id.Equals(comp.DAMFO290Id) && p.CompetenciaId == comp.CompetenciaId).Count();
                            if (addexist == 0)
                            {
                                var c = new CompetenciaGerencialPerfil();
                                c.CompetenciaId = comp.CompetenciaId;
                                c.Nivel = comp.Nivel;
                                c.DAMFO290Id = comp.DAMFO290Id;
                                c.UsuarioAlta = comp.Usuario;
                                db.CompetenciaGerencialPerfil.Add(c);
                                db.SaveChanges();
                                var cardinalId = db.CompetenciaGerencialPerfil
                                    .Where(ca => ca.DAMFO290Id.Equals(comp.DAMFO290Id))
                                    .OrderByDescending(ca => ca.fch_Creacion)
                                    .Select(ca => ca.Id)
                                    .FirstOrDefault();
                                return Ok(cardinalId);
                            }
                            else
                            {
                                return Ok(HttpStatusCode.Ambiguous);
                            }

                        case "update":
                            var upexiste = db.CompetenciaGerencialPerfil
                                .Where(e => e.DAMFO290Id.Equals(comp.DAMFO290Id) && e.CompetenciaId == comp.CompetenciaId && e.Id != comp.Id).Count();
                            if (upexiste == 0)
                            {
                                var u = db.CompetenciaGerencialPerfil.Find(comp.Id);
                                db.Entry(u).State = EntityState.Modified;
                                u.CompetenciaId = comp.CompetenciaId;
                                u.Nivel = comp.Nivel;
                                u.DAMFO290Id = comp.DAMFO290Id;
                                u.UsuarioMod = comp.Usuario;
                                u.fch_Modificacion = DateTime.Now;
                                db.SaveChanges();
                                return Ok(u);
                            }
                            else
                            {
                                return Ok(HttpStatusCode.Ambiguous);
                            }
                        case "delete":
                            var d = db.CompetenciaGerencialPerfil.Find(comp.Id);
                            db.Entry(d).State = EntityState.Deleted;
                            db.SaveChanges();
                            return Ok(HttpStatusCode.OK);
                        default:
                            return Ok(HttpStatusCode.NotAcceptable);
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    return Ok(HttpStatusCode.NotFound);
                }
            }
        #endregion
        #endregion

        #region Crear, Editar, Eliminar, Clonar Perfil
        [HttpPost]
        [Route("crudPerfilReclutamiento")]
        public IHttpActionResult CurdPerfilReclutamiento (PerfilReclutmientoDto pf)
        {
            try
            {
                switch (pf.Action)
                {
                    case "create":
                        var df = new DAMFO_290();
                        df.ClienteId = pf.Headers.ClienteId;
                        df.TipoReclutamientoId = pf.Headers.TipoReclutamientoId;
                        df.ClaseReclutamientoId = pf.Headers.ClaseReclutamientoId;
                        df.NombrePerfil = pf.Headers.NombrePerfil;
                        df.GeneroId = pf.Headers.GeneroId;
                        df.EdadMinima = pf.Headers.EdadMinima;
                        df.EdadMaxima = pf.Headers.EdadMaxima;
                        df.EstadoCivilId = pf.Headers.EstadoCivilId;
                        df.AreaId = pf.Headers.AreaId;
                        df.Experiencia = pf.Headers.Experiencia;
                        df.SueldoMinimo = pf.Headers.SueldoMinimo;
                        df.SueldoMaximo = pf.Headers.SueldoMaximo;
                        df.DiaCorteId = pf.Headers.DiaCorteId;
                        df.TipoNominaId = pf.Headers.TipoNominaId;
                        df.DiaPagoId = pf.Headers.DiaPagoId;
                        df.PeriodoPagoId = pf.Headers.PeriodoPagoId;
                        df.Especifique = pf.Headers.Especifique != null ? pf.Headers.Especifique : "";
                        df.ContratoInicialId = pf.Headers.ContratoInicialId;
                        df.TiempoContratoId = pf.Headers.TiempoContratoId != 0 ? pf.Headers.TiempoContratoId : null;
                        df.Activo = true;
                        df.UsuarioAlta = pf.Headers.Usuario;
                        df.escolardadesPerfil = pf.Collections.escolardadesPerfil;
                        df.aptitudesPerfil = pf.Collections.aptitudesPerfil;
                        df.horariosPerfil = pf.Collections.horariosPerfil;
                        df.actividadesPerfil = pf.Collections.actividadesPerfil;
                        df.observacionesPerfil = pf.Collections.observacionesPerfil;
                        df.psicometriasDamsa = pf.Collections.psicometriasDamsa;
                        df.psicometriasCliente = pf.Collections.psicometriasCliente;
                        df.beneficiosPerfil = pf.Collections.beneficiosPerfil;
                        df.documentosCliente = pf.Collections.documentosCliente;
                        df.procesoPerfil = pf.Collections.procesoPerfil;
                        df.prestacionesCliente = pf.Collections.prestacionesCliente;
                        df.competenciasAreaPerfil = pf.Collections.competenciasAreaPerfil;
                        df.competenciasCardinalPerfil = pf.Collections.competenciasCardinalPerfil;
                        df.competetenciasGerencialPerfil = pf.Collections.competetenciasGerencialPerfil;
                        df.FlexibilidadHorario = false;
                        df.JornadaLaboralId = 0;
                        df.TipoModalidadId = 0;
                        df.Arte = pf.Headers.Arte;
                        db.DAMFO290.Add(df);
                        db.SaveChanges();
                        var PerfilId = db.DAMFO290
                            .Where(d => d.UsuarioAlta.Equals(pf.Headers.Usuario))
                            .OrderByDescending(d => d.fch_Creacion)
                            .Select(d => d.Id)
                            .Take(1)
                            .FirstOrDefault();
                            
                        return Ok(PerfilId);
                    case "update":
                        var apt = db.AptitudesPerfil.Where(x => x.DAMFO290Id == pf.Headers.Id);
                        db.AptitudesPerfil.RemoveRange(apt);
                        db.AptitudesPerfil.AddRange(pf.Collections.aptitudesPerfil);

                        var up = db.DAMFO290.Find(pf.Headers.Id);
                        db.Entry(up).State = EntityState.Modified;
                        up.TipoReclutamientoId = pf.Headers.TipoReclutamientoId;
                        up.ClaseReclutamientoId = pf.Headers.ClaseReclutamientoId;
                        up.NombrePerfil = pf.Headers.NombrePerfil;
                        up.GeneroId = pf.Headers.GeneroId;
                        up.EdadMinima = pf.Headers.EdadMinima;
                        up.EdadMaxima = pf.Headers.EdadMaxima;
                        up.EstadoCivilId = pf.Headers.EstadoCivilId;
                        up.AreaId = pf.Headers.AreaId;
                        up.Experiencia = pf.Headers.Experiencia;
                        up.SueldoMinimo = pf.Headers.SueldoMinimo;
                        up.SueldoMaximo = pf.Headers.SueldoMaximo;
                        up.DiaCorteId = pf.Headers.DiaCorteId;
                        up.TipoNominaId = pf.Headers.TipoNominaId;
                        up.DiaPagoId = pf.Headers.DiaPagoId;
                        up.PeriodoPagoId = pf.Headers.PeriodoPagoId;
                        up.Especifique = pf.Headers.Especifique != null ? pf.Headers.Especifique : "";
                        up.ContratoInicialId = pf.Headers.ContratoInicialId;
                        up.TiempoContratoId = pf.Headers.TiempoContratoId;
                        up.UsuarioMod = pf.Headers.Usuario;
                        up.fch_Modificacion = DateTime.Now;
                        up.Arte = pf.Headers.Arte;

                        db.SaveChanges();
                        return Ok();
                    case "delete":
                        var dl = db.DAMFO290.Find(pf.Headers.Id);
                        db.Entry(dl).State = EntityState.Modified;
                        dl.Activo = false;
                        dl.UsuarioMod = pf.Headers.Usuario;
                        dl.fch_Modificacion = DateTime.Now;
                        db.Entry(dl).Property(x => x.Activo).IsModified = true;
                        db.Entry(dl).Property(x => x.fch_Modificacion).IsModified = true;
                        db.Entry(dl).Property(x => x.UsuarioMod).IsModified = true;
                        db.SaveChanges();
                        return Ok();
                    case "clone":
                        object[] _params = {
                            new SqlParameter("@Id",pf.Headers.Id),
                            new SqlParameter("@IdNWR", Guid.NewGuid())
                        };

                        var returnId = db.Database.SqlQuery<DAMFO_290>("exec CloneDamfo290 @Id, @IdNWR", _params).SingleOrDefault();

                        return Ok(returnId.Id);
                    default:
                        return Ok(HttpStatusCode.NotAcceptable);
                }
                

            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
        #endregion
    }
}