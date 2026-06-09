using StellarMinds.Enums;

namespace StellarMindsWebApp.Models.Equipo
{
    public class EditMonturaModel
    {
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int CantidadDisponible { get; set; }
        public TipoMontura TipoMontura { get; set; }
        public decimal CargaUtil { get; set; }
        public bool EsComputarizada { get; set; }
    }
}