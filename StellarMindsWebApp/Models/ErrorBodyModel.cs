using Newtonsoft.Json;

namespace StellarMindsWebApp.Models
{
    public class ErrorApiModel
    {
        [JsonProperty("mensaje")]
        public string Mensaje { get; set; } = string.Empty;
    }
}
