using StellarMinds.Enums;

namespace StellarMindsWebApp.Models.PrestamoModel
{
    public class PrestamoEvaluacionModel
    {
        public int IdPrestamo { get; set; }

        // Telescopio
        public string? TelescopioMarcaModelo { get; set; }
        public decimal AperturaTelescopio { get; set; }
        public decimal FocalTelescopio { get; set; }
        public string RelacionFocalTelescopio { get; set; }

        // Para saber qué tipo de evaluación hacer
        public bool EsAstrofotografia { get; set; }

        // Cámara
        public string? CamaraMarcaModelo { get; set; }
        public TipoSensor? SensorCamara { get; set; }
        public decimal? ResolucionCamara { get; set; }
        public decimal? TamanoPixelCamara { get; set; }

        // Ocular
        public string? OcularMarcaModelo { get; set; }
        public decimal? FocalOcular { get; set; }
        public decimal? CampoAparenteOcular { get; set; }
    }
}
