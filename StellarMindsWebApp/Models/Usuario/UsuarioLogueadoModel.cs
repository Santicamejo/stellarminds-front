using StellarMindsWebApp.Enums;

namespace StellarMindsWebApp.Models.Usuario
{
    public class UsuarioLogueadoModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public TipoUsuario Rol { get; set; }
    }
}
