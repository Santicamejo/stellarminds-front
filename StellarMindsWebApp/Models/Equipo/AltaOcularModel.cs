using StellarMinds.Enums;

namespace StellarMindsWebApp.Models.Equipo
{
    public class AltaOcularModel : AltaEquipoModel
    {
        public decimal Diametro { get; set; }
        public decimal AnguloVision { get; set; }
    }
}