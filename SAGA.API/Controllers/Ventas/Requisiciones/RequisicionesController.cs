using SAGA.DAL;
using SAGA.BOL;
using SAGA.API.Dtos;
using SAGA.API.Utilerias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Net.Mail;
using System.Configuration;
using static SAGA.API.Controllers.DesignerVacanteController;
using System.IO;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/Requisiciones")]
    public class RequisicionesController : ApiController
    {
        private SAGADBContext db;
        private Damfo290Dto DamfoDto;
        private BusinessDay businessDay;
        private Rastreabilidad rastreabilidad;
        private SendEmails SendEmail;
        private DesignerVacanteController Dvc;

        public RequisicionesController()
        {
            db = new SAGADBContext();
            DamfoDto = new Damfo290Dto();
            businessDay = new BusinessDay();
            rastreabilidad = new Rastreabilidad();
            SendEmail = new SendEmails();
            Dvc = new DesignerVacanteController();
        }

        [HttpGet]
        [Route("getAddress")]
        [Authorize]
        public IHttpActionResult GetAddress(Guid Id)
        {
            try
            {
                DamfoDto.Damfo290Address = (from damfo in db.DAMFO290
                                            join cliente in db.Clientes on damfo.ClienteId equals cliente.Id
                                            join persona in db.Entidad on cliente.Id equals persona.Id
                                            join direccion in db.Direcciones on persona.Id equals direccion.EntidadId
                                            join tipoDireccion in db.TiposDirecciones on direccion.TipoDireccionId equals tipoDireccion.Id
                                            join pais in db.Paises on direccion.PaisId equals pais.Id
                                            join estado in db.Estados on direccion.EstadoId equals estado.Id
                                            join municipio in db.Municipios on direccion.MunicipioId equals municipio.Id
                                            join colonia in db.Colonias on direccion.ColoniaId equals colonia.Id
                                            where damfo.Id == Id && direccion.Activo == true
                                            select new Damfo290AddressDto
                                            {
                                                Id = direccion.Id,
                                                TipoDireccion = tipoDireccion.tipoDireccion,
                                                Pais = pais.pais,
                                                Estado = estado.estado,
                                                Municipio = municipio.municipio,
                                                Colonia = colonia.colonia,
                                                Calle = direccion.Calle + " " + direccion.NumeroExterior + " C.P: " + direccion.CodigoPostal + " Col: " + colonia.colonia,
                                                CodigoPostal = direccion.CodigoPostal,
                                                NumeroExterior = direccion.NumeroExterior,
                                                NumeroInterior = direccion.NumeroInterior
                                            }).ToList();

                return Ok(DamfoDto.Damfo290Address);

            }
            catch (Exception ex)
            {
                string messg = ex.Message;
                return Ok(messg);
            }
        }

        [HttpGet]
        [Route("getRequisicionPDF")]
        [Authorize]
        public IHttpActionResult GetRequisicionPDF(Guid RequisicionId)
        {
            TimeSpan stop;
            TimeSpan start = new TimeSpan(DateTime.Now.Second);
            try
            {
                var requisicion = db.Requisiciones.Where(x => x.Id.Equals(RequisicionId)).Select(r => new
                {
                    Id = r.Id,
                    Arte = "~/utilerias/img/ArteRequi/BG/" + r.DAMFO290.Arte,
                    vBtra = r.VBtra,
                    folio = r.Folio,
                    cumplimiento = r.fch_Cumplimiento,
                    creacion = r.fch_Creacion,
                    limite = r.fch_Limite,
                    prioridad = r.Prioridad,
                    confidenciañ = r.Confidencial,
                    estatus = r.Estatus.Descripcion,
                    solicitante = db.Entidad.Where(x => x.Id.Equals(r.PropietarioId)).Select(S => S.Nombre + " " + S.ApellidoPaterno + " " + S.ApellidoMaterno).FirstOrDefault(),
                    coordinador = db.Entidad.Where(x => x.Id.Equals(r.AprobadorId)).Select(S => S.Nombre + " " + S.ApellidoPaterno + " " + S.ApellidoMaterno).FirstOrDefault(),
                    asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(r.Id)).Select(x => x.GrpUsrId).ToList(),
                    asignadosN = r.AsignacionRequi.Where(x => x.RequisicionId.Equals(r.Id) && !x.GrpUsrId.Equals(r.AprobadorId)).Select(x => new
                    {
                        x.GrpUsr.Nombre,
                        x.GrpUsr.ApellidoMaterno,
                        x.GrpUsr.ApellidoPaterno
                    }),
                    vacantes = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                    horarios = r.horariosRequi.Select(h => new
                    {
                        nombre = h.Nombre,
                        deDia = h.deDia,
                        aDia = h.aDia,
                        deHora = h.deHora,
                        aHora = h.aHora,
                        numeroVacantes = h.numeroVacantes,
                        especificaciones = h.Especificaciones,
                        activo = h.Activo
                    }).ToList(),
                    clienteId = r.ClienteId,
                    cliente = new
                    {
                        nombrecomercial = r.Cliente.Nombrecomercial,
                        razonSocial = r.Cliente.RazonSocial,
                        rfc = r.Cliente.RFC,
                        giroEmpresa = r.Cliente.GiroEmpresas.giroEmpresa,
                        actividadEmpresa = r.Cliente.ActividadEmpresas.actividadEmpresa,
                        telefonos = db.Telefonos
                              .Where(t => t.EntidadId == r.ClienteId)
                              .Select(t => new
                              {
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
                              .Where(c => c.ClienteId == r.ClienteId)
                              .Select(c => new
                              {
                                  Calle = db.DireccionesContactos
                                                  .Where(dc => dc.ContactoId.Equals(c.Id)).FirstOrDefault() != null ? db.DireccionesContactos
                                                  .Where(dc => dc.ContactoId.Equals(c.Id))
                                                  .Select(dc => dc.Direccion.Calle + " No. " + dc.Direccion.NumeroExterior + " C.P. " + dc.Direccion.CodigoPostal)
                                                  .FirstOrDefault() : "Sin Registro",
                                  nombre = c.Nombre,
                                  apellidoPaterno = c.ApellidoPaterno,
                                  apellidoMaterno = c.ApellidoMaterno,
                                  puesto = c.Puesto,
                                  telefonos = db.Telefonos
                                      .Where(t => t.EntidadId == c.Id)
                                      .Select(t => new
                                      {
                                          tipo = t.TipoTelefono.Tipo,
                                          clavePais = t.ClavePais,
                                          claveLada = t.ClaveLada,
                                          telefono = t.telefono,
                                          extension = t.Extension
                                      }).ToList(),
                                  Email = db.Emails
                                      .Where(e => e.EntidadId == c.Id)
                                      .Select(e => new
                                      {
                                          email = e.email
                                      }).ToList(),
                              }).ToList(),

                    },
                    tipoReclutamiento = r.TipoReclutamiento.tipoReclutamiento,
                    claseReclutamiento = r.ClaseReclutamiento.clasesReclutamiento,
                    tipoContrato = r.ContratoInicial.tipoContrato,
                    periodoPrueba = r.ContratoInicial.periodoPrueba,
                    tiempo = r.TiempoContrato.Tiempo != null ? r.TiempoContrato.Tiempo : "",
                    areaExperiencia = r.Area.areaExperiencia,
                    genero = r.Genero.genero,
                    edadMinima = r.EdadMinima,
                    edadMaxima = r.EdadMaxima,
                    estadoCivil = r.EstadoCivil.estadoCivil,
                    sueldoMinimo = r.SueldoMinimo,
                    sueldoMaximo = r.SueldoMaximo,
                    escolaridades = r.escolaridadesRequi.Select(es => new
                    {
                        gradoEstudio = es.Escolaridad.gradoEstudio,
                        estadoEstudio = es.EstadoEstudio.estadoEstudio
                    }).ToList(),
                    aptitudes = r.aptitudesRequi.Select(a => new
                    {
                        aptitud = a.Aptitud.aptitud
                    }).ToList(),
                    experiencia = r.Experiencia,
                    diaCorte = r.DiaCorte.diaSemana,
                    tipoDeNomina = r.TipoNomina.tipoDeNomina,
                    diaPago = r.DiaPago.diaSemana,
                    periodoPago = r.PeriodoPago.periodoPago,
                    especifique = r.Especifique,
                    direccion = r.Direccion.Calle + ", " +
                                  r.Direccion.NumeroExterior + ", " +
                                  r.Direccion.Colonia.colonia + ", " +
                                  r.Direccion.Municipio.municipio + ", " +
                                  r.Direccion.Estado.estado + ", " +
                                  r.Direccion.Pais.pais,
                    rutasCamion = db.RutasPerfil
                          .Where(x => x.DireccionId.Equals(r.DireccionId))
                          .Select(x => new
                          {
                              Direccion = x.Direccion.Calle,
                              Ruta = x.Ruta,
                              Via = string.IsNullOrEmpty(x.Via) ? "sin registro" : x.Via
                          }).ToList(),
                    beneficios = r.beneficiosRequi.Select(bn => new
                    {
                        tipoBeneficio = bn.TipoBeneficio.tipoBeneficio,
                        cantidad = bn.Cantidad,
                        observaciones = bn.Observaciones,
                    }).ToList(),
                    actividades = r.actividadesRequi.Select(ac => new
                    {
                        actividades = ac.Actividades
                    }).ToList(),
                    observaciones = r.observacionesRequi.Select(ob => new
                    {
                        observaciones = ob.Observaciones
                    }).ToList(),
                    procesos = r.procesoRequi.Select(pr => new
                    {
                        proceso = pr.Proceso
                    }).ToList(),
                    documentosCliente = r.documentosClienteRequi.Select(dcr => new
                    {
                        documento = dcr.Documento
                    }).ToList(),
                    prestacionesCliente = r.prestacionesClienteRequi.Select(pcr => new
                    {
                        prestamo = pcr.Prestamo
                    }).ToList(),
                    psicometriasDamsa = r.psicometriasDamsaRequi.Select(pd => new
                    {
                        tipoPsicometria = pd.Psicometria.tipoPsicometria,
                        descripcion = pd.Psicometria.descripcion
                    }).ToList(),
                    psicometriasCliente = r.psicometriasClienteRequi.Select(pc => new
                    {
                        psicometria = pc.Psicometria,
                        descripcion = pc.Descripcion
                    }).ToList(),
                    competenciasCardinal = r.competenciasCardinalRequi.Select(cc => new
                    {
                        competencia = cc.Competencia.competenciaCardinal,
                        nivel = cc.Nivel
                    }).ToList(),
                    competenciasArea = r.competenciasAreaRequi.Select(ca => new
                    {
                        competencia = ca.Competencia.competenciaArea,
                        nivel = ca.Nivel
                    }).ToList(),
                    competenciasGerencial = r.competetenciasGerencialRequi.Select(cg => new
                    {
                        competencia = cg.Competencia.competenciaGerencial,
                        nivel = cg.Nivel
                    }).ToList()
                }).FirstOrDefault();

                //var mocos = requisicion.Select(rr => new
                //{
                //    Id = rr.Id,
                //    horarios = rr.horarios,
                //    clienteId = rr.clienteId,
                //    cliente = rr.cliente,
                //    tipoReclutamiento = rr.tipoReclutamiento,
                //    claseReclutamiento = rr.claseReclutamiento,
                //    tipoContrato = rr.tipoContrato,
                //    periodoPrueba = rr.periodoPrueba,
                //    tiempo = rr.tiempo,
                //    areaExperiencia = rr.areaExperiencia,
                //    genero = rr.genero,
                //    edadMinima = rr.edadMinima,
                //    edadMaxima = rr.edadMaxima,
                //    estadoCivil = rr.estadoCivil,
                //    sueldoMinimo = rr.sueldoMinimo,
                //    sueldoMaximo = rr.sueldoMaximo,
                //    escolaridades = rr.escolaridades,
                //    aptitudes = rr.aptitudes,
                //    experiencia = rr.experiencia,
                //    diaCorte = rr.diaCorte,
                //    tipoDeNomina = rr.tipoDeNomina,
                //    diaPago = rr.diaPago,
                //    periodoPago = rr.periodoPago,
                //    especifique = rr.especifique,
                //    beneficios = rr.beneficios,
                //    actividades = rr.actividades,
                //    observaciones = rr.observaciones,
                //    procesos = rr.procesos,
                //    documentosCliente = rr.documentosCliente,
                //    prestacionesCliente = rr.prestacionesCliente,
                //    psicometriasDamsa = rr.psicometriasDamsa,
                //    psicometriasCliente = rr.psicometriasCliente,
                //    competenciasCardinal = rr.competenciasCardinal,
                //    competenciasArea = rr.competenciasArea,
                //    competenciasGerencial = rr.competenciasGerencial,
                //    Arte = this.GetImage(rr.Arte),
                //    bg = rr.Arte,
                //    estatus = rr.estatus,
                //    prioridad = rr.prioridad,
                //    creacion = rr.creacion,
                //    cumplimiento = rr.cumplimiento,
                //    limite = rr.limite,
                //    coordinadore = rr.coordinador,
                //    solicitante = rr.solicitante,
                //    asignadosN = rr.asignadosN,
                //    rutasCamion = rr.rutasCamion,
                //    direccion = rr.direccion,
                //    vBtra = rr.vBtra,


                //}).FirstOrDefault();

                stop = new TimeSpan(DateTime.Now.Second);
                Console.WriteLine(stop.Subtract(start).Seconds);
                return Ok(requisicion);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        public string GetImage(string ruta)
        {
            string fullPath;

            try
            {
                fullPath = System.Web.Hosting.HostingEnvironment.MapPath(ruta);
                var type = Path.GetExtension(ruta);
                var fileName = Path.GetFileName(ruta);

                FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                byte[] bimage = new byte[fs.Length];
                fs.Read(bimage, 0, Convert.ToInt32(fs.Length));
                fs.Close();

                string img = "data:image/" + type.Substring(1, type.Length - 1) + ";base64," + Convert.ToBase64String(bimage);
                return img;
            }
            catch (Exception ex)
            {
                return "";
            }

        }

        [HttpGet]
        [Route("getById")]
        [Authorize]
        public IHttpActionResult GetRequisicion(Guid Id)
        {
            TimeSpan stop;
            TimeSpan start = new TimeSpan(DateTime.Now.Ticks);
            try
            {
                if (Id != null)
                {
                    var requisicion = db.Requisiciones.Where(x => x.Id.Equals(Id))
                        .Select(r => new
                        {
                            Id = r.Id,
                            vBtra = r.VBtra,
                            estatusId = r.EstatusId,
                            horarios = r.horariosRequi.Select(h => new
                            {
                                Id = h.Id,
                                nombre = h.Nombre,
                                deDia = h.deDia,
                                aDia = h.aDia,
                                deHora = h.deHora,
                                aHora = h.aHora,
                                numeroVacantes = h.numeroVacantes,
                                especificaciones = h.Especificaciones,
                                activo = h.Activo
                            }).ToList(),
                            cliente = new
                            {
                                nombrecomercial = r.Cliente.Nombrecomercial,
                                razonSocial = r.Cliente.RazonSocial,
                                rfc = r.Cliente.RFC,
                                giroEmpresa = r.Cliente.GiroEmpresas.giroEmpresa,
                                actividadEmpresa = r.Cliente.ActividadEmpresas.actividadEmpresa,
                                telefonos = db.Telefonos
                                    .Where(t => t.EntidadId == r.ClienteId)
                                    .Select(t => new
                                    {
                                        tipo = t.TipoTelefono.Tipo,
                                        clavePais = t.ClavePais,
                                        claveLada = t.ClaveLada,
                                        telefono = t.telefono,
                                        extension = t.Extension,
                                        activo = t.Activo,
                                        esPrincipal = t.esPrincipal
                                    }).ToList(),
                                contactos = db.Contactos
                                    .Where(c => c.ClienteId == r.ClienteId)
                                    .Select(c => new
                                    {
                                        nombre = c.Nombre,
                                        apellidoPaterno = c.ApellidoPaterno,
                                        apellidoMaterno = c.ApellidoMaterno,
                                        puesto = c.Puesto,
                                        telefonos = db.Telefonos
                                            .Where(t => t.EntidadId == c.Id)
                                            .Select(t => new
                                            {
                                                tipo = t.TipoTelefono.Tipo,
                                                clavePais = t.ClavePais,
                                                claveLada = t.ClaveLada,
                                                telefono = t.telefono,
                                                extension = t.Extension
                                            }).ToList(),
                                        Email = db.Emails
                                            .Where(e => e.EntidadId == c.Id)
                                            .Select(e => new
                                            {
                                                email = e.email
                                            }).ToList(),
                                    }).ToList(),

                            },
                            tipoReclutamiento = r.TipoReclutamiento.tipoReclutamiento,
                            claseReclutamiento = r.ClaseReclutamiento.clasesReclutamiento,
                            tipoContrato = r.ContratoInicial.tipoContrato,
                            periodoPrueba = r.ContratoInicial.periodoPrueba,
                            tiempo = String.IsNullOrEmpty(r.TiempoContrato.Tiempo) ? "sin registro" : r.TiempoContrato.Tiempo,
                            areaExperiencia = r.Area.areaExperiencia,
                            genero = r.Genero.genero,
                            edadMinima = r.EdadMinima,
                            edadMaxima = r.EdadMaxima,
                            estadoCivil = r.EstadoCivil.estadoCivil,
                            sueldoMinimo = r.SueldoMinimo,
                            sueldoMaximo = r.SueldoMaximo,
                            escolaridades = r.escolaridadesRequi.Select(es => new
                            {
                                gradoEstudio = es.Escolaridad.gradoEstudio,
                                estadoEstudio = es.EstadoEstudio.estadoEstudio
                            }).ToList(),
                            aptitudes = r.aptitudesRequi.Select(a => new
                            {
                                aptitud = a.Aptitud.aptitud
                            }).ToList(),
                            experiencia = r.Experiencia,
                            diaCorte = r.DiaCorte.diaSemana,
                            tipoDeNomina = r.TipoNomina.tipoDeNomina,
                            diaPago = r.DiaPago.diaSemana,
                            periodoPago = r.PeriodoPago.periodoPago,
                            periodoPagoId = r.PeriodoPagoId,
                            especifique = r.Especifique,
                            direccionId = r.DireccionId,
                            direccion = r.Direccion.Calle + ", " +
                                        r.Direccion.NumeroExterior + ", " +
                                        r.Direccion.Colonia.colonia + ", " +
                                        r.Direccion.Municipio.municipio + ", " +
                                        r.Direccion.Estado.estado + ", " +
                                        r.Direccion.Pais.pais,
                            beneficios = r.beneficiosRequi.Select(bn => new
                            {
                                tipoBeneficio = bn.TipoBeneficio.tipoBeneficio,
                                cantidad = bn.Cantidad,
                                observaciones = bn.Observaciones,
                            }).ToList(),
                            actividades = r.actividadesRequi.Select(ac => new
                            {
                                actividades = ac.Actividades
                            }).ToList(),
                            observaciones = r.observacionesRequi.Select(ob => new
                            {
                                observaciones = ob.Observaciones
                            }).ToList(),
                            procesos = r.procesoRequi.Select(pr => new
                            {
                                proceso = pr.Proceso
                            }).ToList(),
                            documentosCliente = r.documentosClienteRequi.Select(dcr => new
                            {
                                documento = dcr.Documento
                            }).ToList(),
                            prestacionesCliente = r.prestacionesClienteRequi.Select(pcr => new
                            {
                                prestamo = pcr.Prestamo
                            }).ToList(),
                            psicometriasDamsa = r.psicometriasDamsaRequi.Select(pd => new
                            {
                                tipoPsicometria = pd.Psicometria.tipoPsicometria,
                                descripcion = pd.Psicometria.descripcion
                            }).ToList(),
                            psicometriasCliente = r.psicometriasClienteRequi.Select(pc => new
                            {
                                psicometria = pc.Psicometria,
                                descripcion = pc.Descripcion
                            }).ToList(),
                            competenciasCardinal = r.competenciasCardinalRequi.Select(cc => new
                            {
                                competencia = cc.Competencia.competenciaCardinal,
                                nivel = cc.Nivel
                            }).ToList(),
                            competenciasArea = r.competenciasAreaRequi.Select(ca => new
                            {
                                competencia = ca.Competencia.competenciaArea,
                                nivel = ca.Nivel
                            }).ToList(),
                            competenciasGerencial = r.competetenciasGerencialRequi.Select(cg => new
                            {
                                competencia = cg.Competencia.competenciaGerencial,
                                nivel = cg.Nivel
                            }).ToList(),
                            Arte = @"https://apisb.damsa.com.mx/utilerias/" + "img/ArteRequi/BG/" + r.DAMFO290.Arte,
                            bg = @"https://apisb.damsa.com.mx/utilerias/" + "img/ArteRequi/BG/" + r.DAMFO290.Arte
                        }).FirstOrDefault();
                    //stop = new TimeSpan(DateTime.Now.Ticks);
                    //Console.WriteLine(stop.Subtract(start).TotalMilliseconds);
                    
                    return Ok(requisicion);
                }
                else
                {
                    return NotFound();
                }
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
            

        }

        [HttpGet]
        [Route("getByFolio")]
        [Authorize]
        public IHttpActionResult GetRequisicionFolio(Int64 folio)
        {
            try
            {
                if (folio != 0)
                {

                    var requisicion = db.Requisiciones.Where(x => x.Folio.Equals(folio)).Select(r => new {
                        r.Id,
                        r.Folio,
                        r.fch_Cumplimiento,
                        r.fch_Creacion,
                        r.fch_Limite,
                        r.Prioridad,
                        r.Confidencial,
                        r.Estatus,
                        solicitante = db.Entidad.Where(x => x.Id.Equals(r.PropietarioId)).Select(S => S.Nombre + " " + S.ApellidoPaterno + " " + S.ApellidoMaterno).FirstOrDefault(),
                        coordinador  = db.Entidad.Where(x => x.Id.Equals(r.AprobadorId)).Select(S => S.Nombre + " " + S.ApellidoPaterno + " " + S.ApellidoMaterno).FirstOrDefault(),
                        asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(r.Id)).Select(x => x.GrpUsrId).ToList(),
                        asignadosN = r.AsignacionRequi.Where(x => x.RequisicionId.Equals(r.Id) && !x.GrpUsrId.Equals(r.AprobadorId)).Select(x => new {
                            x.GrpUsr.Nombre,
                            x.GrpUsr.ApellidoMaterno,
                            x.GrpUsr.ApellidoPaterno
                        }),
                        vacantes = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                        r.VBtra,
                        HorariosDamfo = db.HorariosPerfiles.Where(h => h.DAMFO290Id.Equals(r.DAMFO290Id)).ToList()
                    }).FirstOrDefault();
                    return Ok(requisicion);
                }
                else
                {
                    return Ok(HttpStatusCode.NotFound);
                }
            }
            catch(Exception ex)
            {
                string ms = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
           

        }

        [HttpPost]
        [Route("createRequi")]
        [Authorize]
        public IHttpActionResult Clon(CreateRequiDto cr)
        {
            try
            {
                object[] _params = {
                    new SqlParameter("@Id", cr.IdDamfo),
                    new SqlParameter("@IdAddress", cr.IdAddress),
                    new SqlParameter("@IdEstatus", cr.IdEstatus),
                    new SqlParameter("@UserAlta", cr.Usuario),
                    new SqlParameter("@UsuarioId", cr.UsuarioId),
                    new SqlParameter("@Confidencial", cr.Confidencial)
                };

                var requi = db.Database.SqlQuery<NewrequiInfo>("exec createRequisicion @Id, @IdAddress, @IdEstatus, @UserAlta, @UsuarioId, @Confidencial  ", _params).SingleOrDefault();

                Guid RequisicionId = requi.Id;
                Int64 Folio = requi.Folio;

                var infoRequi = db.Requisiciones
                    .Where(x => x.Id.Equals(RequisicionId))
                    .Select(x => new
                    {
                        x.Id,
                        x.Folio,
                        x.VBtra,
                        x.EstatusId,
                        x.horariosRequi,
                        x.TipoReclutamientoId,
                    }).FirstOrDefault();              

                return Ok(infoRequi);
            }
            catch (Exception ex)
            {
                string messg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }

        }

        [HttpGet]
        [Route("getRequisiciones")]
        public IHttpActionResult GetRequisiciones(Guid propietario)
        {
            List<Guid> uids = new List<Guid>();
            int[] estatusId = new int[] { 8, 9, 34, 35, 36, 37, 47, 48 };

            try
            {
                byte tipo = db.Usuarios.Where(x => x.Id.Equals(propietario)).Select(u => u.TipoUsuarioId).FirstOrDefault();
                if (tipo == 8 || tipo == 3 || tipo == 12 || tipo == 13 || tipo == 14)
                {
                    var requisicion = db.Requisiciones
                   .Where(e => !e.Confidencial && !estatusId.Contains(e.EstatusId))
                   .Select(e => new
                   {
                       Id = e.Id,
                       VBtra = e.VBtra.ToUpper(),
                       TipoReclutamiento = e.TipoReclutamiento.tipoReclutamiento.ToUpper(),
                       tipoReclutamientoId = e.TipoReclutamientoId,
                       SueldoMinimo = e.SueldoMinimo,
                       SueldoMaximo = e.SueldoMaximo,
                       fch_Creacion = e.fch_Creacion,
                       fch_Modificacion = e.fch_Modificacion,
                       fch_Cumplimiento = e.fch_Cumplimiento,
                       Estatus = e.Estatus.Descripcion.ToUpper(),
                       EstatusId = e.EstatusId,
                       EstatusOrden = e.Estatus.Orden,
                       Cliente = e.Cliente.Nombrecomercial.ToUpper(),
                       Sucursal = db.Direcciones.Where(x => x.Id.Equals(e.DireccionId)).Select(d => d.Calle + " " + d.NumeroExterior + " C.P: " + d.CodigoPostal + " Col: " + d.Colonia.colonia).FirstOrDefault().ToUpper(),
                       Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                       Folio = e.Folio,
                       Confidencial = e.Confidencial,
                       Postulados = db.Postulaciones.Where(p => p.RequisicionId.Equals(e.Id) & p.StatusId.Equals(1)).Count(),
                       PostuladosN = db.Postulaciones.Where(p => p.RequisicionId.Equals(e.Id) & p.StatusId.Equals(1)).Select(p => new
                       {
                           p.CandidatoId,
                           p.Candidato.Nombre,
                           p.Candidato.ApellidoPaterno,
                           p.Candidato.ApellidoMaterno,
                           p.Candidato.CURP,
                           email = p.Candidato.emails.Select(m => m.email).FirstOrDefault(),
                           p.StatusId,
                       }),
                       EnProceso = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId != 27 && p.EstatusId != 40 && p.EstatusId != 28 && p.EstatusId != 42).Count(),
                       EnProcesoN = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId != 24 && p.EstatusId != 27 && p.EstatusId != 40 && p.EstatusId != 28 && p.EstatusId != 42).Select(d => new
                       {
                           candidatoId = d.CandidatoId,
                           nombre = db.Candidatos.Where(x => x.Id.Equals(d.CandidatoId)).Select(cc => cc.Nombre + " " + cc.ApellidoPaterno + " " + cc.ApellidoMaterno).FirstOrDefault(),
                           email = db.Emails.Where(x => x.EntidadId.Equals(d.CandidatoId)).Select(m => m.email).FirstOrDefault(),
                           estatusId = d.EstatusId
                       }),
                       Contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId.Equals(24)).Count(),
                       coordinador = string.IsNullOrEmpty(db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper()) ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                       Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                       reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                      db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                                       ).ToList(),

                       ComentarioReclutador = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(e.Id)).Select(c => c.fch_Creacion + " - " + c.UsuarioAlta + " - " + (c.Motivo.Id == 7 ? "" : c.Motivo.Descripcion + " - ") + c.Comentario).ToList()
                   }).OrderBy(x => x.EstatusOrden).ThenByDescending(x => x.Folio).ToList();

                    return Ok(requisicion);
                }
                else if (tipo == 10) //ejecutivo de cuenta
                {

                    var requisicion = db.Requisiciones
                   .Where(e => e.PropietarioId.Equals(propietario) && !estatusId.Contains(e.EstatusId))
                   .Select(e => new
                   {
                       Id = e.Id,
                       VBtra = e.VBtra.ToUpper(),
                       TipoReclutamiento = e.TipoReclutamiento.tipoReclutamiento.ToUpper(),
                       tipoReclutamientoId = e.TipoReclutamientoId,
                       SueldoMinimo = e.SueldoMinimo,
                       SueldoMaximo = e.SueldoMaximo,
                       fch_Creacion = e.fch_Creacion,
                       fch_Modificacion = e.fch_Modificacion,
                       fch_Cumplimiento = e.fch_Cumplimiento,
                       Estatus = e.Estatus.Descripcion.ToUpper(),
                       EstatusId = e.EstatusId,
                       EstatusOrden = e.Estatus.Orden,
                       Cliente = e.Cliente.Nombrecomercial.ToUpper(),
                       Sucursal = db.Direcciones.Where(x => x.Id.Equals(e.DireccionId)).Select(d => d.Calle + " " + d.NumeroExterior + " C.P: " + d.CodigoPostal + " Col: " + d.Colonia.colonia).FirstOrDefault().ToUpper(),
                       Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                       Folio = e.Folio,
                       Confidencial = e.Confidencial,
                       Postulados = db.Postulaciones.Where(p => p.RequisicionId.Equals(e.Id) & p.StatusId.Equals(1)).Count(),
                       PostuladosN = db.Postulaciones.Where(p => p.RequisicionId.Equals(e.Id) & p.StatusId.Equals(1)).Select(p => new
                       {
                           p.CandidatoId,
                           p.Candidato.Nombre,
                           p.Candidato.ApellidoPaterno,
                           p.Candidato.ApellidoMaterno,
                           p.Candidato.CURP,
                           email = p.Candidato.emails.Select(m => m.email).FirstOrDefault(),
                           p.StatusId,
                       }),
                       EnProceso = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId != 27 && p.EstatusId != 40 && p.EstatusId != 28 && p.EstatusId != 42).Count(),
                       EnProcesoN = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId != 24 && p.EstatusId != 27 && p.EstatusId != 40 && p.EstatusId != 28 && p.EstatusId != 42).Select(d => new
                       {
                           candidatoId = d.CandidatoId,
                           nombre = db.Candidatos.Where(x => x.Id.Equals(d.CandidatoId)).Select(cc => cc.Nombre + " " + cc.ApellidoPaterno + " " + cc.ApellidoMaterno).FirstOrDefault(),
                           email = db.Emails.Where(x => x.EntidadId.Equals(d.CandidatoId)).Select(m => m.email).FirstOrDefault(),
                           estatusId = d.EstatusId
                       }),
                       Contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId.Equals(24)).Count(),
                       coordinador = string.IsNullOrEmpty(db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper()) ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                       Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                       reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                      db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                                       ).ToList(),

                       ComentarioReclutador = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(e.Id)).Select(c => c.fch_Creacion + " - " + c.UsuarioAlta + " - " + (c.Motivo.Id == 7 ? "" : c.Motivo.Descripcion + " - ") + c.Comentario).ToList()
                   }).OrderBy(x => x.EstatusOrden).ThenByDescending(x => x.Folio).ToList();
                    return Ok(requisicion);
                }
                else
                {
                    if (db.Subordinados.Count(x => x.LiderId.Equals(propietario)) > 0)
                    {
                        var ids = db.Subordinados.Where(x => !x.UsuarioId.Equals(propietario) && x.LiderId.Equals(propietario)).Select(u => u.UsuarioId).ToList();

                        uids = GetSub(ids, uids);

                    }

                    uids.Add(propietario);

                    var requisicion = db.Requisiciones
                   .Where(e => !estatusId.Contains(e.EstatusId) 
                   && (uids.Contains(e.PropietarioId)))
                   .Select(e => new
                   {
                       Id = e.Id,
                       VBtra = e.VBtra.ToUpper(),
                       TipoReclutamiento = e.TipoReclutamiento.tipoReclutamiento.ToUpper(),
                       tipoReclutamientoId = e.TipoReclutamientoId,
                       SueldoMinimo = e.SueldoMinimo,
                       SueldoMaximo = e.SueldoMaximo,
                       fch_Creacion = e.fch_Creacion,
                       fch_Modificacion = e.fch_Modificacion,
                       fch_Cumplimiento = e.fch_Cumplimiento,
                       Estatus = e.Estatus.Descripcion.ToUpper(),
                       EstatusId = e.EstatusId,
                       EstatusOrden = e.Estatus.Orden,
                       Cliente = e.Cliente.Nombrecomercial.ToUpper(),
                       Sucursal = db.Direcciones.Where(x => x.Id.Equals(e.DireccionId)).Select(d => d.Calle + " " + d.NumeroExterior + " C.P: " + d.CodigoPostal + " Col: " + d.Colonia.colonia).FirstOrDefault().ToUpper(),
                       Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                       Folio = e.Folio,
                       Confidencial = e.Confidencial,
                       Postulados = db.Postulaciones.Where(p => p.RequisicionId.Equals(e.Id) & p.StatusId.Equals(1)).Count(),
                       PostuladosN = db.Postulaciones.Where(p => p.RequisicionId.Equals(e.Id) & p.StatusId.Equals(1)).Select(p => new
                       {
                           p.CandidatoId,
                           p.Candidato.Nombre,
                           p.Candidato.ApellidoPaterno,
                           p.Candidato.ApellidoMaterno,
                           p.Candidato.CURP,
                           email = p.Candidato.emails.Select(m => m.email).FirstOrDefault(),
                           p.StatusId
                       }),
                       EnProceso = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId != 27 && p.EstatusId != 40 && p.EstatusId != 28 && p.EstatusId != 42).Count(),
                       EnProcesoN = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId != 24 && p.EstatusId != 27 && p.EstatusId != 40 && p.EstatusId != 28 && p.EstatusId != 42).Select(d => new
                       {
                           candidatoId = d.CandidatoId,
                           nombre = db.Candidatos.Where(x => x.Id.Equals(d.CandidatoId)).Select(cc => cc.Nombre + " " + cc.ApellidoPaterno + " " + cc.ApellidoMaterno).FirstOrDefault(),
                           email = db.Emails.Where(x => x.EntidadId.Equals(d.CandidatoId)).Select(m => m.email).FirstOrDefault(),
                           estatusId = d.EstatusId
                       }),
                       Contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId.Equals(24)).Count(),
                       coordinador = string.IsNullOrEmpty(db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault()) ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                       Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault().ToUpper(),
                       reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                      db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                                       ).ToList(),
                       ComentarioReclutador = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(e.Id)).Select(c => c.fch_Creacion + " - " + c.UsuarioAlta + " - " + (c.Motivo.Id == 7 ? "" : c.Motivo.Descripcion.ToUpper() + " - ") + c.Comentario.ToUpper()).ToList()
                   }).OrderBy(x => x.EstatusOrden).ThenByDescending(x => x.Folio).ToList();

                    return Ok(requisicion);

                }

            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.ExpectationFailed);
            }
            
        }


        [HttpGet]
        [Route("getRequisicionesHistorial")]
        [Authorize]
        public IHttpActionResult GetRequisicionesHistorial(Guid propietario)
        {
            List<Guid> uids = new List<Guid>();
            int[] estatusId =  { 8, 9, 34, 35, 36, 37,47,48 };
            try
            {
                var tipo = db.Usuarios.Where(x => x.Id.Equals(propietario)).Select(u => u.TipoUsuarioId).FirstOrDefault();
                if (tipo == 8 || tipo == 3 || tipo == 12 || tipo == 13 || tipo == 14)
                {
                    var requisicion = db.Requisiciones
                   .Where(e => !e.Confidencial && estatusId.Contains(e.EstatusId))
                   .Select(e => new
                   {
                       Id = e.Id,
                       VBtra = e.VBtra.ToUpper(),
                       claseReclutamiento = e.ClaseReclutamiento.clasesReclutamiento.ToUpper(),
                       claseReclutamientoId = e.ClaseReclutamientoId,
                       tipoReclutamientoId = e.TipoReclutamientoId,
                       SueldoMinimo = e.SueldoMinimo,
                       fch_Creacion = e.fch_Creacion,
                       fch_Modificacion = e.fch_Modificacion,
                       fch_Cumplimiento = e.fch_Cumplimiento,
                       Estatus = e.Estatus.Descripcion.ToUpper(),
                       EstatusId = e.EstatusId,
                       EstatusOrden = e.Estatus.Orden,
                       Cliente = e.Cliente.Nombrecomercial.ToUpper(),
                       Sucursal = db.Direcciones.Where(x => x.Id.Equals(e.DireccionId)).Select(d => d.Calle + " " + d.NumeroExterior + " C.P: " + d.CodigoPostal + " Col: " + d.Colonia.colonia).FirstOrDefault().ToUpper(),
                       Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                       Folio = e.Folio,
                       Confidencial = e.Confidencial,
                       Contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId.Equals(24)).Count(),
                       coordinador = string.IsNullOrEmpty(db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper()) ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                       Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                       reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                      db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                                       ).ToList(),
                   }).OrderByDescending(x => x.fch_Modificacion).ThenByDescending(x => x.EstatusOrden).ToList();

                    return Ok(requisicion);
                }
                else if(tipo == 10) //ejecutivo de cuenta
                {
                  
                   var requisicion = db.Requisiciones
                  .Where(e => e.PropietarioId.Equals(propietario) && estatusId.Contains(e.EstatusId))
                  .Select(e => new
                  {
                      Id = e.Id,
                      VBtra = e.VBtra.ToUpper(),
                      claseReclutamiento = e.ClaseReclutamiento.clasesReclutamiento.ToUpper(),
                      claseReclutamientoId = e.ClaseReclutamientoId,
                      tipoReclutamientoId = e.TipoReclutamientoId,
                      SueldoMinimo = e.SueldoMinimo,
                      fch_Creacion = e.fch_Creacion,
                      fch_Modificacion = e.fch_Modificacion,
                      fch_Cumplimiento = e.fch_Cumplimiento,
                      Estatus = e.Estatus.Descripcion.ToUpper(),
                      EstatusId = e.EstatusId,
                      EstatusOrden = e.Estatus.Orden,
                      Cliente = e.Cliente.Nombrecomercial.ToUpper(),
                      Sucursal = db.Direcciones.Where(x => x.Id.Equals(e.DireccionId)).Select(d => d.Calle + " " + d.NumeroExterior + " C.P: " + d.CodigoPostal + " Col: " + d.Colonia.colonia).FirstOrDefault().ToUpper(),
                      Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                      Folio = e.Folio,
                      Confidencial = e.Confidencial,
                      //Postulados = db.Postulaciones.Where(p => p.RequisicionId.Equals(e.Id) & p.StatusId.Equals(1)).Count(),
                      //PostuladosN = db.Postulaciones.Where(p => p.RequisicionId.Equals(e.Id) & p.StatusId.Equals(1)).Select(p => new
                      //{
                      //    p.CandidatoId,
                      //    p.Candidato.Nombre,
                      //    p.Candidato.ApellidoPaterno,
                      //    p.Candidato.ApellidoMaterno,
                      //    p.Candidato.CURP,
                      //    email = p.Candidato.emails.Select(m => m.email).FirstOrDefault(),
                      //    p.StatusId,
                      //}),
                      //EnProceso = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId != 27 && p.EstatusId != 40 && p.EstatusId != 28 && p.EstatusId != 42).Count(),
                      //EnProcesoN = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId != 24 && p.EstatusId != 27 && p.EstatusId != 40 && p.EstatusId != 28 && p.EstatusId != 42).Select(d => new
                      //{
                      //    candidatoId = d.CandidatoId,
                      //    nombre = db.Candidatos.Where(x => x.Id.Equals(d.CandidatoId)).Select(cc => cc.Nombre + " " + cc.ApellidoPaterno + " " + cc.ApellidoMaterno).FirstOrDefault(),
                      //    email = db.Emails.Where(x => x.EntidadId.Equals(d.CandidatoId)).Select(m => m.email).FirstOrDefault(),
                      //    estatusId = d.EstatusId
                      //}),
                      Contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId.Equals(24)).Count(),
                      coordinador = string.IsNullOrEmpty(db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper()) ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                      Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                      reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                     db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                                      ).ToList(),

                      //ComentarioReclutador = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(e.Id)).Select(c => c.fch_Creacion + " - " + c.UsuarioAlta + " - " + (c.Motivo.Id == 7 ? "" : c.Motivo.Descripcion + " - ") + c.Comentario).ToList()
                  }).OrderByDescending(x => x.fch_Modificacion).ThenByDescending(x => x.EstatusOrden).ToList();

                    return Ok(requisicion);
                }
                else
                {
                    if (db.Subordinados.Count(x => x.LiderId.Equals(propietario)) > 0)
                    {
                        var ids = db.Subordinados.Where(x => !x.UsuarioId.Equals(propietario) && x.LiderId.Equals(propietario)).Select(u => u.UsuarioId).ToList();

                        uids = GetSub(ids, uids);

                    }

                    uids.Add(propietario);

                    var asignadas = db.AsignacionRequis
                        .Where(x => x.GrpUsrId.Equals(propietario) 
                        && estatusId.Contains(x.Requisicion.EstatusId))
                        .Select(a => a.RequisicionId).ToList();


                    var requis = db.Requisiciones
                   .Where(e => (uids.Contains(e.AprobadorId) || uids.Contains(e.PropietarioId))
                   && estatusId.Contains(e.EstatusId))
                   .Select(a => a.Id).ToList();

                    var AllRequis = requis.Union(asignadas);

                    var requisicion = db.Requisiciones
                   .Where(e => AllRequis.Distinct().Contains(e.Id))
                   .Select(e => new
                   {
                       Id = e.Id,
                       VBtra = e.VBtra.ToUpper(),
                       claseReclutamiento = e.ClaseReclutamiento.clasesReclutamiento.ToUpper(),
                       claseReclutamientoId = e.ClaseReclutamientoId,
                       tipoReclutamientoId = e.TipoReclutamientoId,
                       SueldoMinimo = e.SueldoMinimo,
                       fch_Creacion = e.fch_Creacion,
                       fch_Modificacion = e.fch_Modificacion,
                       fch_Cumplimiento = e.fch_Cumplimiento,
                       Estatus = e.Estatus.Descripcion.ToUpper(),
                       EstatusId = e.EstatusId,
                       EstatusOrden = e.Estatus.Orden,
                       Cliente = e.Cliente.Nombrecomercial.ToUpper(),
                       Sucursal = db.Direcciones.Where(x => x.Id.Equals(e.DireccionId)).Select(d => d.Calle + " " + d.NumeroExterior + " C.P: " + d.CodigoPostal + " Col: " + d.Colonia.colonia).FirstOrDefault().ToUpper(),
                       Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                       Folio = e.Folio,
                       Confidencial = e.Confidencial,
                       Contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId.Equals(24)).Count(),
                       coordinador = string.IsNullOrEmpty(db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault()) ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                       Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault().ToUpper(),
                       reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                      db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                                       ).ToList(),
                   }).OrderByDescending(x => x.fch_Modificacion).ThenByDescending(x => x.EstatusOrden).ToList();

                    return Ok(requisicion);

                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpGet]
        [Route("getRequisicionesTipo")]
        [Authorize]
        public IHttpActionResult GetRequisicionesPuro(Guid propietario, int tipo)
        {
            try
            {
                var UnidadNegocio = db.Usuarios
                    .Where(u => u.Id.Equals(propietario)).Select(u => u.Sucursal.UnidadNegocioId).FirstOrDefault();
                    var requisicion = db.Requisiciones
                        .Where(e => e.Activo.Equals(true) && e.TipoReclutamientoId.Equals(tipo) && (e.EstatusId.Equals(43) || e.EstatusId.Equals(44)) )
                        .Select(e => new
                        {
                            Id = e.Id,
                            EstadoId = e.Direccion.EstadoId,
                            VBtra = e.VBtra,
                            SueldoMinimo = e.SueldoMinimo,
                            SueldoMaximo = e.SueldoMaximo,
                            fch_Creacion = e.fch_Creacion,
                            fch_Modificacion = e.fch_Modificacion,
                            fch_Cumplimiento = e.fch_Cumplimiento,
                            Estatus = e.Estatus.Descripcion,
                            EstatusId = e.EstatusId,
                            EstatusOrden = e.Estatus.Orden,
                            Prioridad = e.Prioridad.Descripcion,
                            PrioridadId = e.PrioridadId,
                            Cliente = e.Cliente.Nombrecomercial,
                            razon = e.Cliente.RazonSocial,
                            factura = e.Cliente.RazonSocial,
                            tipoReclutamiento = e.TipoReclutamiento.tipoReclutamiento,
                            GiroEmpresa = e.Cliente.GiroEmpresas.giroEmpresa,
                            ActividadEmpresa = e.Cliente.ActividadEmpresas.actividadEmpresa,
                            Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                            Folio = e.Folio,
                            Confidencial = e.Confidencial,
                            Porcentaje = db.FacturacionPuro.Where(x => x.RequisicionId.Equals(e.Id)).Select(p => p.Porcentaje).FirstOrDefault(),
                            Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                            ComentarioReclutador = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(e.Id)).Select(c => c.fch_Creacion + " - " + c.UsuarioAlta + " - " + (c.Motivo.Id == 7 ? "" : c.Motivo.Descripcion + " - ") + c.Comentario).ToList()
                        }).OrderBy(x => x.EstatusOrden).ThenByDescending(x => x.Folio).ToList();

                    return Ok(requisicion);
                

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpGet]
        [Route("getRequisicionesEstatus")]
        public IHttpActionResult GetRequisicionesEstatus(int estatus, Guid ReclutadorId)
        {
            try
            {
                //List<Guid> uids = new List<Guid>();
                //if (db.Subordinados.Count(x => x.LiderId.Equals(ReclutadorId)) > 0)
                //{
                //    var ids = db.Subordinados.Where(x => !x.UsuarioId.Equals(ReclutadorId) && x.LiderId.Equals(ReclutadorId)).Select(u => u.UsuarioId).ToList();

                //    uids = GetSub(ids, uids);

                //}
                //uids.Add(ReclutadorId);

                //var asig = db.AsignacionRequis
                //      .OrderByDescending(e => e.Id)
                //      .Where(a => uids.Distinct().Contains(a.GrpUsrId))
                //      .Select(a => a.RequisicionId)
                //      .Distinct()
                //      .ToList();

                var vacantes = db.Requisiciones.OrderByDescending(e => e.Folio)
                    .Where(e => e.Activo.Equals(true) && e.Estatus.Id.Equals(estatus) && e.AprobadorId.Equals(ReclutadorId))
                    .Select(e => new
                    {
                        Id = e.Id,
                        Folio = e.Folio,
                        VBtra = e.VBtra,
                        Cliente = e.Cliente.Nombrecomercial,
                        Solicita = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno ).FirstOrDefault(),
                        coordinador = db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault(),
                        fch_Creacion = e.fch_Creacion,
                        Estatus = e.Estatus.Descripcion,
                        EstatusId = e.EstatusId,
                        Examen = db.RequiExamen.Where(x => x.RequisicionId.Equals(e.Id)).Count() > 0 ? true : false,
                        EnProceso = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId != 27 && p.EstatusId != 40 && p.EstatusId != 42).Count(),
                        EnProcesoEC = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 22).Count(),
                        EnProcesoFC = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 23).Count(),
                        contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 24).Count(),
                        ComentarioReclutador = db.ComentariosVacantes.OrderByDescending(f => f.fch_Creacion).Where(x => x.RequisicionId.Equals(e.Id) && x.Motivo.EstatusId.Equals(39)).Select(c => new {
                            id = c.Id,
                            folio = db.FolioIncidencia.Where(x => x.ComentarioId.Equals(c.Id)).Select(f => f.Folio).FirstOrDefault(),
                            fecha = c.fch_Creacion,
                            motivo = c.Motivo.Descripcion,
                            comentario = c.Comentario,
                            reclutador = db.Usuarios.Where(x => x.Id.Equals(c.ReclutadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault(),
                            respuesta = String.IsNullOrEmpty(db.ComentariosVacantes.Where(x => x.RespuestaId.Equals(c.Id)).Select(r => r.fch_Creacion + " - " + db.Usuarios.Where(x => x.Id.Equals(r.ReclutadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno).FirstOrDefault() + " - " + r.Comentario).FirstOrDefault()) ? "Doble click para editar" : db.ComentariosVacantes.Where(x => x.RespuestaId.Equals(c.Id)).Select(r => r.fch_Creacion + " - " + db.Usuarios.Where(x => x.Id.Equals(r.ReclutadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno).FirstOrDefault() + " - " + r.Comentario).FirstOrDefault()
                        }).FirstOrDefault()
                    }).ToList();

                return Ok(vacantes);

            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }   
        }

     
        [HttpGet]
        [Route("getRequiReclutador")]
        [Authorize]
        public IHttpActionResult GtRequiReclutador(Guid IdUsuario)
        {
            int[] estatusId = new int[] { 8, 9, 34, 35, 36, 37, 47, 48 };
            try
            {
                var tipo = db.Usuarios.Where(x => x.Id.Equals(IdUsuario)).Select(u => u.TipoUsuarioId).FirstOrDefault();
                if (tipo == 8 || tipo == 3 || tipo == 12 || tipo == 13 || tipo == 14)
                {
                    var vacantes = db.Requisiciones.OrderByDescending(e => e.Folio)
                        .Where(e => e.Activo
                        && !e.Confidencial
                        && !estatusId.Contains(e.EstatusId))
                        .Select(e => new
                        {
                            Id = e.Id,
                            VBtra = e.VBtra.ToUpper(),
                            ClaseReclutamiento = e.ClaseReclutamiento.clasesReclutamiento.ToUpper(),
                            ClaseReclutamientoId = e.ClaseReclutamientoId,
                            SueldoMinimo = e.SueldoMinimo,
                            SueldoMaximo = e.SueldoMaximo,
                            fch_Creacion = e.fch_Creacion,
                            fch_Modificacion = e.fch_Modificacion,
                            fch_Cumplimiento = e.fch_Cumplimiento,
                            Estatus = e.Estatus.Descripcion.ToUpper(),
                            EstatusId = e.EstatusId,
                            EstatusOrden = e.Estatus.Orden,
                            Cliente = e.Cliente.Nombrecomercial.ToUpper(),
                            ClienteId = e.Cliente.Id,
                            Sucursal = db.Direcciones.Where(x => x.Id.Equals(e.DireccionId)).Select(d => d.Calle + " " + d.NumeroExterior + " C.P: " + d.CodigoPostal + " Col: " + d.Colonia.colonia).FirstOrDefault().ToUpper(),
                            Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                            Folio = e.Folio,
                            Confidencial = e.Confidencial,
                            Postulados = db.Postulaciones.Where(p => p.RequisicionId.Equals(e.Id) && p.StatusId.Equals(1)).Select(c => c.CandidatoId).Count(),
                            EnProceso = db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId != 27 && p.EstatusId != 40).Count(),
                            EnProcesoFR = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 21).Count(),
                            EnProcesoFC = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 23).Count(),
                            contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 24).Count(),
                            coordinador = string.IsNullOrEmpty(db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault()) ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            Solicita = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            PropietarioId = e.PropietarioId,
                            AprobadorId = e.AprobadorId,
                            Aprobada = e.Aprobada,
                            DiasEnvio = e.DiasEnvio,
                            asignados = e.AsignacionRequi.Select(a => a.GrpUsrId).ToList(),
                            reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                            
                                db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()).ToList(),
                            ComentarioReclutador = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(e.Id)).Select(c => c.fch_Creacion + " - " + c.UsuarioAlta + " - " + (c.Motivo.Id == 7 ? "" : c.Motivo.Descripcion.ToUpper() + " - ") + c.Comentario.ToUpper()).ToList(),
                            Oficio = db.OficiosRequisicion.Where(of => of.RequisicionId.Equals(e.Id)).Count() > 0 ? db.OficiosRequisicion.Where(of => of.RequisicionId.Equals(e.Id)).FirstOrDefault() : null,
                            Ponderacion = db.PonderacionRequisiciones.Where(x => x.RequisicionId.Equals(e.Id)).Count() > 0 ? db.PonderacionRequisiciones.Where(x => x.RequisicionId.Equals(e.Id)).Select(p => new
                            {
                                Id = p.Id,
                                ponderacion = p.Ponderacion
                            }).FirstOrDefault() : null
                        }).OrderBy(x => x.EstatusOrden).ThenByDescending(x => x.Folio).ToList();
                    return Ok(vacantes);
                }
                else
                {
                    List<Guid> uids = new List<Guid>();
                    if (db.Subordinados.Count(x => x.LiderId.Equals(IdUsuario)) > 0)
                    {
                        var ids = db.Subordinados.Where(x => !x.UsuarioId.Equals(IdUsuario) && x.LiderId.Equals(IdUsuario)).Select(u => u.UsuarioId).ToList();

                        uids = GetSub(ids, uids);

                    }
                    uids.Add(IdUsuario);

                    var asignadas = db.AsignacionRequis
                            .OrderByDescending(e => e.Id)
                            .Where(a => uids.Distinct().Contains(a.GrpUsrId)
                                && !estatusId.Contains(a.Requisicion.EstatusId))
                            .Select(a => a.RequisicionId)
                            .Distinct()
                            .ToList();

                    var requis = db.Requisiciones
                    .Where(e => (uids.Contains(e.AprobadorId) || uids.Contains(e.PropietarioId))
                        && !estatusId.Contains(e.EstatusId))
                    .Select(a => a.Id).ToList();

                    var AllRequis = requis.Union(asignadas);

                    var vacantes = db.Requisiciones.OrderByDescending(e => e.Folio)
                        .Where(e => AllRequis.Distinct().Contains(e.Id))
                        .Select(e => new
                        {
                            Id = e.Id,
                            VBtra = e.VBtra.ToUpper(),
                            ClaseReclutamiento = e.ClaseReclutamiento.clasesReclutamiento.ToUpper(),
                            ClaseReclutamientoId = e.ClaseReclutamientoId,
                            SueldoMinimo = e.SueldoMinimo,
                            SueldoMaximo = e.SueldoMaximo,
                            fch_Creacion = e.fch_Creacion,
                            fch_Modificacion = e.fch_Modificacion,
                            fch_Cumplimiento = e.fch_Cumplimiento,
                            Estatus = e.Estatus.Descripcion.ToUpper(),
                            EstatusId = e.EstatusId,
                            EstatusOrden = e.Estatus.Orden,
                            Cliente = e.Cliente.Nombrecomercial.ToUpper(),
                            ClienteId = e.Cliente.Id,
                            Sucursal = db.Direcciones.Where(x => x.Id.Equals(e.DireccionId)).Select(d => d.Calle + " " + d.NumeroExterior + " C.P: " + d.CodigoPostal + " Col: " + d.Colonia.colonia).FirstOrDefault().ToUpper(),
                            Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                            Folio = e.Folio,
                            Confidencial = e.Confidencial,
                            Postulados = db.Postulaciones.Where(p => p.RequisicionId.Equals(e.Id) && p.StatusId.Equals(1)).Select(c => c.CandidatoId).Count(),
                            EnProceso = db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId != 27 && p.EstatusId != 40).Count(),
                            EnProcesoFR = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 21).Count(),
                            EnProcesoFC = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 23).Count(),
                            contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 24).Count(),
                            coordinador = string.IsNullOrEmpty(db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault()) ? "SIN ASIGNAR" : db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            Solicita = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            PropietarioId = e.PropietarioId,
                            AprobadorId = e.AprobadorId,
                            aprobada = e.Aprobada,
                            DiasEnvio = e.DiasEnvio,
                            asignados = e.AsignacionRequi.Select(a => a.GrpUsrId).ToList(),
                            reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(a =>
                               db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                            ).ToList(),
                            ComentarioReclutador = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(e.Id)).Select(c => c.fch_Creacion + " - " + c.UsuarioAlta + " - " + (c.Motivo.Id == 7 ? "" : c.Motivo.Descripcion.ToUpper() + " - ") + c.Comentario.ToUpper()).ToList(),
                            Oficio = db.OficiosRequisicion.Where(of => of.RequisicionId.Equals(e.Id)).Count() > 0 ? db.OficiosRequisicion.Where(of => of.RequisicionId.Equals(e.Id)).FirstOrDefault() : null,
                            Ponderacion = db.PonderacionRequisiciones.Where(x => x.RequisicionId.Equals(e.Id)).Count() > 0 ? db.PonderacionRequisiciones.Where(x => x.RequisicionId.Equals(e.Id)).Select( p => new
                            {
                                Id = p.Id,
                                ponderacion = p.Ponderacion
                            }).FirstOrDefault() : null
                        }).OrderBy(x => x.EstatusOrden).ThenByDescending(x => x.Folio).ToList();
                    return Ok(vacantes);
                }

            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }

        }

        [HttpGet]
        [Route("getRequiEstadisticos")]
        [Authorize]
        public IHttpActionResult GetRequiEstadisticos(Guid IdUsuario)
        {
            try
            {
                var tipo = db.Usuarios.Where(x => x.Id.Equals(IdUsuario)).Select(u => u.TipoUsuarioId).FirstOrDefault();
                if (tipo == 8)
                {
                   
                    var vacantes = db.Requisiciones.OrderByDescending(e => e.Folio)
                        .Where(e => e.Activo.Equals(true))
                        .Select(e => new
                        {
                            Id = e.Id,
                            VBtra = e.VBtra.ToUpper(),
                            TipoReclutamiento = e.TipoReclutamiento.tipoReclutamiento.ToUpper(),
                            tipoReclutamientoId = e.TipoReclutamientoId,
                            SueldoMinimo = e.SueldoMinimo,
                            fch_Creacion = e.fch_Creacion,
                            fch_Cumplimiento = e.fch_Cumplimiento,
                            Estatus = e.Estatus.Descripcion.ToUpper(),
                            EstatusId = e.EstatusId,
                            Cliente = e.Cliente.Nombrecomercial.ToUpper(),
                            ClienteId = e.Cliente.Id,
                            Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                            Folio = e.Folio,
                            Confidencial = e.Confidencial,
                            Asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(x => x.GrpUsrId).ToList(),
                            Postulados = db.Postulaciones.Where(p => p.RequisicionId.Equals(e.Id) && p.StatusId.Equals(1)).Select(c => c.CandidatoId).Except(db.ProcesoCandidatos.Where(xx => xx.RequisicionId.Equals(e.Id) && xx.EstatusId.Equals(27) || xx.EstatusId.Equals(40)).Select(cc => cc.CandidatoId)).Count(),
                            EnProceso = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId != 27 && p.EstatusId != 40).Count(),
                            EnProcesoFR = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 21).Count(),
                            EnProcesoFC = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 23).Count(),
                            contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 24).Count(),
                            Solicita = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(s => s.Nombre + " " + s.ApellidoPaterno).FirstOrDefault(),
                            AreaExperiencia = e.Area.areaExperiencia,
                            Aprobador = e.Aprobador != null ? e.Aprobador : "",

                        }).ToList();
                    return Ok(vacantes);
                }
                else
                {
                    List<Guid> uids = new List<Guid>();
                    if (db.Subordinados.Count(x => x.LiderId.Equals(IdUsuario)) > 0)
                    {
                        var ids = db.Subordinados.Where(x => !x.UsuarioId.Equals(IdUsuario) && x.LiderId.Equals(IdUsuario)).Select(u => u.UsuarioId).ToList();

                        uids = GetSub(ids, uids);

                    }
                    uids.Add(IdUsuario);


                    var asig = db.AsignacionRequis
                        .OrderByDescending(e => e.Id)
                        .Where(a => uids.Distinct().Contains(a.GrpUsrId))
                        .Select(a => a.RequisicionId)
                        .Distinct()
                        .ToList();

                    var vacantes = db.Requisiciones.OrderByDescending(e => e.Folio)
                        .Where(e => asig.Contains(e.Id))
                        .Where(e => e.Activo.Equals(true))
                        .Select(e => new
                        {
                            Id = e.Id,
                            VBtra = e.VBtra.ToUpper(),
                            TipoReclutamiento = e.TipoReclutamiento.tipoReclutamiento.ToUpper(),
                            tipoReclutamientoId = e.TipoReclutamientoId,
                            SueldoMinimo = e.SueldoMinimo,
                            fch_Creacion = e.fch_Creacion,
                            fch_Cumplimiento = e.fch_Cumplimiento,
                            Estatus = e.Estatus.Descripcion.ToUpper(),
                            EstatusId = e.EstatusId,
                            Prioridad = e.Prioridad.Descripcion.ToUpper(),
                            PrioridadId = e.PrioridadId,
                            Cliente = e.Cliente.Nombrecomercial.ToUpper(),
                            ClienteId = e.Cliente.Id,
                            Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                            Folio = e.Folio,
                            Confidencial = e.Confidencial,
                            Asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && !x.GrpUsrId.Equals(e.AprobadorId)).Select(x => x.GrpUsrId).ToList(),
                            Postulados = db.Postulaciones.Where(p => p.RequisicionId.Equals(e.Id) && p.StatusId.Equals(1)).Select(c => c.CandidatoId).Except(db.ProcesoCandidatos.Where(xx => xx.RequisicionId.Equals(e.Id) && xx.EstatusId.Equals(27) || xx.EstatusId.Equals(40)).Select(cc => cc.CandidatoId)).Count(),
                            EnProceso = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId != 27 && p.EstatusId != 40).Count(),
                            EnProcesoFR = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 21).Count(),
                            EnProcesoFC = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 23).Count(),
                            contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 24).Count(),
                            Solicita = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(s => s.Nombre + " " + s.ApellidoPaterno).FirstOrDefault(),
                            AreaExperiencia = e.Area.areaExperiencia,
                            Aprobador = e.Aprobador != null ? e.Aprobador.ToUpper() : "",
                        }).ToList();
                    return Ok(vacantes);
                }

            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }

        }

        // Count days from d0 to d1 inclusive, excluding weekends
        public int countWeekDays(DateTime d0, DateTime d1)
        {
            int ndays = 1 + Convert.ToInt32((d1 - d0).TotalDays);
            int nsaturdays = (ndays + Convert.ToInt32(d0.DayOfWeek)) / 7;
            return ndays - 2 * nsaturdays
                   - (d0.DayOfWeek == DayOfWeek.Sunday ? 1 : 0)
                   + (d1.DayOfWeek == DayOfWeek.Saturday ? 1 : 0);
        }


        [HttpGet]
        [Route("getReporte70")]
        public IHttpActionResult GetReporte70(string clave, string ofc, string tipo, string fini,
           string ffin, string emp, string sol, string trcl, string cor, string stus, string recl, string usuario)
        {

            try
            {
                //Stopwatch stopwatch = new Stopwatch();

                //stopwatch.Start();
                List<int> estatus = new List<int> { 6, 7,8,9, 29, 30, 33, 38 };
                var vacantes = db.Database.SqlQuery<ReporteGeneralDto>("dbo.ReporteGeneral");



                var t = db.EstatusRequisiciones.GroupBy(g => g.RequisicionId)
                    .Select(T => new
                    {

                        RequisicionId = T.Key,

                        Estatus = T.Select(x => new EstatusRequiDto
                        {
                            EstatusId = x.EstatusId,
                            Estatus = x.Estatus.Descripcion.ToUpper(),
                            fch_Modificacion = x.fch_Modificacion.Value,
                            diasTrans = 0,
                            diasTotal = 0,
                        }).OrderBy(o => o.fch_Modificacion).ToList()

                    }).ToList();



                foreach (var r in t)
                {
                    if (r.Estatus.Count > 1)
                    {
                        for (int i = 0; i < r.Estatus.Count() - 1; i++)
                        {
                            int dt = countWeekDays(r.Estatus[i].fch_Modificacion, r.Estatus[i + 1].fch_Modificacion);
                            r.Estatus[i].diasTrans = dt - 1;
                            if (estatus.Contains(r.Estatus[i].EstatusId))
                            {
                                r.Estatus[i].diasTotal += (dt - 1);
                            }
                        }
                    }
                    else
                    {
                        int dt = countWeekDays(r.Estatus[0].fch_Modificacion, DateTime.Now);
                        r.Estatus[0].diasTrans = dt - 1;
                        if (estatus.Contains(r.Estatus[0].EstatusId))
                        {
                            r.Estatus[0].diasTotal += (dt - 1);
                        }
                    }
                }

                DateTime FechaF = DateTime.Now;
                DateTime FechaI = DateTime.Now;
                try
                {
                    if (fini != null)
                    {
                        FechaI = Convert.ToDateTime(fini);
                    }
                    if (ffin != null)
                    {
                        FechaF = Convert.ToDateTime(ffin);
                    }
                }
                catch (Exception error)
                {
                    string errorf = error.Message;
                }
                //  FechaF = FechaF.AddDays(1);
                
                var asig = vacantes.Select(e => e.Id).ToList();
                var requi = db.Requisiciones.Where(e => asig.Contains(e.Id)).ToList();
                
                var objeto = vacantes.Where(e => e.fch_Creacion >= FechaI
                    && e.fch_Creacion < FechaF.AddDays(1)).OrderByDescending(o => o.fch_Creacion).Select(e => new
                {
                    Id = e.Id,
                    Folio = e.Folio,
                    fch_Modificacion = requi.Where(x => x.Id == e.Id).Select(a => a.fch_Modificacion).FirstOrDefault(),
                    fch_Solicitud = e.fch_Creacion,
                    reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id)).Select(a =>
                        db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                                       ).ToList(),
                    sucursal = e.RazonSocial.ToUpper(),
                    Cliente = e.Nombrecomercial.ToUpper(),
                    estado = e.estado.ToUpper(),
                    domicilio_trabajo = e.domicilio_trabajo.ToUpper(),
                    Solicita = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(s => s.Nombre + " " + s.ApellidoPaterno).FirstOrDefault() != null ? db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper() : "SIN REGISTRO",
                    coordinador = db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno).FirstOrDefault() != null ? db.Usuarios.Where(x => x.Id.Equals(e.AprobadorId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper() : "SIN REGISTRO",
                    ClienteId = requi.Where(x => x.Id == e.Id).Select(a => a.ClienteId).FirstOrDefault(),
                    ClaseReclutamientoId = requi.Where(x => x.Id == e.Id).Select(a => a.ClaseReclutamientoId).FirstOrDefault(),
                    TipoReclutamientoId = requi.Where(x => x.Id == e.Id).Select(a => a.TipoReclutamientoId).FirstOrDefault(),
                    EstadoId = requi.Where(x => x.Id == e.Id).Select(a => a.Direccion.EstadoId).FirstOrDefault(),
                    Usuario = db.Usuarios.Where(x => x.Id == e.PropietarioId).FirstOrDefault().Usuario,
                    Vacantes = e.vacantes,
                    porcentaje = e.porcentaje,
                    EnProcesoEC = e.enProcesoEC,
                    EnProcesoFC = e.enProcesoFC,
                    contratados = e.contratados,
                    faltantes = e.faltante,
                    diasTrans = e.diasTrans,
                    VBtra = e.VBtra.ToUpper(),
                    SueldoMaximo = e.SueldoMaximo.ToString("c"),
                    Estatus = t.Where(x => x.RequisicionId.Equals(e.Id)).Select(E => E.Estatus).FirstOrDefault(),
                    EstatusId = requi.Where(a=>a.Id == e.Id).FirstOrDefault().EstatusId,
                    TipoReclutamiento = e.tipoReclutamiento.ToUpper(),
                    ClaseReclutamiento = e.clasesReclutamiento.ToUpper(),
                    comentarios_coord = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(e.Id) && x.ReclutadorId.Equals(e.AprobadorId)).Select(c =>
                          c.fch_Creacion + " " + c.Comentario.ToUpper()).ToList(),
                    comentarios_solicitante = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(e.Id) && x.ReclutadorId.Equals(e.PropietarioId) && !x.ReclutadorId.Equals(e.AprobadorId)).Select(c =>
                    c.fch_Creacion + " " + c.Comentario.ToUpper()).ToList(),
                    comentarios_reclutador = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(e.Id) && !x.ReclutadorId.Equals(e.AprobadorId) && !x.ReclutadorId.Equals(e.PropietarioId)).GroupBy(g => g.ReclutadorId).Select(c => new
                    {

                        reclutador = db.Usuarios.Where(x => x.Id.Equals(c.Key)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault().ToUpper(),
                        comentario = c.Select(cc => new
                        {
                            fch_Creacion = cc.fch_Creacion,
                            comentario = cc.Comentario.ToUpper()
                        }).ToList()
                    }).ToList()
                });
                
                
                if (stus != "0" && stus != null)
                {
                    var obj = stus.Split(',');
                    List<int> listaAreglo = new List<int>();
                    for (int i = 0; i < obj.Count() - 1; i++)
                    {
                        listaAreglo.Add(Convert.ToInt32(obj[i]));
                    }
                    var obb = listaAreglo.Where(e => e.Equals(0)).ToList();
                    if (obb.Count == 0)
                    {
                        objeto = objeto.Where(e => listaAreglo.Contains(e.EstatusId)).ToList();
                    }
                }

                if (clave != null)
                {
                    objeto = objeto.Where(e => e.VBtra.ToLower().Contains(clave.ToLower())).ToList();
                }

                if (sol != "0" && sol != null)
                {
                    var obj = sol.Split(',');
                    List<string> listaAreglo = new List<string>();
                    for (int i = 0; i < obj.Count() - 1; i++)
                    {
                        listaAreglo.Add(obj[i]);
                    }
                    var obb = listaAreglo.Where(e => e.Equals("0")).ToList();
                    if (obb.Count == 0)
                    {
                        objeto = objeto.Where(e => listaAreglo.Contains(e.Usuario)).ToList();
                    }
                }

                if (emp != "0" && emp != "00000000-0000-0000-0000-000000000000," && emp != null)
                {
                    var obj = emp.Split(',');
                    List<Guid> listaAreglo = new List<Guid>();
                    for (int i = 0; i < obj.Count() - 1; i++)
                    {
                        listaAreglo.Add(new Guid(obj[i]));
                    }
                    var obb = listaAreglo.Where(e => e.Equals(new Guid("00000000-0000-0000-0000-000000000000"))).ToList();
                    if (obb.Count == 0)
                    {
                        objeto = objeto.Where(e => listaAreglo.Contains(e.ClienteId)).ToList();
                    }
                }

                if (cor != "0" && cor != null)
                {
                    var obj = cor.Split(',');
                    List<int> listaAreglo = new List<int>();
                    for (int i = 0; i < obj.Count() - 1; i++)
                    {
                        listaAreglo.Add(Convert.ToInt32(obj[i]));
                    }
                    var obb = listaAreglo.Where(e => e.Equals(0)).ToList();
                    if (obb.Count == 0)
                    {
                        objeto = objeto.Where(e => listaAreglo.Contains(e.ClaseReclutamientoId)).ToList();
                    }
                }

                if (trcl != "0" && trcl != null)
                {
                    var obj = trcl.Split(',');
                    List<int> listaAreglo = new List<int>();
                    for (int i = 0; i < obj.Count() - 1; i++)
                    {
                        listaAreglo.Add(Convert.ToInt32(obj[i]));
                    }
                    var obb = listaAreglo.Where(e => e.Equals(0)).ToList();
                    if (obb.Count == 0)
                    {
                        objeto = objeto.Where(e => listaAreglo.Contains(e.TipoReclutamientoId)).ToList();
                    }
                }

                Guid id = new Guid(usuario);
                int usertipo = db.Usuarios.Where(e => e.Id == id).Select(e => e.TipoUsuarioId).FirstOrDefault();

                if (usertipo == 11)
                {
                    var asigna = db.AsignacionRequis.Where(e => e.GrpUsrId == id).Select(e => e.RequisicionId).ToList();
                    objeto = objeto.Where(e => asigna.Contains(e.Id)).ToList();
                }
                else
                {

                    if (recl != "0" && recl != "00000000-0000-0000-0000-000000000000," && recl != null)
                    {
                        var obj = recl.Split(',');
                        List<Guid> listaAreglo = new List<Guid>();
                        for (int i = 0; i < obj.Count() - 1; i++)
                        {
                            listaAreglo.Add(new Guid(obj[i]));
                        }
                        var obb = listaAreglo.Where(e => e.Equals(new Guid("00000000-0000-0000-0000-000000000000"))).ToList();
                        if (obb.Count == 0)
                        {
                            var asigna = db.AsignacionRequis.Where(e => listaAreglo.Contains(e.GrpUsrId)).Select(e => e.RequisicionId).ToList();
                            objeto = objeto.Where(e => asigna.Contains(e.Id)).ToList();
                        }
                    }
                }

                if (ofc != "0" && ofc != "0," && ofc != null)
                {
                    var obj = ofc.Split(',');
                    List<int> listaAreglo = new List<int>();
                    for (int i = 0; i < obj.Count() - 1; i++)
                    {
                        listaAreglo.Add(Convert.ToInt32(obj[i]));
                    }

                    var obb = listaAreglo.Where(e => e.Equals(0)).ToList();
                    if (obb.Count == 0)
                    {
                        // Estado = db.Direcciones.Where(x => x.EntidadId == db.Usuarios.Where(a => a.Usuario == e.Propietario).FirstOrDefault().SucursalId).FirstOrDefault().Estado.estado,
                        //int unidad = Convert.ToInt32(ofc);

                        var negocio = db.OficinasReclutamiento.Where(e => listaAreglo.Contains(e.UnidadNegocioId)).Select(e => e.Id).ToList();
                        var estado = db.Direcciones.Where(e => negocio.Contains(e.EntidadId)).Select(e => e.EstadoId).Distinct().ToList();
                        foreach (var item in listaAreglo)
                        {
                            string monterrey = "6,7,10,28,19,24";
                            string jalisco = "1,32,3,8,10,11,14,16,18,2,25,26";
                            string mexico = "4,5,9,13,15,22,27,30,20,12,31,23,21,29,17";
                            if (item == 1)
                            {
                                var array = jalisco.Split(',');
                                for (int i = 0; i < array.Length; i++)
                                {
                                    estado.Add(Int32.Parse(array[i]));
                                }
                            }
                            if (item == 2)
                            {
                                var array = mexico.Split(',');
                                for (int i = 0; i < array.Length; i++)
                                {
                                    estado.Add(Int32.Parse(array[i]));
                                }
                            }
                            if (item == 3)
                            {
                                var array = monterrey.Split(',');
                                for (int i = 0; i < array.Length; i++)
                                {
                                    estado.Add(Int32.Parse(array[i]));
                                }
                            }
                        }
                        estado = estado.Distinct().ToList();
                        objeto = objeto.Where(e => estado.Contains(e.EstadoId)).ToList();
                    }
                }
                return Ok(objeto);
            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("getConteoVacante")]
        [Authorize]
        public IHttpActionResult GetConteoVacante(Guid RequisicionId, Guid ClienteId)
        {
            try
            {
                var horarios = db.HorariosRequis.Where(x => x.RequisicionId.Equals(RequisicionId) && x.numeroVacantes > 0).Select(h => new
                {
                    Id = h.Id,
                    Vacantes = h.numeroVacantes,
                    Postulados = db.Postulaciones.Where(p => p.RequisicionId.Equals(RequisicionId) && p.StatusId.Equals(1)).Select(c => c.CandidatoId).Except(db.ProcesoCandidatos.Where(xx => xx.RequisicionId.Equals(RequisicionId) && xx.EstatusId.Equals(27) || xx.EstatusId.Equals(40)).Select(cc => cc.CandidatoId)).Count(),
                    Apartados = db.ProcesoCandidatos.Where(p => p.HorarioId.Equals(h.Id) && p.EstatusId.Equals(12)).Count(),
                    Abandono = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 26).Count(),
                    Descartados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 27).Count(),
                    EnProceso = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId != 27 && p.EstatusId != 40).Count(),
                    EnProcesoEnt = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 18).Count(),
                    EnProcesoFR = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 21).Count(),
                    EnProcesoFC = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 23).Count(),
                    contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 24).Count(),
                    Enviados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 21).Count(),
                    EntCliente = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 22).Count(),
                    rechazados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 40).Count(),
                    porcentaje = h.numeroVacantes > 0 ? (db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 24).Count()) * 100 / h.numeroVacantes : 0,
                    horario = h.Nombre + " de " + h.deHora.Hour + " a " + h.aHora.Hour,
                    totalVacantes = db.HorariosRequis.Where(x => x.RequisicionId.Equals(RequisicionId) && x.numeroVacantes > 0).Sum(v => v.numeroVacantes)
                }).ToList();

                return Ok(horarios);

            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }

        }

        [HttpGet]
        [Route("getInformeVacantes")]
        [Authorize]
        public IHttpActionResult GetInformeVacantes(Guid reclutadorId)
        {
            try
            {
                var tipo = db.Usuarios.Where(x => x.Id.Equals(reclutadorId)).Select(u => u.TipoUsuarioId).FirstOrDefault();
                if (tipo == 8)
                {
                    var informe = db.Requisiciones.OrderByDescending(f => f.fch_Cumplimiento)
                       .Where(e => e.Activo.Equals(true) && e.EstatusId != 9).Select(h => new
                       {
                           Id = h.Id,
                           Folio = h.Folio,
                           vBtra = h.VBtra.ToUpper(),
                           Estatus = h.Estatus.Descripcion.ToUpper(),
                           EstatusId = h.EstatusId,
                           cliente = h.Cliente.Nombrecomercial.ToUpper(),
                           Vacantes = h.horariosRequi.Count() > 0 ? h.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                           Fch_limite = h.fch_Cumplimiento,
                           Postulados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId.Equals(10)).Select(c => c.CandidatoId).Distinct().Count(),
                           Abandono = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 26).Count(),
                           Descartados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 27).Count(),
                           EnProceso = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId != 27 && p.EstatusId != 40 && p.EstatusId != 28).Count(),
                           entrevista = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 18).Count(),
                            //EnProcesoFR = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 21).Count(),
                            //EnProcesoFC = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 23).Count(),
                           contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 24).Count(),
                           Enviados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId.Equals(21)).Count(),
                            //EntCliente = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 22).Count(),
                            rechazados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 40).Count(),
                           porcentaje = h.horariosRequi.Count() > 0 && db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 24).Count() > 0 ? (db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 24).Count() * 100) / h.horariosRequi.Sum(s => s.numeroVacantes) : 0,
                       }).ToList();

                    return Ok(informe);
                }
                else
                {
                    List<Guid> uids = new List<Guid>();
                    if (db.Subordinados.Count(x => x.LiderId.Equals(reclutadorId)) > 0)
                    {
                        var ids = db.Subordinados.Where(x => !x.UsuarioId.Equals(reclutadorId) && x.LiderId.Equals(reclutadorId)).Select(u => u.UsuarioId).ToList();

                        uids = GetSub(ids, uids);

                    }
                    uids.Add(reclutadorId);

                    var asig = db.AsignacionRequis
                        .OrderByDescending(e => e.Id)
                        .Where(a => uids.Distinct().Contains(a.GrpUsrId))
                        .Select(a => a.RequisicionId)
                        .Distinct()
                        .ToList();

                    var informe = db.Requisiciones.OrderByDescending(f => f.fch_Cumplimiento).Where(e => asig.Contains(e.Id))
                        .Where(e => e.Activo.Equals(true) && e.EstatusId != 9).Select(h => new
                        {
                            Id = h.Id,
                            Folio = h.Folio,
                            vBtra = h.VBtra,
                            Estatus = h.Estatus.Descripcion,
                            EstatusId = h.EstatusId,
                            cliente = h.Cliente.Nombrecomercial,
                            Vacantes = h.horariosRequi.Count() > 0 ? h.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                            Fch_limite = h.fch_Cumplimiento,
                            Postulados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId.Equals(10)).Select(c => c.CandidatoId).Distinct().Count(),
                            Abandono = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 26).Count(),
                            Descartados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 27).Count(),
                            EnProceso = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId != 27 && p.EstatusId != 40 && p.EstatusId != 28 && p.EstatusId != 42).Count(),
                            entrevista = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 18).Count(),
                        //EnProcesoFR = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 21).Count(),
                        //EnProcesoFC = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 23).Count(),
                        contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 24).Count(),
                            Enviados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId.Equals(21)).Count(),
                        //EntCliente = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 22).Count(),
                        rechazados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 40).Count(),
                            porcentaje = h.horariosRequi.Count() > 0 && db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 24).Count() > 0 ? (db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 24).Count() * 100) / h.horariosRequi.Sum(s => s.numeroVacantes) : 0,
                        }).ToList();

                    return Ok(informe);
                }

            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }

        }

        [HttpGet]
        [Route("getInformeClientes")]
        // Seccion que olicita el trakin vacante. 
        public IHttpActionResult GetInformeClientes(string cc)
        {
            try
            {
                var clave = db.RelacionClientesSistemas.Where(x => x.Clave_Unica.Equals(cc)).Select(CC => CC.Id).ToList().Distinct();
                var asig = db.Requisiciones
                    .OrderByDescending(e => e.Id)
                    .Where(a => clave.Contains(a.ClienteId))
                    .Select(a => a.Id)
                    .Distinct()
                    .ToList();

                var informe = db.Requisiciones.OrderByDescending(f => f.fch_Cumplimiento).Where(e => asig.Contains(e.Id))
                    .Where(e => e.Activo.Equals(true) && e.EstatusId != 9).Select(h => new
                    {
                        Id = h.Id,
                        Folio = h.Folio,
                        vBtra = h.VBtra,
                        Estatus = h.Estatus.Descripcion,
                        EstatusId = h.EstatusId,
                        cliente = h.Cliente.Nombrecomercial,
                        Vacantes = h.horariosRequi.Count() > 0 ? h.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                        Fch_limite = h.fch_Cumplimiento,
                        Postulados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId.Equals(10)).Select(c => c.CandidatoId).Distinct().Count(),
                        Abandono = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 26).Count(),
                        Descartados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 27).Count(),
                        EnProceso = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId != 27 && p.EstatusId != 40 && p.EstatusId != 28).Count(),
                        entrevista = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 18).Count(),
                        //EnProcesoFR = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 21).Count(),
                        //EnProcesoFC = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 23).Count(),
                        contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 24).Count(),
                        Enviados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId.Equals(21)).Count(),
                        //EntCliente = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 22).Count(),
                        rechazados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 40).Count(),
                        porcentaje = (db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 24).Count()) > 0 ? (db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 24).Count()) * 100 / h.horariosRequi.Sum(s => s.numeroVacantes) : 0,
                    }).ToList();

                return Ok(informe);

            }
            catch (Exception ex)
            {
                string mensaje = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }

        }

        [HttpGet]
        [Route("getUltimoEstatus")]
        [Authorize]
        public IHttpActionResult GetUltimoEstatus(Guid RequisicionId)
        {
            try
            {
                var estatus = db.EstatusRequisiciones.OrderByDescending(x => x.fch_Modificacion).Where(x => x.RequisicionId.Equals(RequisicionId) && x.EstatusId != 39).Select(e => new
                {
                    e.EstatusId,
                    e.Estatus.Descripcion
                }).FirstOrDefault();

                return Ok(estatus);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("getDireccionRequisicon")]
        [Authorize]
        public IHttpActionResult GetDireccionRequisicon(Guid Id)
        {
            try
            {
                var direccion = db.Direcciones.Where(x => x.Id.Equals(Id))
                    .Select(d => new {
                        TipoDireccion = d.TipoDireccion.tipoDireccion,
                        Pais = d.Pais.pais,
                        Estado = d.Estado.estado,
                        Municipio = d.Municipio.municipio,
                        Colonia = d.Colonia.colonia,
                        CodigoPostal = d.Colonia.CP,
                        Calle = d.Calle,
                        NumeroExterior = d.NumeroExterior,
                        NumeroInterior = d.NumeroInterior != null ? d.NumeroInterior : "S/N",
                        Activo = d.Activo,
                        Principal = d.esPrincipal,
                        RutasCamion = db.RutasPerfil
                                        .Where(r => r.DireccionId.Equals(d.Id))
                                        .Select(r => new {
                                            Ruta = r.Ruta,
                                            Via = r.Via
                                        }).ToList()
                    })
                    .ToList();
                return Ok(direccion);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return NotFound();
            }
        }

        [HttpGet]
        [Route("getRutasCamion")]
        [Authorize]
        public IHttpActionResult GetRutasCamion(Guid Id)
        {
            try
            {
                var rutasCamion = db.RutasPerfil
                    .Where(r => r.DireccionId.Equals(Id))
                    .Select(x => new
                    {
                        Id = x.Id,
                        DireccionId = x.DireccionId,
                        Ruta = x.Ruta,
                        Via = x.Via
                    }).ToList();
                return Ok(rutasCamion);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("addRutaCamion")]
        [Authorize]
        public IHttpActionResult AddRutasCamion(RutaCamionDto ruta)
        {
            try
            {
                var rutaCamion = new RutasPerfil();
                rutaCamion.DireccionId = ruta.DireccionId;
                rutaCamion.Ruta = ruta.Ruta;
                rutaCamion.Via = ruta.Via;
                rutaCamion.UsuarioAlta = ruta.Usuario;
                db.RutasPerfil.Add(rutaCamion);
                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("updateRutaCamion")]
        [Authorize]
        public IHttpActionResult UpdateRutasCamion(RutaCamionDto ruta)
        {
            try
            {
                var rutaCamion = db.RutasPerfil.Find(ruta.Id);
                db.Entry(rutaCamion).State = EntityState.Modified;
                rutaCamion.Ruta = ruta.Ruta;
                rutaCamion.Via = ruta.Via;
                rutaCamion.UsuarioMod = ruta.Usuario;
                rutaCamion.fch_Modificacion = DateTime.Now;
                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("deleteRutaCamion")]
        [Authorize]
        public IHttpActionResult DeleteRutasCamion(RutaCamionDto ruta)
        {
            try
            {
                RutasPerfil rutaCamion = (RutasPerfil)db.RutasPerfil.Find(ruta.Id);
                db.RutasPerfil.Remove(rutaCamion);
                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("upadateVacantes")]
        [Authorize]
        public IHttpActionResult UpdateVacantes(HorariosRequi horario)
        {
            try
            {
                if(horario.numeroVacantes == 0)
                {
                    var vacante = db.HorariosRequis
                                    .Where(h => h.RequisicionId.Equals(horario.RequisicionId) && h.Id != horario.Id)
                                    .Select(h => new
                                    {
                                        vacantes = h.numeroVacantes
                                    }).ToList();
                    var suma = vacante.Count() > 0 ? vacante.Sum(s => s.vacantes) : 0;

                    if(suma == 0)
                    {
                        return Ok(HttpStatusCode.NoContent);
                    }
                    else
                    {
                        var hr = db.HorariosRequis.Find(horario.Id);
                        db.Entry(hr).State = EntityState.Modified;
                        hr.numeroVacantes = horario.numeroVacantes;
                        hr.UsuarioMod = horario.Usuario;
                        hr.fch_Modificacion = DateTime.Now;
                        db.SaveChanges();
                        return Ok(HttpStatusCode.OK);
                    }

                }else
                {
                    var hr = db.HorariosRequis.Find(horario.Id);
                    db.Entry(hr).State = EntityState.Modified;
                    hr.numeroVacantes = horario.numeroVacantes;
                    hr.UsuarioMod = horario.Usuario;
                    hr.fch_Modificacion = DateTime.Now;
                    db.SaveChanges();
                    return Ok(HttpStatusCode.OK);
                }
            }
            catch (Exception)
            {
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("getHorariosRequisicion")]
        [Authorize]
        public IHttpActionResult GetHorariosRequisicion(Guid Id)
        {
            try
            {
                var horarios = db.HorariosRequis
                        .Where(x => x.RequisicionId.Equals(Id))
                        .Select(x => new
                        {
                            Id = x.Id,
                            RequisicionId = x.RequisicionId,
                            Nombre = x.Nombre,
                            DeDia = x.deDia,
                            ADia = x.aDia,
                            deHora = x.deHora,
                            aHora = x.aHora,
                            numeroVacantes = x.numeroVacantes,
                            especificaciones = x.Especificaciones,
                            activo = x.Activo
                        })
                        .ToList();
                return Ok(horarios);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("getHorariosRequiConteo")]
        [Authorize]
        public IHttpActionResult GetHorariosRequiConteo(Guid requisicionId)
        {
            try
            {
                var horarios = db.HorariosRequis.Where(x => x.RequisicionId.Equals(requisicionId) && x.numeroVacantes > 0).Select(h => new
                {
                    id = h.Id,
                    nombre = h.Nombre + " de " + h.deHora.Hour + " a " + h.aHora.Hour,
                    deHora = h.deHora.Hour > 12 ? h.deHora.Hour + ":00 pm" : h.deHora.Hour + ":00 am",
                    aHora = h.aHora.Hour > 12 ? h.deHora.Hour + ":00 " : h.deHora.Hour + ":00 pm",
                    vacantes = h.numeroVacantes == db.ProcesoCandidatos.Where(x => x.HorarioId.Equals(h.Id) && x.EstatusId.Equals(24)).Count() ? true : false
                }).ToList();

                return Ok(horarios);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("updateRequisiciones")]
        [Authorize]
        public IHttpActionResult UpdateRequi(RequisicionDto requi)
        {
            db.Database.Log = Console.Write;
            List<CoincidenciasDto> CandidatosFiltro = new List<CoincidenciasDto>();
            using (DbContextTransaction beginTran = db.Database.BeginTransaction())
            {
                try
                {
                    var requisicion = db.Requisiciones.Find(requi.Id);

                    // Requisicon a coincidir.
                    var Requi = db.Requisiciones
                             .Where(r => r.Folio == requisicion.Folio)
                             .Select(r => new
                             {
                                 Categoria = r.Area.Id,
                                 SalarioMinimo = r.SueldoMinimo,
                                 SalarioMaximo = r.SueldoMaximo,
                                 Genero = r.GeneroId,
                                 EdadMinima = r.EdadMinima,
                                 EdadMaxima = r.EdadMaxima,
                                 EstadoCivil = r.EstadoCivilId,
                                 Escolaridades = r.escolaridadesRequi.Select(e => e.Escolaridad.Id).ToList()
                             })
                            .FirstOrDefault();
                    db.Entry(requisicion).State = EntityState.Modified;

                    requisicion.fch_Cumplimiento = requi.fch_Cumplimiento;
                    requisicion.PrioridadId = requi.PrioridadId;
                    requisicion.Confidencial = requi.Confidencial;
                    requisicion.fch_Modificacion = DateTime.Now;
                    requisicion.UsuarioMod = requi.Usuario;
                    if (requi.AsignacionRequi.Count() > 1)
                    {
                        requisicion.Asignada = true;
                        // Candidatos con todas las coincidencias.
                        var Candidatos = db.PerfilCandidato
                            .Select(c => new CoincidenciasDto
                            {
                                Nombre = c.Candidato.Nombre + " " + c.Candidato.ApellidoPaterno + " " + c.Candidato.ApellidoMaterno,
                                Subcategoria = c.AboutMe.Select(a => a.AreaInteres.areaInteres).FirstOrDefault(),
                                AreaExpId = c.AboutMe.Select(a => a.AreaExperienciaId).Count() > 0 ? c.AboutMe.Select(a => a.AreaExperienciaId).FirstOrDefault() : 0,
                                SueldoMinimo = c.AboutMe.Select(a => a.SalarioAceptable).FirstOrDefault(),
                                SueldoMaximo = c.AboutMe.Select(a => a.SalarioDeseado).FirstOrDefault(),
                                Genero = c.Candidato.Genero.genero,
                                GeneroId = c.Candidato.GeneroId,
                                EstadoCivil = c.Candidato.EstadoCivil.estadoCivil,
                                EstadoCivilId = c.Candidato.EstadoCivilId.Value > 0 ? c.Candidato.EstadoCivilId.Value : 0,
                                FormacionId = c.Formaciones.Select(x => x.GradoEstudioId).FirstOrDefault(),
                                Formaciones = c.Formaciones.Select(x => x.GradosEstudio.gradoEstudio).FirstOrDefault(),
                                Edad = DateTime.Now.Year - c.Candidato.FechaNacimiento.Value.Year
                            })
                            .ToList();

                        CandidatosFiltro = Candidatos
                                    .Where(c => c.AreaExpId == Requi.Categoria).ToList();

                        CandidatosFiltro = CandidatosFiltro
                               .Where(c => Requi.Escolaridades.Contains(c.FormacionId))
                               .ToList();

                        CandidatosFiltro = CandidatosFiltro
                            .Where(c => c.SueldoMinimo >= Requi.SalarioMinimo && c.SueldoMinimo <= Requi.SalarioMaximo
                                     || c.SueldoMaximo >= Requi.SalarioMinimo && c.SueldoMaximo <= Requi.SalarioMaximo)
                            .ToList();

                        CandidatosFiltro = CandidatosFiltro
                            .Where(c => c.Edad >= Requi.EdadMinima && c.Edad <= Requi.EdadMaxima)
                            .ToList();

                        if (Requi.Genero > 0)
                        {
                            CandidatosFiltro = CandidatosFiltro.Where(c => c.GeneroId.Equals(Requi.Genero)).ToList();
                        }

                        if (Requi.EstadoCivil > 0)
                        {
                            CandidatosFiltro = CandidatosFiltro.Where(c => c.EstadoCivilId == Requi.EstadoCivil).ToList();
                        }
                    }
                    else
                    {
                        requisicion.Asignada = false;
                    }
                    if (requisicion.Confidencial)
                    {
                        requisicion.Publicado = false;
                    }
                    else
                    {
                        requisicion.Publicado = true;
                        UpdatePublicarDto UpDto = new UpdatePublicarDto();
                        UpDto.ListaPublicar = null;
                        UpDto.RequiId = requisicion.Id.ToString();
                        Dvc.PublicarVacante(UpDto);

                    }
                    db.SaveChanges();
                    AlterAsignacionRequi(requi.AsignacionRequi, requi.Id, requi.Folio, requi.Usuario, requisicion.VBtra, CandidatosFiltro);

                    Int64 Folio = requisicion.Folio;
                    //Creacion de Trazabalidad par ala requisición.
                    Guid trazabilidadId = db.TrazabilidadesMes.Where(x => x.Folio.Equals(Folio)).Select(x => x.Id).FirstOrDefault();
                    //Isertar el registro de la rastreabilidad. 
                    rastreabilidad.RastreabilidadInsert(trazabilidadId, requi.Usuario, 3);

                    beginTran.Commit();

                    return Ok(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    beginTran.Rollback();
                    Console.Write(ex.Message);
                    return NotFound();
                }
            }


        }

        [HttpPost]
        [Route("deleteRequisiciones")]
        [Authorize]
        public IHttpActionResult DeleteRequi(RequisicionDeleteDto requi)
        {
            try
            {
                var asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(requi.Id)).ToList();

                var requisicion = db.Requisiciones.Find(requi.Id);
                db.Entry(requisicion).State = EntityState.Modified;
                requisicion.Activo = false;
                requisicion.UsuarioMod = requi.UsuarioMod;
                requisicion.fch_Modificacion = DateTime.Now;
                requisicion.EstatusId = 9;
                if (requisicion.Publicado)
                    requisicion.Publicado = false;

                Int64 Folio = requisicion.Folio;
                string VBra = requisicion.VBtra;
                Guid trazabilidadId = db.TrazabilidadesMes.Where(x => x.Folio.Equals(Folio)).Select(x => x.Id).FirstOrDefault();
                //Isertar el registro de la rastreabilidad. 
                rastreabilidad.RastreabilidadInsert(trazabilidadId, requi.UsuarioMod, 4);

                SendEmail.ConstructEmail(asignados, null, "RD", Folio, string.Empty, VBra, null);

                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch
            {
                return Ok(HttpStatusCode.NotAcceptable);
            }
        }

        [HttpPost]
        [Route("cancelRequisiciones")]
        [Authorize]
        public IHttpActionResult CancelRequi(RequisicionDeleteDto requi)
        {
            try
            {
                var asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(requi.Id)).ToList();

                var requisicion = db.Requisiciones.Find(requi.Id);
                db.Entry(requisicion).State = EntityState.Modified;
                requisicion.EstatusId = 8;
                requisicion.Aprobada = false;
                requisicion.Aprobador = string.Empty;
                requisicion.UsuarioMod = requi.UsuarioMod;
                requisicion.fch_Modificacion = DateTime.Now;
                if (requisicion.Publicado)
                    requisicion.Publicado = false;



                Int64 Folio = requisicion.Folio;
                string VBra = requisicion.VBtra;

                Guid trazabilidadId = db.TrazabilidadesMes.Where(x => x.Folio.Equals(Folio)).Select(x => x.Id).FirstOrDefault();
                //Isertar el registro de la rastreabilidad. 
                rastreabilidad.RastreabilidadInsert(trazabilidadId, requi.UsuarioMod, 6);
                
                SendEmail.ConstructEmail(asignados, null, "RU", Folio, string.Empty, VBra, null);

                db.SaveChanges();

                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotAcceptable);
            }
        }

        [HttpPost]
        [Route("asignacionRequisiciones")]
        [Authorize]
        public IHttpActionResult AsginarRequi(AsignarVacanteReclutador requi)
        {
            //db.Database.Log = Console.Write;
            using (DbContextTransaction beginTran = db.Database.BeginTransaction())
            {
                try
                {
                    var requisicion = db.Requisiciones.Find(requi.Id);

                    // Requisicion a coincidir.
                    var Requi = db.Requisiciones
                             .Where(r => r.Folio == requisicion.Folio)
                             .Select(r => new RequisicionCoin
                             {
                                 Categoria = r.Area.Id,
                                 CategoriaDesc = r.Area.areaExperiencia,
                                 SalarioMinimo = r.SueldoMinimo,
                                 SalarioMaximo = r.SueldoMaximo,
                                 Genero = r.GeneroId,
                                 GeneroDesc = r.Genero.genero,
                                 EdadMinima = r.EdadMinima,
                                 EdadMaxima = r.EdadMaxima,
                                 EstadoCivil = r.EstadoCivilId,
                                 EstadoCivilDesc = r.EstadoCivil.estadoCivil,
                                 Escolaridades = r.escolaridadesRequi.Select(e => e.Escolaridad.Id).ToList(),
                                 EscolaridadesDesc = r.escolaridadesRequi.Select(e => e.Escolaridad.gradoEstudio).ToList()
                             })
                            .FirstOrDefault();

                    // Candidatos Activos
                    var Activos = db.AspNetUsers
                        .Where(c => c.Activo == 0)
                        .OrderBy(c => c.UltimoInicio)
                        .Select(c => c.IdPersona)
                        .ToList();

                    // Candidatos con todas las coincidencias.
                //    var Candidatos = db.PerfilCandidato
                //        .Where(c => Activos.Contains(c.CandidatoId))
                //        .Select(c => new CoincidenciasDto
                //        {
                //            Nombre = c.Candidato.Nombre + " " + c.Candidato.ApellidoPaterno + " " + (c.Candidato.ApellidoMaterno != null ? c.Candidato.ApellidoMaterno : ""),
                //            Subcategoria = db.AreasInteres.Where(a => a.Id.Equals(c.AboutMe.Select(x => x.AreaExperienciaId).FirstOrDefault())).Select(s => s.AreaExperienciaId).FirstOrDefault() != 0 ? 
                //                           db.AreasInteres.Where(x => x.Id.Equals(c.AboutMe.Select(a => a.AreaExperienciaId).FirstOrDefault())).Select(s => s.areaInteres).FirstOrDefault() : "",
                //            AreaExpId = db.AreasInteres.Where(a => a.Id.Equals(c.AboutMe.Select(x => x.AreaExperienciaId).FirstOrDefault())).Select(s => s.AreaExperienciaId).FirstOrDefault() != 0 ?
                //                        db.AreasInteres.Where(a => a.Id.Equals(c.AboutMe.Select(x => x.AreaExperienciaId).FirstOrDefault())).Select(s => s.AreaExperienciaId).FirstOrDefault() : 0,
                //            //Subcategoria = c.AboutMe.Select(a => a.AreaInteres.areaInteres).FirstOrDefault() != null ? c.AboutMe.Select(a => a.AreaInteres.areaInteres).FirstOrDefault() : "",
                //            //AreaExpId = c.AboutMe.Select(a => a.AreaInteres.AreaExperienciaId).FirstOrDefault() != 0 ? c.AboutMe.Select(a => a.AreaInteres.AreaExperienciaId).FirstOrDefault() : 0,
                //            SueldoMinimo = c.AboutMe.Select(a => a.SalarioAceptable).FirstOrDefault() > 0 ? c.AboutMe.Select(a => a.SalarioAceptable).FirstOrDefault() : 0,
                //            SueldoMaximo = c.AboutMe.Select(a => a.SalarioDeseado).FirstOrDefault() > 0 ? c.AboutMe.Select(a => a.SalarioDeseado).FirstOrDefault() : 0,
                //            Genero = c.Candidato.Genero.genero,
                //            GeneroId = c.Candidato.GeneroId > 0 ? c.Candidato.GeneroId : 0,
                //            EstadoCivil = c.Candidato.EstadoCivil.estadoCivil,
                //            EstadoCivilId = c.Candidato.EstadoCivilId.Value > 0 ? c.Candidato.EstadoCivilId.Value : 0,
                //            FormacionId = c.Formaciones.Select(x => x.GradoEstudioId).FirstOrDefault(),
                //            Formaciones = c.Formaciones.Select(x => x.GradosEstudio.gradoEstudio).FirstOrDefault(),
                //            Edad = DateTime.Now.Year - c.Candidato.FechaNacimiento.Value.Year,
                //            Requisicion = db.Requisiciones
                //             .Where(r => r.Folio == requisicion.Folio)
                //             .Select(r => new RequisicionCoin
                //             {
                //                 Categoria = r.Area.Id,
                //                 CategoriaDesc = r.Area.areaExperiencia,
                //                 SalarioMinimo = r.SueldoMinimo,
                //                 SalarioMaximo = r.SueldoMaximo,
                //                 Genero = r.GeneroId,
                //                 GeneroDesc = r.Genero.genero,
                //                 EdadMinima = r.EdadMinima,
                //                 EdadMaxima = r.EdadMaxima,
                //                 EstadoCivil = r.EstadoCivilId,
                //                 EstadoCivilDesc = r.EstadoCivil.estadoCivil,
                //                 Escolaridades = r.escolaridadesRequi.Select(e => e.Escolaridad.Id).ToList(),
                //                 EscolaridadesDesc = r.escolaridadesRequi.Select(e => e.Escolaridad.gradoEstudio).ToList()
                //             })
                //            .FirstOrDefault()
                //})
                //        .ToList();

                //    List<CoincidenciasDto> CandidatosFiltro = new List<CoincidenciasDto>();

                //    CandidatosFiltro = Candidatos
                //        .Where(c => c.AreaExpId == Requi.Categoria)
                //        .ToList();

                //    CandidatosFiltro = CandidatosFiltro
                //           .Where(c => Requi.Escolaridades.Contains(c.FormacionId))
                //           .ToList();

                //    CandidatosFiltro = CandidatosFiltro
                //        .Where(c => c.SueldoMinimo >= Requi.SalarioMinimo && c.SueldoMinimo <= Requi.SalarioMaximo 
                //                 || c.SueldoMaximo >= Requi.SalarioMinimo && c.SueldoMaximo <= Requi.SalarioMaximo)
                //        .ToList();

                //    CandidatosFiltro = CandidatosFiltro
                //        .Where(c => c.Edad >= Requi.EdadMinima && c.Edad <= Requi.EdadMaxima)
                //        .ToList();

                    //if (Requi.Genero > 0)
                    //{
                    //   CandidatosFiltro = CandidatosFiltro.Where(c => c.GeneroId.Equals(Requi.Genero)).ToList();
                    //}

                    //if (Requi.EstadoCivil > 0)
                    //{
                    //    CandidatosFiltro =  CandidatosFiltro.Where(c => c.EstadoCivilId == Requi.EstadoCivil).ToList();
                    //}

                    //// Solo tomamos el top 5 de candidatos que coinciderion.
                    //CandidatosFiltro = CandidatosFiltro.Take(5).ToList();

                    db.Entry(requisicion).State = EntityState.Modified;
                    requisicion.fch_Cumplimiento = requi.fch_Cumplimiento;
                    if (requisicion.EstatusId == 4 || requisicion.EstatusId == 46)
                    {
                        requisicion.EstatusId = 6;
                        requisicion.Aprobador = requi.Usuario;
                        requisicion.AprobadorId = requi.AprobadorId;
                        requisicion.Aprobada = true;
                        requisicion.fch_Aprobacion = DateTime.Now;
                        
                    }
                    else
                    {
                        db.Entry(requisicion).Property(x => x.EstatusId).IsModified = false;
                        db.Entry(requisicion).Property(x => x.Aprobador).IsModified = false;
                        db.Entry(requisicion).Property(x => x.AprobadorId).IsModified = false;
                        db.Entry(requisicion).Property(x => x.Aprobada).IsModified = false;
                        db.Entry(requisicion).Property(x => x.fch_Aprobacion).IsModified = false;
                    }
                    if (requi.AprobadorId == requisicion.AprobadorId)
                    {
                        requisicion.DiasEnvio = requi.DiasEnvio;
                        requisicion.fch_Modificacion = DateTime.Now;
                        requisicion.UsuarioMod = requi.Usuario;
                        if (requi.AsignacionRequi.ToList().Count() > 1)
                            requisicion.Asignada = true;
                        else
                            requisicion.Asignada = false;
                    }
                    else
                    {
                        return Ok(HttpStatusCode.Ambiguous);
                    }

                    if(requi.Ponderacion.Id.ToString() == "00000000-0000-0000-0000-000000000000")
                    {
                        PonderacionRequisiciones pon = new PonderacionRequisiciones();
                        pon.Ponderacion = requi.Ponderacion.Ponderacion;
                        pon.RequisicionId = requi.Id;
                        pon.fch_Creacion = DateTime.Now;
                        pon.fch_Modificacion = DateTime.Now;
                        db.PonderacionRequisiciones.Add(pon);
                    }
                    else if(requi.AprobadorId == requisicion.AprobadorId)
                    {
                        var pon = db.PonderacionRequisiciones.Find(requi.Ponderacion.Id);
                        db.Entry(pon).State = EntityState.Modified;
                        db.Entry(pon).Property(x => x.RequisicionId).IsModified = false;
                        db.Entry(pon).Property(x => x.fch_Creacion).IsModified = false;
                        pon.Ponderacion = requi.Ponderacion.Ponderacion;
                        pon.fch_Modificacion = DateTime.Now;
                    }
                    db.SaveChanges();
                    AlterAsignacionRequi(requi.AsignacionRequi, requi.Id, requisicion.Folio, requi.Usuario, requisicion.VBtra, null);    
                    db.SaveChanges();
                    Int64 Folio = requisicion.Folio;
                    //Creacion de Trazabalidad par ala requisición.
                    Guid trazabilidadId = db.TrazabilidadesMes.Where(x => x.Folio.Equals(Folio)).Select(x => x.Id).FirstOrDefault();
                    //Isertar el registro de la rastreabilidad. 
                    rastreabilidad.RastreabilidadInsert(trazabilidadId, requi.Usuario, 5);


                    beginTran.Commit();

                    return Ok(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    beginTran.Rollback();
                    Console.Write(ex.Message);
                    return Ok(HttpStatusCode.NotFound);
                }
            }


        }

        private void Save()
        {
            bool saveFailed;
            do
            {
                saveFailed = false;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;
                    // Update the values of the entity that failed to save from the store 
                    ex.Entries.Single().Reload();
                }

            } while (saveFailed);
        }

        public void AlterAsignacionRequi(List<AsignacionRequi> asignaciones, Guid RequiId, Int64 Folio, string Usuario, string VBra, List<CoincidenciasDto> Coincidencias)
        {
            var user = db.Usuarios.Where(x => x.Usuario.Equals(Usuario)).Select(x =>
               x.Nombre + " " + x.ApellidoPaterno + " " + x.ApellidoMaterno
            ).FirstOrDefault();

            List<AsignacionRequi> NotChange = new List<AsignacionRequi>();
            List<AsignacionRequi> CheckExcept = new List<AsignacionRequi>();
            List<AsignacionRequi> AddElmt = new List<AsignacionRequi>();
            List<AsignacionRequi> Delete = new List<AsignacionRequi>();
            var asg = db.AsignacionRequis
                .Where(x => x.RequisicionId.Equals(RequiId))
                .ToList();
            if (asg.Count() > 0)
            {
                for (int i = 0; i < asg.Count(); i++)
                {
                    if (asignaciones.Count() > 0)
                    {
                        for (int x = 0; x < asignaciones.Count(); x++)
                        {
                            if (asignaciones[x].GrpUsrId.Equals(asg[i].GrpUsrId))
                            {
                                NotChange.Add(asignaciones[x]);
                                CheckExcept.Add(asg[i]);

                            }
                            if (asignaciones[x].GrpUsrId != asg[i].GrpUsrId)
                            {
                                AddElmt.Add(asignaciones[x]);

                            }
                        }
                    }
                }

                var filterAdd = AddElmt.Except(NotChange).ToList();
                var delet = asg.Except(CheckExcept).ToList();

                if (delet.Count() > 0)
                {
                    db.AsignacionRequis.RemoveRange(delet);
                    SendEmail.ConstructEmail(delet, NotChange, "D", Folio, user, VBra, Coincidencias);
                }
                if (filterAdd.Count() > 0)
                {
                    db.AsignacionRequis.AddRange(filterAdd);
                    SendEmail.ConstructEmail(filterAdd, NotChange, "C", Folio, user, VBra, Coincidencias);
                }
            }
            else
            {
                db.AsignacionRequis.AddRange(asignaciones);
                SendEmail.ConstructEmail(asignaciones, NotChange, "C", Folio, user, VBra, Coincidencias);
            }
            db.SaveChanges();

        }

        public List<Guid> GetGrupo(Guid grupo, List<Guid> listaIds)
        {
            if (!listaIds.Contains(grupo))
            {
                listaIds.Add(grupo);
                var listadoNuevo = db.GruposUsuarios
                    .Where(g => g.EntidadId.Equals(grupo) & g.Grupo.Activo)
                           .Select(g => g.GrupoId)
                           .ToList();
                foreach (Guid g in listadoNuevo)
                {
                    var gp = db.GruposUsuarios
                        .Where(x => x.EntidadId.Equals(g))
                        .Select(x => x.GrupoId)
                        .ToList();
                    foreach (Guid gr in gp)
                    {
                        GetGrupo(gr, listaIds);
                    }
                }
            }
            return listaIds;
        }

        public List<Guid> GetSub(List<Guid> uid, List<Guid> listaIds)
        {
            foreach(var u in uid)
            {
                listaIds.Add(u);
                var listadoNuevo = db.Subordinados
                  .Where(g => g.LiderId.Equals(u))
                         .Select(g => g.UsuarioId)
                         .ToList();

                GetSub(listadoNuevo, listaIds);

            }
            //if (!listaIds.Contains(uid))
            //{
            //    listaIds.Add(uid);
            //    var listadoNuevo = db.Subordinados
            //        .Where(g => g.LiderId.Equals(uid))
            //               .Select(g => g.UsuarioId)
            //               .ToList();
            //    foreach (Guid g in listadoNuevo)
            //    {
            //        var gp = db.Subordinados
            //            .Where(x => x.LiderId.Equals(g))
            //            .Select(x => x.UsuarioId)
            //            .ToList();
            //        foreach (Guid gr in gp)
            //        {
            //            GetSub(gr, listaIds);
            //        }
            //    }
            //}
            return listaIds;
        }
        [HttpGet]
        [Route("execProcedurePause")]
        public IHttpActionResult ExecProcedurePause()
        {
            int[] estatus = { 8, 9, 34, 35, 36, 37 };
            try
            {
                string from = "noreply@damsa.com.mx";
                MailMessage m = new MailMessage();
                m.From = new MailAddress(from, "SAGA INN");
                m.Subject = "Vacantes en Pausa";

                var datos = db.Database.SqlQuery<PausadasDto>("dbo.sp_RequisPausadas").ToList();


                if (datos.Count > 0)
                {
                    var aprobadores = datos.Select(x => x.aprobadorId).Distinct().ToList();

                    var inicio = "<html><head><style>td {border: solid black 1px;padding-left:5px;padding-right:5px;padding-top:1px;padding-bottom:1px;font-size:9pt;color:Black;font-family:'calibri';} " +
                                 "</style></head><body style=\"text-align:center; font-family:'calibri'; font-size:10pt;\"><table class='table'><tr><th align=center>DIAS SIN MODIFICAR</th><th align=center>FOLIO</th><th align=center>PERFIL</th><th align=center>FECHA ALTA</th><th align=center>FECHA CUMPLIMIENTO</th><th align=center>CLIENTE</th><th align=center>SOLICITA</th><th align=center>COORDINADOR</th><th align=center>CUB/VAC</th><th align=center>ESTATUS</th><th align=center>CAMBIO DE ESTATUS</th><th align=center>ASIGNADA A</th></tr>";

                    var body = "";
                    string reclutadoresList = "";
                    foreach (var a in aprobadores)
                    {
                    
                        var aux = datos.Where(x => x.aprobadorId.Equals(a)).ToList();
                        var email = aux[0].email;
                        var emailSol = aux[0].emailSol;
                        m.To.Add(email);
                        m.Bcc.Add(emailSol);
                        m.Bcc.Add("idelatorre@damsa.com.mx");
                        m.Bcc.Add("mventura@damsa.com.mx");
                        m.Bcc.Add("bmorales@damsa.com.mx");
                        foreach (var r in aux)
                        {
                            var reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(r.Id) && !x.GrpUsrId.Equals(r.aprobadorId)).Select(rec => new
                            {
                                nombre = db.Usuarios.Where(x => x.Id.Equals(rec.GrpUsrId)).Select(N => N.Nombre + " " + N.ApellidoPaterno + " " + N.ApellidoMaterno).FirstOrDefault()
                            }).ToList();

                            if (reclutadores.Count() > 1)
                            {
                                reclutadoresList = "<ul>";
                                foreach (var rs in reclutadores)
                                {
                                    reclutadoresList = reclutadoresList + "<li>" + rs.nombre + "</li>";
                                }
                                reclutadoresList = reclutadoresList + "</ul>";
                            }
                            else if (reclutadores.Count() == 1)
                            {
                                reclutadoresList = reclutadores[0].nombre;
                            }
                            else
                            {
                                reclutadoresList = "SIN ASIGNAR";
                            }

                            body = body + string.Format("<tr><td align=center>{0}</td><td align=center>{1}</td><td align=center>{2}</td><td align=center>{3}</td><td align=center>{4}</td>" +
                                                       "<td align=center>{5}</td><td align=center>{6}</td><td align=center>{7}</td><td align=center>{8}</td><td align=center>{9}</td><td align=center>{10}</td><td>{11}</td></tr>",
                                                       r.dias, r.Folio, r.VBtra, r.fch_Aprobacion, r.fch_Cumplimiento, r.Cliente, r.solicitante, r.aprobador, r.cubiertas.ToString() + "/" + r.vacantes.ToString(), r.estatus, r.fch_Modificacion, reclutadoresList);
                        }

                        body = inicio + body + "</table></body></html><br/>";
                        m.Body = body;
                        m.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                        smtp.EnableSsl = true;
                        smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                        smtp.Send(m);

                        body = "";

                        m.To.Clear();
                        m.Bcc.Clear();

                    }
                }
                return Ok(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpGet]
        [Route("execProcedureSinCambio")]
        public IHttpActionResult ExecProcedureSinCambio()
        {
            try
            {
                string from = "noreply@damsa.com.mx";
                MailMessage m = new MailMessage();
                m.From = new MailAddress(from, "SAGA INN");
                m.Subject = "Requisiciones sin cambio de estatus";

                var datos = db.Database.SqlQuery<PausadasDto>("dbo.sp_RequisSinCambio").ToList();


                if (datos.Count > 0)
                {
                    var aprobadores = datos.Select(x => x.aprobadorId).Distinct().ToList();

                    var inicio = "<html><head><style>td {border: solid black 1px;padding-left:5px;padding-right:5px;padding-top:1px;padding-bottom:1px;font-size:9pt;color:Black;font-family:'calibri';} " +
                                                "</style></head><body style=\"text-align:center; font-family:'calibri'; font-size:10pt;\"><table class='table'><tr><th align=center>DIAS SIN MODIFICAR</th><th align=center>FOLIO</th><th align=center>PERFIL</th><th align=center>FECHA ALTA</th><th align=center>FECHA CUMPLIMIENTO</th><th align=center>CLIENTE</th><th align=center>SOLICITA</th><th align=center>COORDINADOR</th><th align=center>CUB/VAC</th><th align=center>ESTATUS</th><th align=center>CAMBIO DE ESTATUS</th><th>ASIGNADA A</th></tr>";

                    var body = "";
                    string reclutadoresList = "";
                    foreach (var a in aprobadores)
                    {
                        var aux = datos.Where(x => x.aprobadorId.Equals(a)).ToList();

                        m.To.Add(aux[0].email);
                        m.Bcc.Add(aux[0].emailSol);
                        m.Bcc.Add("idelatorre@damsa.com.mx");
                        m.Bcc.Add("mventura@damsa.com.mx");
                        m.Bcc.Add("bmorales@damsa.com.mx");
                        foreach (var r in aux)
                        {
                            var reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(r.Id) && !x.GrpUsrId.Equals(r.aprobadorId)).Select(rec => new
                            {
                                nombre = db.Usuarios.Where(x => x.Id.Equals(rec.GrpUsrId)).Select(N => N.Nombre + " " + N.ApellidoPaterno + " " + N.ApellidoMaterno).FirstOrDefault()
                            }).ToList();

                            if(reclutadores.Count() > 1)
                            {
                                reclutadoresList = reclutadoresList + "<ul>";
                                foreach (var rs in reclutadores)
                                {
                                    reclutadoresList = reclutadoresList + "<li>" + rs.nombre + "</li>";
                                }
                                reclutadoresList = "</ul>";
                            }
                            else if(reclutadores.Count() == 1)
                            {
                                reclutadoresList = reclutadores[0].nombre;
                            }
                            else
                            {
                                reclutadoresList = "SIN ASIGNAR";
                            }

                            body = body + string.Format("<tr><td align=center>{0}</td><td align=center>{1}</td><td align=center>{2}</td><td align=center>{3}</td><td align=center>{4}</td>" +
                                                       "<td align=center>{5}</td><td align=center>{6}</td><td align=center>{7}</td><td align=center>{8}</td><td align=center>{9}</td><td align=center>{10}</td><td align=center>{11}</td></tr>",
                                                       r.dias, r.Folio, r.VBtra, r.fch_Aprobacion, r.fch_Cumplimiento, r.Cliente, r.solicitante, r.aprobador, r.cubiertas.ToString() + "/" + r.vacantes.ToString(), r.estatus, r.fch_Modificacion, reclutadoresList);
                        }

                        body = inicio + body + "</table><p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
                        body = body + "</body></html>";
                        m.Body = body;
                        m.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                        smtp.EnableSsl = true;
                        smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                        smtp.Send(m);

                        body = "";

                        m.To.Clear();
                        m.Bcc.Clear();

                    }

                }
                return Ok(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }
        [HttpGet]
        [Route("execProcedurePendientesPuro")]
        public IHttpActionResult ExecProcedurePendientesPuro()
        {
            try
            {
                string from = "noreply@damsa.com.mx";
                MailMessage m = new MailMessage();
                m.From = new MailAddress(from, "SAGA INN");
                m.Subject = "Requisiciones pendientes autorizar - Reclutamiento Puro";

                var datos = db.Database.SqlQuery<PausadasDto>("dbo.sp_RequisPuroPendientes").ToList();
            
                m.To.Add("idelatorre@damsa.com.mx");
                m.Bcc.Add("mventura@damsa.com.mx");
                m.Bcc.Add("bmorales@damsa.com.mx");

                if (datos.Count > 0)
                {
                    var inicio = "<html><head><style>td {border: solid black 1px;padding-left:5px;padding-right:5px;padding-top:1px;padding-bottom:1px;font-size:9pt;color:Black;font-family:'calibri';} " +
                                                "</style></head><body style=\"text-align:center; font-family:'calibri'; font-size:10pt;\"><table class='table'><tr><th align=center>DIAS SIN MODIFICAR</th><th align=center>FOLIO</th><th align=center>PERFIL</th><th align=center>FECHA CREACI&Oacute;N</th><th align=center>FECHA CUMPLIMIENTO</th><th align=center>CLIENTE</th><th align=center>SOLICITA</th><th align=center>ESTATUS</th><th align=center>CAMBIO DE ESTATUS</th></tr>";

                    var body = "";
                    foreach (var r in datos)
                    {
                        body = body + string.Format("<tr><td align=center>{0}</td><td align=center>{1}</td><td align=center>{2}</td><td align=center>{3}</td><td align=center>{4}</td>" +
                                                       "<td align=center>{5}</td><td align=center>{6}</td><td align=center>{7}</td><td align=center>{8}</td></tr>",
                                                       r.dias, r.Folio, r.VBtra, r.fch_Creacion, r.fch_Cumplimiento, r.Cliente, r.solicitante, r.estatus, r.fch_Modificacion);
                    }

                    body = inicio + body + "</table><p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
                    body = body + "</body></html>";
                    m.Body = body;
                    m.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                    smtp.EnableSsl = true;
                    smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                    smtp.Send(m);

                    body = "";

                    m.To.Clear();
                    m.Bcc.Clear();
                }
                return Ok(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }
        [HttpGet]
        [Route("execProcedureSinAsignar")]
        public IHttpActionResult ExecProcedureSinAignar()
        {
            try
            {
                string from = "noreply@damsa.com.mx";
                MailMessage m = new MailMessage();
                m.From = new MailAddress(from, "SAGA INN");
                m.Subject = "Requisiciones sin asignar";

                var datos = db.Database.SqlQuery<PausadasDto>("dbo.sp_RequisSinAsignar").ToList();


                if (datos.Count > 0)
                {
                    var aprobadores = datos.Select(x => x.aprobadorId).Distinct().ToList();


                    var inicio = "<html><head><style>td {border: solid black 1px;padding-left:5px;padding-right:5px;padding-top:1px;padding-bottom:1px;font-size:9pt;color:Black;font-family:'calibri';} " +
                                                "</style></head><body style=\"text-align:center; font-family:'calibri'; font-size:10pt;\"><table class='table'><tr><th align=center>DIAS SIN MODIFICAR</th><th align=center>FOLIO</th><th align=center>PERFIL</th><th align=center>FECHA ALTA</th><th align=center>FECHA CUMPLIMIENTO</th><th align=center>CLIENTE</th><th align=center>SOLICITA</th><th align=center>COORDINADOR</th><th align=center>CUB/VAC</th><th align=center>ESTATUS</th><th align=center>CAMBIO DE ESTATUS</th><th align=center>ASIGNADA A</th></tr>";

                    var body = "";
                    string reclutadoresList = "";
                    foreach (var a in aprobadores)
                    {
                        var aux = datos.Where(x => x.aprobadorId.Equals(a)).ToList();

                        m.To.Add(aux[0].email);
                        m.Bcc.Add(aux[0].emailSol);
                        m.Bcc.Add("idelatorre@damsa.com.mx");
                        m.Bcc.Add("mventura@damsa.com.mx");
                        m.Bcc.Add("bmorales@damsa.com.mx");
                        foreach (var r in aux)
                        {
                            var reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(r.Id) && !x.GrpUsrId.Equals(r.aprobadorId)).Select(rec => new
                            {
                                nombre = db.Usuarios.Where(x => x.Id.Equals(rec.GrpUsrId)).Select(N => N.Nombre + " " + N.ApellidoPaterno + " " + N.ApellidoMaterno).FirstOrDefault()
                            }).ToList();

                            if (reclutadores.Count() > 1)
                            {
                                reclutadoresList = reclutadoresList + "<ul>";
                                foreach (var rs in reclutadores)
                                {
                                    reclutadoresList = reclutadoresList + "<li>" + rs.nombre + "</li>";
                                }
                                reclutadoresList = "</ul>";
                            }
                            else if (reclutadores.Count() == 1)
                            {
                                reclutadoresList = reclutadores[0].nombre;
                            }
                            else
                            {
                                reclutadoresList = "SIN ASIGNAR";
                            }

                            body = body + string.Format("<tr><td align=center>{0}</td><td align=center>{1}</td><td align=center>{2}</td><td align=center>{3}</td><td align=center>{4}</td>" +
                                                       "<td align=center>{5}</td><td align=center>{6}</td><td align=center>{7}</td><td align=center>{8}</td><td align=center>{9}</td><td align=center>{10}</td><td align=center>{11}</td></tr>",
                                                       r.dias, r.Folio, r.VBtra, r.fch_Aprobacion, r.fch_Cumplimiento, r.Cliente, r.solicitante, r.aprobador, r.cubiertas.ToString() + "/" + r.vacantes.ToString(), r.estatus, r.fch_Modificacion, reclutadoresList);
                        }

                        body = inicio + body + "</table><p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
                        body = body + "<br/></body></html>";
                        m.Body = body;
                        m.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                        smtp.EnableSsl = true;
                        smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                        smtp.Send(m);

                        body = "";

                        m.To.Clear();
                        m.Bcc.Clear();

                    }

                }
                return Ok(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpGet]
        [Route("execProcedureVencidas")]
        public IHttpActionResult ExecProcedureVencidas()
        {
            try
            {
                string from = "noreply@damsa.com.mx";
                MailMessage m = new MailMessage();
                m.From = new MailAddress(from, "SAGA INN");
                m.Subject = "Requisiciones Vencidas";

                var datos = db.Database.SqlQuery<PausadasDto>("dbo.sp_RequisVencidas").ToList();


                if (datos.Count > 0)
                {
                    var aprobadores = datos.Select(x => x.aprobadorId).Distinct().ToList();

                    var inicio = "<html><head><style>td {border: solid black 1px;padding-left:5px;padding-right:5px;padding-top:1px;padding-bottom:1px;font-size:9pt;color:Black;font-family:'calibri';} " +
                                                "</style></head><body style=\"text-align:center; font-family:'calibri'; font-size:10pt;\"><table class='table'><tr><th align=center>DIAS SIN MODIFICAR</th><th align=center>FOLIO</th><th align=center>PERFIL</th><th align=center>FECHA ALTA</th><th align=center>FECHA CUMPLIMIENTO</th><th align=center>CLIENTE</th><th align=center>SOLICITA</th><th align=center>COORDINADOR</th><th align=center>CUB/VAC</th><th align=center>ESTATUS</th><th align=center>CAMBIO DE ESTATUS</th><th align=center>ASIGNADA A</th></tr>";

                    var body = "";
                    string reclutadoresList = "";
                    foreach (var a in aprobadores)
                    {
                        var aux = datos.Where(x => x.aprobadorId.Equals(a)).ToList();

                        m.To.Add(aux[0].email);
                        m.Bcc.Add(aux[0].emailSol);
                        m.Bcc.Add("idelatorre@damsa.com.mx");
                        m.Bcc.Add("mventura@damsa.com.mx");
                        m.Bcc.Add("bmorales@damsa.com.mx");
                        foreach (var r in aux)
                        {
                            var reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(r.Id) && !x.GrpUsrId.Equals(r.aprobadorId)).Select(rec => new
                            {
                                nombre = db.Usuarios.Where(x => x.Id.Equals(rec.GrpUsrId)).Select(N => N.Nombre + " " + N.ApellidoPaterno + " " + N.ApellidoMaterno).FirstOrDefault()
                            }).ToList();

                            if (reclutadores.Count() > 1)
                            {
                                reclutadoresList = reclutadoresList + "<ul>";
                                foreach (var rs in reclutadores)
                                {
                                    reclutadoresList = reclutadoresList + "<li>" + rs.nombre + "</li>";
                                }
                                reclutadoresList = "</ul>";
                            }
                            else if (reclutadores.Count() == 1)
                            {
                                reclutadoresList = reclutadores[0].nombre;
                            }
                            else
                            {
                                reclutadoresList = "SIN ASIGNAR";
                            }

                            body = body + string.Format("<tr><td align=center>{0}</td><td align=center>{1}</td><td align=center>{2}</td><td align=center>{3}</td><td align=center>{4}</td>" +
                                                       "<td align=center>{5}</td><td align=center>{6}</td><td align=center>{7}</td><td align=center>{8}</td><td align=center>{9}</td><td align=center>{10}</td><td>{11}</td></tr>",
                                                       r.dias, r.Folio, r.VBtra, r.fch_Aprobacion, r.fch_Cumplimiento, r.Cliente, r.solicitante, r.aprobador, r.cubiertas.ToString() + "/" + r.vacantes.ToString(), r.estatus, r.fch_Modificacion, reclutadoresList);
                        }

                        body = inicio + body + "</table><p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
                        body = body + "<br/></body></html>";
                        m.Body = body;
                        m.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                        smtp.EnableSsl = true;
                        smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                        smtp.Send(m);

                        body = "";

                        m.To.Clear();
                        m.Bcc.Clear();

                    }

                }
                return Ok(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.ExpectationFailed);
            }

        }

        [HttpGet]
        [Route("sendEmailRequiPura")]
        public IHttpActionResult SendEmailREquiPura(Guid IdRequisicion)
        {
            if (SendEmail.SendEmailRequisPuras(IdRequisicion))
            {
                return Ok(HttpStatusCode.OK);
            }
            else
            {
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("sendEmailRedesSociales")]
        public IHttpActionResult SendEmailRedesSociales(PublicarRedesSocialesDto info)
        {
            try
            {
                if (db.OficiosRequisicion.Where(r => r.RequisicionId.Equals(info.RequisicionId)).Count() == 0)
                {
                    OficioRequisicion or = new OficioRequisicion();
                    or.Oficio = info.Oficio;
                    or.Comentario = info.Comentario;
                    or.RequisicionId = info.RequisicionId;

                    db.OficiosRequisicion.Add(or);
                    db.SaveChanges();
                }
                else
                {
                    var of = db.OficiosRequisicion.Find(info.Id);
                    db.Entry(of).State = EntityState.Modified;
                    of.Comentario = info.Comentario;
                    of.Oficio = info.Oficio;
                    db.SaveChanges();
                }

                SendEmail.SendEmailRedesSociales(info.RequisicionId, info.Oficio, info.Comentario);
                return Ok(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("senEmailNuevaRequi")]
        public IHttpActionResult SenEmailNuevaRequi(SendEmailNuevaRequiDto sendEmail)
        {
            try
            {
                SendEmail.EmailNuevaRequisicion(sendEmail.Folio, sendEmail.VBtra, sendEmail.Email);
                return Ok();

            }catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("publicarNuevaRequi")]
        public IHttpActionResult PublicarNuevaRequi(Guid Id)
        {
            try
            {
                UpdatePublicarDto UpDto = new UpdatePublicarDto();
                UpDto.ListaPublicar = null;
                UpDto.RequiId = Id.ToString();
                Dvc.PublicarVacante(UpDto);
                return Ok();

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
    }
}
