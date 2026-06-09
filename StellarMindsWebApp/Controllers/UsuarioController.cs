using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StellarMindsWebApp.Auxiliar;
using StellarMindsWebApp.Enums;
using StellarMindsWebApp.Filtros;
using StellarMindsWebApp.Models;
using StellarMindsWebApp.Models.Equipo;
using StellarMindsWebApp.Models.Usuario;

namespace StellarMindsWebApp.Controllers
{
    public class UsuarioController : Controller
    {

        private string baseUrl = "http://localhost:5196/api/usuario";

        [RolAuthorizeAttribute(new string[] { "ADMINISTRADOR", "COORDINADOR" })]
        public IActionResult Index()
        {
            string? token = HttpContext.Session.GetString("TokenJWT");

            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Error"] = "Debe iniciar sesión.";
                return RedirectToAction("Login", "Usuario");
            }

            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/todos",
                VerbosHttp.GET,
                null,
                token
            );

            if (respuesta.IsSuccessStatusCode)
            {
                string objetoComoTexto = ClienteHttpAuxiliar.ObtenerBody(respuesta);

                IEnumerable<UsuarioModel>? usuarios = JsonConvert.DeserializeObject<IEnumerable<UsuarioModel>>(objetoComoTexto);

                return View(usuarios);
            }

            string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = "Ocurrio un error inesperado obteniendo los usuarios.";

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

        [RolAuthorizeAttribute(new string[] { "ADMINISTRADOR" })]
        public IActionResult Crear()
        {
            return View(new AltaUsuarioModel());
        }

        [RolAuthorizeAttribute(new string[] { "ADMINISTRADOR" })]
        [HttpPost]
        public IActionResult Crear(AltaUsuarioModel altaUsuario)
        {
            string? token = HttpContext.Session.GetString("TokenJWT");

            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Error"] = "Debe iniciar sesión.";
                return RedirectToAction("Login", "Usuario");
            }

            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(baseUrl, VerbosHttp.POST, altaUsuario, token);

            if (respuesta.IsSuccessStatusCode)
            {
                TempData["Exito"] = "Usuario creado exitosamente";
                return RedirectToAction(nameof(Index));
            }

            string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = "Ocurrio un error inesperado creando el usuario.";

            if (!string.IsNullOrWhiteSpace(body))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(body);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    TempData["Error"] = error.Mensaje;
                }
            }

            return View(altaUsuario);
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginUsuarioModel());
        }

        [HttpPost]
        public IActionResult Login(string password, string email)
        {
            LoginUsuarioModel loginUsuario = new LoginUsuarioModel();
            loginUsuario.Email = email;
            loginUsuario.Password = password;

            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(baseUrl + "/login", VerbosHttp.POST, loginUsuario);

            if (respuesta.IsSuccessStatusCode)
            {
                string jsonResponse = ClienteHttpAuxiliar.ObtenerBody(respuesta);
                UsuarioLogueadoModel usuario = JsonConvert.DeserializeObject<UsuarioLogueadoModel>(jsonResponse);

                HttpContext.Session.SetInt32("UsuarioLogeadoId", usuario.Id);
                HttpContext.Session.SetString("UsuarioLogeadoEmail", usuario.Email);
                HttpContext.Session.SetString("UsuarioLogeadoRol", usuario.Rol.ToString());
                HttpContext.Session.SetString("TokenJWT", usuario.Token);

                TempData["Exito"] = "Bienvenido";
                return RedirectToAction("Index");
            }
            string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = "Ocurrio un error inesperado al iniciar sesion.";

            if (!string.IsNullOrWhiteSpace(body))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(body);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    TempData["Error"] = error.Mensaje;
                }
            }

            return View(loginUsuario);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            TempData["Exito"] = "Sesión cerrada correctamente.";

            return RedirectToAction("Login", "Usuario");
        }


        [RolAuthorizeAttribute(new string[] { "COORDINADOR", "ADMINISTRADOR" })]
        [HttpGet]
        public IActionResult UsuariosPorTelescopio(UsuariosPorTelescopioModel model)
        {
            string? token = HttpContext.Session.GetString("TokenJWT");

            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Error"] = "Debe iniciar sesión.";
                return RedirectToAction("Login", "Usuario");
            }

            HttpResponseMessage respuestaTelescopios = ClienteHttpAuxiliar.EnviarSolicitud(
                "http://localhost:5196/api/Equipo/telescopios",
                VerbosHttp.GET,
                null,
                token
            );

            if (respuestaTelescopios.IsSuccessStatusCode)
            {
                string bodyTelescopios = ClienteHttpAuxiliar.ObtenerBody(respuestaTelescopios);

                IEnumerable<TelescopioModel>? telescopios =
                    JsonConvert.DeserializeObject<IEnumerable<TelescopioModel>>(bodyTelescopios);

                model.Telescopios = telescopios ?? new List<TelescopioModel>();
            }
            else
            {
                model.Telescopios = new List<TelescopioModel>();
            }

            if (model.TelescopioId == null || model.TelescopioId <= 0)
            {
                return View(model);
            }

            HttpResponseMessage respuestaUsuarios = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/por-telescopio/" + model.TelescopioId.Value,
                VerbosHttp.GET,
                null,
                token
            );

            if (respuestaUsuarios.IsSuccessStatusCode)
            {
                string bodyUsuarios = ClienteHttpAuxiliar.ObtenerBody(respuestaUsuarios);

                IEnumerable<UsuarioModel>? usuarios =
                    JsonConvert.DeserializeObject<IEnumerable<UsuarioModel>>(bodyUsuarios);

                model.Usuarios = usuarios ?? new List<UsuarioModel>();

                return View(model);
            }

            string bodyError = ClienteHttpAuxiliar.ObtenerBody(respuestaUsuarios);

            TempData["Error"] = $"{(int)respuestaUsuarios.StatusCode} {respuestaUsuarios.StatusCode} - Ocurrió un error obteniendo los usuarios.";

            if (!string.IsNullOrWhiteSpace(bodyError))
            {
                try
                {
                    ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(bodyError);

                    if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                    {
                        TempData["Error"] = error.Mensaje;
                    }
                }
                catch (JsonReaderException)
                {
                    TempData["Error"] = bodyError;
                }
            }

            model.Usuarios = new List<UsuarioModel>();

            return View(model);
        }


    }
}
