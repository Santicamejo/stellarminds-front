using StellarMinds.Enums;

namespace StellarMindsWebApp.Models.ObjetoCeleste
{
    public class ObjetoCelesteModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public TipoObjetoCeleste Tipo { get; set; }
        public decimal MagnitudAparente { get; set; }
    }
}
