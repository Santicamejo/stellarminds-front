using DTOs.DTOs.ObjetoCeleste;
using DTOs.DTOs.Observaciones;
using DTOs.DTOs.Prestamo;
using LogicaAplicacion.CasosDeUso.Prestamos;
using LogicaAplicacion.InterfacesCasosDeUso;
using LogicaAplicacion.InterfacesCasosDeUso.ObjetosCelestes;
using LogicaAplicacion.InterfacesCasosDeUso.Observaciones;
using LogicaAplicacion.InterfacesCasosDeUso.Prestamos;
using Microsoft.AspNetCore.Mvc;

namespace Presentacion.Controllers
{
    public class ObservacionController : Controller
    {
        private readonly IAltaObservacion _altaObservacion;
        private readonly IEvaluarObservacion _evaluarObservacion;
        private readonly IObtenerTodosOCEvaluacion _obtenerObjetosCelestes;
        private readonly IObtenerOCEvaluacionIDCU _obtenerObjetoCelestePorId;
        private readonly IObtenerPrestamosVigentes _obtenerPrestamosParaEvaluacion;
        private readonly IObtenerPrestamoPorId _obtenerPrestamoPorId;

        public ObservacionController(
            IAltaObservacion altaObservacion,
            IEvaluarObservacion evaluarObservacion,
            IObtenerTodosOCEvaluacion obtenerObjetosCelestes,
            IObtenerOCEvaluacionIDCU obtenerObjetoCelestePorId,
            IObtenerPrestamosVigentes obtenerPrestamosParaEvaluacion,
            IObtenerPrestamoPorId obtenerPrestamoPorId)
        {
            _altaObservacion = altaObservacion;
            _evaluarObservacion = evaluarObservacion;
            _obtenerObjetosCelestes = obtenerObjetosCelestes;
            _obtenerObjetoCelestePorId = obtenerObjetoCelestePorId;
            _obtenerPrestamosParaEvaluacion = obtenerPrestamosParaEvaluacion;
            _obtenerPrestamoPorId = obtenerPrestamoPorId;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: Observacion/Create
        public IActionResult Create()
        {
            try
            {
                CargarDatosCreate();

                return View(new AltaObservacionDTO());
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AltaObservacionDTO observacion)
        {
            try
            {
                int? socioLogeado = HttpContext.Session.GetInt32("UsuarioId");

                if (socioLogeado == null)
                    throw new Exception("No hay socio logueado.");

                _altaObservacion.Ejecutar(observacion, socioLogeado.Value);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;

                CargarDatosCreate();

                return View(observacion);
            }
        }

        // POST: Observacion/Evaluar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Evaluar(EvaluarObvservacionDTO dto)
        {
            try
            {
                int? usuarioLogeado = HttpContext.Session.GetInt32("UsuarioId");

                if (usuarioLogeado == null)
                {
                    TempData["Error"] = "Debe estar logueado";
                    return RedirectToAction("Index", "Home");
                }

                if (dto.PrestamoVinculadoId <= 0)
                    throw new Exception("Debe seleccionar un préstamo.");

                if (dto.ObjetoVisualizadoId <= 0)
                    throw new Exception("Debe seleccionar un objeto celeste.");

                PrestamoEvaluacionDTO prestamoSeleccionado =
                    _obtenerPrestamoPorId.Ejecutar(dto.PrestamoVinculadoId);

                ObjetoCelesteEvaluacionDTO objetoCelesteSeleccionado =
                    _obtenerObjetoCelestePorId.Ejecutar(dto.ObjetoVisualizadoId);

                if (prestamoSeleccionado == null)
                    throw new Exception("No se encontró el préstamo seleccionado.");

                if (objetoCelesteSeleccionado == null)
                    throw new Exception("No se encontró el objeto celeste seleccionado.");

                RespuestaGeminiDTO respuestaDeGemini;

                if (prestamoSeleccionado.EsAstrofotografia)
                {
                    if (prestamoSeleccionado.SensorCamara == null ||
                        prestamoSeleccionado.ResolucionCamara == null ||
                        prestamoSeleccionado.TamanoPixelCamara == null)
                    {
                        throw new Exception("Faltan datos técnicos de la cámara para evaluar la astrofotografía.");
                    }

                    SolicitudEvaluacionAstrofotografiaDTO solicitud =
                        new SolicitudEvaluacionAstrofotografiaDTO
                        {
                            nombre = objetoCelesteSeleccionado.Nombre,
                            tipo = objetoCelesteSeleccionado.Tipo.ToString(),

                            apertura_mm = prestamoSeleccionado.AperturaTelescopio,
                            focal_mm = prestamoSeleccionado.FocalTelescopio,
                            relacion_focal = prestamoSeleccionado.RelacionFocalTelescopio,

                            sensor = prestamoSeleccionado.SensorCamara,
                            resolucion_px = prestamoSeleccionado.ResolucionCamara.Value,
                            pixel_size_um = prestamoSeleccionado.TamanoPixelCamara.Value
                        };

                    respuestaDeGemini = await _evaluarObservacion.Ejecutar(solicitud);
                }
                else
                {
                    if (prestamoSeleccionado.FocalOcular == null ||
                        prestamoSeleccionado.CampoAparenteOcular == null)
                    {
                        throw new Exception("Faltan datos técnicos del ocular para evaluar la observación visual.");
                    }

                    SolicitudEvaluacionVisualDTO solicitud =
                        new SolicitudEvaluacionVisualDTO
                        {
                            nombre = objetoCelesteSeleccionado.Nombre,
                            tipo = objetoCelesteSeleccionado.Tipo.ToString(),

                            apertura_mm = prestamoSeleccionado.AperturaTelescopio,
                            focal_mm = prestamoSeleccionado.FocalTelescopio,
                            relacion_focal = prestamoSeleccionado.RelacionFocalTelescopio,

                            ocular_focal_mm = prestamoSeleccionado.FocalOcular.Value,
                            campo_aparente_grados = prestamoSeleccionado.CampoAparenteOcular.Value
                        };

                    respuestaDeGemini = await _evaluarObservacion.Ejecutar(solicitud);
                }

                AltaObservacionDTO altaEvaluacion = new AltaObservacionDTO
                {
                    PrestamoVinculadoId = dto.PrestamoVinculadoId,
                    ObjetoVisualizadoId = dto.ObjetoVisualizadoId,
                    ResultadoEvaluacion = respuestaDeGemini.Indicador,
                    Detalle = respuestaDeGemini.Detalle
                };

                CargarDatosCreate();

                return View("Create", altaEvaluacion);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;

                AltaObservacionDTO altaEvaluacion = new AltaObservacionDTO
                {
                    PrestamoVinculadoId = dto.PrestamoVinculadoId,
                    ObjetoVisualizadoId = dto.ObjetoVisualizadoId,
                    FechaObservacion = DateTime.Today
                };

                CargarDatosCreate();

                return View("Create", altaEvaluacion);
            }
        }

        private void CargarDatosCreate()
        {
            int? usuarioLogeado = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioLogeado == null)
                throw new Exception("No hay usuario logueado.");

            ViewBag.PrestamosDeUsuario = _obtenerPrestamosParaEvaluacion.Ejecutar(usuarioLogeado.Value);
            ViewBag.ObjetosCelestes = _obtenerObjetosCelestes.Ejecutar();
        }
    }
}