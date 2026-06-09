using StellarMinds.Enums;
using System.Text.Json.Serialization;

namespace StellarMindsWebApp.Models.Observacion;

public class RespuestaGeminiModel
{
    [JsonPropertyName("indicador")]
    public ResultadoEvaluacion Indicador { get; set; }

    [JsonPropertyName("detalle")]
    public string? Detalle { get; set; } = string.Empty;
}   