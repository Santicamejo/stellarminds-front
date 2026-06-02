using DTOs.DTOs.Equipo;
using DTOs.DTOs.Prestamo;
using DTOs.DTOs.Usuario;
using Humanizer;
using LogicaAplicacion.InterfacesCasosDeUso.AuditoriaPrestamos;
using LogicaAplicacion.InterfacesCasosDeUso.Equipos;
using LogicaAplicacion.InterfacesCasosDeUso.Observaciones;
using LogicaAplicacion.InterfacesCasosDeUso.Prestamos;
using LogicaAplicacion.InterfacesCasosDeUso.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StellarMinds.Entidades;
using StellarMinds.Enums;
using StellarMinds.ValueObjects;

namespace Presentacion.Controllers
{
    public class PrestamoController : Controller
    {
        #region Inyeccion de dependencias
        private IAltaPrestamo _altaPrestamo;
        private IObtenerTodosEquiposDisponibles _obtenerTodosEquiposDisponibles;
        private IObtenerTodosUsuarios _obtenerSocios;
        private IObtenerPrestamosUsuario _obtenerPrestamosUsuario;
        private IDevolverPrestamo _devolverPrestamo;
        private IAltaAuditoriaPrestamo _altaAuditoriaPrestamo;
        private IObtenerPrestamosEnPeriodo _obtenerPrestamosEnPeriodo;

        public PrestamoController(
            IAltaPrestamo altaPrestamo,
            IObtenerTodosEquiposDisponibles todosEquiposDisponibles,
            IObtenerTodosUsuarios obtenerSocios,
            IObtenerPrestamosUsuario obtenerPrestamosUsuario,
            IDevolverPrestamo devolverPrestamo,
            IAltaAuditoriaPrestamo altaAuditoriaPrestamo, 
            IObtenerPrestamosEnPeriodo obtenerPrestamosEnPeriodo
            )
        {
            _altaPrestamo = altaPrestamo;
            _devolverPrestamo = devolverPrestamo;
            _obtenerPrestamosUsuario = obtenerPrestamosUsuario;
            _obtenerTodosEquiposDisponibles = todosEquiposDisponibles;
            _obtenerSocios = obtenerSocios;
            _altaAuditoriaPrestamo = altaAuditoriaPrestamo;
            _obtenerPrestamosEnPeriodo = obtenerPrestamosEnPeriodo;
        }
        #endregion

        // GET: PrestamosController/Alta
        public ActionResult Alta()
        {
            ViewBag.Usuarios = _obtenerSocios.Ejecutar();
            ViewBag.Equipos = _obtenerTodosEquiposDisponibles.Ejecutar();
            return View(new AltaPrestamoDTO());
        }

        // POST: PrestamosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Alta(AltaPrestamoDTO dto)
        {
            try
            {
                int prestamoId = _altaPrestamo.Ejecutar(dto);
                int? responsableId = HttpContext.Session.GetInt32("UsuarioId");
                _altaAuditoriaPrestamo.Ejecutar(prestamoId, responsableId.Value, EstadoPrestamo.PRESTADO );

                return RedirectToAction("Index","Usuario");
            }
            //ToDo
            catch (Exception ex)
            {
                ViewBag.Usuarios = _obtenerSocios.Ejecutar();
                ViewBag.Equipos = _obtenerTodosEquiposDisponibles.Ejecutar();
                ViewBag.ErrorMessage = ex.Message;
                return View(dto);
            }
        }

        // POST: PrestamosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Devolver(int id)
        {
            try
            {
                int? responsableId = HttpContext.Session.GetInt32("UsuarioId");
                //ToCheck
                _devolverPrestamo.Ejecutar(id, (int)responsableId);

                _altaAuditoriaPrestamo.Ejecutar(id, responsableId.Value, EstadoPrestamo.DEVUELTO);

                return RedirectToAction("Index", "Usuario");
            }
            //ToDo
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Usuario");
            }
        }

        public IActionResult PrestamosUsuario(int id)
        {
            try
            {
                IEnumerable<PrestamoDTO> prestamos = _obtenerPrestamosUsuario.Ejecutar(id);
                return View(prestamos);
            }
            //ToDo
            catch (Exception)
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult MisPrestamos()
        {
            int? usuarioLogeado = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioLogeado == null)
            {
                TempData["ErrorMessage"] = "Se debe iniciar sesión para ver sus préstamos.";
                return RedirectToAction("Login", "Usuario");
            }

            return View(new List<PrestamoDTO>());
        }
        
        [HttpPost]
        public IActionResult MisPrestamos(DateTime inicio, DateTime fin)
        {
            try
            {
                int? usuarioLogeado = HttpContext.Session.GetInt32("UsuarioId");

                if (usuarioLogeado == null)
                {
                    ViewBag.ErrorMessage = "Se debe iniciar sesion para ver sus prestamos.";
                    return RedirectToAction("Login", "Usuario");
                }
                
                Periodo periodo = new Periodo(inicio, fin);
                
                IEnumerable<PrestamoDTO> prestamosDTO = _obtenerPrestamosEnPeriodo.Ejecutar(periodo, (int)usuarioLogeado);

                return View(prestamosDTO);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

    }
}
    