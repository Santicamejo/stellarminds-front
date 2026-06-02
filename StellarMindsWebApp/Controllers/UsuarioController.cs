using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StellarMindsWebApp.Auxiliar;
using StellarMindsWebApp.Enums;
using StellarMindsWebApp.Models;
using StellarMindsWebApp.Models.Usuario;

namespace StellarMindsWebApp.Controllers
{
    public class UsuarioController : Controller
    {

        private string baseUrl = "http://localhost:5196/api/usuario";
        
        public IActionResult Index()
        {
            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/todos",
                VerbosHttp.GET
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


        public IActionResult Crear()
        {
            return View(new AltaUsuarioModel());
        }

        [HttpPost]
        public IActionResult Crear(AltaUsuarioModel altaUsuario)
        {
            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(baseUrl, VerbosHttp.POST, altaUsuario);

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
                LoginUsuarioModel login = JsonConvert.DeserializeObject<LoginUsuarioModel>(jsonResponse);
                //HttpContext.Session.SetString("token", login.Token);
                HttpContext.Session.SetInt32("UsuarioLogeadoId", 1);

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


        //ToDo
        [HttpGet("usuarios-por-equipo/{id}")]
        public ActionResult<IEnumerable<UsuarioModel>> UsuariosPorEquipo(int id)
        {
            try
            {
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }


    }
}
