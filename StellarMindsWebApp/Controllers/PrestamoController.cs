using DTOs.DTOs.Equipo;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using NuGet.Common;
using StellarMindsWebApp.Auxiliar;
using StellarMindsWebApp.Enums;
using StellarMindsWebApp.Filtros;
using StellarMindsWebApp.Models;
using StellarMindsWebApp.Models.Equipo;
using StellarMindsWebApp.Models.Prestamo;
using StellarMindsWebApp.Models.PrestamoModel;
using StellarMindsWebApp.Models.Usuario;

namespace StellarMindsWebApp.Controllers
{
    public class PrestamoController : Controller
    {

        private void CargarDatosAltaPrestamo(string token)
        {
            HttpResponseMessage respuestaTelescopios = ClienteHttpAuxiliar.EnviarSolicitud(
                "http://localhost:5196/api/Equipo/telescopios",
                VerbosHttp.GET,
                null,
                token
            );

            if (respuestaTelescopios.IsSuccessStatusCode)
            {
                string body = ClienteHttpAuxiliar.ObtenerBody(respuestaTelescopios);

                IEnumerable<TelescopioModel>? telescopios =
                    JsonConvert.DeserializeObject<IEnumerable<TelescopioModel>>(body);

                ViewBag.Telescopios = telescopios ?? new List<TelescopioModel>();
            }
            else
            {
                ViewBag.Telescopios = new List<TelescopioModel>();
            }

            HttpResponseMessage respuestaMonturas = ClienteHttpAuxiliar.EnviarSolicitud(
                "http://localhost:5196/api/Equipo/monturas",
                VerbosHttp.GET,
                null, 
                token
            );

            if (respuestaMonturas.IsSuccessStatusCode)
            {
                string body = ClienteHttpAuxiliar.ObtenerBody(respuestaMonturas);

                IEnumerable<MonturaModel>? monturas =
                    JsonConvert.DeserializeObject<IEnumerable<MonturaModel>>(body);

                ViewBag.Monturas = monturas ?? new List<MonturaModel>();
            }
            else
            {
                ViewBag.Monturas = new List<MonturaModel>();
            }

            HttpResponseMessage respuestaOculares = ClienteHttpAuxiliar.EnviarSolicitud(
                "http://localhost:5196/api/Equipo/oculares",
                VerbosHttp.GET,
                null,
                token
            );

            if (respuestaOculares.IsSuccessStatusCode)
            {
                string body = ClienteHttpAuxiliar.ObtenerBody(respuestaOculares);

                IEnumerable<OcularModel>? oculares =
                    JsonConvert.DeserializeObject<IEnumerable<OcularModel>>(body);

                ViewBag.Oculares = oculares ?? new List<OcularModel>();
            }
            else
            {
                ViewBag.Oculares = new List<OcularModel>();
            }

            HttpResponseMessage respuestaCamaras = ClienteHttpAuxiliar.EnviarSolicitud(
                "http://localhost:5196/api/Equipo/camaras",
                VerbosHttp.GET,
                null,
                token
            );

            if (respuestaCamaras.IsSuccessStatusCode)
            {
                string body = ClienteHttpAuxiliar.ObtenerBody(respuestaCamaras);

                IEnumerable<CamaraModel>? camaras =
                    JsonConvert.DeserializeObject<IEnumerable<CamaraModel>>(body);

                ViewBag.Camaras = camaras ?? new List<CamaraModel>();
            }
            else
            {
                ViewBag.Camaras = new List<CamaraModel>();
            }

            HttpResponseMessage respuestaSocios = ClienteHttpAuxiliar.EnviarSolicitud(
                "http://localhost:5196/api/Usuario/todos",
                VerbosHttp.GET,
                null, 
                token
            );

            if (respuestaSocios.IsSuccessStatusCode)
            {
                string body = ClienteHttpAuxiliar.ObtenerBody(respuestaSocios);

                IEnumerable<UsuarioModel>? socios =
                    JsonConvert.DeserializeObject<IEnumerable<UsuarioModel>>(body);

                ViewBag.Socios = socios ?? new List<UsuarioModel>();
            }
            else
            {
                ViewBag.Socios = new List<UsuarioModel>();
            }
        }

