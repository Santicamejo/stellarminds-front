using StellarMinds.Enums;

namespace StellarMindsWebApp.Models.Equipo
{
    public class AltaTelescopioModel : AltaEquipoModel
    {
        public decimal Apertura { get; set; }
        public string RelacionFocal { get; set; } = string.Empty;
        public decimal DistanciaFocal { get; set; }
        public decimal Peso { get; set; }
    }
}