using StellarMinds.Enums;

namespace StellarMindsWebApp.Models.PrestamoModel
{
    public class PrestamoSelectEvaluacionModel
    {
        public int IdPrestamo { get; set; }
        public string? TelescopioMarcaModelo { get; set; }
        public string? CamaraMarcaModelo { get; set; }
        public string? OcularMarcaModelo { get; set; }
    }
}
