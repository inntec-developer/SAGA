using SAGA.API.Dtos;
using SAGA.BOL;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/Damfo290")]
    [Authorize]
    public class DAMFOController : ApiController
    {
        private SAGADBContext db;
        Damfo290Dto DamfoDto;

        public DAMFOController()
        {
            db = new SAGADBContext();
            DamfoDto = new Damfo290Dto();
        }

        //api/Damfo290/getViewDamfos
        [HttpGet]
        [Route("getViewDamfos")]
        public IHttpActionResult Get()
        {
            var damfo290 = db.DAMFO290
                .Where(df => df.Activo)
                .OrderByDescending(df => df.fch_Creacion)
                .Select(df => new {
                    Id = df.Id,
                    Cliente = df.Cliente.Nombrecomercial,
                    NombrePerfil = df.NombrePerfil,
                    Vacantes = df.horariosPerfil.Count() > 0 ? df.horariosPerfil.Sum(h => h.numeroVacantes) : 0,
                    SueldoMinimo = df.SueldoMinimo,
                    SueldoMaximo = df.SueldoMaximo,
                    TipoReclutamiento = df.TipoReclutamiento.tipoReclutamiento,
                    ClaseReclutamiento = df.ClaseReclutamiento.clasesReclutamiento,
                    fch_Creacion = df.fch_Creacion,
                    horariosActivos = df.horariosPerfil.Where(hp => hp.Activo).Count() > 0 ? df.horariosPerfil.Where(hp => hp.Activo).Count() : 0,
                    usuarioAlta = df.UsuarioAlta,
                    usuarioCreacion = db.Usuarios.Where(x => x.Usuario.Equals(df.UsuarioAlta)).Select(n => n.Nombre + " " + n.ApellidoPaterno + " " + n.ApellidoMaterno).FirstOrDefault()
                }).ToList();


            return Ok(damfo290);
        }
        //api/Damfo290/getById

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
        public IHttpActionResult GetById(Guid Id)
        {
            try
            {
                var damfoGetById = db.DAMFO290.Where(x => x.Id.Equals(Id)).Select(r => new
                {
                    Id = r.Id,
                    arte = @"https://apisb.damsa.com.mx/utilerias/" + "img/ArteRequi/BG/" + r.Arte,
                    usuarioAlta = r.UsuarioAlta,
                    nombrePerfil = r.NombrePerfil,
                    clienteId = r.ClienteId,
                    horarios = r.horariosPerfil.Select(h => new
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
                        direcciones = r.Cliente.direcciones.Select(d => new
                        {
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
                                        infoAdicional = c.InfoAdicional,
                                        telefonos = db.Telefonos
                                            .Where(t => t.EntidadId == c.Id)
                                            .Select(t => new
                                            {
                                                tipo = t.TipoTelefono.Tipo,
                                                clavePais = t.ClavePais,
                                                claveLada = t.ClaveLada,
                                                telefono = t.telefono,
                                                extension = t.Extension
                                            })
                                            .ToList(),
                                        Email = db.Emails
                                            .Where(e => e.EntidadId == c.Id)
                                            .Select(e => new { email = e.email })
                                            .ToList(),
                                    }).ToList(),

                    },
                    tipoReclutamiento = r.TipoReclutamiento.tipoReclutamiento,
                    claseReclutamiento = r.ClaseReclutamiento.clasesReclutamiento,
                    tipoContrato = r.ContratoInicial.tipoContrato,
                    tipoContratoId = r.ContratoInicial.Id,
                    periodoPrueba = r.ContratoInicial.periodoPrueba,
                    tiempo = string.IsNullOrEmpty(r.TiempoContrato.Tiempo.ToString()) ? "Sin registro" : r.TiempoContrato.Tiempo,
                    areaExperiencia = r.Area.areaExperiencia,
                    genero = r.Genero.genero,
                    edadMinima = r.EdadMinima,
                    edadMaxima = r.EdadMaxima,
                    estadoCivil = r.EstadoCivil.estadoCivil,
                    sueldoMinimo = r.SueldoMinimo,
                    sueldoMaximo = r.SueldoMaximo,
                    escolaridades = r.escolardadesPerfil.Select(es => new
                    {
                        gradoEstudio = es.Escolaridad.gradoEstudio,
                        estadoEstudio = es.EstadoEstudio.estadoEstudio
                    }).ToList(),
                    aptitudes = r.aptitudesPerfil.Select(a => new
                    {
                        aptitud = a.Aptitud.aptitud
                    }).ToList(),
                    experiencia = r.Experiencia,
                    diaCorte = r.DiaCorte.diaSemana,
                    tipoDeNomina = r.TipoNomina.tipoDeNomina,
                    diaPago = r.DiaPago.diaSemana,
                    periodoPago = r.PeriodoPago.periodoPago,
                    especifique = r.Especifique,
                    beneficios = r.beneficiosPerfil.Select(bn => new
                    {
                        tipoBeneficio = bn.TipoBeneficio.tipoBeneficio,
                        cantidad = bn.Cantidad,
                        observaciones = bn.Observaciones,
                    }).ToList(),
                    actividades = r.actividadesPerfil.Select(ac => new
                    {
                        actividades = ac.Actividades
                    }).ToList(),
                    observaciones = r.observacionesPerfil.Select(ob => new
                    {
                        observaciones = ob.Observaciones
                    }).ToList(),
                    procesos = r.procesoPerfil
                    .OrderBy(pr => pr.Orden)
                    .Select(pr => new
                    {
                        proceso = pr.Proceso
                    }).ToList(),
                    documentosCliente = r.documentosCliente.Select(dcr => new
                    {
                        documento = dcr.Documento
                    }).ToList(),
                    prestacionesCliente = r.prestacionesCliente.Select(pcr => new
                    {
                        prestamo = pcr.Prestamo
                    }).ToList(),
                    psicometriasDamsa = r.psicometriasDamsa.Select(pd => new
                    {
                        tipoPsicometria = pd.Psicometria.tipoPsicometria,
                        descripcion = pd.Psicometria.descripcion
                    }).ToList(),
                    psicometriasCliente = r.psicometriasCliente.Select(pc => new
                    {
                        psicometria = pc.Psicometria,
                        descripcion = pc.Descripcion
                    }).ToList(),
                    competenciasCardinal = r.competenciasCardinalPerfil.Select(cc => new
                    {
                        competencia = cc.Competencia.competenciaCardinal,
                        nivel = cc.Nivel
                    }).ToList(),
                    competenciasArea = r.competenciasAreaPerfil.Select(ca => new
                    {
                        competencia = ca.Competencia.competenciaArea,
                        nivel = ca.Nivel
                    }).ToList(),
                    competenciasGerencial = r.competetenciasGerencialPerfil.Select(cg => new
                    {
                        competencia = cg.Competencia.competenciaGerencial,
                        nivel = cg.Nivel
                    }).ToList()


                }).FirstOrDefault();

                //var mocos = damfoGetById.Select(rr => new
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
                //    Arte = this.GetImage(rr.arte),
                //    bg = rr.arte,
                //    nombrePerfil = rr.nombrePerfil

                //}).FirstOrDefault();


                return Ok(damfoGetById);
            }
            catch (Exception ex)
            {
                string mssg = ex.Message;
                return Ok(mssg);
            }
        }

        [HttpGet]
        [Route("getDamfoRutasCamion")]
        public IHttpActionResult GetRutasCamion(Guid Id)
        {
            try
            {
                var direccion = db.Direcciones
                    .Where(x => x.EntidadId.Equals(Id))
                    .Select(x => x.Id).ToList();
                var rutas = db.RutasPerfil
                    .Where(r => direccion.Contains(r.DireccionId))
                    .Select(r => new
                    {
                        Direccion = r.Direccion.Calle,
                        Ruta = r.Ruta,
                        Via = r.Via
                    }).ToList().OrderBy(r => r.Direccion);
                return Ok(rutas);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("getVacantesDamfo")]
        public IHttpActionResult GetHorarios(Guid Id)
        {
            try
            {
                // Recuperamos el nombre de los hprarios y las vacantes diponibles
                var vacantes = db.HorariosPerfiles
                                .Where(h => h.DAMFO290Id.Equals(Id))
                                .Where(h => h.Activo.Equals(true))
                                .Select(h => new
                                {
                                    Nombre = h.Nombre,
                                    vacantes = h.numeroVacantes
                                })
                                .ToList();
                return Ok(vacantes);
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
                return Ok(HttpStatusCode.NotFound);
            }
        }
    }
}
