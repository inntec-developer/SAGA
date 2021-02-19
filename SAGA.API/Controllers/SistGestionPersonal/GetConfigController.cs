using SAGA.API.Utilerias;
using SAGA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.API.Dtos;
using SAGA.BOL;

namespace SAGA.API.Controllers.SistGestionPersonal
{
    [RoutePrefix("api/gestionpersonal")]
    public class GetConfigController : ApiController
    {
        private SAGADBContext db;
        APISAGALog apilog = new APISAGALog();
        public GetConfigController()
        {
            db = new SAGADBContext();
        }

        [HttpGet]
        [Route("getConfigVacaciones")]
        [Authorize]
        public IHttpActionResult GetConfigVacaciones(Guid empleadoId)
        {
            try
            {
                var datos = db.ConfigVacaciones.Where(x => x.Id.Equals(db.GrupoVacaciones.Where(xx => xx.empleadoId.Equals(empleadoId)).Select(c => c.ConfigVacacionesId).FirstOrDefault()))
                    .Select(d => new
                    {
                        d.Id,
                        d.DiasContinuos,
                        d.DiasExpiran,
                        d.DiasIncremento,
                        diasUtilizados = db.PeriodoVacaciones.Where(x => x.CandidatosInfoId.Equals(empleadoId)).Count() > 0 ? db.PeriodoVacaciones.Where(x => x.CandidatosInfoId.Equals(empleadoId)).Select(dd => dd.dias).Sum() : 0,
                        periodo = db.PeriodoVacaciones.Where(x => x.CandidatosInfoId.Equals(empleadoId)).Select(dd => new
                        {
                            dd.dias,
                            fchInicio = dd.fchIncio,
                            dd.fchFin,
                            usuarioAlta = dd.UsuarioAlta.Nombre + " " + dd.UsuarioAlta.ApellidoPaterno + " " + dd.UsuarioAlta.ApellidoMaterno
                        }).ToList(),
                        antiguedad = db.ConfigVacacionesDias.Where(x => x.ConfigVacacionesId.Equals(d.Id)).Select(dd => new
                        {
                            dd.TiempoAntiguedad.Tiempo,
                            dd.Dias,
                         }).ToList(),
                    }).FirstOrDefault();

                return Ok(datos);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
        [HttpGet]
        [Route("getConfigDias")]
        [Authorize]
        public IHttpActionResult GetConfigDias(Guid empleadoId)
        {
            try
            {
                var datos = db.ConfigDiasEconomicosDias.Where(x => x.ConfigDiasEconomicosId.Equals(db.EmpleadoDiasEconomicos.Where(xx => xx.empleadoId.Equals(empleadoId)).Select(c => c.ConfigDiasEconomicosId).FirstOrDefault()))
                    .Select(d => new
                    {
                        Id = d.ConfigDiasEconomicosId,
                        dias = d.Dias,
                        diasUtilizados = db.PeriodoDE.Where(x => x.CandidatosInfoId.Equals(empleadoId) && x.TiposDiasEconomicosId.Equals(d.TiposDiasEconomicosId)).Count() > 0 ? db.PeriodoDE.Where(x => x.CandidatosInfoId.Equals(empleadoId) && x.TiposDiasEconomicosId.Equals(d.TiposDiasEconomicosId))
                        .Select(dd => dd.dias).Sum() : 0,
                        tipo = d.TiposDiasEconomicos.Nombre,
                        tipoId = d.TiposDiasEconomicosId
                    }).ToList();

                var periodo = db.PeriodoDE.Where(x => x.CandidatosInfoId.Equals(empleadoId)).Select(dd => new
                {
                    dd.fchFin,
                    fchInicio = dd.fchIncio,
                    tipoId = dd.TiposDiasEconomicosId,
                    tipo = dd.TiposDiasEconomicos.Nombre,
                    dd.dias
                }).ToList();
                return Ok(new { datos, periodo });
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
        [HttpGet]
        [Route("getConfigIncapacidades")]
        [Authorize]
        public IHttpActionResult GetConfigIncapacidades(Guid empleadoId)
        {
            try
            {
                var datos = db.ConfigIncapacidadesDias.Where(x => x.ConfigIncapacidadesId.Equals(db.EmpleadoIncapacidad.OrderByDescending(o => o.fch_Modificacion).Where(xx => xx.empleadoId.Equals(empleadoId)).Select(c => c.ConfigIncapacidadesId).FirstOrDefault()))
                    .Select(d => new
                    {
                        Id = d.ConfigIncapacidadesId,
                        dias = d.Dias,
                        tipo = d.TiposIncapacidad.Nombre,
                        tipoId = d.TiposIncapacidadId,
                        diasUtilizados = db.PeriodoIncapacidad.Where(x => x.CandidatosInfoId.Equals(empleadoId) && x.TiposIncapacidadId.Equals(d.TiposIncapacidadId)).Count() > 0 ? db.PeriodoIncapacidad.Where(x => x.CandidatosInfoId.Equals(empleadoId) && x.TiposIncapacidadId.Equals(d.TiposIncapacidadId))
                        .Select(dd => dd.dias).Sum() : 0,
                    }).ToList();

                var periodo = db.PeriodoIncapacidad.Where(x => x.CandidatosInfoId.Equals(empleadoId)).Select(dd => new
                {
                    dd.fchFin,
                    fchInicio = dd.fchIncio,
                    tipoId = dd.TiposIncapacidadId,
                    tipo = dd.TiposIncapacidad.Nombre,
                    dd.dias
                }).ToList();
                return Ok(new { datos, periodo });
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
        [HttpGet]
        [Route("getConfigPermisos")]
        [Authorize]
        public IHttpActionResult GetConfigPermisos(Guid empleadoId)
        {
            try
            {
                //var datos = db.ConfigJus.Where(x => x.ConfigIncapacidadesId.Equals(db.EmpleadoIncapacidad.OrderByDescending(o => o.fch_Modificacion).Where(xx => xx.empleadoId.Equals(empleadoId)).Select(c => c.ConfigIncapacidadesId).FirstOrDefault()))
                //    .Select(d => new
                //    {
                //        Id = d.ConfigIncapacidadesId,
                //        dias = d.Dias,
                //        tipo = d.TiposIncapacidad.Nombre,
                //        tipoId = d.TiposIncapacidadId,
                //        diasUtilizados = db.PeriodoIncapacidad.Where(x => x.CandidatosInfoId.Equals(empleadoId) && x.TiposIncapacidadId.Equals(d.TiposIncapacidadId)).Count() > 0 ? db.PeriodoIncapacidad.Where(x => x.CandidatosInfoId.Equals(empleadoId) && x.TiposIncapacidadId.Equals(d.TiposIncapacidadId))
                //        .Select(dd => dd.dias).Sum() : 0,
                //    }).ToList();

                var periodo = db.PeriodoPermisos.Where(x => x.CandidatosInfoId.Equals(empleadoId)).Select(dd => new
                {
                    dd.fchFin,
                    fchInicio = dd.fchIncio,
                    tipoId = dd.TipoJustificacionId,
                    tipo = dd.TipoJustificacion.Descripcion,
                    tipoP = dd.Tipo == 1 ? "PERMISO" : "FALTA",
                    dd.dias
                }).ToList();
                return Ok(periodo);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
        [HttpGet]
        [Route("getConfigHorasExtra")]
        [Authorize]
        public IHttpActionResult GetConfigHorasExtra(Guid clienteId)
        {
            try
            {
                    var candidatos = db.ProcesoCandidatos.Where(x => x.Estatus.Descripcion.ToLower().Equals("contratado")
                    && x.Requisicion.ClienteId.Equals(clienteId)).Select(C => C.CandidatoId).ToList();
                    var datos = db.CandidatosInfo.Where(x => candidatos.Contains(x.CandidatoId))
                    .Select(p => new
                    {
                        id = p.Id,
                        vacante = db.ProcesoCandidatos.OrderByDescending(o => o.Fch_Modificacion)
                        .Where(x => x.CandidatoId.Equals(p.CandidatoId)).Select(v => new
                        {
                            v.RequisicionId,
                            v.Requisicion.SueldoMinimo,
                            v.Requisicion.SueldoMaximo,
                            v.Requisicion.Cliente.Nombrecomercial,
                            v.Requisicion.ClienteId,
                            v.Requisicion.VBtra
                        }).FirstOrDefault(),
                        candidatoId = p.CandidatoId,
                        reclutador = db.Usuarios.Where(x => x.Id.Equals(p.ReclutadorId)).Select(r => r.Nombre + " " + r.ApellidoPaterno + " " + r.ApellidoMaterno).FirstOrDefault(),
                        nom = p.Nombre + " " + p.ApellidoPaterno + " " + p.ApellidoMaterno,
                        p.Nombre,
                        apellidoPaterno = p.ApellidoPaterno,
                        apellidoMaterno = p.ApellidoMaterno,
                        fch_Ingreso = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(p.Id)).Select(f => f.FechaIngreso).FirstOrDefault(),
                        edad = p.FechaNacimiento,
                        rfc = String.IsNullOrEmpty(p.RFC) ? "" : p.RFC,
                        curp = String.IsNullOrEmpty(p.CURP) ? "" : p.CURP,
                        nss = String.IsNullOrEmpty(p.NSS) ? "" : p.NSS,
                        genero = p.Genero.genero,
                        lada = db.Telefonos.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(l => l.ClaveLada).FirstOrDefault(),
                        telefono = db.Telefonos.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(l => l.telefono).FirstOrDefault(),
                        email = String.IsNullOrEmpty(db.Emails.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(e => e.email).FirstOrDefault()) ? "SIN REGISTRO" : db.Emails.Where(x => x.EntidadId.Equals(p.CandidatoId)).Select(e => e.email).FirstOrDefault(),
                        puesto = db.CandidatoLaborales.Where(x => x.CandidatoInfoId.Equals(p.Id)).Select(v => v.PuestosIngresos.Nombre).FirstOrDefault(),
                        foto = db.DocumentosCandidato.Where(x => x.candidatoId.Equals(p.Id) && x.Documento.Descripcion.ToLower().Equals("foto")).Select(r => r.Ruta).FirstOrDefault(),
                        horas = db.ConfigTiempoExtra.Where(x => x.Id.Equals(db.EmpleadoTiempoExtra.Where(xx => xx.empleadoId.Equals(p.Id)).Select(c => c.ConfigTiempoExtraId).FirstOrDefault()))
                        .Select(d => d.TE_Total).FirstOrDefault(),
                        tiempo = 2
                }).OrderBy(o => o.fch_Ingreso).ThenBy(oo => oo.apellidoPaterno).ToList();


                return Ok(datos);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
        [HttpGet]
        [Route("getConfigSuspensionByCliente")]
        [Authorize]
        public IHttpActionResult GetConfigSuspensionByCliente(Guid clienteId)
        {
            try
            {
                var datos = db.ConfigSuspensionNotasDias.Where(x => x.ConfigSuspensionNotas.ClienteId.Equals(clienteId)).Select(d => new
                {
                    id = d.ConfigSuspensionNotasId,
                    //maxDias = d.Tipo == 1 ? d.Dias : 0,
                    //retardos = d.Tipo == 2 ? d.Retardos : 0,
                    //maxDiasRetardo = d.Tipo == 2 ? d.Dias : 0,
                    //faltasMemo = d.Tipo == 3 ? d.Dias : 0,
                    //retardosmemo = d.Tipo == 3 ? d.Retardos : 0,
                    //recoRetardos = d.Tipo == 4 ? d.Retardos : 0,
                    //recoFaltas = d.Tipo == 4 ? d.Dias : 0,
                    //actaFaltas = d.Tipo == 5 ? d.Dias : 0,
                    d.Dias,
                    d.Retardos,
                    tipoId = d.Tipo,
                    tipo = d.Tipo == 2 ? "retardos" : d.Tipo == 3 ? "memo" : d.Tipo == 4 ? "reco" : d.Tipo == 5 ? "acta" : "maxdias"
                }).ToList();
                return Ok(datos);
            }
            catch (Exception ex)
            {
                return Ok(HttpStatusCode.BadRequest);
            }
        }
    }
}