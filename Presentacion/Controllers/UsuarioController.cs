using Microsoft.AspNetCore.Mvc;
using DTOs.DTOs.Usuario;
using Exceptions.InfraestructuraExceptions;
using LogicaAplicacion.InterfacesCasosDeUso.Usuarios;
using Exceptions.LogicaNegocioExceptions;

namespace Presentacion.Controllers
{
    public class UsuarioController : Controller
    {
        #region Inyeccion de dependencias
        private IAgregarUsuario _altaUsuarioCU;
        private ILoginUsuario _loginUsuarioCU;
        private IObtenerTodosUsuarios _obtenerTodosLosUsuariosCU;

        public UsuarioController(IAgregarUsuario altaUsuario, ILoginUsuario loginUsuario, IObtenerTodosUsuarios obtenerTodosLosUsuarios)
        {
            _altaUsuarioCU = altaUsuario;
            _loginUsuarioCU = loginUsuario;
            _obtenerTodosLosUsuariosCU = obtenerTodosLosUsuarios;
        }
        #endregion

        #region Validacion de permisos
        private bool EstaLogeado()
        {
            string? email = HttpContext.Session.GetString("Email");
            return email != null;
        }
        private bool EsAdministrador()
        {
            int? rol = HttpContext.Session.GetInt32("Rol");
            return rol == 0;
        }
        private ActionResult RedirigirSiNoEstaLogeado()
        {
            TempData["LoginRequired"] = "Debes iniciar sesion para ver esta pagina";
            return RedirectToAction("Login", "Usuario");
        }
        private ActionResult RedirigirSiNoEsAdmin()
        {
            TempData["ErrorMessage"] = "No tenés permisos para realizar esta acción.";
            return RedirectToAction("Index", "Home");
        }
        #endregion

        public IActionResult Index()
        {
            #region Login y permisos
            if (!EstaLogeado())
            {
                return RedirigirSiNoEstaLogeado();
            }

            if (!EsAdministrador())
            {
                return RedirigirSiNoEsAdmin();
            }
            #endregion

            try
            {
                IEnumerable<UsuarioDTO> lista = _obtenerTodosLosUsuariosCU.Ejecutar();
                return View(lista);
            }
            catch (BaseDeDatosException ex)
            {
                ViewBag.ErrorMessage = "Ocurrió un error al acceder a la base de datos: " + ex.Message;
                IEnumerable<UsuarioDTO> empty = new List<UsuarioDTO>();
                return View(empty);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Ocurrió un error inesperado: " + ex.Message;
                IEnumerable<UsuarioDTO> empty = new List<UsuarioDTO>();
                return View(empty);
            }
        }


        [HttpGet]
        public IActionResult Alta()
        {
            #region Login y permisos
            if (!EstaLogeado())
            {
                return RedirigirSiNoEstaLogeado();
            }

            if (!EsAdministrador())
            {
                return RedirigirSiNoEsAdmin();
            }
            #endregion

            return View(new AltaUsuarioDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Alta(AltaUsuarioDTO dto)
        {
            #region Login y permisos
            if (!EstaLogeado())
            {
                return RedirigirSiNoEstaLogeado();
            }

            if (!EsAdministrador())
            {
                return RedirigirSiNoEsAdmin();
            }
            #endregion

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                _altaUsuarioCU.Ejecutar(dto);
                TempData["SuccessMessage"] = "Usuario creado correctamente.";
                return RedirectToAction("Index");
            }
            catch (DatosInvalidosException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            catch (OperacionNoPermitidaException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            catch (BaseDeDatosException ex)
            {
                ViewBag.ErrorMessage = "Ocurrió un error al acceder a la base de datos: " + ex.Message;
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = "Ocurrio un error inesperado: " + ex.Message;
            }
            return View(dto);
        }
 

        [HttpGet]
        public IActionResult Login()
        {
            if (EstaLogeado())
            {
                TempData["InfoMessage"] = "Ya has iniciado sesión.";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginUsuarioDTO modeloUsuario)
        {
            if (EstaLogeado())
            {
                TempData["InfoMessage"] = "Ya has iniciado sesión.";
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid) 
                return View(modeloUsuario);

            try
            {
                UsuarioLogueadoDTO usuarioAutenticado = _loginUsuarioCU.Ejecutar(modeloUsuario);

                HttpContext.Session.SetInt32("UsuarioId", usuarioAutenticado.Id);
                HttpContext.Session.SetString("Email", usuarioAutenticado.Email);
                HttpContext.Session.SetString("Nombre", usuarioAutenticado.NombreUsuario);
                //ToCheck
                HttpContext.Session.SetInt32("Rol", (int)usuarioAutenticado.Rol);


                TempData["SuccessMessage"] = "Bienvenido, " + usuarioAutenticado.NombreUsuario;
                return RedirectToAction("Index", "Home");
            }

            catch (CredencialesInvalidasException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            catch (DatosInvalidosException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            catch (BaseDeDatosException ex)
            {
                ViewBag.ErrorMessage = "Ocurrió un error al acceder a la base de datos: " + ex.Message;
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Ocurrió un error inesperado: " + ex.Message;
            }
            return View(modeloUsuario);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            if (EstaLogeado())
            {
                HttpContext.Session.Clear();
                TempData["SuccessMessage"] = "Sesión cerrada correctamente.";
            }

            return RedirectToAction("Login", "Usuario");
        }

        public IActionResult UsuariosPorEquipo()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return View();
            }
        }
        
    }
}