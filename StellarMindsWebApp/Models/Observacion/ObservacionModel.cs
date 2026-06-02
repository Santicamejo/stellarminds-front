using StellarMinds.Enums;

namespace Models.Observacion;
    public class ObservacionModel
    {
        public int Id { get; set; }
        public int PrestamoVinculadoId { get; set; }
        public int ObjetoVisualizadoId { get; set; }
        public string NombreObjetoVisualizado { get; set; } = string.Empty;
        public DateTime FechaObservacion { get; set; }
        public ResultadoEvaluacion ResultadoEvaluacion { get; set; }
        public string? Detalle { get; set; }
        public bool EsAstrofotografia { get; set; }
    }
