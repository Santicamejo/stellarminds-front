using StellarMindsWebApp.Models.Equipo;

namespace StellarMindsWebApp.Models.Usuario
{
    public class UsuariosPorTelescopioModel
    {
        public int? TelescopioId { get; set; } = null;

        public IEnumerable<TelescopioModel> Telescopios { get; set; } = new List<TelescopioModel>();
        public IEnumerable<UsuarioModel> Usuarios { get; set; } = new List<UsuarioModel>();
    }
}
