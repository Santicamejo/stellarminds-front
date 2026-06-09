namespace StellarMindsWebApp.Models.ObjetoCeleste
{
    public class RankingObjetoCelesteModel
    {
        public int ObjetoCelesteId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int CantidadObservaciones { get; set; }
    }
}
