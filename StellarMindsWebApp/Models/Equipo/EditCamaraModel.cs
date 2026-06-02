using StellarMinds.Enums;

namespace DTOs.DTOs.Equipo
{
    public class EditCamaraModel
    {
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int CantidadDisponible { get; set; }
        public TipoSensor TipoSensor { get; set; }
        public decimal Resolucion { get; set; }
        public decimal TamanoPixel { get; set; }
    }
}