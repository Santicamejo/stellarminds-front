using DTOs.DTOs.Equipo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StellarMindsWebApp.Auxiliar;
using StellarMindsWebApp.Enums;
using StellarMindsWebApp.Models;
using StellarMindsWebApp.Models.Equipo;
using StellarMindsWebApp.Models.Usuario;

namespace StellarMindsWebApp.Controllers
{
    public class EquipoController : Controller
    {

        private string baseUrl = "http://localhost:5196/api/Equipo";

        public IActionResult Index()
        {
            HttpResponseMessage respuesta = 
                ClienteHttpAuxiliar.EnviarSolicitud(baseUrl + "/todos", VerbosHttp.GET);

            if (respuesta.IsSuccessStatusCode)
            {
                string objetoComoTexto = ClienteHttpAuxiliar.ObtenerBody(respuesta);

                IEnumerable<EquipoModel>? equipos = JsonConvert.DeserializeObject<IEnumerable<EquipoModel>>(objetoComoTexto);

                return View(equipos);
            }

            string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = "Ocurrio un error inesperado obteniendo los equipos.";

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


        //[HttpGet("disponibles")]
        //public ActionResult<IEnumerable<EquipoModel>> ObtenerEquiposDisponibles()
        //{
        //    HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(baseUrl + "/disponibles", Enums.VerbosHttp.GET);

        //    if (respuesta.IsSuccessStatusCode)
        //    {
        //        string objetoComoTexto = ClienteHttpAuxiliar.ObtenerBody(respuesta);
        //        IEnumerable<EquipoModel> equipos = JsonConvert.DeserializeObject<IEnumerable<EquipoModel>>(objetoComoTexto);
        //        return View(equipos);
        //    }
        //    return View();
        //}


        public IActionResult Detalles(int id)
        {
            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/" + id,
                VerbosHttp.GET
            );

            if (respuesta.IsSuccessStatusCode)
            {
                string objetoComoTexto = ClienteHttpAuxiliar.ObtenerBody(respuesta);

                EquipoDetalleModel? equipo = JsonConvert.DeserializeObject<EquipoDetalleModel>(objetoComoTexto);

                return View(equipo);
            }

            string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = "Ocurrio un error inesperado obteniendo el detalle.";

            if (!string.IsNullOrWhiteSpace(body))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(body);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    TempData["Error"] = error.Mensaje;
                }
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/" + id,
                VerbosHttp.DELETE
            );

            if (respuesta.IsSuccessStatusCode)
            {
                TempData["Exito"] = "Equipo eliminado correctamente";
                return RedirectToAction(nameof(Index));
            }

            string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = "Ocurrio un error inesperado eliminando el equipo.";

