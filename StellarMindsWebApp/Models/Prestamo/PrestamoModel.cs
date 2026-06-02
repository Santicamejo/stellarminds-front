using StellarMindsWebApp.Enums;

namespace StellarMindsWebApp.Models.PrestamoModel
{
    public class PrestamoModel
    {
        public int IdPrestamo { get; set; }

        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }

        public int? EquipoVisualId { get; set; }
        public string EquipoVisualMarcaModelo { get; set; } = string.Empty;

        public int? MonturaId { get; set; }
        public string MonturaMarcaModelo { get; set; } = string.Empty;

        public int? TelescopioId { get; set; }
        public string TelescopioMarcaModelo { get; set; } = string.Empty;

        public int? SocioId { get; set; }
        public string SocioNombreApellido { get; set; } = string.Empty;

        public EstadoPrestamo Estado { get; set; }
    }
}