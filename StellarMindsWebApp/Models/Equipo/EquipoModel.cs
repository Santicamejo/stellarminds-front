
namespace StellarMindsWebApp.Models.Equipo
{
    public class EquipoModel
    {
        public int Id { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int CantidadDisponible { get; set; }
    }
}
