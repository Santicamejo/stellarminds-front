using StellarMinds.Enums;

namespace StellarMindsWebApp.Models.Equipo
{
    public class AltaCamaraModel : AltaEquipoModel
    {
        public TipoSensor TipoSensor { get; set; }
        public decimal Resolucion { get; set; }
        public decimal TamanoPixel { get; set; }
    }
}