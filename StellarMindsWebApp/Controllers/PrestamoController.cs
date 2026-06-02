using DTOs.DTOs.Equipo;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StellarMindsWebApp.Auxiliar;
using StellarMindsWebApp.Enums;
using StellarMindsWebApp.Models;
using StellarMindsWebApp.Models.Equipo;
using StellarMindsWebApp.Models.Prestamo;
using StellarMindsWebApp.Models.PrestamoModel;
using StellarMindsWebApp.Models.Usuario;

namespace StellarMindsWebApp.Controllers
{
    public class PrestamoController : Controller
    {

        private void CargarDatosAltaPrestamo()
        {
            HttpResponseMessage respuestaTelescopios = ClienteHttpAuxiliar.EnviarSolicitud(
                "http://localhost:5196/api/Equipo/telescopios",
                VerbosHttp.GET
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
                VerbosHttp.GET
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
                VerbosHttp.GET
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
                VerbosHttp.GET
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
                VerbosHttp.GET
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

        [HttpGet]
        public IActionResult Alta()
        {
            CargarDatosAltaPrestamo();
            return View(new AltaPrestamoModel());
        }

        [HttpPost]
        public ActionResult Alta(AltaPrestamoModel prestamo)
        {

            int? responsableId = HttpContext.Session.GetInt32("UsuarioLogeadoId");

            if (responsableId == null)
            {
                TempData["Error"] = "Debe iniciar sesión para crear un préstamo.";
                return RedirectToAction("Login", "Usuario");
            }

            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/alta?responsableId=" + responsableId.Value,
                VerbosHttp.POST,
                prestamo
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

            CargarDatosAltaPrestamo();
            return View(prestamo);
        }


        [HttpPost]
        public IActionResult Devolver(int prestamoId)
        {
            int? responsableId = HttpContext.Session.GetInt32("UsuarioLogeadoId");

            if (responsableId == null)
            {
                TempData["Error"] = "Debe iniciar sesión para devolver un préstamo.";
                return RedirectToAction("Login", "Usuario");
            }

            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/devolver?prestamoId=" + prestamoId + "&responsableId=" + responsableId.Value,
                VerbosHttp.POST
            );

            if (respuesta.IsSuccessStatusCode)
            {
                TempData["Exito"] = "Préstamo devuelto correctamente";
                return RedirectToAction("Index", "Home");
            }

            string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = "Ocurrió un error inesperado devolviendo el préstamo.";

            if (!string.IsNullOrWhiteSpace(body))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(body);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    TempData["Error"] = error.Mensaje;
                }
            }

            return RedirectToAction("Index", "Usuario");
        }


        [HttpGet]
        public IActionResult PrestamosUsuario(int usuarioId)
        {
            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/usuario/" + usuarioId,
                VerbosHttp.GET
            );

            if (respuesta.IsSuccessStatusCode)
            {
                string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

                IEnumerable<PrestamoModel>? prestamos =
                    JsonConvert.DeserializeObject<IEnumerable<PrestamoModel>>(body);

                return View(prestamos);
            }

            string bodyError = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = "Ocurrió un error obteniendo los préstamos del usuario.";

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


        [HttpGet]
        public IActionResult PrestamosUsuarioEnPeriodo(int usuarioId, DateTime inicio, DateTime fin )
        {
            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/usuario/" + usuarioId + "/en-periodo?inicio=" + inicio.ToString("yyyy-MM-dd") + "&fin=" + fin.ToString("yyyy-MM-dd"),
                VerbosHttp.GET
            );

            if (respuesta.IsSuccessStatusCode)
            {
                string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

                IEnumerable<PrestamoModel>? prestamos =
                    JsonConvert.DeserializeObject<IEnumerable<PrestamoModel>>(body);

                return View(prestamos);
            }

            string bodyError = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = "Ocurrió un error obteniendo los préstamos del usuario.";

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


        //Sin View
        [HttpGet]
        public IActionResult ObtenerPrestamoId(int prestamoId)
        {
            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
            baseUrl + "/" + prestamoId,
            VerbosHttp.GET
            );

            if (respuesta.IsSuccessStatusCode)
            {
                string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

                PrestamoModel? prestamo =
                    JsonConvert.DeserializeObject<PrestamoModel>(body);

                return View(prestamo);
            }

            string bodyError = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = "Ocurrió un error el prestamo.";

            if (!string.IsNullOrWhiteSpace(bodyError))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(bodyError);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    TempData["Error"] = error.Mensaje;
                }
            }

            return RedirectToAction("Index", "Home");

        }


        [HttpGet]
        public IActionResult ConsultarDisponibilidad(int equipoId)
        {
            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                "http://localhost:5196/api/Equipo/disponibilidad/" + equipoId,
                VerbosHttp.GET
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