            if (!string.IsNullOrWhiteSpace(body))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(body);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    TempData["Error"] = error.Mensaje;
                }
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult CrearTelescopio()
        {
            return View(new AltaTelescopioModel());
        }

        [HttpPost]
        public ActionResult CrearTelescopio(AltaTelescopioModel telescopio)
        {
            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/crear-telescopio",
                VerbosHttp.POST,
                telescopio
            );

            if (respuesta.IsSuccessStatusCode)
            {
                TempData["Exito"] = "Telescopio creado correctamente";
                return RedirectToAction(nameof(Index));
            }

            string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = "Ocurrio un error inesperado creando el telescopio.";

            if (!string.IsNullOrWhiteSpace(body))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(body);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    TempData["Error"] = error.Mensaje;
                }
            }

            return View(telescopio);
        }


        [HttpGet]
        public IActionResult CrearMontura()
        {
            return View(new AltaMonturaModel());
        }

        [HttpPost]
        public ActionResult CrearMontura(AltaMonturaModel montura)
        {
            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/crear-montura",
                VerbosHttp.POST,
                montura
            );

            if (respuesta.IsSuccessStatusCode)
            {
                TempData["Exito"] = "Montura creada correctamente";
                return RedirectToAction(nameof(Index));
            }

            string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = "Ocurrio un error inesperado creando la montura.";

            if (!string.IsNullOrWhiteSpace(body))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(body);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    TempData["Error"] = error.Mensaje;
                }
            }

            return View(montura);
        }


        [HttpGet]
        public IActionResult CrearCamara()
        {
            return View(new AltaCamaraModel());
        }

        [HttpPost]
        public ActionResult CrearCamara(AltaCamaraModel camara)
        {
            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/crear-camara",
                VerbosHttp.POST,
                camara
            );

            if (respuesta.IsSuccessStatusCode)
            {
                TempData["Exito"] = "camara creada correctamente";
                return RedirectToAction(nameof(Index));
            }

            string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = "Ocurrio un error inesperado creando la camara.";

            if (!string.IsNullOrWhiteSpace(body))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(body);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    TempData["Error"] = error.Mensaje;
                }
            }

            return View(camara);
        }


        [HttpGet]
        public IActionResult CrearOcular()
        {
            return View(new AltaOcularModel());
        }

        [HttpPost]
        public ActionResult CrearOcular(AltaOcularModel ocular)
        {
            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/crear-ocular",
                VerbosHttp.POST,
                ocular
            );

            if (respuesta.IsSuccessStatusCode)
            {
                TempData["Exito"] = "ocular creado correctamente";
                return RedirectToAction(nameof(Index));
            }

            string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = "Ocurrio un error inesperado creando el ocular.";

            if (!string.IsNullOrWhiteSpace(body))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(body);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    TempData["Error"] = error.Mensaje;
                }
            }

            return View(ocular);
        }


        [HttpGet]
        public IActionResult EditarTelescopio(int id)
        {
            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/" + id,
                VerbosHttp.GET
            );

            if (respuesta.IsSuccessStatusCode)
            {
                string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

                EditTelescopioModel? telescopio =
                    JsonConvert.DeserializeObject<EditTelescopioModel>(body);

                return View(telescopio);
            }

            TempData["Error"] = "No se pudo obtener el telescopio.";

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult EditarTelescopio(int id, EditTelescopioModel telescopio)
        {
            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/telescopio/" + id,
                VerbosHttp.PUT,
                telescopio
            );

            if (respuesta.IsSuccessStatusCode)
            {
                TempData["Exito"] = "Telescopio editado correctamente";
                return RedirectToAction(nameof(Index));
            }

            string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = "Ocurrió un error inesperado editando el telescopio.";

            if (!string.IsNullOrWhiteSpace(body))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(body);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    TempData["Error"] = error.Mensaje;
                }
            }

            return View(telescopio);
        }


        [HttpGet]
        public IActionResult EditarMontura(int id)
        {
            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/" + id,
                VerbosHttp.GET
            );

            if (respuesta.IsSuccessStatusCode)
            {
                string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

                EditMonturaModel? montura =
                    JsonConvert.DeserializeObject<EditMonturaModel>(body);

                return View(montura);
            }

            TempData["Error"] = "No se pudo obtener la montura.";

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult EditarMontura(int id, EditMonturaModel montura)
        {
            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/montura/" + id,
                VerbosHttp.PUT,
                montura
            );

            if (respuesta.IsSuccessStatusCode)
            {
                TempData["Exito"] = "Montura editada correctamente";
                return RedirectToAction(nameof(Index));
            }

            string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = "Ocurrio un error inesperado editando la montura.";

            if (!string.IsNullOrWhiteSpace(body))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(body);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    TempData["Error"] = error.Mensaje;
                }
            }

            return View(montura);
        }


        [HttpGet]
        public IActionResult EditarCamara(int id)
        {
            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/" + id,
                VerbosHttp.GET
            );

            if (respuesta.IsSuccessStatusCode)
            {
                string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

                EditCamaraModel? camara =
                    JsonConvert.DeserializeObject<EditCamaraModel>(body);

                return View(camara);
            }

            TempData["Error"] = "No se pudo obtener la camara.";

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult EditarCamara(int id, EditCamaraModel camara)
        {
            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/camara/" + id,
                VerbosHttp.PUT,
                camara
            );

            if (respuesta.IsSuccessStatusCode)
            {
                TempData["Exito"] = "camara editada correctamente";
                return RedirectToAction(nameof(Index));
            }

            string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = "Ocurrió un error inesperado editando el telescopio.";

            if (!string.IsNullOrWhiteSpace(body))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(body);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    TempData["Error"] = error.Mensaje;
                }
            }

            return View(camara);
        }


        [HttpGet]
        public IActionResult EditarOcular(int id)
        {
            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/" + id,
                VerbosHttp.GET
            );

            if (respuesta.IsSuccessStatusCode)
            {
                string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

                EditOcularModel? ocular =
                    JsonConvert.DeserializeObject<EditOcularModel>(body);

                return View(ocular);
            }

            TempData["Error"] = "No se pudo obtener el ocular.";

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult EditarOcular(int id, EditOcularModel ocular)
        {
            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/ocular/" + id,
                VerbosHttp.PUT,
                ocular
            );

            if (respuesta.IsSuccessStatusCode)
            {
                TempData["Exito"] = "ocular editado correctamente";
                return RedirectToAction(nameof(Index));
            }

            string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            TempData["Error"] = "Ocurrió un error inesperado editando el telescopio.";

            if (!string.IsNullOrWhiteSpace(body))
            {
                ErrorApiModel? error = JsonConvert.DeserializeObject<ErrorApiModel>(body);

                if (error != null && !string.IsNullOrWhiteSpace(error.Mensaje))
                {
                    TempData["Error"] = error.Mensaje;
                }
            }

            return View(ocular);
        }


    }
}
