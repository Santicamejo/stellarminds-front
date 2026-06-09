using StellarMinds.Enums;

namespace StellarMindsWebApp.Models.Observacion;

public class AltaObservacionModel
{
    public int PrestamoVinculadoId { get; set; }
    public int ObjetoVisualizadoId { get; set; }
    public ResultadoEvaluacion ResultadoEvaluacion { get; set; }
    public string? Detalle { get; set; }
    public DateTime FechaObservacion { get; set; } = DateTime.Today;
}