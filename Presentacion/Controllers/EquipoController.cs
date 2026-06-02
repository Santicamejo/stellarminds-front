using DTOs.DTOs.Equipo;
using Exceptions.InfraestructuraExceptions;
using Exceptions.LogicaNegocioExceptions;
using LogicaAplicacion.InterfacesCasosDeUso.Equipos;
using Microsoft.AspNetCore.Mvc;
using StellarMinds.Enums;

namespace Presentacion.Controllers
{
    public class EquipoController : Controller
    {
        #region Inyeccion de dependencias
        private IObtenerTodosEquipos _obtenerTodosEquiposCU;
        private IAgregarEquipo _agregarEquipoCU;
        private IActualizarEquipo _actualizarEquipoCU;
        private IRemoverEquipo _eliminarEquipoCU;
        private IObtenerEquipoPorId _obtenerEquipoPorIdCU;

        public EquipoController(
            IObtenerTodosEquipos obtenerTodosEquiposCU,
            IAgregarEquipo agregarEquipoCU,
            IActualizarEquipo actualizarEquipoCU,
            IRemoverEquipo eliminarEquipoCU,
            IObtenerEquipoPorId obtenerEquipoPorIdCU)
        {
            _obtenerTodosEquiposCU = obtenerTodosEquiposCU;
            _agregarEquipoCU = agregarEquipoCU;
            _actualizarEquipoCU = actualizarEquipoCU;
            _eliminarEquipoCU = eliminarEquipoCU;
            _obtenerEquipoPorIdCU = obtenerEquipoPorIdCU;
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

        public ActionResult Index()
        {
            #region Validacion login
            if (!EstaLogeado())
            {
                return RedirigirSiNoEstaLogeado();
            }
            #endregion

            try
            {
                IEnumerable<EquipoDTO> equipos = _obtenerTodosEquiposCU.Ejecutar();
                return View(equipos);
            }
            catch (BaseDeDatosException ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al acceder a la base de datos: " + ex.Message;
                return View(new List<EquipoDTO>());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado: " + ex.Message;
                return View(new List<EquipoDTO>());
            }
        }

        public ActionResult Details(int id)
        {
            #region Validacion login
            if (!EstaLogeado())
            {
                return RedirigirSiNoEstaLogeado();
            }
            #endregion
            try
            {
                EquipoDTO equipo = _obtenerEquipoPorIdCU.Ejecutar(id);
                return View(equipo);
            }

            catch (DatosInvalidosException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (RecursoNoEncontradoException ex)
            {
                TempData["ErrorMessage"] =  ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (BaseDeDatosException ex)
            {
                TempData["ErrorMessage"] = "Ocurrio un error accediendo a la base de datos: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrio un error inesperado: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
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
                EquipoDTO equipo = _obtenerEquipoPorIdCU.Ejecutar(id);
                return View(equipo);
            }
            catch (DatosInvalidosException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (RecursoNoEncontradoException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (BaseDeDatosException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado: " + ex.Message;
            }
                return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
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
                _eliminarEquipoCU.Ejecutar(id);
                TempData["SuccessMessage"] = "Equipo eliminado correctamente.";
            }
            catch (DatosInvalidosException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (RecursoNoEncontradoException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (OperacionNoPermitidaException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (BaseDeDatosException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public ActionResult CreateTelescopio()
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

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTelescopio(AltaTelescopioDTO dto)
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
                _agregarEquipoCU.Ejecutar(dto);

                TempData["SuccessMessage"] = "Telescopio agregado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DatosInvalidosException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(dto);
            }
            catch (BaseDeDatosException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(dto);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Ocurrió un error inesperado: " + ex.Message;
                return View(dto);
            }
        }


        [HttpGet]
        public ActionResult CreateMontura()
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

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMontura(AltaMonturaDTO dto)
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
                _agregarEquipoCU.Ejecutar(dto);

                TempData["SuccessMessage"] = "Montura agregada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DatosInvalidosException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(dto);
            }
            catch (BaseDeDatosException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(dto);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Ocurrió un error inesperado: " + ex.Message;
                return View(dto);
            }
        }


        [HttpGet]
        public ActionResult CreateCamara()
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

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCamara(AltaCamaraDTO dto)
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
                _agregarEquipoCU.Ejecutar(dto);

                TempData["SuccessMessage"] = "Cámara agregada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DatosInvalidosException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(dto);
            }
            catch (BaseDeDatosException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(dto);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Ocurrió un error inesperado: " + ex.Message;
                return View(dto);
            }
        }


        [HttpGet]
        public ActionResult CreateOcular()
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

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOcular(AltaOcularDTO dto)
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
                _agregarEquipoCU.Ejecutar(dto);

                TempData["SuccessMessage"] = "Ocular agregado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DatosInvalidosException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(dto);
            }
            catch (BaseDeDatosException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(dto);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Ocurrió un error inesperado: " + ex.Message;
                return View(dto);
            }
        }


        [HttpGet]
        public ActionResult EditTelescopio(int id)
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
                EquipoDTO equipo = _obtenerEquipoPorIdCU.Ejecutar(id);

                if (equipo.GetType() != typeof(TelescopioDTO))
                {
                    TempData["ErrorMessage"] = "El equipo seleccionado no es un telescopio.";
                    return RedirectToAction(nameof(Index));
                }

                return View(equipo);
            }
            catch (DatosInvalidosException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (RecursoNoEncontradoException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (BaseDeDatosException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTelescopio(TelescopioDTO dto)
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
                _actualizarEquipoCU.Ejecutar(dto);

                TempData["SuccessMessage"] = "Telescopio actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DatosInvalidosException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(dto);
            }
            catch (RecursoNoEncontradoException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(dto);
            }
            catch (BaseDeDatosException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(dto);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Ocurrió un error inesperado: " + ex.Message;
                return View(dto);
            }
        }


        [HttpGet]
        public ActionResult EditMontura(int id)
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
                EquipoDTO equipo = _obtenerEquipoPorIdCU.Ejecutar(id);

                if (equipo.GetType() != typeof(MonturaDTO))
                {
                    TempData["ErrorMessage"] = "El equipo seleccionado no es una montura.";
                    return RedirectToAction(nameof(Index));
                }

                return View(equipo);
            }
            catch (DatosInvalidosException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (RecursoNoEncontradoException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (BaseDeDatosException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMontura(MonturaDTO dto)
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
                _actualizarEquipoCU.Ejecutar(dto);

                TempData["SuccessMessage"] = "Montura actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DatosInvalidosException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(dto);
            }
            catch (RecursoNoEncontradoException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(dto);
            }
            catch (BaseDeDatosException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(dto);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Ocurrió un error inesperado: " + ex.Message;
                return View(dto);
            }
        }


        [HttpGet]
        public ActionResult EditCamara(int id)
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
                EquipoDTO equipo = _obtenerEquipoPorIdCU.Ejecutar(id);

                if (equipo.GetType() != typeof(CamaraDTO))
                {
                    TempData["ErrorMessage"] = "El equipo seleccionado no es una cámara.";
                    return RedirectToAction(nameof(Index));
                }

                return View(equipo);
            }
            catch (DatosInvalidosException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (RecursoNoEncontradoException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (BaseDeDatosException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCamara(CamaraDTO dto)
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
                _actualizarEquipoCU.Ejecutar(dto);

                TempData["SuccessMessage"] = "Cámara actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DatosInvalidosException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(dto);
            }
            catch (RecursoNoEncontradoException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(dto);
            }
            catch (BaseDeDatosException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(dto);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Ocurrió un error inesperado: " + ex.Message;
                return View(dto);
            }
        }


        [HttpGet]
        public ActionResult EditOcular(int id)
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
                EquipoDTO equipo = _obtenerEquipoPorIdCU.Ejecutar(id);

                if (equipo.GetType() != typeof(OcularDTO))
                {
                    TempData["ErrorMessage"] = "El equipo seleccionado no es un ocular.";
                    return RedirectToAction(nameof(Index));
                }

                return View(equipo);
            }
            catch (DatosInvalidosException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (RecursoNoEncontradoException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (BaseDeDatosException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOcular(OcularDTO dto)
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
                _actualizarEquipoCU.Ejecutar(dto);

                TempData["SuccessMessage"] = "Ocular actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DatosInvalidosException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(dto);
            }
            catch (RecursoNoEncontradoException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(dto);
            }
            catch (BaseDeDatosException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(dto);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Ocurrió un error inesperado: " + ex.Message;
                return View(dto);
            }
        }


        [HttpPut("telescopio/{id}")]
        public ActionResult EditarTelescopio(int id, [FromBody] TelescopioDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new { mensaje = "Los datos del telescopio son obligatorios." });
                }

