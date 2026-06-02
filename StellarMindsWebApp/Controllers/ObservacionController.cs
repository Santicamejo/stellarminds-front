using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Observacion;
using Newtonsoft.Json;
using StellarMindsWebApp.Auxiliar;
using StellarMindsWebApp.Enums;
using StellarMindsWebApp.Models;
using StellarMindsWebApp.Models.ObjetoCeleste;
using StellarMindsWebApp.Models.PrestamoModel;

namespace StellarMindsWebApp.Controllers
{
    public class ObservacionController : Controller
    {
        private string baseUrl = "http://localhost:5196/api/Observacion";

        private void CargarDatosCrearObservacion(int usuarioId)
        {
            HttpResponseMessage respuestaPrestamos = ClienteHttpAuxiliar.EnviarSolicitud(
                "http://localhost:5196/api/Prestamo/vigentes/" + usuarioId,
                VerbosHttp.GET
            );

            if (respuestaPrestamos.IsSuccessStatusCode)
            {
                string body = ClienteHttpAuxiliar.ObtenerBody(respuestaPrestamos);

                IEnumerable<PrestamoEvaluacionModel>? prestamos =
                    JsonConvert.DeserializeObject<IEnumerable<PrestamoEvaluacionModel>>(body);

                ViewBag.Prestamos = prestamos ?? new List<PrestamoEvaluacionModel>();
            }
            else
            {
                ViewBag.Prestamos = new List<PrestamoEvaluacionModel>();
            }

            HttpResponseMessage respuestaObjetosCelestes = ClienteHttpAuxiliar.EnviarSolicitud(
                "http://localhost:5196/api/ObjetoCeleste/para-evaluacion",
                VerbosHttp.GET
            );

            if (respuestaObjetosCelestes.IsSuccessStatusCode)
            {
                string body = ClienteHttpAuxiliar.ObtenerBody(respuestaObjetosCelestes);

                IEnumerable<ObjetoCelesteModel>? objetos =
                    JsonConvert.DeserializeObject<IEnumerable<ObjetoCelesteModel>>(body);

                ViewBag.ObjetosCelestes = objetos ?? new List<ObjetoCelesteModel>();
            }
            else
            {
                ViewBag.ObjetosCelestes = new List<ObjetoCelesteModel>();
            }
        }

        [HttpGet]
        public IActionResult Crear()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioLogeadoId");

            if (usuarioId == null)
            {
                TempData["Error"] = "Debe iniciar sesión para crear una observación.";
                return RedirectToAction("Login", "Usuario");
            }

            CargarDatosCrearObservacion(usuarioId.Value);

            ViewBag.Evaluado = false;

            return View();
        }

        [HttpPost]
        public IActionResult Crear(AltaObservacionModel observacion)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioLogeadoId");

            if (usuarioId == null)
            {
                TempData["Error"] = "Debe iniciar sesión para crear una observación.";
                return RedirectToAction("Login", "Usuario");
            }

            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/crear/" + usuarioId.Value,
                VerbosHttp.POST,
                observacion
            );

            if (respuesta.IsSuccessStatusCode)
            {
                TempData["Exito"] = "Observación creada correctamente.";
                return RedirectToAction("Index", "Home");
            }

            string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = "Ocurrió un error creando la observación.";

            if (!string.IsNullOrWhiteSpace(body))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(body);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    TempData["Error"] = error.Mensaje;
                }
            }

            return View(observacion);
        }

        [HttpPost]
        public IActionResult EvaluarVisual(AltaObservacionModel observacion)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioLogeadoId");

            if (usuarioId == null)
            {
                TempData["Error"] = "Debe iniciar sesión para evaluar una observación.";
                return RedirectToAction("Login", "Usuario");
            }


            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/evaluar-visual",
                VerbosHttp.POST,
                observacion
            );

            if (respuesta.IsSuccessStatusCode)
            {
                string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

                RespuestaGeminiModel? resultado =
                    JsonConvert.DeserializeObject<RespuestaGeminiModel>(body);

                if (resultado != null)
                {
                    observacion.ResultadoEvaluacion = resultado.Indicador;
                    observacion.Detalle = resultado.Detalle;
                    observacion.EsAstrofotografia = false;
                }

                ViewBag.Evaluado = true;
                TempData["Exito"] = "Evaluación visual realizada correctamente.";

                CargarDatosCrearObservacion((int)usuarioId);

                ViewBag.Evaluado = true;

                return View("Crear", observacion);
            }

            string bodyError = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = "Ocurrió un error evaluando la observación visual.";

            if (!string.IsNullOrWhiteSpace(bodyError))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(bodyError);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    TempData["Error"] = error.Mensaje;
                }
            }

            CargarDatosCrearObservacion((int)usuarioId);

            ViewBag.Evaluado = false;

            return View("Crear", observacion);
        }

        [HttpPost]
        public IActionResult EvaluarAstrofotografia(AltaObservacionModel observacion)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioLogeadoId");

            if (usuarioId == null)
            {
                TempData["Error"] = "Debe iniciar sesión para evaluar una observación.";
                return RedirectToAction("Login", "Usuario");
            }

            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/evaluar-astrofotografia",
                VerbosHttp.POST,
                observacion
            );

            if (respuesta.IsSuccessStatusCode)
            {
                string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

                RespuestaGeminiModel? resultado =
                    JsonConvert.DeserializeObject<RespuestaGeminiModel>(body);

                if (resultado != null)
                {
                    observacion.ResultadoEvaluacion = resultado.Indicador;
                    observacion.Detalle = resultado.Detalle;
                    observacion.EsAstrofotografia = true;
                }

                ViewBag.Evaluado = true;
                TempData["Exito"] = "Evaluación de astrofotografía realizada correctamente.";

                CargarDatosCrearObservacion((int)usuarioId);

                ViewBag.Evaluado = true;

                return View("Crear", observacion);
            }

            string bodyError = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = "Ocurrió un error evaluando la astrofotografía.";

            if (!string.IsNullOrWhiteSpace(bodyError))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(bodyError);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    TempData["Error"] = error.Mensaje;
                }
            }

            CargarDatosCrearObservacion((int)usuarioId);

            ViewBag.Evaluado = false;

            return View("Crear", observacion);
        }
    }
}
