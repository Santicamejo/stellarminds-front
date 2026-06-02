using StellarMinds.Enums;

namespace DTOs.DTOs.Equipo
{
    public class AltaOcularModel : AltaEquipoModel
    {
        public decimal Diametro { get; set; }
        public decimal AnguloVision { get; set; }
    }
}