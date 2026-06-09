using StellarMinds.Enums;

namespace StellarMindsWebApp.Models.Prestamo
{
    public class PrestamoSelectEvaluacionModel
    {
        public int PrestamoId { get; set; }
        public string? TelescopioMarcaModelo { get; set; }
        public string? CamaraMarcaModelo { get; set; }
        public string? OcularMarcaModelo { get; set; }
    }
}
