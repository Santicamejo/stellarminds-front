using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StellarMindsWebApp.Auxiliar;
using StellarMindsWebApp.Enums;
using StellarMindsWebApp.Filtros;
using StellarMindsWebApp.Models;
using StellarMindsWebApp.Models.AuditadoPrestamos;
using StellarMindsWebApp.Models.Prestamo;
using StellarMindsWebApp.Models.Usuario;

namespace StellarMindsWebApp.Controllers
{
    public class AuditadoPrestamoController : Controller
    {

        private string baseUrl = "http://ObliStellarMindsM3A.somee.com/api/AuditadoPrestamo";

        [RolAuthorizeAttribute(new string[] { "ADMINISTRADOR" })]
        public ActionResult PrestamosPorCoordinador(int? coordinadorId)
        {
            string? token = HttpContext.Session.GetString("TokenJWT");

            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Error"] = "Debe iniciar sesión.";
                return RedirectToAction("Login", "Usuario");
            }

            PrestamosPorCoordinadorModel prestamosPorCoordinadorModel = new PrestamosPorCoordinadorModel();

            prestamosPorCoordinadorModel.CoordinadorIdSeleccionado = coordinadorId;

            HttpResponseMessage respuestaUsuarios = ClienteHttpAuxiliar.EnviarSolicitud(
                "http://ObliStellarMindsM3A.somee.com/api/usuario/coordinadores",
                VerbosHttp.GET,
                null,
                token
            );

            if (respuestaUsuarios.IsSuccessStatusCode)
            {
                string objetoComoTexto = ClienteHttpAuxiliar.ObtenerBody(respuestaUsuarios);

                IEnumerable<UsuarioModel>? coordinadores =
                    JsonConvert.DeserializeObject<IEnumerable<UsuarioModel>>(objetoComoTexto);

                prestamosPorCoordinadorModel.Coordinadores = coordinadores ?? new List<UsuarioModel>();
            }
            else
            {
                TempData["Error"] = "No se pudieron obtener los coordinadores.";
                return View(prestamosPorCoordinadorModel);
            }

            if (coordinadorId != null && coordinadorId > 0)
            {
                HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                    baseUrl + "/por-coordinador/" + coordinadorId,                    
                    VerbosHttp.GET,
                    null,
                    token
                );

                if (respuesta.IsSuccessStatusCode)
                {
                    string objetoComoTexto = ClienteHttpAuxiliar.ObtenerBody(respuesta);

                    IEnumerable<AuditadoPrestamoListaModel>? prestamosAuditados = JsonConvert.DeserializeObject<IEnumerable<AuditadoPrestamoListaModel>>(objetoComoTexto);

                    prestamosPorCoordinadorModel.PrestamosAuditados = prestamosAuditados ?? new List<AuditadoPrestamoListaModel>();
                }
                else
                {
                    string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

                    if (!string.IsNullOrWhiteSpace(body))
                    {
                        ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(body);

                        if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                        {
                            TempData["Error"] = error.Mensaje;
                        }
                    }
                }
            }
            return View(prestamosPorCoordinadorModel);
        }


        [RolAuthorizeAttribute(new string[] { "ADMINISTRADOR" })]
        public ActionResult AuditoriaPrestamo(int IdAuditoria)
        {
            string? token = HttpContext.Session.GetString("TokenJWT");

            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Error"] = "Debe iniciar sesión.";
                return RedirectToAction("Login", "Usuario");
            }

            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/" + IdAuditoria,
                VerbosHttp.GET,
                null,
                token
            );

            if (respuesta.IsSuccessStatusCode)
            {
                string objetoComoTexto = ClienteHttpAuxiliar.ObtenerBody(respuesta);

                AuditadoPrestamoModel? auditado =
                    JsonConvert.DeserializeObject<AuditadoPrestamoModel>(objetoComoTexto);

                if (auditado == null)
                {
                    TempData["Error"] = "No se pudo obtener la auditoría.";
                    return RedirectToAction(nameof(PrestamosPorCoordinador));
                }

                return View(auditado);
            }

            string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] =
                "Error obteniendo la auditoría. Código: " +
                (int)respuesta.StatusCode +
                " - " + respuesta.ReasonPhrase +
                ". Detalle: " + body;

            if (!string.IsNullOrWhiteSpace(body))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(body);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    TempData["Error"] = error.Mensaje;
                }
            }

            return RedirectToAction(nameof(PrestamosPorCoordinador));
        }

    }
}
