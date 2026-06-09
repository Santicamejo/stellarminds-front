using StellarMinds.Enums;

namespace StellarMindsWebApp.Models.Equipo
{
    public abstract class AltaEquipoModel
    {
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int CantidadDisponible { get; set; }
    }
}