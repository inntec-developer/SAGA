using SAGA.DAL;
using SAGA.BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SAGA.API.Dtos;
using SAGA.API.Controllers.Admin;

namespace SAGA.API.Controllers
{
    [RoutePrefix("api/Catalogos")]
    public class CatalogosController : ApiController
    {
        private SAGADBContext db;
        public CatalogosController()
        {
            db = new SAGADBContext();
        }

        #region Catalogos

        [HttpGet]
        [Route("getCatalogoForId")]
        public IHttpActionResult GetCatalgoForId(int IdCatalogo)
        {
            switch (IdCatalogo)
            {
                #region Sistemas
                case 6: // Tipo de telefonos

                    var TpTelefono = db.TiposTelefonos
                        .Where(x => x.Activo.Equals(true))
                        .Select(t => new TpTelefonosDto
                        {
                            Id = t.Id,
                            Tipo = t.Tipo,
                            Activo = t.Activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(TpTelefono);

                case 7: // Estados Civiles

                    var EstadoCivil = db.EstadosCiviles
                        .Where(x => x.Activo.Equals(true))
                        .Select(t => new EstadoCivilDto
                        {
                            Id = t.Id,
                            estadoCivil = t.estadoCivil,
                            Activo = t.Activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(EstadoCivil);

                case 41: // Tipo de usuario

                    var TpUsuario = db.TiposUsuarios
                        .Select(t => new TpUsuarioDto
                        {
                            Id = t.Id,
                            tipo = t.Tipo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(TpUsuario);

                case 42: // Departamentos

                    var Departamentos = db.Departamentos
                        .Select(d => new
                        {
                            Id = d.Id,
                            nombre = d.Nombre,
                            Area = d.AreaId,
                            clave = d.Clave,
                            orden = d.Orden
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(Departamentos);

                case 43: // Areas

                    var Areas = db.Areas
                        .Select(a => new AreaDto
                        {
                            Id = a.Id,
                            Nombre = a.Nombre,
                            Clave = a.Clave,
                            Orden = a.Orden
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(Areas);

                case 44: // Roles

                    var Roles = db.Roles
                        .Where(x => x.Activo.Equals(true))
                        .Select(a => new RolesDto
                        {
                            Id = a.Id,
                            Rol = a.Rol,
                            Activo = a.Activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(Roles);

                #endregion

                #region Reclutamiento
                case 34: // Escolaridades

                    var Escolaridades = db.GradosEstudios
                        .Select(e => new EscolaridadesDto
                        {
                            Id = e.Id,
                            gradoEstudio = e.gradoEstudio
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(Escolaridades);

                case 35: // Nivel estudios

                    var Nivel = db.Niveles
                        .Select(e => new NivelDto
                        {
                            Id = e.Id,
                            nivel = e.nivel
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(Nivel);

                case 36: // Medios

                    var Medio = db.Medios
                        .Where(x => x.Activo.Equals(true))
                        .Select(e => new MedioDto
                        {
                            Id = e.Id,
                            Nombre = e.Nombre,
                            Activo = e.Activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(Medio);

                case 37: // Idiomas

                    var Idioma = db.Idiomas
                        .Where(x => x.Activo.Equals(true))
                        .Select(e => new IdiomaDto
                        {
                            Id = e.Id,
                            idioma = e.idioma,
                            activo = e.Activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(Idioma);

                case 38: // Discapacidades

                    var Discapacidad = db.TiposDiscapacidades
                        .Select(e => new DiscapacidadDto
                        {
                            Id = e.Id,
                            tipoDiscapacidad = e.tipoDiscapacidad,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(Discapacidad);

                case 39: // Tipo Licencia

                    var TipoLicencia = db.TiposLicencias
                        .Select(e => new TipoLicenciaDto
                        {
                            Id = e.Id,
                            Descripcion = e.Descripcion,
                            tipoLicencia = e.tipoLicencia,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(TipoLicencia);

                case 40: // Tipo Examen

                    var TipoExamen = db.TipoExamen
                        .Where(x => x.Activo.Equals(true))
                        .Select(e => new TipoExamenDto
                        {
                            Id = e.Id,
                            Nombre = e.Nombre,
                            Descripcion = e.Descripcion,
                            Activo = e.Activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(TipoExamen);

                #endregion

                #region Ventas
                case 8: // Giro del cliente

                    var GiroEmpresa = db.GirosEmpresas
                        .Select(e => new
                        {
                            Id = e.Id,
                            giroEmpresa = e.giroEmpresa,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(GiroEmpresa);

                case 9: // Actividad cliente

                    var ActividadEmpresa = db.ActividadesEmpresas
                        .Select(e => new
                        {
                            Id = e.Id,
                            actividadEmpresa = e.actividadEmpresa,
                            GiroEmpresa = e.GiroEmpresaId,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(ActividadEmpresa);

                case 10: // Tamaño de empresa

                    var TamanoEmpresa = db.TamanoEmpresas
                        .Select(e => new TamanoEmpresaDto
                        {
                            Id = e.Id,
                            tamanoEmpresa = e.tamanoEmpresa,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(TamanoEmpresa);

                case 11: // Tipo base

                    var TiposBase = db.TiposBases
                        .Select(e => new TiposBasesDto
                        {
                            Id = e.Id,
                            tipoBase = e.tipoBase,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(TiposBase);

                case 16: // Perfil puesto

                    var PerfilExperiencia = db.PerfilExperiencia
                        .Select(e => new PerfilExpDto
                        {
                            Id = e.Id,
                            perfilExperiencia = e.perfilExperiencia,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(PerfilExperiencia);

                case 17: // Aptitudes

                    var Aptitud = db.Aptitudes
                        .Select(e => new AptitudDto
                        {
                            Id = e.Id,
                            aptitud = e.aptitud,
                            activo = e.activo
                        })
                        .OrderBy(c => c.aptitud)
                        .ToList();

                    return Ok(Aptitud);

                case 18: // Categorias

                    var AreaExperiencia = db.AreasExperiencia
                        .Where(x => x.Activo.Equals(true))
                        .Select(e => new AreaExpDto
                        {
                            Id = e.Id,
                            areaExperiencia = e.areaExperiencia,
                            Icono = e.Icono,
                            Activo = e.Activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(AreaExperiencia);

                case 19: // Subcategorias

                    
                    var AreaInteres = db.AreasInteres
                        .Where(x => x.Activo.Equals(true))
                        .Select(e => new
                        {
                            Id = e.Id,
                            AreaExperienciaId = e.AreaExperienciaId,
                            areaInteres = e.areaInteres,
                            Activo = e.Activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();


                    return Ok(AreaInteres);

                case 20: // Jornada Loboral

                    var JornadaLaboral = db.JornadasLaborales
                        .Select(e => new
                        {
                            Id = e.Id,
                            Jornada = e.Jornada,
                            VariosHorarios = e.VariosHorarios,
                            Orden = e.Orden,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(JornadaLaboral);

                case 21: // Modalidad Loboral

                    var TipoModalidad = db.TiposModalidades
                        .Select(e => new
                        {
                            Id = e.Id,
                            Modalidad = e.Modalidad,
                            Orden = e.Orden,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(TipoModalidad);

                case 22: // Psicometrias

                    var TipoPsicometria = db.TiposPsicometrias
                        .Select(e => new TiposPiscoDto
                        {
                            Id = e.Id,
                            tipoPsicometria = e.tipoPsicometria,
                            descripcion = e.descripcion,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(TipoPsicometria);

                case 23: // Días de la semana

                    var DiasSemana = db.DiasSemanas
                        .Select(e => new DiasSemanaDto
                        {
                            Id = e.Id,
                            diaSemana = e.diaSemana,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(DiasSemana);

                case 24: // Tipos de nomina

                    var TipoNomina = db.TiposNominas
                        .Select(e => new TpNominaDto
                        {
                            Id = e.Id,
                            tipoDeNomina = e.tipoDeNomina,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(TipoNomina);

                case 26: // Periodos de pago

                    var PeriodoPago = db.PeriodosPagos
                        .Select(e => new PeriodoPagoDto
                        {
                            Id = e.Id,
                            periodoPago = e.periodoPago,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(PeriodoPago);

                case 27: // Beneficios

                    var BeneficioPerfil = db.TiposBeneficios
                        .Select(e => new BeneficiosPerfilDto
                        {
                            Id = e.Id,
                            tipoBeneficio = e.tipoBeneficio,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(BeneficioPerfil); 

                case 28: // Tipo de contrato

                    var TipoContrato = db.TiposContrato
                        .Select(e => new TipoContratoDto
                        {
                            Id = e.Id,
                            tipoContrato = e.tipoContrato,
                            periodoPrueba = e.periodoPrueba,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(TipoContrato);

                case 29: // Tiempos contrato

                    var TiemposContrato = db.TiemposContratos
                        .Select(e => new TiemposContratoDto
                        {
                            Id = e.Id,
                            Tiempo = e.Tiempo,
                            Orden = e.Orden,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(TiemposContrato);

                case 31: // Documentos DAMSA

                    var DocDamsa = db.DocumentosDamsa
                        .Select(e => new DocDamsaDto
                        {
                            Id = e.Id,
                            documentoDamsa = e.documentoDamsa,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(DocDamsa);

                case 32: // Prestaciones Ley

                    var PrestacionesLey = db.PrestacionesLey
                        .Select(e => new PrestacionesdeLeyDto
                        {
                            Id = e.Id,
                            prestacionLey = e.prestacionLey,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    return Ok(PrestacionesLey);
                case 45: // Estados Estudio
                    var EstadosEstudio = db.EstadosEstudios
                        .Select(e => new EstadoEstudioDto
                        {
                            Id = e.Id,
                            Nivel = e.estadoEstudio
                        }).ToList();
                    return Ok(EstadosEstudio);
                #endregion

                default:
                    break;
            }
            return Ok(HttpStatusCode.NotFound);
        }

        [HttpGet]
        [Route("getDocDamsa")]
        [Authorize]
        public IHttpActionResult GetDocDamsa()
        {
            try
            {
                var documentosDamsa = db.DocumentosDamsa.ToList();
                return Ok(documentosDamsa);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpGet]
        [Route("getPrestacionesLey")]
        [Authorize]
        public IHttpActionResult GetPrestacionesLey()
        {
            try
            {
                var prestacionesLey = db.PrestacionesLey.ToList();
                return Ok(prestacionesLey);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpGet]
        [Route("getPaises")]
        [Authorize]
        public IHttpActionResult GetPaises()
        {
            CandidatosDto Paises = new CandidatosDto();

            Paises.Paises = (from pais in db.Paises
                             where pais.Id == 42
                             select pais).ToList();

            return Ok(Paises);
        }

        [HttpGet]
        [Route("getDepa")]
        public IHttpActionResult getDepartamento()
        {
            var tu = db.Departamentos.Select(t => new { t.Id, t.Nombre }).OrderBy(x => x.Nombre).ToList();
            return Ok(tu);
        }

        [HttpGet]
        [Route("getTipos")]
        [Authorize]
        public IHttpActionResult getTiposUsuarios()
        {
            var tu = db.TiposUsuarios.Select(t => new { t.Id, t.Tipo }).ToList();
            return Ok(tu);
        }

        [HttpGet]
        [Route("getGrupos")]
        [Authorize]
        public IHttpActionResult getGrupos()
        {
            PersonalController obj = new PersonalController();
            List<GruposDtos> data = new List<GruposDtos>();
            var grupos = db.Grupos.Select(g => new
            {
                Id = g.Id,
                Foto = g.Foto,
                Activo = g.Activo,
                Descripcion = g.Descripcion,
                Nombre = g.Nombre,
                UsuarioAlta = g.UsuarioAlta,
                TipoGrupoId = db.TiposUsuarios.Where(x => x.Id.Equals(g.TipoGrupoId)).Select(id => id.Id).FirstOrDefault(),
                TipoGrupo = db.TiposUsuarios.Where(x => x.Id.Equals(g.TipoGrupoId)).Select(n => n.Tipo).FirstOrDefault()
            }).OrderBy(g => g.Nombre).ToList();

            foreach (var g in grupos)
            {
                var aux = obj.GetImage(g.Foto);
                data.Add(new GruposDtos
                {
                    Id = g.Id,
                    Foto = g.Foto,
                    Activo = g.Activo,
                    Descripcion = g.Descripcion,
                    Nombre = g.Nombre,
                    UsuarioAlta = g.UsuarioAlta,
                    TipoGrupoId = g.TipoGrupoId,
                    TipoGrupo = g.TipoGrupo,
                    FotoAux = aux
                });
            }

            obj = null;
            return Ok(data);
        }

        [HttpGet]
        [Route("getRoles")]
        [Authorize]
        public IHttpActionResult getRoles()
        {
            var roles = db.Roles.Where(x => x.Activo).ToList();
            return Ok(roles);
        }

        [HttpGet]
        [Route("getPrioridades")]
        [Authorize]
        public IHttpActionResult GetRPioridad()
        {
            var prioridad = db.Prioridades
                .Where(x => x.Activo.Equals(true))
                .ToList();
            return Ok(prioridad);
        }

        [HttpGet]
        [Route("getEstatus")]
        [Authorize]
        public IHttpActionResult getEstatus(int tipoMov)
        {
            var estatus = db.Estatus
                    .Where(x => x.TipoMovimiento.Equals(tipoMov))
                    .Where(x => x.Activo.Equals(true))
                    .ToList();
            return Ok(estatus);
        }

        [HttpGet]
        [Route("getMotivosLiberacion")]
        [Authorize]
        public IHttpActionResult GetMotivosLiberacion()
        {
            var motivo = db.MotivosLiberacion.Where(m => m.Activo.Equals(true)).ToList();
            return Ok(motivo);
        }

        [HttpGet]
        [Route("getTiposActividadesRecl")]
        [Authorize]
        public IHttpActionResult GetTiposActividadesRecl()
        {
            var actividad = db.TipoActividadReclutador
                                .Where(x => x.Activo.Equals(true))
                                .ToList().OrderBy(x => x.Actividad);
            return Ok(actividad);
        }

        [HttpGet]
        [Route("getTipoTelefono")]
        [Authorize]
        public IHttpActionResult GetTipoTelefono()
        {
            var tipo = db.TiposTelefonos.ToList();
            return Ok(tipo);
        }

        [HttpGet]
        [Route("getTipoDireccion")]
        [Authorize]
        public IHttpActionResult GetTipoDireccion()
        {
            var tipo = db.TiposDirecciones.ToList();
            return Ok(tipo);
        }

        #endregion

        #region Catalogos para Prospectos Clientes

        [HttpGet]
        [Route("getGiroEmp")]
        [Authorize]
        public IHttpActionResult GetGiroEmpresa()
        {
            var giro = db.GirosEmpresas.ToList();
            return Ok(giro);
        }

        [HttpGet]
        [Route("getActividadEmp")]
        [Authorize]
        public IHttpActionResult GetActivadadEmpresa(int GiroId)
        {
            var Actividad = db.ActividadesEmpresas
                .Select(x => new
                {
                    x.Id,
                    x.GiroEmpresaId,
                    x.actividadEmpresa
                })
                .Where(x => x.GiroEmpresaId.Equals(GiroId)).ToList();
            return Ok(Actividad);
        }

        [HttpGet]
        [Route("getTamanioEmp")]
        [Authorize]
        public IHttpActionResult GetTamanioEmpresa()
        {
            var tamanio = db.TamanoEmpresas.ToList();
            return Ok(tamanio);
        }

        [HttpGet]
        [Route("getTipoEmp")]
        [Authorize]
        public IHttpActionResult GetTipoempresa()
        {
            var tipo = db.TiposEmpresas.ToList();
            return Ok(tipo);
        }

        [HttpGet]
        [Route("getTipoBase")]
        [Authorize]
        public IHttpActionResult GetTipoBase()
        {
            var tipo = db.TiposBases.ToList();
            return Ok(tipo);
        }
        #endregion

        #region Localidades
        [HttpGet]
        [Route("getPais")]
        [Authorize]
        public IHttpActionResult GetPais()
        {
            var pais = db.Paises.Where(x => x.Id.Equals(42)).ToList();
            return Ok(pais);
        }

        [HttpGet]
        [Route("getEstado")]
        [Authorize]
        public IHttpActionResult GetEstado(int PaisId)
        {
            var estado = db.Estados
                .OrderBy(x => x.estado)
                .Where(x => x.PaisId.Equals(PaisId))
                .Select(x => new
                {
                    x.Id,
                    x.estado,
                    x.Clave
                })
                .ToList();
            return Ok(estado);
        }

        [HttpGet]
        [Route("getMunicipio")]
        [Authorize]
        public IHttpActionResult GetMunicipo(int EstadoId)
        {
            var municipio = db.Municipios
                .OrderBy(x => x.municipio)
                .Where(x => x.EstadoId.Equals(EstadoId))
                .Select(x => new
                {
                    x.Id,
                    x.municipio
                })
                .ToList();
            return Ok(municipio);
        }

        [HttpGet]
        [Route("getColonia")]
        [Authorize]
        public IHttpActionResult GetColonias(int MunicipioId)
        {
            var municipio = db.Colonias
                .OrderBy(x => x.colonia)
                .Where(x => x.MunicipioId.Equals(MunicipioId))
                .Select(x => new
                {
                    x.Id,
                    x.colonia,
                    x.CP
                })
                .ToList();
            return Ok(municipio);
        }

        [HttpGet]
        [Route("getInfoCP")]
        [Authorize]
        public IHttpActionResult GetInfoCP(string CP)
        {
            var info = db.Colonias
                .Where(x => x.CP.Equals(CP))
                .Select(x => new
                {
                    x.Id,
                    x.colonia,
                    x.CP,
                    x.PaisId,
                    x.EstadoId,
                    x.MunicipioId,
                }).ToList().OrderBy(x => x.colonia);
            return Ok(info);
        }
        #endregion

        #region Menu de Catalogos

        [HttpGet]
        [Route("getCatalogos")]
        [Authorize]
        public IHttpActionResult GetCatalogos()
        {
            var Catalogos = db.Estructuras
                .Select(x => new
                {
                    x.Id,
                    x.IdPadre,
                    x.Nombre,
                    x.Descripcion,
                    x.Activo,
                    Catalogos = db.Catalogos.Where( c => c.EstructuraId == x.Id).ToList()
                })
                .Where(e => e.Activo.Equals(true) && e.IdPadre.Equals(1) && e.Id != 1 && e.Catalogos.Count > 0)
                .OrderBy(e => e.IdPadre)
                .ToList();

            return Ok(Catalogos);
        }

        [HttpGet]
        [Route("getCatalogosComplete")]
        [Authorize]
        public IHttpActionResult getCatalogosComplete(int IdCatalogo )
        {
            CatalogosDto Catalogo = new CatalogosDto();
            //Buscamos los datos del catalogo
            Catalogo.Catalogos = db.Catalogos
             .Where(c => c.Id.Equals(IdCatalogo))
             .SingleOrDefault();

            Catalogo.Log = db.LogCatalogos
                        .Where(c => c.CatalogoId.Equals(IdCatalogo))
                        .OrderBy(c => c.FechaAct)
                        .ToList();

            switch (IdCatalogo)
            {
                #region Sistemas
                case 1: // Paises

                    Catalogo.Pais = db.Paises
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 2: // Estados

                    Catalogo.Pais = db.Paises.ToList();
                    Catalogo.Estado = db.Estados
                        .Select( e => new EstadoDto
                        {
                            Id = e.Id,
                            estado = e.estado,
                            Clave = e.Clave,
                            Pais = e.Pais.pais,
                            Activo = e.Activo                   
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 3: // Municipios

                    //Catalogo.Pais = db.Paises.ToList();
                    Catalogo.Estado = db.Estados
                        .Select(e => new EstadoDto
                        {
                            Id = e.Id,
                            estado = e.estado,
                            Clave = e.Clave,
                            Pais = e.Pais.pais,
                            Activo = e.Activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 4: // Colonias

                    Catalogo.Pais = db.Paises.ToList();
                    Catalogo.Estado = db.Estados
                        .Select(e => new EstadoDto
                        {
                            Id = e.Id,
                            estado = e.estado,
                            Clave = e.Clave,
                            Pais = e.Pais.pais,
                            Activo = e.Activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();
                    Catalogo.Municipio = db.Municipios
                        .Select(m => new MunicipioDto
                        {
                            Id = m.Id,
                            municipio = m.municipio,
                            Estado = m.Estado.estado,
                            Activo = m.Activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();
                    

                    break;

                case 6: // Tipo de telefonos

                    Catalogo.TpTelefono = db.TiposTelefonos
                        .Select(t => new TpTelefonosDto
                        {
                            Id = t.Id,
                            Tipo = t.Tipo,
                            Activo = t.Activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 7: // Estados Civiles

                    Catalogo.EstadoCivil = db.EstadosCiviles
                        .Select(t => new EstadoCivilDto
                        {
                            Id = t.Id,
                            estadoCivil = t.estadoCivil,
                            Activo = t.Activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 41: // Tipo de usuario

                    Catalogo.TpUsuario = db.TiposUsuarios
                        .Select(t => new TpUsuarioDto
                        {
                            Id = t.Id,
                            tipo = t.Tipo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 42: // Departamentos

                    Catalogo.Areas = db.Areas
                        .Select(a => new AreaDto
                        {
                            Id = a.Id,
                            Nombre = a.Nombre,
                            Clave = a.Clave,
                            Orden = a.Orden
                        })
                        .OrderBy(c => c.Id)
                        .ToList();
                    Catalogo.Departamentos = db.Departamentos
                        .Select(d => new DepartamentosDto
                        {
                            Id = d.Id,
                            nombre = d.Nombre,
                            Area = d.Area.Nombre,
                            clave = d.Clave,
                            orden = d.Orden
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 43: // Areas

                    Catalogo.Areas = db.Areas
                        .Select(a => new AreaDto
                        {
                            Id = a.Id,
                            Nombre = a.Nombre,
                            Clave = a.Clave,
                            Orden = a.Orden
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 44: // Roles

                    Catalogo.Roles = db.Roles
                        .Select(a => new RolesDto
                        {
                            Id = a.Id,
                            Rol = a.Rol,
                            Activo = a.Activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                #endregion

                #region Reclutamiento
                case 34: // Escolaridades

                    Catalogo.Escolaridades = db.GradosEstudios
                        .Select(e => new EscolaridadesDto
                        {
                            Id = e.Id,
                            gradoEstudio = e.gradoEstudio
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 35: // Nivel estudios

                    Catalogo.Nivel = db.Niveles
                        .Select(e => new NivelDto
                        {
                            Id = e.Id,
                            nivel = e.nivel
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 36: // Medios

                    Catalogo.Medio = db.Medios
                        .Select(e => new MedioDto
                        {
                            Id = e.Id,
                            Nombre = e.Nombre,
                            Activo = e.Activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 37: // Idiomas

                    Catalogo.Idioma = db.Idiomas
                        .Select(e => new IdiomaDto
                        {
                            Id = e.Id,
                            idioma = e.idioma,
                            activo = e.Activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();                   

                    break;

                case 38: // Discapacidades

                    Catalogo.Discapacidad = db.TiposDiscapacidades
                        .Select(e => new DiscapacidadDto
                        {
                            Id = e.Id,
                            tipoDiscapacidad = e.tipoDiscapacidad,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 39: // Tipo Licencia

                    Catalogo.TipoLicencia = db.TiposLicencias
                        .Select(e => new TipoLicenciaDto
                        {
                            Id = e.Id,
                            Descripcion = e.Descripcion,
                            tipoLicencia = e.tipoLicencia,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 40: // Tipo Examen

                    Catalogo.TipoExamen = db.TipoExamen
                        .Select(e => new TipoExamenDto
                        {
                            Id = e.Id,
                            Nombre = e.Nombre,
                            Descripcion = e.Descripcion,
                            Activo = e.Activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                #endregion

                #region Ventas
                case 8: // Giro del cliente

                    Catalogo.GiroEmpresa = db.GirosEmpresas
                        .Select(e => new GiroEmpresaDto
                        {
                            Id = e.Id,
                            giroEmpresa = e.giroEmpresa,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 9: // Actividad cliente

                    Catalogo.GiroEmpresa = db.GirosEmpresas
                        .Where( g => g.activo.Equals(true))
                        .Select(g => new GiroEmpresaDto
                        {
                            Id = g.Id,
                            giroEmpresa = g.giroEmpresa,
                            activo = g.activo
                        })
                        .ToList();

                    Catalogo.ActividadEmpresa = db.ActividadesEmpresas
                        .Select(e => new ActividadEmpresaDto
                        {
                            Id = e.Id,
                            actividadEmpresa = e.actividadEmpresa,
                            GiroEmpresa = e.GiroEmpresas.giroEmpresa,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 10: // Tamaño de empresa

                    Catalogo.TamanoEmpresa = db.TamanoEmpresas
                        .Select(e => new TamanoEmpresaDto
                        {
                            Id = e.Id,
                            tamanoEmpresa = e.tamanoEmpresa,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 11: // Tipo base

                    Catalogo.TiposBase = db.TiposBases
                        .Select(e => new TiposBasesDto
                        {
                            Id = e.Id,
                            tipoBase = e.tipoBase,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 16: // Perfil puesto

                    Catalogo.PerfilExperiencia = db.PerfilExperiencia
                        .Select(e => new PerfilExpDto
                        {
                            Id = e.Id,
                            perfilExperiencia = e.perfilExperiencia,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 17: // Aptitudes

                    Catalogo.Aptitud = db.Aptitudes
                        .Select(e => new AptitudDto
                        {
                            Id = e.Id,
                            aptitud = e.aptitud,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 18: // Categorias

                    Catalogo.AreaExperiencia = db.AreasExperiencia
                        .Select(e => new AreaExpDto
                        {
                            Id = e.Id,
                            areaExperiencia = e.areaExperiencia,
                            Icono = e.Icono,
                            Activo = e.Activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 19: // Subcategorias

                    Catalogo.AreaExperiencia = db.AreasExperiencia
                       .Where(a => a.Activo.Equals(true))
                       .Select(e => new AreaExpDto
                       {
                           Id = e.Id,
                           areaExperiencia = e.areaExperiencia,
                           Icono = e.Icono,
                           Activo = e.Activo
                       })
                       .OrderBy(c => c.Id)
                       .ToList();

                    Catalogo.AreaInteres = db.AreasInteres
                        .Select(e => new AreaInteresDto
                        {
                            Id = e.Id,
                            AreaExperiencia = e.AreaExperiencia.areaExperiencia,
                            areaInteres = e.areaInteres,
                            Activo = e.Activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 20: // Jornada Loboral

                    Catalogo.JornadaLaboral = db.JornadasLaborales
                        .Select(e => new JornadaLaboralDto
                        {
                            Id = e.Id,
                            Jornada = e.Jornada,
                            VariosHorarios = e.VariosHorarios,
                            Orden = e.Orden,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 21: // Modalidad Loboral

                    Catalogo.TipoModalidad = db.TiposModalidades
                        .Select(e => new TpModalidadDto
                        {
                            Id = e.Id,
                            Modalidad = e.Modalidad,
                            Orden = e.Orden,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 22: // Psicometrias

                    Catalogo.TipoPsicometria = db.TiposPsicometrias
                        .Select(e => new TiposPiscoDto
                        {
                            Id = e.Id,
                            tipoPsicometria = e.tipoPsicometria,
                            descripcion = e.descripcion,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 23: // Días de la semana

                    Catalogo.DiasSemana = db.DiasSemanas
                        .Select(e => new DiasSemanaDto
                        {
                            Id = e.Id,
                            diaSemana = e.diaSemana,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 24: // Tipos de nomina

                    Catalogo.TipoNomina = db.TiposNominas
                        .Select(e => new TpNominaDto
                        {
                            Id = e.Id,
                            tipoDeNomina = e.tipoDeNomina,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 26: // Periodos de pago

                    Catalogo.PeriodoPago = db.PeriodosPagos
                        .Select(e => new PeriodoPagoDto
                        {
                            Id = e.Id,
                            periodoPago = e.periodoPago,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 27: // Beneficios

                    Catalogo.BeneficioPerfil = db.TiposBeneficios
                        .Select(e => new BeneficiosPerfilDto
                        {
                            Id = e.Id,
                            tipoBeneficio = e.tipoBeneficio,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 28: // Tipo de contrato

                    Catalogo.TipoContrato = db.TiposContrato
                        .Select(e => new TipoContratoDto
                        {
                            Id = e.Id,
                            tipoContrato = e.tipoContrato,
                            periodoPrueba = e.periodoPrueba,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 29: // Tiempos contrato

                    Catalogo.TiemposContrato = db.TiemposContratos
                        .Select(e => new TiemposContratoDto
                        {
                            Id = e.Id,
                            Tiempo = e.Tiempo,
                            Orden = e.Orden,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 31: // Documentos DAMSA

                    Catalogo.DocDamsa = db.DocumentosDamsa
                        .Select(e => new DocDamsaDto
                        {
                            Id = e.Id,
                            documentoDamsa = e.documentoDamsa,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;

                case 32: // Prestaciones Ley

                    Catalogo.PrestacionesLey = db.PrestacionesLey
                        .Select(e => new PrestacionesdeLeyDto
                        {
                            Id = e.Id,
                            prestacionLey = e.prestacionLey,
                            activo = e.activo
                        })
                        .OrderBy(c => c.Id)
                        .ToList();

                    break;
                #endregion

                default:
                    break;
            }

            return Ok(Catalogo);
        }

        [HttpPost]
        [Route("FilterCatalogo")]
        [Authorize]
        public IHttpActionResult getCatalogofilter(ParamsDto Parametros)
        {
            CatalogosDto Catalogo = new CatalogosDto();
            //Buscamos los datos del catalogo
            Catalogo.Catalogos = db.Catalogos
             .Where(c => c.Id.Equals(Parametros.IdCat))
             .SingleOrDefault();

            if (Parametros.IdCat == 3) // Municipios
            {
                Catalogo.Estado = db.Estados
                       .Select(e => new EstadoDto
                       {
                           Id = e.Id,
                           estado = e.estado,
                           Clave = e.Clave,
                           Pais = e.Pais.pais,
                           Activo = e.Activo
                       })
                       .ToList();
                Catalogo.Municipio = db.Municipios
                    .Where(m => m.Estado.Id.Equals(Parametros.IdEstado))
                    .Select(m => new MunicipioDto
                    {
                        Id = m.Id,
                        municipio = m.municipio,
                        Estado = m.Estado.estado,
                        Activo = m.Activo
                    })
                    .OrderBy(c => c.Id)
                    .ToList();
            }
            else // Colonias
            {
                Catalogo.Pais = db.Paises.ToList();
                Catalogo.Estado = db.Estados
                    .Select(e => new EstadoDto
                    {
                        Id = e.Id,
                        estado = e.estado,
                        Clave = e.Clave,
                        Pais = e.Pais.pais,
                        Activo = e.Activo
                    })
                    .ToList();
                Catalogo.Municipio = db.Municipios
                    .Select(m => new MunicipioDto
                    {
                        Id = m.Id,
                        municipio = m.municipio,
                        Estado = m.Estado.estado,
                        Activo = m.Activo
                    })
                    .ToList();

                Catalogo.Colonia = db.Colonias
                    .Where(c => c.Municipio.Id == Parametros.IdMunicipio)
                    .Select(c => new ColoniasDto
                    {
                        Id = c.Id,
                        colonia = c.colonia,
                        TipoColonia = c.TipoColonia,
                        CP = c.CP,
                        Activo = c.Activo,
                        Pais = c.Pais.pais,
                        Estado = c.Estado.estado,
                        Municipio = c.Municipio.municipio
                    })
                    .OrderBy(c => c.Id)
                    .ToList();
            }

            return Ok(Catalogo);
        }

        [HttpPost]
        [Route("postCatalogo")]
        [Authorize]
        public IHttpActionResult postCatalogos(CatalogosDto Catalogo)
        {
            LogCatalogos log = new LogCatalogos();

            if (Catalogo.opt == 1) // Agregar
            {
                switch(Catalogo.Catalogos.Id) // ¿ Que catalogo es ?
                {
                    #region sistemas
                    case 1: // País

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);

                        db.Paises.Add(Catalogo.Pais[0]);
                        db.SaveChanges();

                        break;

                    case 2: // Estado

                        Estado estado = new Estado();
                        
                        estado.Activo = Catalogo.Estado[0].Activo;
                        estado.estado = Catalogo.Estado[0].estado;
                        estado.Clave = Catalogo.Estado[0].Clave;
                        estado.PaisId = Convert.ToInt32(Catalogo.Estado[0].Pais);

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);

                        db.Estados.Add(estado);
                        db.SaveChanges();

                        break;

                    case 3: // Municipios

                        Municipio municipio = new Municipio();

                        municipio.municipio = Catalogo.Municipio[0].municipio;
                        municipio.EstadoId = Convert.ToInt32(Catalogo.Municipio[0].Estado);
                        municipio.Activo = Catalogo.Municipio[0].Activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);

                        db.Municipios.Add(municipio);
                        db.SaveChanges();

                        break;

                    case 4: // Colonias

                        Colonia colonia = new Colonia();

                        colonia.colonia = Catalogo.Colonia[0].colonia;
                        colonia.TipoColonia = Catalogo.Colonia[0].TipoColonia;
                        colonia.CP = Catalogo.Colonia[0].CP;
                        colonia.EstadoId = Convert.ToInt32(Catalogo.Colonia[0].Estado);
                        colonia.MunicipioId = Convert.ToInt32(Catalogo.Colonia[0].Municipio);
                        colonia.PaisId = Convert.ToInt32(Catalogo.Colonia[0].Pais);
                        colonia.Activo = Catalogo.Colonia[0].Activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);

                        db.Colonias.Add(colonia);
                        db.SaveChanges();

                        break;

                    case 6: // Tipo de telefonos

                        TipoTelefono TpTelefonos = new TipoTelefono();

                        TpTelefonos.Tipo = Catalogo.TpTelefono[0].Tipo;
                        TpTelefonos.Activo = Catalogo.TpTelefono[0].Activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);

                        db.TiposTelefonos.Add(TpTelefonos);
                        db.SaveChanges();

                        break;

                    case 7: // Estado civil

                        EstadoCivil estadocivil = new EstadoCivil();

                        estadocivil.estadoCivil = Catalogo.EstadoCivil[0].estadoCivil;
                        estadocivil.Activo = Catalogo.EstadoCivil[0].Activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);

                        db.EstadosCiviles.Add(estadocivil);
                        db.SaveChanges();

                        break;

                    case 41: // Tipo de usuarios

                        TipoUsuario TpUsuario = new TipoUsuario();

                        TpUsuario.Tipo = Catalogo.TpUsuario[0].tipo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);

                        db.TiposUsuarios.Add(TpUsuario);
                        db.SaveChanges();

                        break;

                    case 42: // Departamentos

                        Departamento departamento = new Departamento();

                        departamento.Nombre = Catalogo.Departamentos[0].nombre;
                        departamento.AreaId = Convert.ToInt32(Catalogo.Departamentos[0].Area);
                        departamento.Clave = Catalogo.Departamentos[0].clave;
                        departamento.Orden = Catalogo.Departamentos[0].orden;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);

                        db.Departamentos.Add(departamento);
                        db.SaveChanges();

                        break;

                    case 43: // Areas

                        Area area = new Area();

                        area.Nombre = Catalogo.Areas[0].Nombre;
                        area.Clave = Catalogo.Areas[0].Clave;
                        area.Orden = Catalogo.Areas[0].Orden;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);

                        db.Areas.Add(area);
                        db.SaveChanges();

                        break;

                    case 44: // Roles

                        Roles roles = new Roles();

                        roles.Rol = Catalogo.Roles[0].Rol;
                        roles.Activo = Catalogo.Roles[0].Activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);

                        db.Roles.Add(roles);
                        db.SaveChanges();

                        break;

                    #endregion

                    #region Reclutamiento
                    case 34: // Escolaridades

                        GradoEstudio escolaridad = new GradoEstudio();


                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);
                        escolaridad.gradoEstudio = Catalogo.Escolaridades[0].gradoEstudio;

                        db.GradosEstudios.Add(escolaridad);
                        db.SaveChanges();

                        break;

                    case 35: // Nivel estudios

                        Nivel nivel = new Nivel();

                        nivel.nivel = Catalogo.Nivel[0].nivel;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);

                        db.Niveles.Add(nivel);
                        db.SaveChanges();

                        break;

                    case 36: // Medios

                        Medios medio = new Medios();

                        medio.Nombre = Catalogo.Medio[0].Nombre;
                        medio.Activo = Catalogo.Medio[0].Activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);

                        db.Medios.Add(medio);
                        db.SaveChanges();

                        break;

                    case 37: // Idiomas

                        Idioma idioma = new Idioma();

                        idioma.idioma = Catalogo.Idioma[0].idioma;
                        idioma.Activo = Catalogo.Idioma[0].activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);
                        db.Idiomas.Add(idioma);
                        db.SaveChanges();

                        break;

                    case 38: // Discapacidades

                        TipoDiscapacidad tpdiscapacidad = new TipoDiscapacidad();

                        tpdiscapacidad.tipoDiscapacidad = Catalogo.Discapacidad[0].tipoDiscapacidad;
                        tpdiscapacidad.activo = Catalogo.Discapacidad[0].activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);
                        db.TiposDiscapacidades.Add(tpdiscapacidad);
                        db.SaveChanges();

                        break;

                    case 39: // Tipo Licencia

                        TipoLicencia tplicencia = new TipoLicencia();

                        tplicencia.Descripcion = Catalogo.TipoLicencia[0].Descripcion;
                        tplicencia.tipoLicencia = Catalogo.TipoLicencia[0].tipoLicencia;
                        tplicencia.activo = Catalogo.TipoLicencia[0].activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);
                        db.TiposLicencias.Add(tplicencia);
                        db.SaveChanges();

                        break;

                    case 40: // Tipo Examen

                        TipoExamen tpexamen = new TipoExamen();

                        tpexamen.Nombre = Catalogo.TipoExamen[0].Nombre;
                        tpexamen.Descripcion = Catalogo.TipoExamen[0].Descripcion;
                        tpexamen.Activo = Catalogo.TipoExamen[0].Activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);
                        db.TipoExamen.Add(tpexamen);
                        db.SaveChanges();

                        break;
                    #endregion

                    #region Ventas
                    case 8: // Giro de empresa

                        GiroEmpresa giro = new GiroEmpresa();

                        giro.giroEmpresa = Catalogo.GiroEmpresa[0].giroEmpresa;
                        giro.activo = Catalogo.GiroEmpresa[0].activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);
                        db.GirosEmpresas.Add(giro);
                        db.SaveChanges();

                        break;
                    case 9: // Actvidades de empresa

                        ActividadEmpresa actividad = new ActividadEmpresa();

                        actividad.actividadEmpresa = Catalogo.ActividadEmpresa[0].actividadEmpresa;
                        actividad.GiroEmpresaId = Convert.ToInt16(Catalogo.ActividadEmpresa[0].GiroEmpresa);
                        actividad.activo = Catalogo.ActividadEmpresa[0].activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);
                        db.ActividadesEmpresas.Add(actividad);
                        db.SaveChanges();

                        break;

                    case 10: // Tamaño de empresa

                        TamanoEmpresa tamano = new TamanoEmpresa();

                        tamano.tamanoEmpresa = Catalogo.TamanoEmpresa[0].tamanoEmpresa;
                        tamano.activo = Catalogo.TamanoEmpresa[0].activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);
                        db.TamanoEmpresas.Add(tamano);
                        db.SaveChanges();

                        break;

                    case 11: // Tipos base

                        TipoBase tpbase = new TipoBase();

                        tpbase.tipoBase = Catalogo.TiposBase[0].tipoBase;
                        tpbase.activo = Catalogo.TiposBase[0].activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);
                        db.TiposBases.Add(tpbase);
                        db.SaveChanges();

                        break;

                    case 16: // Perfil experiencia

                        PerfilExperiencia perfil = new PerfilExperiencia();

                        perfil.perfilExperiencia = Catalogo.PerfilExperiencia[0].perfilExperiencia;
                        perfil.activo = Catalogo.PerfilExperiencia[0].activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);
                        db.PerfilExperiencia.Add(perfil);
                        db.SaveChanges();

                        break;

                    case 17: // Aptitudes

                        Aptitud aptitud = new Aptitud();

                        aptitud.aptitud = Catalogo.Aptitud[0].aptitud;
                        aptitud.activo = Catalogo.Aptitud[0].activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);
                        db.Aptitudes.Add(aptitud);
                        db.SaveChanges();

                        break;

                    case 18: // Area Experiencia

                        AreaExperiencia areaexp = new AreaExperiencia();

                        areaexp.areaExperiencia = Catalogo.AreaExperiencia[0].areaExperiencia;
                        areaexp.Activo = Catalogo.AreaExperiencia[0].Activo;
                        areaexp.Icono = Catalogo.AreaExperiencia[0].Icono;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);
                        db.AreasExperiencia.Add(areaexp);
                        db.SaveChanges();

                        break;

                    case 19: // Area Interes

                        AreaInteres areaint = new AreaInteres();

                        areaint.areaInteres = Catalogo.AreaInteres[0].areaInteres;
                        areaint.AreaExperienciaId = Convert.ToInt16(Catalogo.AreaInteres[0].AreaExperiencia);
                        areaint.Activo = Catalogo.AreaInteres[0].Activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);
                        db.AreasInteres.Add(areaint);
                        db.SaveChanges();

                        break;

                    case 20: // Jornada laboral

                        JornadaLaboral jl = new JornadaLaboral();

                        jl.Jornada = Catalogo.JornadaLaboral[0].Jornada;
                        jl.Orden = Catalogo.JornadaLaboral[0].Orden;
                        jl.VariosHorarios = Catalogo.JornadaLaboral[0].VariosHorarios;
                        jl.activo = Catalogo.JornadaLaboral[0].activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);
                        db.JornadasLaborales.Add(jl);
                        db.SaveChanges();

                        break;

                    case 21: // Modalidad

                        TipoModalidad tpmodalidad = new TipoModalidad();

                        tpmodalidad.Modalidad = Catalogo.TipoModalidad[0].Modalidad;
                        tpmodalidad.Orden = Catalogo.TipoModalidad[0].Orden;
                        tpmodalidad.activo = Catalogo.TipoModalidad[0].activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);
                        db.TiposModalidades.Add(tpmodalidad);
                        db.SaveChanges();

                        break;

                    case 22: // Psicometrias

                        TipoPsicometria tppsi = new TipoPsicometria();

                        tppsi.tipoPsicometria = Catalogo.TipoPsicometria[0].tipoPsicometria;
                        tppsi.descripcion = Catalogo.TipoPsicometria[0].descripcion;
                        tppsi.activo = Catalogo.TipoPsicometria[0].activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);
                        db.TiposPsicometrias.Add(tppsi);
                        db.SaveChanges();

                        break;

                    case 23: // Dias de la semana

                        DiaSemana DiasSemana = new DiaSemana();

                        DiasSemana.diaSemana = Catalogo.DiasSemana[0].diaSemana;
                        DiasSemana.activo = Catalogo.DiasSemana[0].activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);
                        db.DiasSemanas.Add(DiasSemana);
                        db.SaveChanges();

                        break;


                    case 24: // Tipos de nomina

                        TipodeNomina tpnomina = new TipodeNomina();

                        tpnomina.tipoDeNomina = Catalogo.TipoNomina[0].tipoDeNomina;
                        tpnomina.activo = Catalogo.TipoNomina[0].activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);
                        db.TiposNominas.Add(tpnomina);
                        db.SaveChanges();

                        break;

                    case 26: // Periodos de pago

                        PeriodoPago periodo = new PeriodoPago();

                        periodo.periodoPago = Catalogo.PeriodoPago[0].periodoPago;
                        periodo.activo = Catalogo.PeriodoPago[0].activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);
                        db.PeriodosPagos.Add(periodo);
                        db.SaveChanges();

                        break;

                    case 27: // Beneficios perfil

                        TipoBeneficio tpbeneficio = new TipoBeneficio();

                        tpbeneficio.tipoBeneficio = Catalogo.BeneficioPerfil[0].tipoBeneficio;
                        tpbeneficio.activo = Catalogo.BeneficioPerfil[0].activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);
                        db.TiposBeneficios.Add(tpbeneficio);
                        db.SaveChanges();

                        break;

                    case 28: // Tipo de contrato

                        TipoContrato tpcontrato = new TipoContrato();

                        tpcontrato.tipoContrato = Catalogo.TipoContrato[0].tipoContrato;
                        tpcontrato.periodoPrueba = Catalogo.TipoContrato[0].periodoPrueba;
                        tpcontrato.activo = Catalogo.TipoContrato[0].activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);
                        db.TiposContrato.Add(tpcontrato);
                        db.SaveChanges();

                        break;

                    case 29: // Tiempo de contrato

                        TiempoContrato tiempocontrato = new TiempoContrato();

                        tiempocontrato.Tiempo = Catalogo.TiemposContrato[0].Tiempo;
                        tiempocontrato.Orden = Catalogo.TiemposContrato[0].Orden;
                        tiempocontrato.activo = Catalogo.TiemposContrato[0].activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);
                        db.TiemposContratos.Add(tiempocontrato);
                        db.SaveChanges();

                        break;

                    case 31: // Documentos DAMSA

                        DocumentosDamsa docdamsa = new DocumentosDamsa();

                        docdamsa.documentoDamsa = Catalogo.DocDamsa[0].documentoDamsa;
                        docdamsa.activo = Catalogo.DocDamsa[0].activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);
                        db.DocumentosDamsa.Add(docdamsa);
                        db.SaveChanges();

                        break;

                    case 32: // Documentos DAMSA

                        PrestacionLey pley = new PrestacionLey();

                        pley.prestacionLey = Catalogo.PrestacionesLey[0].prestacionLey;
                        pley.activo = Catalogo.PrestacionesLey[0].activo;

                        log.CatalogoId = Catalogo.Catalogos.Id;
                        log.TpMov = "N";
                        log.Usuario = Catalogo.Usuario;
                        log.FechaAct = DateTime.UtcNow;
                        log.Campo = "";

                        db.LogCatalogos.Add(log);
                        db.PrestacionesLey.Add(pley);
                        db.SaveChanges();

                        break;

                        #endregion
                }
            }
            else // Modificar
            {
                LogCatalogos logm = new LogCatalogos();
                switch (Catalogo.Catalogos.Id) // ¿ Que catalogo es ?
                {
                    #region Sistema

                    case 1: // Países

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);

                        db.Entry(Catalogo.Pais[0]).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 2: // Estados

                        Estado estado = new Estado();

                        estado.Id = Catalogo.Estado[0].Id;
                        estado.Activo = Catalogo.Estado[0].Activo;
                        estado.estado = Catalogo.Estado[0].estado;
                        estado.Clave = Catalogo.Estado[0].Clave;
                        estado.PaisId = Convert.ToInt32(Catalogo.Estado[0].Pais);

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);

                        db.Entry(estado).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 3: // Municipios

                        Municipio municipio = new Municipio();

                        municipio.Id = Catalogo.Municipio[0].Id;
                        municipio.municipio = Catalogo.Municipio[0].municipio;
                        municipio.EstadoId = Convert.ToInt32(Catalogo.Municipio[0].Estado);
                        municipio.Activo = Catalogo.Municipio[0].Activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);

                        db.Entry(municipio).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 4: // Colonias

                        Colonia colonia = new Colonia();

                        colonia.Id = Catalogo.Colonia[0].Id;
                        colonia.colonia = Catalogo.Colonia[0].colonia;
                        colonia.TipoColonia = Catalogo.Colonia[0].TipoColonia;
                        colonia.CP = Catalogo.Colonia[0].CP;
                        colonia.EstadoId = Convert.ToInt32(Catalogo.Colonia[0].Estado);
                        colonia.MunicipioId = Convert.ToInt32(Catalogo.Colonia[0].Municipio);
                        colonia.PaisId = Convert.ToInt32(Catalogo.Colonia[0].Pais);
                        colonia.Activo = Catalogo.Colonia[0].Activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);

                        db.Entry(colonia).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 6: // Tipo de telefonos

                        TipoTelefono TpTelefonos = new TipoTelefono();

                        TpTelefonos.Id = Catalogo.TpTelefono[0].Id;
                        TpTelefonos.Tipo = Catalogo.TpTelefono[0].Tipo;
                        TpTelefonos.Activo = Catalogo.TpTelefono[0].Activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);


                        db.Entry(TpTelefonos).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 7: // Estado civil

                        EstadoCivil estadocivil = new EstadoCivil();

                        estadocivil.Id = Catalogo.EstadoCivil[0].Id;
                        estadocivil.estadoCivil = Catalogo.EstadoCivil[0].estadoCivil;
                        estadocivil.Activo = Catalogo.EstadoCivil[0].Activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);

                        db.Entry(estadocivil).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 41: // Tipo de usuarios

                        TipoUsuario TpUsuario = new TipoUsuario();

                        TpUsuario.Id = Catalogo.TpUsuario[0].Id;
                        TpUsuario.Tipo = Catalogo.TpUsuario[0].tipo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);

                        db.Entry(TpUsuario).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 42: // Departamentos

                        Departamento departamento = new Departamento();

                        departamento.Id = Catalogo.Departamentos[0].Id;
                        departamento.Nombre = Catalogo.Departamentos[0].nombre;
                        departamento.AreaId = Convert.ToInt32(Catalogo.Departamentos[0].Area);
                        departamento.Clave = Catalogo.Departamentos[0].clave;
                        departamento.Orden = Catalogo.Departamentos[0].orden;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);

                        db.Entry(departamento).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 43: // Areas

                        Area area = new Area();

                        area.Id = Catalogo.Areas[0].Id;
                        area.Nombre = Catalogo.Areas[0].Nombre;
                        area.Clave = Catalogo.Areas[0].Clave;
                        area.Orden = Catalogo.Areas[0].Orden;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);

                        db.Entry(area).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 44: // Roles

                        Roles roles = new Roles();

                        roles.Id = Catalogo.Roles[0].Id;
                        roles.Rol = Catalogo.Roles[0].Rol;
                        roles.Activo = Catalogo.Roles[0].Activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);

                        db.Entry(roles).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    #endregion

                    #region Reclutamiento
                    case 34: // Escolaridades

                        GradoEstudio escolaridad = new GradoEstudio();

                        escolaridad.Id = Catalogo.Escolaridades[0].Id;
                        escolaridad.gradoEstudio = Catalogo.Escolaridades[0].gradoEstudio;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);

                        db.Entry(escolaridad).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 35: // Nivel estudios  

                        Nivel nivel = new Nivel();

                        nivel.Id = Catalogo.Nivel[0].Id;
                        nivel.nivel = Catalogo.Nivel[0].nivel;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);

                        db.Entry(nivel).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 36: // Medios

                        Medios medio = new Medios();

                        medio.Id = Catalogo.Medio[0].Id;
                        medio.Nombre = Catalogo.Medio[0].Nombre;
                        medio.Activo = Catalogo.Medio[0].Activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);

                        db.Entry(medio).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 37: // Idiomas

                        Idioma idioma = new Idioma();

                        idioma.Id = Catalogo.Idioma[0].Id;
                        idioma.idioma = Catalogo.Idioma[0].idioma;
                        idioma.Activo = Catalogo.Idioma[0].activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);

                        db.Entry(idioma).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 38: // Discapacidades

                        TipoDiscapacidad tpdicapacidad = new TipoDiscapacidad();

                        tpdicapacidad.Id = Catalogo.Discapacidad[0].Id;
                        tpdicapacidad.tipoDiscapacidad = Catalogo.Discapacidad[0].tipoDiscapacidad;
                        tpdicapacidad.activo = Catalogo.Discapacidad[0].activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);

                        db.Entry(tpdicapacidad).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 39: // Tipo Licencia

                        TipoLicencia tplicencia = new TipoLicencia();

                        tplicencia.Id = Catalogo.TipoLicencia[0].Id;
                        tplicencia.Descripcion = Catalogo.TipoLicencia[0].Descripcion;
                        tplicencia.tipoLicencia = Catalogo.TipoLicencia[0].tipoLicencia;
                        tplicencia.activo = Catalogo.TipoLicencia[0].activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);
                        db.Entry(tplicencia).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 40: // Tipo Examen

                        TipoExamen tpexamen = new TipoExamen();

                        tpexamen.Id = Catalogo.TipoExamen[0].Id;
                        tpexamen.Nombre = Catalogo.TipoExamen[0].Nombre;
                        tpexamen.Descripcion = Catalogo.TipoExamen[0].Descripcion;
                        tpexamen.Activo = Catalogo.TipoExamen[0].Activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);
                        db.Entry(tpexamen).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;
                    #endregion

                    #region Ventas
                    case 8: // Giro de empresas

                        GiroEmpresa giro = new GiroEmpresa();

                        giro.Id = Catalogo.GiroEmpresa[0].Id;
                        giro.giroEmpresa = Catalogo.GiroEmpresa[0].giroEmpresa;
                        giro.activo = Catalogo.GiroEmpresa[0].activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);
                        db.Entry(giro).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 9: // ACtividades de empresas

                        ActividadEmpresa actividad = new ActividadEmpresa();

                        actividad.Id = Catalogo.ActividadEmpresa[0].Id;
                        actividad.actividadEmpresa = Catalogo.ActividadEmpresa[0].actividadEmpresa;
                        actividad.GiroEmpresaId = Convert.ToInt16(Catalogo.ActividadEmpresa[0].GiroEmpresa);
                        actividad.activo = Catalogo.ActividadEmpresa[0].activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);
                        db.Entry(actividad).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 10: // Tamano de empresas

                        TamanoEmpresa tamano = new TamanoEmpresa();

                        tamano.Id = Catalogo.TamanoEmpresa[0].Id;
                        tamano.tamanoEmpresa = Catalogo.TamanoEmpresa[0].tamanoEmpresa;
                        tamano.activo = Catalogo.TamanoEmpresa[0].activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);
                        db.Entry(tamano).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 11: // Tamano de empresas

                        TipoBase tpbase = new TipoBase();

                        tpbase.Id = Catalogo.TiposBase[0].Id;
                        tpbase.tipoBase = Catalogo.TiposBase[0].tipoBase;
                        tpbase.activo = Catalogo.TiposBase[0].activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);
                        db.Entry(tpbase).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 16: // Perfil experiencia

                        PerfilExperiencia perfil = new PerfilExperiencia();

                        perfil.Id = Catalogo.PerfilExperiencia[0].Id;
                        perfil.perfilExperiencia = Catalogo.PerfilExperiencia[0].perfilExperiencia;
                        perfil.activo = Catalogo.PerfilExperiencia[0].activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);
                        db.Entry(perfil).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 17: // Aptitudes

                        Aptitud aptitud = new Aptitud();

                        aptitud.Id = Catalogo.Aptitud[0].Id;
                        aptitud.aptitud = Catalogo.Aptitud[0].aptitud;
                        aptitud.activo = Catalogo.Aptitud[0].activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);
                        db.Entry(aptitud).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 18: // Area experiencia

                        AreaExperiencia areaexp = new AreaExperiencia();

                        areaexp.Id = Catalogo.AreaExperiencia[0].Id;
                        areaexp.areaExperiencia = Catalogo.AreaExperiencia[0].areaExperiencia;
                        areaexp.Activo = Catalogo.AreaExperiencia[0].Activo;
                        areaexp.Icono = Catalogo.AreaExperiencia[0].Icono;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);
                        db.Entry(areaexp).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 19: // Area Interes

                        AreaInteres areaint = new AreaInteres();

                        areaint.Id = Catalogo.AreaInteres[0].Id;
                        areaint.areaInteres = Catalogo.AreaInteres[0].areaInteres;
                        areaint.AreaExperienciaId = Convert.ToInt16(Catalogo.AreaInteres[0].AreaExperiencia);
                        areaint.Activo = Catalogo.AreaInteres[0].Activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);
                        db.Entry(areaint).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 20: // Jornada Laboral

                        JornadaLaboral jl = new JornadaLaboral();

                        jl.Id = Catalogo.JornadaLaboral[0].Id;
                        jl.Jornada = Catalogo.JornadaLaboral[0].Jornada;
                        jl.Orden = Catalogo.JornadaLaboral[0].Orden;
                        jl.VariosHorarios = Catalogo.JornadaLaboral[0].VariosHorarios;
                        jl.activo = Catalogo.JornadaLaboral[0].activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);
                        db.Entry(jl).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 21: // Modalidad

                        TipoModalidad tpmodalidad = new TipoModalidad();

                        tpmodalidad.Id = Catalogo.TipoModalidad[0].Id;
                        tpmodalidad.Modalidad = Catalogo.TipoModalidad[0].Modalidad;
                        tpmodalidad.Orden = Catalogo.TipoModalidad[0].Orden;
                        tpmodalidad.activo = Catalogo.TipoModalidad[0].activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);
                        db.Entry(tpmodalidad).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 22: // Psicometrias

                        TipoPsicometria tppsi = new TipoPsicometria();

                        tppsi.Id = Catalogo.TipoPsicometria[0].Id;
                        tppsi.tipoPsicometria = Catalogo.TipoPsicometria[0].tipoPsicometria;
                        tppsi.descripcion = Catalogo.TipoPsicometria[0].descripcion;
                        tppsi.activo = Catalogo.TipoPsicometria[0].activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);
                        db.Entry(tppsi).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 23: // Dias de la semana

                        DiaSemana DiasSemana = new DiaSemana();

                        DiasSemana.Id = Catalogo.DiasSemana[0].Id;
                        DiasSemana.diaSemana = Catalogo.DiasSemana[0].diaSemana;
                        DiasSemana.activo = Catalogo.DiasSemana[0].activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);
                        db.Entry(DiasSemana).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 24: // Tipos de nomina

                        TipodeNomina tpnomina = new TipodeNomina();

                        tpnomina.Id = Catalogo.TipoNomina[0].Id;
                        tpnomina.tipoDeNomina = Catalogo.TipoNomina[0].tipoDeNomina;
                        tpnomina.activo = Catalogo.TipoNomina[0].activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);
                        db.Entry(tpnomina).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 26: // Periodos pago

                        PeriodoPago periodo = new PeriodoPago();

                        periodo.Id = Catalogo.PeriodoPago[0].Id;
                        periodo.periodoPago = Catalogo.PeriodoPago[0].periodoPago;
                        periodo.activo = Catalogo.PeriodoPago[0].activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);
                        db.Entry(periodo).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;


                    case 27: // Beneficios perfil

                        TipoBeneficio tpbeneficio = new TipoBeneficio();

                        tpbeneficio.Id = Catalogo.BeneficioPerfil[0].Id;
                        tpbeneficio.tipoBeneficio = Catalogo.BeneficioPerfil[0].tipoBeneficio;
                        tpbeneficio.activo = Catalogo.BeneficioPerfil[0].activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);
                        db.Entry(tpbeneficio).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 28: // Tipo de contrato

                        TipoContrato tpcontrato = new TipoContrato();

                        tpcontrato.Id = Catalogo.TipoContrato[0].Id;
                        tpcontrato.tipoContrato = Catalogo.TipoContrato[0].tipoContrato;
                        tpcontrato.periodoPrueba = Catalogo.TipoContrato[0].periodoPrueba;
                        tpcontrato.activo = Catalogo.TipoContrato[0].activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);
                        db.Entry(tpcontrato).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 29: // Tiempo de contrato

                        TiempoContrato tiempocontrato = new TiempoContrato();

                        tiempocontrato.Id = Catalogo.TiemposContrato[0].Id;
                        tiempocontrato.Tiempo = Catalogo.TiemposContrato[0].Tiempo;
                        tiempocontrato.Orden = Catalogo.TiemposContrato[0].Orden;
                        tiempocontrato.activo = Catalogo.TiemposContrato[0].activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);
                        db.Entry(tiempocontrato).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 31: // Documentos DAMSA

                        DocumentosDamsa docdamsa = new DocumentosDamsa();

                        docdamsa.Id = Catalogo.DocDamsa[0].Id;
                        docdamsa.documentoDamsa = Catalogo.DocDamsa[0].documentoDamsa;
                        docdamsa.activo = Catalogo.DocDamsa[0].activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);
                        db.Entry(docdamsa).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;

                    case 32: // Documentos DAMSA

                        PrestacionLey pley = new PrestacionLey();

                        pley.Id = Catalogo.PrestacionesLey[0].Id;
                        pley.prestacionLey = Catalogo.PrestacionesLey[0].prestacionLey;
                        pley.activo = Catalogo.PrestacionesLey[0].activo;

                        logm.CatalogoId = Catalogo.Catalogos.Id;
                        logm.TpMov = "M";
                        logm.Usuario = Catalogo.Usuario;
                        logm.FechaAct = DateTime.UtcNow;
                        logm.Campo = "";

                        db.LogCatalogos.Add(logm);
                        db.Entry(pley).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        break;
                        #endregion
                }
            }

            return Ok(true);
        }
        #endregion
    }
}