                dto.Id = id;

                _actualizarEquipoCU.Ejecutar(dto);

                return Ok(new
                {
                    mensaje = "Telescopio actualizado correctamente."
                });
            }
            catch (DatosInvalidosException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (RecursoNoEncontradoException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (BaseDeDatosException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    mensaje = "Ocurrió un error al acceder a la base de datos."
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    mensaje = "Ocurrió un error inesperado."
                });
            }
        }


        [HttpPut("montura/{id}")]
        public ActionResult EditarMontura(int id, [FromBody] MonturaDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new { mensaje = "Los datos de la montura son obligatorios." });
                }

                dto.Id = id;

                _actualizarEquipoCU.Ejecutar(dto);

                return Ok(new
                {
                    mensaje = "Montura actualizada correctamente."
                });
            }
            catch (DatosInvalidosException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (RecursoNoEncontradoException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (BaseDeDatosException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    mensaje = "Ocurrió un error al acceder a la base de datos."
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    mensaje = "Ocurrió un error inesperado."
                });
            }
        }


        [HttpPut("camara/{id}")]
        public ActionResult EditarCamara(int id, [FromBody] CamaraDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new { mensaje = "Los datos de la cámara son obligatorios." });
                }

                dto.Id = id;

                _actualizarEquipoCU.Ejecutar(dto);

                return Ok(new
                {
                    mensaje = "Cámara actualizada correctamente."
                });
            }
            catch (DatosInvalidosException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (RecursoNoEncontradoException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (BaseDeDatosException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    mensaje = "Ocurrió un error al acceder a la base de datos."
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    mensaje = "Ocurrió un error inesperado."
                });
            }
        }


        [HttpPut("ocular/{id}")]
        public ActionResult EditarOcular(int id, [FromBody] OcularDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new { mensaje = "Los datos del ocular son obligatorios." });
                }

                dto.Id = id;

                _actualizarEquipoCU.Ejecutar(dto);

                return Ok(new
                {
                    mensaje = "Ocular actualizado correctamente."
                });
            }
            catch (DatosInvalidosException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (RecursoNoEncontradoException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (BaseDeDatosException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    mensaje = "Ocurrió un error al acceder a la base de datos."
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    mensaje = "Ocurrió un error inesperado."
                });
            }
        }
    }
}