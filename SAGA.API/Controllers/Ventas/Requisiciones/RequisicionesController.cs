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
using System.Diagnostics;
using SAGA.API.Dtos.Reporte;

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
        private PrivilegiosController objP;
        public Guid auxID = new Guid("00000000-0000-0000-0000-000000000000");
        public RequisicionesController()
        {
            db = new SAGADBContext();
            DamfoDto = new Damfo290Dto();
            businessDay = new BusinessDay();
            rastreabilidad = new Rastreabilidad();
            SendEmail = new SendEmails();
            Dvc = new DesignerVacanteController();
            objP = new PrivilegiosController();
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
                    coordinador = r.AsignacionRequi.Where(xx => xx.Tipo.Equals(1)).Select(c => db.Usuarios.Where(x => x.Id.Equals(c.GrpUsrId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()),
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
            //TimeSpan stop;
            //TimeSpan start = new TimeSpan(DateTime.Now.Ticks);
            try
            {
                var requisicion2 = db.Requisiciones.Where(x => x.Id.Equals(Id)).ToList();
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
                        coordinador  = r.AsignacionRequi.Where(xx => xx.Tipo.Equals(1)).Select(c => db.Usuarios.Where(x => x.Id.Equals(c.GrpUsrId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()),
                        asignados = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(r.Id)).Select(x => x.GrpUsrId).ToList(),
                        asignadosN = r.AsignacionRequi.Where(x => x.RequisicionId.Equals(r.Id) && !x.GrpUsrId.Equals(r.AprobadorId)).Select(x => new {
                            x.GrpUsr.Nombre,
                            x.GrpUsr.ApellidoMaterno,
                            x.GrpUsr.ApellidoPaterno,
                            x.Tipo
                        }),
                        vacantes = r.horariosRequi.Count() > 0 ? r.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                        r.VBtra,
                       // HorariosDamfo = db.HorariosPerfiles.Where(h => h.DAMFO290Id.Equals(r.DAMFO290Id)).ToList()
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
                        horariosRequi = x.horariosRequi.Select(h => new
                        {
                            h.Id,
                            requisicionId = x.Id,
                            h.Nombre,
                            h.deDia,
                            h.aDia,
                            h.deHora,
                            h.aHora,
                            h.numeroVacantes,
                            h.Especificaciones
                        }),
                        x.TipoReclutamientoId,
                        x.ClienteId
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

            Stopwatch stopwatch = new Stopwatch();

            try
            {
                //stopwatch.Start();
                var tipo = db.Usuarios.Where(x => x.Id.Equals(propietario)).Select(u => u.TipoUsuarioId).FirstOrDefault();
                var permisos = objP.GetPrivilegios(propietario);
                bool confidencial = permisos.Where(x => x.Nombre.Equals("Confidencial") && x.TipoEstructuraId.Equals(8)).Select(c => c.Read).FirstOrDefault();
                //var mm = stopwatch.Elapsed;
                //stopwatch.Start();
                if (tipo == 8 || tipo == 3 || tipo == 12 || tipo == 13 || tipo == 14)
                {
                    var requisicion = db.Requisiciones
                   .Where(e => !estatusId.Contains(e.EstatusId))
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
                       coordinador = e.AsignacionRequi.Where(xx => xx.Tipo.Equals(1)).Select(c => db.Usuarios.Where(x => x.Id.Equals(c.GrpUsrId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()),
                       Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                       reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && x.Tipo.Equals(2)).Select(a =>
                      db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                                       ).ToList(),

                       ComentarioReclutador = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(e.Id)).Select(c => c.fch_Creacion + " - " + c.UsuarioAlta + " - " + (c.Motivo.Id == 7 ? "" : c.Motivo.Descripcion + " - ") + c.Comentario).ToList()
                   }).OrderBy(x => x.EstatusOrden).ThenByDescending(x => x.Folio).ToList();

                    if(!confidencial)
                    {
                        requisicion = requisicion.Where(x => !x.Confidencial).ToList();
                    }
                    //var mm = stopwatch.Elapsed;

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
                       coordinador = e.AsignacionRequi.Where(xx => xx.Tipo.Equals(1)).Select(c => db.Usuarios.Where(x => x.Id.Equals(c.GrpUsrId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()),
                       Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                       reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && x.Tipo.Equals(2)).Select(a =>
                      db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                                       ).ToList(),

                       ComentarioReclutador = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(e.Id)).Select(c => c.fch_Creacion + " - " + c.UsuarioAlta + " - " + (c.Motivo.Id == 7 ? "" : c.Motivo.Descripcion + " - ") + c.Comentario).ToList()
                   }).OrderBy(x => x.EstatusOrden).ThenByDescending(x => x.Folio).ToList();

                    if (!confidencial)
                    {
                        requisicion = requisicion.Where(x => !x.Confidencial).ToList();
                    }

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
                       coordinador = e.AsignacionRequi.Where(xx => xx.Tipo.Equals(1)).Select(c => db.Usuarios.Where(x => x.Id.Equals(c.GrpUsrId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()),
                       Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault().ToUpper(),
                       reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && x.Tipo.Equals(2)).Select(a =>
                      db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                                       ).ToList(),
                       ComentarioReclutador = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(e.Id)).Select(c => c.fch_Creacion + " - " + c.UsuarioAlta + " - " + (c.Motivo.Id == 7 ? "" : c.Motivo.Descripcion.ToUpper() + " - ") + c.Comentario.ToUpper()).ToList()
                   }).OrderBy(x => x.EstatusOrden).ThenByDescending(x => x.Folio).ToList();

                    if (!confidencial)
                    {
                        requisicion = requisicion.Where(x => !x.Confidencial).ToList();
                    }
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
        public IHttpActionResult GetRequisicionesHistorial(Guid Id, int page, int pageCount, string fini, string ffin)
        {
            List<Guid> uids = new List<Guid>();
            int[] estatusId =  { 8, 9, 34, 35, 36, 37,47,48 };
            DateTime FechaI = Convert.ToDateTime("2019-01-01 00:00:00");
            DateTime FechaF = DateTime.Now;
            var paginationMetadata = new PaginationDto
            {
                pageSize = pageCount,
                currentPage = page,
                totalPages = 0
            };
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
                FechaF = FechaF.AddDays(1);
            }

            catch(Exception ex) { }
            try
            {
                var tipo = db.Usuarios.Where(x => x.Id.Equals(Id)).Select(u => u.TipoUsuarioId).FirstOrDefault();
                var permisos = objP.GetPrivilegios(Id);

                bool confidencial = permisos.Where(x => x.Nombre.Equals("Confidencial") && x.TipoEstructuraId.Equals(8)).Select(c => c.Read).FirstOrDefault();

                if (tipo == 8 || tipo == 3 || tipo == 12 || tipo == 13 || tipo == 14)
                {
                    var Allrequis = db.Requisiciones.OrderByDescending(o => o.Folio).Where(x => estatusId.Contains(x.EstatusId) && (x.fch_Creacion >= FechaI && x.fch_Creacion <= FechaF))
                        .Select(r => r.Id).ToList();
                    if (!confidencial)
                    {
                        Allrequis = db.Requisiciones.OrderByDescending(o => o.Folio).Where(x => Allrequis.Contains(x.Id) && !x.Confidencial && (x.fch_Creacion >= FechaI && x.fch_Creacion <= FechaF))
                                    .Select(r => r.Id).ToList();
                    }

                    var totalContratados = db.ProcesoCandidatos.Where(p => Allrequis.Contains(p.RequisicionId) && p.EstatusId.Equals(24)).Count();
                    paginationMetadata.totalCount = Allrequis.Count();

                    if (paginationMetadata.currentPage > 0)
                    {
                        Allrequis = Allrequis.Skip(paginationMetadata.pageSize * (paginationMetadata.currentPage - 1)).Take(paginationMetadata.pageSize).ToList();
                    }
                   var requisicion = db.Requisiciones
                       .Where(e => Allrequis.Contains(e.Id))
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
                       }).ToList().OrderByDescending(o => o.Folio);
                   
                    return Ok( new
                    {
                        requisicion,
                        totalContratados,
                        paginationMetadata
                    });
                }
                else if (tipo == 10) //ejecutivo de cuenta
                {

                    var Allrequis = db.Requisiciones.OrderByDescending(o => o.Folio).Where(x => estatusId.Contains(x.EstatusId) && x.PropietarioId.Equals(Id) && (x.fch_Creacion >= FechaI && x.fch_Creacion <= FechaF)).Select(r => r.Id).ToList();
                    if (!confidencial)
                    {
                        Allrequis = db.Requisiciones.OrderByDescending(o => o.Folio).Where(x => Allrequis.Contains(x.Id) && !x.Confidencial && (x.fch_Creacion >= FechaI && x.fch_Creacion <= FechaF))
                                    .Select(r => r.Id).ToList();
                    }

                    var totalContratados = db.ProcesoCandidatos.Where(p => Allrequis.Contains(p.RequisicionId) && p.EstatusId.Equals(24)).Count();
                    paginationMetadata.totalCount = Allrequis.Count();

                    if (paginationMetadata.currentPage > 0)
                    {
                        Allrequis = Allrequis.Skip(paginationMetadata.pageSize * (paginationMetadata.currentPage - 1)).Take(paginationMetadata.pageSize).ToList();
                    }

                    var requisicion = db.Requisiciones.OrderByDescending(o => o.Folio)
                   .Where(e => Allrequis.Contains(e.Id))
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

                       //ComentarioReclutador = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(e.Id)).Select(c => c.fch_Creacion + " - " + c.UsuarioAlta + " - " + (c.Motivo.Id == 7 ? "" : c.Motivo.Descripcion + " - ") + c.Comentario).ToList()
                   }).ToList();
                                      
                    return Ok( new
                    {
                        requisicion,
                        totalContratados,
                        paginationMetadata
                    });
                }
                else
                {
                    if (db.Subordinados.Count(x => x.LiderId.Equals(Id)) > 0)
                    {
                        var ids = db.Subordinados.Where(x => !x.UsuarioId.Equals(Id) && x.LiderId.Equals(Id)).Select(u => u.UsuarioId).ToList();

                        uids = GetSub(ids, uids);

                    }

                    uids.Add(Id);

                    var asignadas = db.AsignacionRequis.OrderByDescending(o => o.Requisicion.Folio)
                        .Where(x => x.GrpUsrId.Equals(Id)
                        && estatusId.Contains(x.Requisicion.EstatusId) && (x.Requisicion.fch_Creacion >= FechaI && x.Requisicion.fch_Creacion <= FechaF))
                        .Select(a => a.RequisicionId).ToList();


                    var requis = db.Requisiciones.OrderByDescending(o => o.Folio)
                   .Where(e => (uids.Contains(e.AprobadorId) || uids.Contains(e.PropietarioId))
                   && estatusId.Contains(e.EstatusId) && (e.fch_Creacion >= FechaI && e.fch_Creacion <= FechaF))
                   .Select(a => a.Id).ToList();

                    var AllRequis = requis.Union(asignadas).Distinct().ToList();

                    if (!confidencial)
                    {
                        AllRequis = db.Requisiciones.OrderByDescending(o => o.Folio).Where(e => AllRequis.Contains(e.Id) && !e.Confidencial).Select(rc => rc.Id).ToList();
                    }
                    var totalContratados = db.ProcesoCandidatos.Where(p => AllRequis.Contains(p.RequisicionId) && p.EstatusId.Equals(24)).Count();
                        
                    paginationMetadata.totalCount = AllRequis.Count();

                    if (paginationMetadata.currentPage > 0)
                    {
                        AllRequis = AllRequis.Skip(paginationMetadata.pageSize * (paginationMetadata.currentPage - 1)).Take(paginationMetadata.pageSize).ToList();
                    }

                    var requisicion = db.Requisiciones.OrderByDescending(o => o.Folio)
                   .Where(e => AllRequis.Contains(e.Id))
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
                       reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && x.Tipo.Equals(2)).Select(a =>
                      db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault()
                                       ).ToList(),
                   }).ToList();
                   
                    return Ok(new
                    {
                        requisicion,
                        totalContratados,
                        paginationMetadata
                    });
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
                var permisos = objP.GetPrivilegios(propietario);

                bool confidencial = permisos.Where(x => x.Nombre.Equals("Confidencial") && x.TipoEstructuraId.Equals(8)).Select(c => c.Read).FirstOrDefault();

                //var UnidadNegocio = db.Usuarios
                //    .Where(u => u.Id.Equals(propietario)).Select(u => 
                //    db.UnidadNegocioEstados.Where(x => x.estadoId.Equals(u.Sucursal.estadoId))
                //    .Select(un => un.unidadnegocioId).FirstOrDefault()).FirstOrDefault();
                var requisicion = db.Requisiciones
                    .Where(e => e.Activo.Equals(true) && e.TipoReclutamientoId.Equals(tipo) && (e.EstatusId.Equals(43) || e.EstatusId.Equals(44)))
                    .Select(e => new RequisicionGrallDto
                    {
                        Id = e.Id,
                        EstadoId = e.Direccion.EstadoId,
                        UnidadNegocioId = db.UnidadNegocioEstados.Where(x => x.estadoId.Equals(e.Direccion.EstadoId)).Select(u => u.unidadnegocioId).FirstOrDefault(),
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
                        TipoReclutamiento = e.TipoReclutamiento.tipoReclutamiento,
                        GiroEmpresa = e.Cliente.GiroEmpresas.giroEmpresa,
                        ActividadEmpresa = e.Cliente.ActividadEmpresas.actividadEmpresa,
                        Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                        Folio = e.Folio,
                        Confidencial = e.Confidencial,
                        coordinador = e.AsignacionRequi.Where(xx => xx.Tipo.Equals(1)).Select(c => db.Usuarios.Where(x => x.Id.Equals(c.GrpUsrId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()).ToList(),
                        Porcentaje = db.FacturacionPuro.Where(x => x.RequisicionId.Equals(e.Id)).Select(p => p.Porcentaje).FirstOrDefault(),
                        Propietario = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(P => P.Nombre + " " + P.ApellidoPaterno + " " + P.ApellidoMaterno).FirstOrDefault(),
                        ComentarioReclutador = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(e.Id)).Select(c => c.fch_Creacion + " - " + c.UsuarioAlta + " - " + (c.Motivo.Id == 7 ? "" : c.Motivo.Descripcion + " - ") + c.Comentario).ToList()
                    }).OrderBy(x => x.EstatusOrden).ThenByDescending(x => x.Folio).ToList();

                if (!confidencial)
                {
                    requisicion = requisicion.Where(x => !x.Confidencial).ToList();
                }

                foreach (var r in requisicion)
                {
                    r.diasTrans = this.countWeekDays(Convert.ToDateTime(r.fch_Creacion), DateTime.Now);
                }


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
                var permisos = objP.GetPrivilegios(ReclutadorId);

                bool confidencial = permisos.Where(x => x.Nombre.Equals("Confidencial") && x.TipoEstructuraId.Equals(8)).Select(c => c.Read).FirstOrDefault();
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
                    .Where(e => e.Activo.Equals(true) && e.Estatus.Id.Equals(estatus) && e.AprobadorId.Equals(ReclutadorId) )
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
                        Confidencial = e.Confidencial,
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

                if (!confidencial)
                {
                    vacantes = vacantes.Where(x => !x.Confidencial).ToList();
                }
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
                var permisos = objP.GetPrivilegios(IdUsuario);

                bool confidencial = permisos.Where(x => x.Nombre.Equals("Confidencial") && x.TipoEstructuraId.Equals(8)).Select(c => c.Read).FirstOrDefault();
                var tipo = db.Usuarios.Where(x => x.Id.Equals(IdUsuario)).Select(u => u.TipoUsuarioId).FirstOrDefault();
                if (tipo == 8 || tipo == 3 || tipo == 12 || tipo == 13 || tipo == 14)
                {
                    var vacantes = db.Requisiciones.OrderByDescending(e => e.Folio)
                        .Where(e => e.Activo
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
                            tipoReclutamientoId = e.TipoReclutamientoId,
                            Sucursal = db.Direcciones.Where(x => x.Id.Equals(e.DireccionId)).Select(d => d.Calle + " " + d.NumeroExterior + " C.P: " + d.CodigoPostal + " Col: " + d.Colonia.colonia).FirstOrDefault().ToUpper(),
                            Vacantes = e.horariosRequi.Count() > 0 ? e.horariosRequi.Sum(h => h.numeroVacantes) : 0,
                            Folio = e.Folio,
                            Confidencial = e.Confidencial,
                            Postulados = db.Postulaciones.Where(p => p.RequisicionId.Equals(e.Id) && p.StatusId.Equals(1)).Select(c => c.CandidatoId).Count(),
                            EnProceso = db.ProcesoCandidatos.OrderByDescending(f => f.Fch_Modificacion).Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId != 27 && p.EstatusId != 40).Count(),
                            EnProcesoFR = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 21).Count(),
                            EnProcesoFC = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 23).Count(),
                            contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(e.Id) && p.EstatusId == 24).Count(),
                            coordinador = e.AsignacionRequi.Where(x => x.Tipo.Equals(1)).Select(n => db.Usuarios.Where(x => x.Id.Equals(n.GrpUsrId)).Select(u => u.Nombre + " " + u.ApellidoPaterno + " " + u.ApellidoMaterno).FirstOrDefault()),
                            Solicita = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            PropietarioId = e.PropietarioId,
                            AprobadorId = e.AsignacionRequi.Where(x => x.Tipo.Equals(1)).Select(i => i.GrpUsrId).FirstOrDefault(),
                            Aprobada = e.Aprobada,
                            DiasEnvio = e.DiasEnvio,
                            asignados = e.AsignacionRequi.Select(a => a.GrpUsrId).ToList(),
                            reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && x.Tipo.Equals(2)).Select(a =>
                                db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()).ToList(),
                            ComentarioReclutador = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(e.Id)).Select(c => c.fch_Creacion + " - " + c.UsuarioAlta + " - " + (c.Motivo.Id == 7 ? "" : c.Motivo.Descripcion + " - ") + c.Comentario).ToList(),
                            Oficio = db.OficiosRequisicion.Where(of => of.RequisicionId.Equals(e.Id)).Count() > 0 ? db.OficiosRequisicion.Where(of => of.RequisicionId.Equals(e.Id)).FirstOrDefault() : null,
                            Ponderacion = db.PonderacionRequisiciones.Where(x => x.RequisicionId.Equals(e.Id)).Count() > 0 ? db.PonderacionRequisiciones.Where(x => x.RequisicionId.Equals(e.Id)).Select(p => new
                            {
                                Id = p.Id,
                                ponderacion = p.Ponderacion
                            }).FirstOrDefault() : null
                        }).OrderBy(x => x.EstatusOrden).ThenByDescending(x => x.Folio).ToList();

                    if (!confidencial)
                    {
                        vacantes = vacantes.Where(x => !x.Confidencial).ToList();
                    }
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
                                && !estatusId.Contains(a.Requisicion.EstatusId) )
                            .Select(a => a.RequisicionId)
                            .Distinct()
                            .ToList();

                    var requis = db.Requisiciones
                    .Where(e => (uids.Contains(e.AprobadorId) || uids.Contains(e.PropietarioId))
                        && !estatusId.Contains(e.EstatusId) )
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
                            coordinador = e.AsignacionRequi.Where(x => x.Tipo.Equals(1)).Select(n => db.Usuarios.Where(x => x.Id.Equals(n.GrpUsrId)).Select(u => u.Nombre + " " + u.ApellidoPaterno + " " + u.ApellidoMaterno).FirstOrDefault()),
                            Solicita = db.Usuarios.Where(x => x.Id.Equals(e.PropietarioId)).Select(s => s.Nombre + " " + s.ApellidoPaterno + " " + s.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            PropietarioId = e.AsignacionRequi.Where(x => x.Tipo.Equals(1)).Select(i => i.GrpUsrId),
                            AprobadorId = e.AprobadorId,
                            aprobada = e.Aprobada,
                            DiasEnvio = e.DiasEnvio,
                            asignados = e.AsignacionRequi.Select(a => a.GrpUsrId).ToList(),
                            reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(e.Id) && x.Tipo.Equals(2)).Select(a =>
                               db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault().ToUpper()
                            ).ToList(),
                            ComentarioReclutador = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(e.Id)).Select(c => c.fch_Creacion + " - " + c.UsuarioAlta + " - " + (c.Motivo.Id == 7 ? "" : c.Motivo.Descripcion + " - ") + c.Comentario).ToList(),
                            Oficio = db.OficiosRequisicion.Where(of => of.RequisicionId.Equals(e.Id)).Count() > 0 ? db.OficiosRequisicion.Where(of => of.RequisicionId.Equals(e.Id)).FirstOrDefault() : null,
                            Ponderacion = db.PonderacionRequisiciones.Where(x => x.RequisicionId.Equals(e.Id)).Count() > 0 ? db.PonderacionRequisiciones.Where(x => x.RequisicionId.Equals(e.Id)).Select( p => new
                            {
                                Id = p.Id,
                                ponderacion = p.Ponderacion
                            }).FirstOrDefault() : null
                        }).OrderBy(x => x.EstatusOrden).ThenByDescending(x => x.Folio).ToList();

                    if (!confidencial)
                    {
                        vacantes = vacantes.Where(x => !x.Confidencial).ToList();
                    }

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
                var permisos = objP.GetPrivilegios(IdUsuario);

                bool confidencial = permisos.Where(x => x.Nombre.Equals("Confidencial") && x.TipoEstructuraId.Equals(8)).Select(c => c.Read).FirstOrDefault();

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
                    if (!confidencial)
                    {
                        vacantes = vacantes.Where(x => !x.Confidencial).ToList();
                    }
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
                        .Where(e => asig.Contains(e.Id) && e.Activo.Equals(true))
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

                    if (!confidencial)
                    {
                        vacantes = vacantes.Where(x => !x.Confidencial).ToList();
                    }

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


        [HttpPost]
        [Route("getReporte70")]
        [Authorize]
        public IHttpActionResult GetReporte70(ReportesDto source)
        {
            try
            {
                DateTime FechaF = DateTime.Now;
                DateTime FechaI = DateTime.Now;

                var paginationMetadata = new PaginationDto
                {
                    pageSize = source.rowIndex[1],
                    currentPage = source.rowIndex[0],
                    totalPages = 0
                };

                List<int> estatus = new List<int> { 6, 7, 8, 9, 29, 30, 33, 38 };
                int[] estatusId = new int[] { 8, 9, 34, 35, 36, 37, 47, 48 };
                if (source.fini != null)
                {
                    FechaI = Convert.ToDateTime(source.fini);
                }
                if (source.ffin != null)
                {
                    FechaF = Convert.ToDateTime(source.ffin);
                }

                FechaF = FechaF.AddDays(1);

                //primero saco los datos a filtrar 
                var objeto = db.Requisiciones.Where(e => e.fch_Creacion >= FechaI && e.fch_Creacion <= FechaF).OrderByDescending(o => o.fch_Creacion).Select(e => new
                {
                    e.Id,
                    e.Confidencial,
                    e.fch_Modificacion,
                    e.ClienteId,
                    e.AprobadorId,
                    e.PropietarioId,
                    e.Direccion.EstadoId,
                    numero = e.horariosRequi.Sum(a => a.numeroVacantes),
                    e.EstatusId,
                    e.TipoReclutamientoId,
                    e.ClaseReclutamientoId,
                    e.VBtra
                }).ToList();
                
                // se tiene que omptimizar esta parte
                if (source.clave != null && source.clave != "" && objeto.Count() > 0)
                {
                    objeto = objeto.Where(e => e.VBtra.ToLower().Contains(source.clave.ToLower())).ToList();
                }

                if (source.stus.Count() > 0 && source.stus != null && objeto.Count() > 0)
                {
                    objeto = objeto.Where(e => source.stus.Contains(e.EstatusId)).ToList();
                }
                if (source.sol.Count() > 0 && objeto.Count() > 0)
                {
                    objeto = objeto.Where(e => source.sol.Contains(e.PropietarioId)).ToList();
                }
                if (source.coord.Count() > 0 && objeto.Count() > 0)
                {
                    objeto = objeto.Where(e => source.coord.Contains(e.ClaseReclutamientoId)).ToList();
                }
                if (source.trcl.Count() > 0 && objeto.Count() > 0)
                {
                    objeto = objeto.Where(e => source.trcl.Contains(e.TipoReclutamientoId)).ToList();
                }
                if (source.emp.Count() > 0 && objeto.Count() > 0)
                {
                    objeto = objeto.Where(e => source.emp.Contains(e.ClienteId)).ToList();
                }

                Guid id = source.usuario;
                int usertipo = db.Usuarios.Where(e => e.Id == id).Select(e => e.TipoUsuarioId).FirstOrDefault();
                if (usertipo == 11 && objeto.Count() > 0)
                {
                    var requiID = objeto.Select(x => x.Id).ToList();
                    var asigna = db.AsignacionRequis.Where(e => requiID.Contains(e.RequisicionId)).ToList();
                    var requien = asigna.Where(e => e.GrpUsrId == id).Select(x => x.RequisicionId).ToList();
                    objeto = objeto.Where(e => e.AprobadorId == id || requien.Contains(e.Id)).ToList();
                }
                else
                {
                    if (source.usercoor.Count() > 0 && objeto.Count() > 0)
                    {

                        var asigna = db.AsignacionRequis.Where(e => source.usercoor.Contains(e.GrpUsrId) && e.Tipo.Equals(1)).Select(e => e.RequisicionId).Distinct().ToList();
                        objeto = objeto.Where(e => asigna.Contains(e.Id)).ToList();
                    }
                }

                if (usertipo == 11 && objeto.Count() > 0)
                {
                    var asigna = db.AsignacionRequis.Where(e => e.GrpUsrId == id).Select(e => e.RequisicionId).ToList();
                    objeto = objeto.Where(e => asigna.Contains(e.Id)).ToList();
                }
                else
                {
                    if (source.recl.Count() > 0 && objeto.Count() > 0)
                    {
                        var asigna = db.AsignacionRequis.Where(e => source.recl.Contains(e.GrpUsrId) && e.Tipo.Equals(2)).Select(e => e.RequisicionId).ToList();
                        objeto = objeto.Where(e => asigna.Contains(e.Id)).ToList();
                    }

                }

                if (source.ofc.Count() > 0 && objeto.Count() > 0)
                {
                    var estado = db.UnidadNegocioEstados.Where(e => source.ofc.Contains(e.unidadnegocioId)).Select(e => e.estadoId).Distinct().ToList();
                    objeto = objeto.Where(e => estado.Contains(e.EstadoId)).ToList();
                }
                var permisos = objP.GetPrivilegios(source.usuario);
                bool confidencial = permisos.Where(x => x.Nombre.Equals("Confidencial") && x.TipoEstructuraId.Equals(8)).Select(c => c.Read).FirstOrDefault();

                if (!confidencial && objeto.Count() > 0)
                {
                    objeto = objeto.Where(x => !x.Confidencial).ToList();
                }

                var totalFolios = objeto.Count();
                var posActivas = objeto.Count() > 0 ? objeto.Where(x => !estatusId.Contains(x.EstatusId)).Select(r => r.numero).ToList().Sum() : 0;

                if (objeto.Count() > 0)
                {
                    List<Guid> requis = new List<Guid>();

                    if (paginationMetadata.currentPage > 0)
                    {
                        requis = objeto.Select(r => r.Id).Skip(paginationMetadata.pageSize * (paginationMetadata.currentPage - 1))
                       .Take(paginationMetadata.pageSize).ToList();
                    }
                    else
                    {
                        requis = objeto.Select(r => r.Id).ToList();
                    }
                    //var t = db.EstatusRequisiciones.Where(x => requis.Contains(x.RequisicionId)).GroupBy(g => g.RequisicionId).Select(T => new
                    //{
                    //    RequisicionId = T.Key,
                    //    Estatus = T.Select(x => new EstatusRequiDto
                    //    {
                    //        EstatusId = x.EstatusId,
                    //        Estatus = x.Estatus.Descripcion.ToUpper(),
                    //        fch_Modificacion = x.fch_Modificacion.Value,
                    //        diasTrans = 0,
                    //        diasTotal = 0,
                    //    }).OrderBy(o => o.fch_Modificacion).ToList()
                    //}).ToList();

                    //foreach (var r in t)
                    //{
                    //    if (r.Estatus.Count > 1)
                    //    {
                    //        for (int i = 0; i < r.Estatus.Count() - 1; i++)
                    //        {
                    //            int dt = countWeekDays(r.Estatus[i].fch_Modificacion, r.Estatus[i + 1].fch_Modificacion);
                    //            r.Estatus[i].diasTrans = dt - 1;
                    //            if (estatus.Contains(r.Estatus[i].EstatusId))
                    //            {
                    //                r.Estatus[i].diasTotal += (dt - 1);
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        int dt = countWeekDays(r.Estatus[0].fch_Modificacion, DateTime.Now);
                    //        r.Estatus[0].diasTrans = dt - 1;
                    //        if (estatus.Contains(r.Estatus[0].EstatusId))
                    //        {
                    //            r.Estatus[0].diasTotal += (dt - 1);
                    //        }
                    //    }
                    //}

                    var data = db.Requisiciones.Where(x => requis.Contains(x.Id)).Select(row => new
                    {
                        row.Id,
                        row.fch_Cumplimiento,
                        row.AprobadorId,
                        row.PropietarioId,
                        row.Folio,
                        row.fch_Creacion,
                        row.Cliente.RazonSocial,
                        row.Cliente.Nombrecomercial,
                        row.Direccion.Estado.estado,
                        row.VBtra,
                        row.SueldoMaximo,
                        row.TipoReclutamiento.tipoReclutamiento,
                        row.ClaseReclutamiento.clasesReclutamiento,
                        row.Confidencial,
                        row.ClienteId,
                        Vacantes = row.horariosRequi.Count() > 0 ? row.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                        contratados = db.ProcesoCandidatos.Where(x => x.EstatusId == 24 && x.RequisicionId == row.Id).Select(a => a.CandidatoId).Distinct().ToList().Count(),
                        faltante = (row.horariosRequi.Count() > 0 ? row.horariosRequi.Sum(v => v.numeroVacantes) : 0) - db.ProcesoCandidatos.Where(x => x.EstatusId == 24 && x.RequisicionId == row.Id).Select(a => a.CandidatoId).Distinct().ToList().Count(),
                        porcentaje = row.horariosRequi.Sum(v => v.numeroVacantes) == 0 ? 0 : db.ProcesoCandidatos.Where(x => x.EstatusId == 24 && x.RequisicionId == row.Id).Select(a => a.CandidatoId).Distinct().ToList().Count() * 100 / row.horariosRequi.Sum(v => v.numeroVacantes),
                        enProcesoEC = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(row.Id) && p.EstatusId.Equals(22)).Count(),
                        enProcesoFC = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(row.Id) && p.EstatusId.Equals(23)).Count(),
                        domicilio_trabajo = row.Direccion.Calle + " " + row.Direccion.NumeroExterior + " " + row.Direccion.Colonia.colonia + " " + row.Direccion.Municipio.municipio + " " + row.Direccion.Estado.estado,
                        solicita = db.Usuarios.Where(x => x.Id.Equals(row.PropietarioId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault(),
                        coordinador = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(row.Id) && x.Tipo.Equals(1)).Select(c => db.Usuarios.Where(x => x.Id.Equals(c.GrpUsrId)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()).FirstOrDefault(),
                        reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(row.Id) && x.Tipo.Equals(2)).Select(a =>
                            db.Usuarios.Where(x => x.Id.Equals(a.GrpUsrId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno + " ").FirstOrDefault().ToUpper()
                                             ).ToList(),
                        comentarios_coord = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(row.Id) && x.ReclutadorId.Equals(row.AprobadorId)).GroupBy(g => g.ReclutadorId)
                          .Select(c => new CR
                          {
                              reclutador = db.Usuarios.Where(x => x.Id.Equals(c.Key)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault().ToUpper(),
                              comentario = c.Select(cc => new comentariosRecl
                              {
                                  fch_Creacion = cc.fch_Creacion,
                                  comentario = cc.Comentario.ToUpper()
                              }).ToList()
                          }).ToList(),
                        comentarios_solicitante = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(row.Id) && x.ReclutadorId.Equals(row.PropietarioId) && !x.ReclutadorId.Equals(row.AprobadorId)).GroupBy(g => g.ReclutadorId).Select(c => new CR
                        {
                            reclutador = db.Usuarios.Where(x => x.Id.Equals(c.Key)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            comentario = c.Select(cc => new comentariosRecl
                            {
                                fch_Creacion = cc.fch_Creacion,
                                comentario = cc.Comentario.ToUpper()
                            }).ToList()
                        }).ToList(),
                        comentarios_reclutador = db.ComentariosVacantes.Where(x => x.RequisicionId.Equals(row.Id) && !x.ReclutadorId.Equals(row.AprobadorId) && !x.ReclutadorId.Equals(row.PropietarioId)).GroupBy(g => g.ReclutadorId).Select(c => new CR
                        {

                            reclutador = db.Usuarios.Where(x => x.Id.Equals(c.Key)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault().ToUpper(),
                            comentario = c.Select(cc => new comentariosRecl
                            {
                                fch_Creacion = cc.fch_Creacion,
                                comentario = cc.Comentario.ToUpper()
                            }).ToList()
                        }).ToList(),
                        Estatus = db.EstatusRequisiciones.Where(x => x.RequisicionId.Equals(row.Id)).GroupBy(g => g.RequisicionId).Select(T => new
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
                        }).ToList()
                }).ToList();

                    foreach (var d in data)
                    {
                        foreach (var r in d.Estatus)
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
                    }
                    return Ok(new
                    {
                        data,
                        posActivas,
                        totalFolios,
                        paginationMetadata
                    });
                }
                else
                {
                    return Ok(objeto);
                }
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
                var permisos = objP.GetPrivilegios(reclutadorId);

                bool confidencial = permisos.Where(x => x.Nombre.Equals("Confidencial") && x.TipoEstructuraId.Equals(8)).Select(c => c.Read).FirstOrDefault();

                var tipo = db.Usuarios.Where(x => x.Id.Equals(reclutadorId)).Select(u => u.TipoUsuarioId).FirstOrDefault();
                if (tipo == 8)
                {
                    var informe = db.Requisiciones.OrderByDescending(f => f.fch_Cumplimiento)
                       .Where(e => e.Activo.Equals(true) && e.EstatusId != 9 && e.EstatusId != 8).Select(h => new
                       {
                           Id = h.Id,
                           Folio = h.Folio,
                           vBtra = h.VBtra.ToUpper(),
                           Estatus = h.Estatus.Descripcion.ToUpper(),
                           EstatusId = h.EstatusId,
                           cliente = h.Cliente.Nombrecomercial.ToUpper(),
                           Vacantes = h.horariosRequi.Count() > 0 ? h.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                           Confidencial = h.Confidencial,
                           Fch_limite = h.fch_Cumplimiento,
                           h.TipoReclutamientoId,
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

                    if (!confidencial)
                    {
                        informe = informe.Where(x => !x.Confidencial).ToList();
                    }
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
                        .Where(a => uids.Distinct().Contains(a.GrpUsrId) && a.Requisicion.Activo && a.Requisicion.EstatusId != 8)
                        .Select(a => a.RequisicionId)
                        .Distinct()
                        .ToList();

                    var informe = db.Requisiciones.OrderByDescending(f => f.fch_Cumplimiento).Where(e => asig.Contains(e.Id)).Select(h => new
                        {
                            Id = h.Id,
                            Folio = h.Folio,
                            vBtra = h.VBtra,
                            Estatus = h.Estatus.Descripcion,
                            EstatusId = h.EstatusId,
                            cliente = h.Cliente.Nombrecomercial,
                            Vacantes = h.horariosRequi.Count() > 0 ? h.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                            Fch_limite = h.fch_Cumplimiento,
                            h.TipoReclutamientoId,
                            Postulados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId.Equals(10)).Select(c => c.CandidatoId).Distinct().Count(),
                            Abandono = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 26).Count(),
                            Descartados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 27).Count(),
                            EnProceso = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId != 27 && p.EstatusId != 40 && p.EstatusId != 28 && p.EstatusId != 42).Count(),
                            entrevista = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 18).Count(),
                            Confidencial = h.Confidencial,
                        //EnProcesoFR = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 21).Count(),
                        //EnProcesoFC = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 23).Count(),
                        contratados = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 24).Count(),
                            Enviados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId.Equals(21)).Count(),
                        //EntCliente = db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(RequisicionId) && p.HorarioId.Equals(h.Id) && p.EstatusId == 22).Count(),
                        rechazados = db.InformeRequisiciones.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 40).Count(),
                            porcentaje = h.horariosRequi.Count() > 0 && db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 24).Count() > 0 ? (db.ProcesoCandidatos.Where(p => p.RequisicionId.Equals(h.Id) && p.EstatusId == 24).Count() * 100) / h.horariosRequi.Sum(s => s.numeroVacantes) : 0,
                        }).ToList();

                    if (!confidencial)
                    {
                        informe = informe.Where(x => !x.Confidencial).ToList();
                    }
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
                    .Where(e => e.Activo.Equals(true) && e.EstatusId != 9 && e.EstatusId != 8).Select(h => new
                    {
                        Id = h.Id,
                        Folio = h.Folio,
                        vBtra = h.VBtra,
                        Estatus = h.Estatus.Descripcion,
                        EstatusId = h.EstatusId,
                        cliente = h.Cliente.Nombrecomercial,
                        Vacantes = h.horariosRequi.Count() > 0 ? h.horariosRequi.Sum(v => v.numeroVacantes) : 0,
                        fch_Creacion = h.fch_Creacion,
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
                    AlterAsignacionRequi(requi.AsignacionRequi, requi.Id, requi.Folio, requi.Usuario, requisicion.VBtra, CandidatosFiltro, requi.EstatusId);

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
                    AlterAsignacionRequi(requi.AsignacionRequi, requi.Id, requisicion.Folio, requi.Usuario, requisicion.VBtra, null, 0);    
                    db.SaveChanges();
                    Int64 Folio = requisicion.Folio;
                    //Creacion de Trazabalidad par ala requisición.
                    Guid trazabilidadId = db.TrazabilidadesMes.Where(x => x.Folio.Equals(Folio)).Select(x => x.Id).FirstOrDefault();
                    //Isertar el registro de la rastreabilidad. 
                    rastreabilidad.RastreabilidadInsert(trazabilidadId, requi.Usuario, 5);

                    //stopwatch.Stop();
                    //var t2 = stopwatch.Elapsed;
                    //stopwatch.Start();
                    beginTran.Commit();
                    //stopwatch.Stop();
                    //var t3 = stopwatch.Elapsed;
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

        public void AlterAsignacionRequi(List<AsignacionRequi> asignaciones, Guid RequiId, Int64 Folio, string Usuario, string VBra, List<CoincidenciasDto> Coincidencias, int estatusRequi)
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
                    db.AsignacionRequis.AddRange(filterAdd); //lo asigno despues veo si envío correo
                    if (estatusRequi != 43)
                    {
                        SendEmail.ConstructEmail(filterAdd, NotChange, "C", Folio, user, VBra, Coincidencias);
                    }
                }
            }
            else
            {
                db.AsignacionRequis.AddRange(asignaciones); //lo asigno despues veo si envío correo
                if (estatusRequi != 43)
                {
                    SendEmail.ConstructEmail(asignaciones, NotChange, "C", Folio, user, VBra, Coincidencias);
                }
            }
            db.SaveChanges();

        }

        [HttpPost]
        [Route("topCandidatos")]
        [Authorize]
        public void TopCandidatos(AsignarVacanteReclutador requi)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start();
                var requisicion = db.Requisiciones.Find(requi.Id);

                //Requisicion a coincidir.
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
                stopwatch.Stop();
                var t = stopwatch.Elapsed;
                //  Candidatos Activos
                var Activos = db.AspNetUsers
                    .Where(c => c.Activo == 0)
                    .OrderBy(c => c.UltimoInicio)
                    .Select(c => c.IdPersona)
                    .ToList();

                // Candidatos con todas las coincidencias.
                var Candidatos = db.PerfilCandidato
                    .Where(c => Activos.Contains(c.CandidatoId))
                    .Select(c => new CoincidenciasDto
                    {
                        Nombre = c.Candidato.Nombre + " " + c.Candidato.ApellidoPaterno + " " + (c.Candidato.ApellidoMaterno != null ? c.Candidato.ApellidoMaterno : ""),
                        Subcategoria = db.AreasInteres.Where(a => a.Id.Equals(c.AboutMe.Select(x => x.AreaExperienciaId).FirstOrDefault())).Select(s => s.AreaExperienciaId).FirstOrDefault() != 0 ?
                                       db.AreasInteres.Where(x => x.Id.Equals(c.AboutMe.Select(a => a.AreaExperienciaId).FirstOrDefault())).Select(s => s.areaInteres).FirstOrDefault() : "",
                        AreaExpId = db.AreasInteres.Where(a => a.Id.Equals(c.AboutMe.Select(x => x.AreaExperienciaId).FirstOrDefault())).Select(s => s.AreaExperienciaId).FirstOrDefault() != 0 ?
                                    db.AreasInteres.Where(a => a.Id.Equals(c.AboutMe.Select(x => x.AreaExperienciaId).FirstOrDefault())).Select(s => s.AreaExperienciaId).FirstOrDefault() : 0,
                        //Subcategoria = c.AboutMe.Select(a => a.AreaInteres.areaInteres).FirstOrDefault() != null ? c.AboutMe.Select(a => a.AreaInteres.areaInteres).FirstOrDefault() : "",
                        //AreaExpId = c.AboutMe.Select(a => a.AreaInteres.AreaExperienciaId).FirstOrDefault() != 0 ? c.AboutMe.Select(a => a.AreaInteres.AreaExperienciaId).FirstOrDefault() : 0,
                        SueldoMinimo = c.AboutMe.Select(a => a.SalarioAceptable).FirstOrDefault() > 0 ? c.AboutMe.Select(a => a.SalarioAceptable).FirstOrDefault() : 0,
                        SueldoMaximo = c.AboutMe.Select(a => a.SalarioDeseado).FirstOrDefault() > 0 ? c.AboutMe.Select(a => a.SalarioDeseado).FirstOrDefault() : 0,
                        Genero = c.Candidato.Genero.genero,
                        GeneroId = c.Candidato.GeneroId > 0 ? c.Candidato.GeneroId : 0,
                        EstadoCivil = c.Candidato.EstadoCivil.estadoCivil,
                        EstadoCivilId = c.Candidato.EstadoCivilId.Value > 0 ? c.Candidato.EstadoCivilId.Value : 0,
                        FormacionId = c.Formaciones.Select(x => x.GradoEstudioId).FirstOrDefault(),
                        Formaciones = c.Formaciones.Select(x => x.GradosEstudio.gradoEstudio).FirstOrDefault(),
                        Edad = DateTime.Now.Year - c.Candidato.FechaNacimiento.Value.Year,
                        Requisicion = db.Requisiciones
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
                        .FirstOrDefault()
                    })
                    .ToList();

                List<CoincidenciasDto> CandidatosFiltro = new List<CoincidenciasDto>();

                CandidatosFiltro = Candidatos
                    .Where(c => c.AreaExpId == Requi.Categoria)
                    .ToList();

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
                if (CandidatosFiltro != null && CandidatosFiltro.Count() > 0)
                {
                    // Solo tomamos el top 5 de candidatos que coinciderion.
                    CandidatosFiltro = CandidatosFiltro.Take(5).ToList();
                var distintEmails = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(requi.Id) && x.Tipo.Equals(2))
                    .Select(E => new
                    {
                        email = db.Emails.Where(x => x.EntidadId.Equals(E.GrpUsrId)).Select(ee => ee.email).FirstOrDefault()
                    }).ToList();
                if (distintEmails.Count() > 0)
                {
                    string body = string.Empty;
                    string Escolaridades = string.Empty;
                    string from = ConfigurationManager.AppSettings["ToEmail"];
                    MailMessage m = new MailMessage();
                        m.Bcc.Add("mventura@damsa.com.mx");
                        m.Bcc.Add("bmorales@damsa.com.mx");
                    m.From = new MailAddress(from, "SAGA Inn");

                        var index = CandidatosFiltro[0].Requisicion.EscolaridadesDesc.Count() - 1;

                        for (int e = 0; e < CandidatosFiltro[0].Requisicion.EscolaridadesDesc.Count(); e++)
                        {
                            if (e < index)
                            {
                                Escolaridades = Escolaridades + CandidatosFiltro[0].Requisicion.EscolaridadesDesc[e] + ", ";
                            }
                            else
                            {
                                Escolaridades = Escolaridades + CandidatosFiltro[0].Requisicion.EscolaridadesDesc[e];
                            }
                        }
                        foreach (var x in distintEmails)
                        {
                            m.To.Add(x.email.ToString());
                        }

                        m.Subject = "[SAGA] Asignacion de Requisicion " + requisicion.Folio;

                        var inicio = "<html><head><style>td { border: solid #2471A3 1px; padding-left:5px; padding-right:5px;padding-top:1px; padding-bottom:1px;font-size:9pt; color: #3498DB;font-family:'calibri'; width: 25%; text-align: left; vertical-align: top; border-spacing: 0; border-collapse: collapse;} ";
                        inicio = inicio + "p { font - family:'calibri'; } th { font - family:'calibri'; width: 25 %; text - align: left; vertical - align: top; border: solid blue 1px; border - spacing: 0; border - collapse: collapse; background: #3498DB; color:white;}";
                        inicio = inicio + "h3 { font - family:'calibri'; } table { width: 100 %; }</style></head><body style =\"text-align:center; font-family:'calibri'; font-size:10pt;\"><br><br><p> Asignación de Requisición:</p>";


                        body = inicio;
                        //body = body + string.Format("<br/>El usuario <strong>{0}</strong> te ha asignado para trabajar la vacante <strong>{1}</strong> la cual se encuentra con un folio de requisición: <strong style='background-color:yellow;'><big><a href=\"{3}/login/{2}\">{2}</a></big></strong>.", Usuario, VBr, Folio, sitioWeb);


                        body = body + string.Format("<br><p>Los siguientes candidatos coinciden para <b>Categoría</b> (" + CandidatosFiltro[0].Requisicion.CategoriaDesc + "), <b>Salario</b> ($" + CandidatosFiltro[0].Requisicion.SalarioMinimo + "- $" + CandidatosFiltro[0].Requisicion.SalarioMaximo + "), <b>Genero</b> (" + CandidatosFiltro[0].Requisicion.GeneroDesc + "),");
                        body = body + string.Format(" <b>Edad:</b> (" + CandidatosFiltro[0].Requisicion.EdadMinima + "-" + CandidatosFiltro[0].Requisicion.EdadMaxima + "), <b>Estado civil</b> (" + CandidatosFiltro[0].Requisicion.EstadoCivilDesc + "), <b>Escolaridad</b> (" + Escolaridades + ")</p>");
                        body = body + "<table class='table'>";
                        body = body + "<tr><th align=center>Candidato</th><th align=center>Subcategoria</th><th align=center>Rango Salarial</th><th align=center>Edad</th></tr>";
                        for (int i = 0; i < CandidatosFiltro.Count(); i++)
                        {
                            body = body + "<tr><td align=center> " + CandidatosFiltro[i].Nombre + "</td><td align=center>" + CandidatosFiltro[i].Subcategoria + "</td><td align=center>" + CandidatosFiltro[i].SueldoMinimo + " - " + CandidatosFiltro[i].SueldoMaximo + "</td><td align=center>" + CandidatosFiltro[i].Edad + "</td></tr>";
                        }
                        body = body + "</table>";

                        body = body + "<p>Para ver tus requisiciones asignadas ingresa a tu panel de <b>Reclutamiento</b>, selecciona la opción de <b>Vacantes</b> para dar el seguimiento correspondiente.</p> ";
                        body = body + "<p>Podrás acceder mediante la siguiente dirección: https://weberp.damsa.com.mx/login" + "</p>";
                        body = body + "<p>Quedamos a tus órdenes para cualquier relativo al correo inntec@damsa.com.mx</p><p>Gracias por tu atención. </p><p>Saludos</p>";
                        body = body + "</body></html>";


                        m.Body = body;
                        m.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                        smtp.EnableSsl = true;
                        smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                        smtp.Send(m);
                        stopwatch.Start();
                    }
                }
                else
                {
                    string from = ConfigurationManager.AppSettings["ToEmail"];
                    MailMessage m = new MailMessage();
                    m.From = new MailAddress(from, "SAGA Inn");
                    m.Subject = "[SAGA] Asignacion de Requisicion " + requisicion.Folio;
                    m.To.Add("mventura@damsa.com.mx");
                    m.Bcc.Add("bmorales@damsa.com.mx");
                    m.Body = "<p>esta cosa no encuentra coincidencias de candidatos conshepshon :( </p>";
                    m.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                    smtp.EnableSsl = true;
                    smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                    smtp.Send(m);
                }
            }
            catch (Exception ex)
            {

            }
        }
        public List<Guid> GetGrupo(Guid grupo, List<Guid> listaIds)
        {
            if (!listaIds.Contains(grupo))
            {
                listaIds.Add(grupo);
                //var listadoNuevo = db.GruposUsuarios
                //    .Where(g => g.EntidadId.Equals(grupo) & g.Grupo.Activo)
                //           .Select(g => g.GrupoId)
                //           .ToList();
                //foreach (Guid g in listadoNuevo)
                //{
                //    var gp = db.GruposUsuarios
                //        .Where(x => x.EntidadId.Equals(g))
                //        .Select(x => x.GrupoId)
                //        .ToList();
                //    foreach (Guid gr in gp)
                //    {
                //        GetGrupo(gr, listaIds);
                //    }
                //}
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
            int[] estatus = { 8, 9, 34, 35, 36, 37, 47, 48 };
            try
            {
                string from = "noreply@damsa.com.mx";
                MailMessage m = new MailMessage();
                m.From = new MailAddress(from, "SAGA INN");
                m.Subject = "[SAGA] Vacantes en Pausa";

                var datos = db.Database.SqlQuery<PausadasDto>("dbo.sp_RequisPausadas").ToList();


                if (datos.Count > 0)
                {
                    var propietario = datos.Select(x => x.propietarioId).Distinct().ToList();

                    var inicio = "<html><head><style>td {border: solid black 1px;padding-left:5px;padding-right:5px;padding-top:1px;padding-bottom:1px;font-size:9pt;color:Black;font-family:'calibri';} " +
                                 "</style></head><body style=\"text-align:center; font-family:'calibri'; font-size:10pt;\"><table class='table'><tr><th align=center>DIAS SIN MODIFICAR</th><th align=center>FOLIO</th><th align=center>PERFIL</th><th align=center>FECHA ALTA</th><th align=center>FECHA CUMPLIMIENTO</th><th align=center>CLIENTE</th><th align=center>SOLICITA</th><th align=center>COORDINADOR</th><th align=center>CUB/VAC</th><th align=center>ESTATUS</th><th align=center>CAMBIO DE ESTATUS</th><th align=center>ASIGNADA A</th></tr>";

                    var body = "";
                    string reclutadoresList = "";
                    foreach (var p in propietario)
                    {
                        var aux = datos.Where(x => x.propietarioId.Equals(p)).ToList();
                        var emailSol = aux[0].emailSol;
                        m.To.Add(emailSol);

                        foreach (var r in aux)
                        {
                            var reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(r.Id) && x.Tipo.Equals(2)).Select(rec => new
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
                    }

                    var aprobadores = datos.Select(x => x.aprobadorId).Distinct().ToList();
                    body = "";
                    reclutadoresList = "";
                    foreach (var a in aprobadores)
                    {
                    
                        var aux = datos.Where(x => x.aprobadorId.Equals(a)).ToList();
                        var email = aux[0].email;
                        m.To.Add(email);

                        foreach (var r in aux)
                        {
                            var reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(r.Id) && x.Tipo.Equals(2)).Select(rec => new
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
                m.Subject = "[SAGA] Requisiciones sin cambio de estatus";

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

                        foreach (var r in aux)
                        {
                            var reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(r.Id) && x.Tipo.Equals(2)).Select(rec => new
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
                        body = body + "</body></html>";
                        m.Body = body;
                        m.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                        smtp.EnableSsl = true;
                        smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                        smtp.Send(m);

                        body = "";

                        m.To.Clear();
                    }
                    var propietario = datos.Select(x => x.propietarioId).Distinct().ToList();
                    body = "";
                    reclutadoresList = "";
                    foreach (var a in propietario)
                    {
                        var aux = datos.Where(x => x.propietarioId.Equals(a)).ToList();

                        m.To.Add(aux[0].emailSol);

                        foreach (var r in aux)
                        {
                            var reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(r.Id) && x.Tipo.Equals(2)).Select(rec => new
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
                        body = body + "</body></html>";
                        m.Body = body;
                        m.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpDamsa"], Convert.ToInt16(ConfigurationManager.AppSettings["SMTPPort"]));
                        smtp.EnableSsl = true;
                        smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                        smtp.Send(m);

                        body = "";

                        m.To.Clear();

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
               
                string GGEmail = "";

                string from = "noreply@damsa.com.mx";
                MailMessage m = new MailMessage();
                m.From = new MailAddress(from, "SAGA INN");
                m.Subject = "[SAGA] Requisiciones pendientes autorizar - Reclutamiento Puro";

                var datos = db.Database.SqlQuery<PausadasDto>("dbo.sp_RequisPuroPendientes").ToList();
                //var datosPA = datos.Where(x => x.estatusId.Equals(43)).ToList();

                GGEmail = db.Usuarios
                        .Where(u => u.TipoUsuarioId.Equals(14) && u.Departamento.Clave.Equals("GRTS") && u.Activo.Equals(true))
                        .Select(u => u.emails.Select(e => e.email).FirstOrDefault())
                        .FirstOrDefault();
                m.To.Add(GGEmail);

                m.Bcc.Add("mventura@damsa.com.mx");
                var body = "";
                var inicio = "<html><head><style>td {border: solid black 1px;padding-left:5px;padding-right:5px;padding-top:1px;padding-bottom:1px;font-size:9pt;color:Black;font-family:'calibri';} " +
                                               "</style></head><body style=\"text-align:center; font-family:'calibri'; font-size:10pt;\"><table class='table'><tr><th align=center>DIAS SIN MODIFICAR</th><th align=center>FOLIO</th><th align=center>PERFIL</th><th align=center>FECHA CREACI&Oacute;N</th><th align=center>FECHA CUMPLIMIENTO</th><th align=center>CLIENTE</th><th align=center>SOLICITA</th><th align=center>ESTATUS</th><th align=center>CAMBIO DE ESTATUS</th></tr>";
                if (datos.Count > 0)
                {
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
     
                    var unidadnegocio = datos.Select(x => x.estadoId).Distinct().ToList();
                    foreach (var ee in unidadnegocio)
                    {
                        var dtos = datos.Where(x => x.estadoId.Equals(ee));
                        var estados = db.UnidadNegocioEstados.Where(x => x.unidadnegocioId.Equals(ee)).Select(ide => ide.estadoId).ToList();
                        var GrtsEmail = db.Usuarios
                           .Where(u => u.TipoUsuarioId.Equals(12) && estados.Contains( u.Sucursal.estadoId ) && u.Activo.Equals(true))
                           .Select(u => u.emails.Select(e => e.email).FirstOrDefault())
                           .ToList();
                        if (GrtsEmail.Count() > 0)
                        {
                            foreach (var e in GrtsEmail)
                            {
                                m.To.Add(e);
                            }
       
                            body = "";
                            foreach (var r in dtos)
                            {
                                body = body + string.Format("<tr><td align=center>{0}</td><td align=center>{1}</td><td align=center>{2}</td><td align=center>{3}</td><td align=center>{4}</td>" +
                                                               "<td align=center>{5}</td><td align=center>{6}</td><td align=center>{7}</td><td align=center>{8}</td></tr>",
                                                               r.dias, r.Folio, r.VBtra, r.fch_Creacion, r.fch_Cumplimiento, r.Cliente, r.solicitante, r.estatus, r.fch_Modificacion);
                            }

                            body = inicio + body + "</table><p>Este correo es enviado de manera autom&aacute;tica con fines informativos, por favor no responda a esta direcci&oacute;n</p>";
                            body = body + "</body></html>";
                            m.Body = body;
                            m.IsBodyHtml = true;
                            smtp.EnableSsl = true;
                            smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["UserDamsa"], ConfigurationManager.AppSettings["PassDamsa"]);
                            smtp.Send(m);
                            body = "";
                            m.Body = "";
                            m.To.Clear();
                        }
                    }
                }
                var propietario = datos.Select(x => x.propietarioId).Distinct().ToList();
                foreach (var p in propietario)
                {
                    var aux = datos.Where(x => x.propietarioId.Equals(p)).ToList();
                    var emailSol = aux[0].emailSol;
                    m.To.Add(emailSol);
                    body = "";
                    foreach (var r in aux)
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
                    m.Body = "";
                    m.To.Clear();
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
            int[] estatusId = new int[] { 8, 9, 34, 35, 36, 37, 47, 48, 43 };
            try
            {
                string from = "noreply@damsa.com.mx";
                MailMessage m = new MailMessage();
                m.From = new MailAddress(from, "SAGA INN");
                m.Subject = "[SAGA] Requisiciones sin asignar";

                var requisSol = db.Database.SqlQuery<PausadasDto>("dbo.sp_RequisSinAsignar").ToList();
             
                if (requisSol.Count() > 0)
                {
                    var solicitantes = requisSol.Select(x => x.propietarioId).Distinct().ToList();
                    var inicio = "<html><head><style>td {border: solid black 1px;padding-left:5px;padding-right:5px;padding-top:1px;padding-bottom:1px;font-size:9pt;color:Black;font-family:'calibri';} " +
                                                "</style></head><body style=\"text-align:center; font-family:'calibri'; font-size:10pt;\"><table class='table'><tr><th align=center>DIAS SIN MODIFICAR</th><th align=center>FOLIO</th><th align=center>PERFIL</th><th align=center>FECHA ALTA</th><th align=center>FECHA CUMPLIMIENTO</th><th align=center>CLIENTE</th><th align=center>SOLICITA</th><th align=center># VACANTES</th><th align=center>ESTATUS</th><th align=center>CAMBIO DE ESTATUS</th></tr>";

                    var body = "";

                    foreach (var a in solicitantes)
                    {
                        var aux = requisSol.Where(x => x.propietarioId.Equals(a)).ToList();
                        m.To.Add(aux[0].emailSol);

                        foreach (var r in aux)
                        {
                            body = body + string.Format("<tr><td align=center>{0}</td><td align=center>{1}</td><td align=center>{2}</td><td align=center>{3}</td><td align=center>{4}</td>" +
                                                       "<td align=center>{5}</td><td align=center>{6}</td><td align=center>{7}</td><td align=center>{8}</td><td align=center>{9}</td></tr>",
                                                       r.dias, r.Folio, r.VBtra, r.fch_Creacion, r.fch_Cumplimiento, r.Cliente, r.solicitante, r.vacantes.ToString(), r.estatus, r.fch_Modificacion);
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

                    var aprobadores = requisSol.Select(x => x.aprobadorId).Distinct().ToList();

                    body = "";

                    foreach (var a in aprobadores)
                    {
                        if (a != auxID)
                        {
                            var aux = requisSol.Where(x => x.aprobadorId.Equals(a)).ToList();
                            if (aux.Count > 0)
                            {
                                m.To.Add(aux[0].email);

                                foreach (var r in aux)
                                {
                                    body = body + string.Format("<tr><td align=center>{0}</td><td align=center>{1}</td><td align=center>{2}</td><td align=center>{3}</td><td align=center>{4}</td>" +
                                                               "<td align=center>{5}</td><td align=center>{6}</td><td align=center>{7}</td><td align=center>{8}</td><td align=center>{9}</td></tr>",
                                                               r.dias, r.Folio, r.VBtra, r.fch_Creacion, r.fch_Cumplimiento, r.Cliente, r.solicitante, r.vacantes.ToString(), r.estatus, r.fch_Modificacion);
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
                m.Subject = "[SAGA] Requisiciones Vencidas";

                var datos = db.Database.SqlQuery<PausadasDto>("dbo.sp_RequisVencidas").ToList();

                string reclutadoresList = "";
                if (datos.Count > 0)
                {
                    var solicitantes = datos.Select(x => x.propietarioId).Distinct().ToList();
                    var inicio = "<html><head><style>td {border: solid black 1px;padding-left:5px;padding-right:5px;padding-top:1px;padding-bottom:1px;font-size:9pt;color:Black;font-family:'calibri';} " +
                                                "</style></head><body style=\"text-align:center; font-family:'calibri'; font-size:10pt;\"><table class='table'><tr><th align=center>DIAS SIN MODIFICAR</th><th align=center>FOLIO</th><th align=center>PERFIL</th><th align=center>FECHA ALTA</th><th align=center>FECHA CUMPLIMIENTO</th><th align=center>CLIENTE</th><th align=center>SOLICITA</th><th align=center>COORDINADOR</th><th align=center># VACANTES</th><th align=center>ESTATUS</th><th align=center>CAMBIO DE ESTATUS</th><th align=center>RECLUTADORES</th></tr>";

                    var body = "";

                    foreach (var a in solicitantes)
                    {
                        var aux = datos.Where(x => x.propietarioId.Equals(a)).ToList();
                        m.To.Add(aux[0].emailSol);

                        foreach (var r in aux)
                        {
                            var reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(r.Id) && x.Tipo.Equals(2)).Select(rec => new
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

                    var aprobadores = datos.Select(x => x.aprobadorId).Distinct().ToList();
                    body = "";
                    reclutadoresList = "";
                    foreach (var a in aprobadores)
                    {
                        var aux = datos.Where(x => x.aprobadorId.Equals(a)).ToList();

                        m.To.Add(aux[0].email);

                        foreach (var r in aux)
                        {
                            var reclutadores = db.AsignacionRequis.Where(x => x.RequisicionId.Equals(r.Id) && x.Tipo.Equals(2)).Select(rec => new
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
