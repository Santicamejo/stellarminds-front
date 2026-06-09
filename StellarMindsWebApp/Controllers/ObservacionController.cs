using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StellarMindsWebApp.Auxiliar;
using StellarMindsWebApp.Enums;
using StellarMindsWebApp.Filtros;
using StellarMindsWebApp.Models;
using StellarMindsWebApp.Models.ObjetoCeleste;
using StellarMindsWebApp.Models.Observacion;
using StellarMindsWebApp.Models.Prestamo;

namespace StellarMindsWebApp.Controllers
{
    public class ObservacionController : Controller
    {
        private string baseUrl = "http://localhost:5196/api/Observacion";

        private void CargarDatosCrearObservacion(int usuarioId, string token)
        {
            HttpResponseMessage respuestaPrestamos = ClienteHttpAuxiliar.EnviarSolicitud(
                "http://localhost:5196/api/Prestamo/vigentes/" + usuarioId,
                VerbosHttp.GET,
                null,
                token
            );

            if (respuestaPrestamos.IsSuccessStatusCode)
            {
                string body = ClienteHttpAuxiliar.ObtenerBody(respuestaPrestamos);

                IEnumerable<PrestamoSelectEvaluacionModel>? prestamos =
                    JsonConvert.DeserializeObject<IEnumerable<PrestamoSelectEvaluacionModel>>(body);

                ViewBag.Prestamos = prestamos ?? new List<PrestamoSelectEvaluacionModel>();
            }
            else
            {
                ViewBag.Prestamos = new List<PrestamoSelectEvaluacionModel>();
            }

            HttpResponseMessage respuestaObjetosCelestes = ClienteHttpAuxiliar.EnviarSolicitud(
                "http://localhost:5196/api/ObjetoCeleste/para-evaluacion",
                VerbosHttp.GET,
                null,
                token
            );

            if (respuestaObjetosCelestes.IsSuccessStatusCode)
            {
                string body = ClienteHttpAuxiliar.ObtenerBody(respuestaObjetosCelestes);

                IEnumerable<ObjetoCelesteSelectEvaluacionModel>? objetos =
                    JsonConvert.DeserializeObject<IEnumerable<ObjetoCelesteSelectEvaluacionModel>>(body);

                ViewBag.ObjetosCelestes = objetos ?? new List<ObjetoCelesteSelectEvaluacionModel>();
            }
            else
            {
                ViewBag.ObjetosCelestes = new List<ObjetoCelesteSelectEvaluacionModel>();
            }
        }

        [RolAuthorizeAttribute(new string[] { "SOCIO" })]
        [HttpGet]
        public IActionResult Crear()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioLogeadoId");
            string? token = HttpContext.Session.GetString("TokenJWT");

            if (usuarioId == null)
            {
                TempData["Error"] = "Debe iniciar sesión para crear una observación.";
                return RedirectToAction("Login", "Usuario");
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Error"] = "Debe iniciar sesión.";
                return RedirectToAction("Login", "Usuario");
            }

            CargarDatosCrearObservacion(usuarioId.Value, token);

            AltaObservacionModel model = new AltaObservacionModel
            {
                FechaObservacion = DateTime.Today
            };

            return View(model);
        }

        [RolAuthorizeAttribute(new string[] { "SOCIO" })]
        [HttpPost]
        public IActionResult Crear(AltaObservacionModel observacion)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioLogeadoId");
            string? token = HttpContext.Session.GetString("TokenJWT");

            if (usuarioId == null)
            {
                TempData["Error"] = "Debe iniciar sesión para crear una observación.";
                return RedirectToAction("Login", "Usuario");
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Error"] = "Debe iniciar sesión.";
                return RedirectToAction("Login", "Usuario");
            }

            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/crear/" + usuarioId.Value,
                VerbosHttp.POST,
                observacion,
                token
            );

            if (respuesta.IsSuccessStatusCode)
            {
                TempData["Exito"] = "Observación creada correctamente.";
                return RedirectToAction("Index", "Home");
            }

            string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = $"{(int)respuesta.StatusCode} {respuesta.StatusCode} - Ocurrió un error creando la observación.";

            if (!string.IsNullOrWhiteSpace(body))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(body);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    TempData["Error"] = error.Mensaje;
                }
            }

            CargarDatosCrearObservacion(usuarioId.Value, token);

            return View(observacion);
        }


        [RolAuthorizeAttribute(new string[] { "SOCIO" })]
        [HttpPost]
        public IActionResult Evaluar(AltaObservacionModel aEvaluar)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioLogeadoId");
            string? token = HttpContext.Session.GetString("TokenJWT");

            if (usuarioId == null)
            {
                TempData["Error"] = "Debe iniciar sesión para crear una observación.";
                return RedirectToAction("Login", "Usuario");
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Error"] = "Debe iniciar sesión.";
                return RedirectToAction("Login", "Usuario");
            }

            try
            {
                SolicitudEvaluacionModel solicitud = new SolicitudEvaluacionModel
                {
                    PrestamoId = aEvaluar.PrestamoVinculadoId,
                    ObjetoCelesteId = aEvaluar.ObjetoVisualizadoId
                };

                HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                    baseUrl + "/evaluar",
                    VerbosHttp.POST,
                    solicitud,
                    token
                );

                if (respuesta.IsSuccessStatusCode)
                {
                    string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

                    RespuestaGeminiModel? resultado =
                        JsonConvert.DeserializeObject<RespuestaGeminiModel>(body);

                    ViewBag.Respuesta = resultado;
                }
                else
                {

                    string bodyError = ClienteHttpAuxiliar.ObtenerBody(respuesta);
                    TempData["Error"] = bodyError;
                }

                CargarDatosCrearObservacion(usuarioId.Value, token);

                return View("Crear", aEvaluar);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Ocurrió un error al evaluar la observación: " + ex.Message;

                CargarDatosCrearObservacion(usuarioId.Value, token);

                return View("Crear", aEvaluar);
            }
        }

    }
}
