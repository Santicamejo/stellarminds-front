using StellarMinds.Enums;

namespace StellarMindsWebApp.Models.Equipo
{
    public class EquipoDetalleModel
    {
        public int Id { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int CantidadDisponible { get; set; }

        // Telescopio
        public decimal? Apertura { get; set; }
        public string? RelacionFocal { get; set; }
        public decimal? DistanciaFocal { get; set; }
        public decimal? Peso { get; set; }

        // Montura
        public TipoMontura? TipoMontura { get; set; }
        public decimal? CargaUtil { get; set; }
        public bool? EsComputarizada { get; set; }

        // Cámara
        public TipoSensor? TipoSensor { get; set; }
        public decimal? Resolucion { get; set; }
        public decimal? TamanoPixel { get; set; }

        // Ocular
        public decimal? Diametro { get; set; }
        public decimal? AnguloVision { get; set; }
    }
}
