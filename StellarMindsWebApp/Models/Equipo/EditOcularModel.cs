namespace StellarMindsWebApp.Models.Equipo
{
    public class EditOcularModel
    {
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int CantidadDisponible { get; set; }
        public decimal Diametro { get; set; }
        public decimal AnguloVision { get; set; }
    }
}