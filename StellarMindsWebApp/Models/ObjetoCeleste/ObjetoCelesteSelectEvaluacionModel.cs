using StellarMinds.Enums;

namespace StellarMindsWebApp.Models.ObjetoCeleste
{
    public class ObjetoCelesteSelectEvaluacionModel
    {
        public int IdObjetoCeleste { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public TipoObjetoCeleste Tipo { get; set; }
    }
}
