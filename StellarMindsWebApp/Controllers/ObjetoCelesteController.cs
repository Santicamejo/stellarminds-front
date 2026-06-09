using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StellarMindsWebApp.Auxiliar;
using StellarMindsWebApp.Enums;
using StellarMindsWebApp.Models.ObjetoCeleste;

namespace StellarMindsWebApp.Controllers
{
    public class ObjetoCelesteController : Controller
    {
        
        private string baseUrl = "http://localhost:5196/api/ObjetoCeleste";

        [HttpGet]
        public IActionResult Ranking()
        {
            string? token = HttpContext.Session.GetString("TokenJWT");

            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Error"] = "Debe iniciar sesión.";
                return RedirectToAction("Login", "Usuario");
            }

            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/ranking",
                VerbosHttp.GET,
                null,
                token
            );

            if (respuesta.IsSuccessStatusCode)
            {
                string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

                IEnumerable<RankingObjetoCelesteModel>? ranking =
                    JsonConvert.DeserializeObject<IEnumerable<RankingObjetoCelesteModel>>(body);

                return View(ranking ?? new List<RankingObjetoCelesteModel>());
            }

            TempData["Error"] = "No se pudo obtener el ranking de objetos celestes.";

            return View(new List<RankingObjetoCelesteModel>());
        }
    }

}
