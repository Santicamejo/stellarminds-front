using StellarMindsWebApp.Enums;
using System.ComponentModel.DataAnnotations;

namespace StellarMindsWebApp.Models.Usuario
{
    public class AltaUsuarioModel
    {
        // Valores primitivos de NombreCompleto
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;


        // Valores primitivos de Direccion
        public string Calle { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;


        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio.")]
        public TipoUsuario Rol { get; set; }
    }
}