        private string baseUrl = "http://localhost:5196/api/Prestamo";

        [RolAuthorizeAttribute(new string[] { "COORDINADOR" })]
        [HttpGet]
        public IActionResult Alta()
        {
            string? token = HttpContext.Session.GetString("TokenJWT");

            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Error"] = "Debe iniciar sesión.";
                return RedirectToAction("Login", "Usuario");
            }

            CargarDatosAltaPrestamo(token);
            return View(new AltaPrestamoModel());
        }

        [RolAuthorizeAttribute(new string[] { "COORDINADOR" })]
        [HttpPost]
        public ActionResult Alta(AltaPrestamoModel prestamo)
        {

            int? responsableId = HttpContext.Session.GetInt32("UsuarioLogeadoId");
            string? token = HttpContext.Session.GetString("TokenJWT");

            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Error"] = "Debe iniciar sesión.";
                return RedirectToAction("Login", "Usuario");
            }
            if (responsableId == null)
            {
                TempData["Error"] = "Debe iniciar sesión para crear un préstamo.";
                return RedirectToAction("Login", "Usuario");
            }

            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/alta?responsableId=" + responsableId.Value,
                VerbosHttp.POST,
                prestamo,
                token
            );

            if (respuesta.IsSuccessStatusCode)
            {
                TempData["Exito"] = "Prestamo creado correctamente";
                return RedirectToAction("Index", "Home");
            }

            string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = "Ocurrio un error inesperado creando el prestamo.";

            if (!string.IsNullOrWhiteSpace(body))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(body);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    TempData["Error"] = error.Mensaje;
                }
            }

            CargarDatosAltaPrestamo(token);
            return View(prestamo);
        }

        [RolAuthorizeAttribute(new string[] { "COORDINADOR" })]
        [HttpPost]
        public IActionResult Devolver(int prestamoId)
        {
            int? responsableId = HttpContext.Session.GetInt32("UsuarioLogeadoId");
            string? token = HttpContext.Session.GetString("TokenJWT");

            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Error"] = "Debe iniciar sesión.";
                return RedirectToAction("Login", "Usuario");
            }
            if (responsableId == null)
            {
                TempData["Error"] = "Debe iniciar sesión para devolver un préstamo.";
                return RedirectToAction("Login", "Usuario");
            }

            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/devolver?prestamoId=" + prestamoId + "&responsableId=" + responsableId.Value,
                VerbosHttp.PUT,
                null,
                token
            );

            if (respuesta.IsSuccessStatusCode)
            {
                TempData["Exito"] = "Préstamo devuelto correctamente";
                return RedirectToAction("Index", "Home");
            }

            string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = $"{(int)respuesta.StatusCode} {respuesta.StatusCode} - Ocurrió un error inesperado devolviendo el préstamo.";

            if (!string.IsNullOrWhiteSpace(body))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(body);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    TempData["Error"] = error.Mensaje;
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [RolAuthorizeAttribute(new string[] { "COORDINADOR" })]
        [HttpGet]
        public IActionResult PrestamosUsuario(int usuarioId)
        {
            string? token = HttpContext.Session.GetString("TokenJWT");

            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Error"] = "Debe iniciar sesión.";
                return RedirectToAction("Login", "Usuario");
            }

            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/usuario/" + usuarioId,
                VerbosHttp.GET,
                null,
                token
            );

            if (respuesta.IsSuccessStatusCode)
            {
                string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

                IEnumerable<PrestamoModel>? prestamos =
                    JsonConvert.DeserializeObject<IEnumerable<PrestamoModel>>(body);

                return View(prestamos ?? new List<PrestamoModel>());
            }

            string bodyError = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = $"{(int)respuesta.StatusCode} {respuesta.StatusCode} - Ocurrió un error obteniendo los préstamos del usuario.";

            if (!string.IsNullOrWhiteSpace(bodyError))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(bodyError);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    TempData["Error"] = error.Mensaje;
                }
            }

            return RedirectToAction("Index", "Usuario");
        }

        [RolAuthorizeAttribute(new string[] { "SOCIO" })]
        [HttpGet]
        public IActionResult PrestamosUsuarioEnPeriodo()
        {
            string? token = HttpContext.Session.GetString("TokenJWT");
            int? usuarioLogeadoId = HttpContext.Session.GetInt32("UsuarioLogeadoId");

            if (usuarioLogeadoId <= 0)
            {
                TempData["Error"] = "Debe iniciar sesión.";
                return RedirectToAction("Login", "Usuario");
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Error"] = "Debe iniciar sesión.";
                return RedirectToAction("Login", "Usuario");
            }

            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/usuario/" + usuarioLogeadoId,
                VerbosHttp.GET,
                null,
                token
            );

            if (respuesta.IsSuccessStatusCode)
            {
                string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

                IEnumerable<PrestamoModel>? prestamos =
                    JsonConvert.DeserializeObject<IEnumerable<PrestamoModel>>(body);

                return View(prestamos ?? new List<PrestamoModel>());
            }

            string bodyError = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = $"{(int)respuesta.StatusCode} {respuesta.StatusCode} - Ocurrió un error obteniendo los préstamos del usuario.";
            
            if (!string.IsNullOrWhiteSpace(bodyError))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(bodyError);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    TempData["Error"] = error.Mensaje;
                }
            }

            return RedirectToAction("Index", "Usuario");
        }

        [RolAuthorizeAttribute(new string[] { "SOCIO" })]
        [HttpPost]
        public IActionResult PrestamosUsuarioEnPeriodo(DateTime inicio, DateTime fin )
        {
            int? usuarioLogeadoId = HttpContext.Session.GetInt32("UsuarioLogeadoId");
            string? token = HttpContext.Session.GetString("TokenJWT");

            if (usuarioLogeadoId <= 0)
            {
                TempData["Error"] = "Debe iniciar sesión.";
                return RedirectToAction("Login", "Usuario");
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Error"] = "Debe iniciar sesión.";
                return RedirectToAction("Login", "Usuario");
            }

            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/usuario/" + usuarioLogeadoId.Value + "/en-periodo?inicio=" + inicio.ToString("yyyy-MM-dd") + "&fin=" + fin.ToString("yyyy-MM-dd"),
                VerbosHttp.GET,
                null,
                token
            );

            if (respuesta.IsSuccessStatusCode)
            {
                string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

                IEnumerable<PrestamoModel>? prestamos =
                    JsonConvert.DeserializeObject<IEnumerable<PrestamoModel>>(body);

                return View(prestamos ?? new List<PrestamoModel>());
            }

            string bodyError = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = $"{(int)respuesta.StatusCode} {respuesta.StatusCode} - Ocurrió un error obteniendo los préstamos del usuario.";

            if (!string.IsNullOrWhiteSpace(bodyError))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(bodyError);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    TempData["Error"] = error.Mensaje;
                }
            }

            return RedirectToAction("Index", "Usuario");
        }

        [RolAuthorizeAttribute(new string[] { "COORDINADOR" })]
        [HttpGet]
        public IActionResult ConsultarDisponibilidad(int equipoId)
        {
            string? token = HttpContext.Session.GetString("TokenJWT");

            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Error"] = "Debe iniciar sesión.";
                return RedirectToAction("Login", "Usuario");
            }

            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                "http://localhost:5196/api/Equipo/disponibilidad/" + equipoId,
                VerbosHttp.GET,
                null,
                token
            );

            if (respuesta.IsSuccessStatusCode)
            {
                return Ok();
            }

            string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            if (!string.IsNullOrWhiteSpace(body))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(body);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    return StatusCode((int)respuesta.StatusCode, new
                    {
                        mensaje = error.Mensaje
                    });
                }
            }

            return StatusCode((int)respuesta.StatusCode, new
            {
                mensaje = "El equipo seleccionado no tiene disponibilidad."
            });
        }


    }
}
