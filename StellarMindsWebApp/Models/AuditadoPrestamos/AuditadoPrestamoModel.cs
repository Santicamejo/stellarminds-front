using StellarMindsWebApp.Enums;

namespace StellarMindsWebApp.Models.AuditadoPrestamos
{
    public class AuditadoPrestamoModel
    {
        public int AuditadoPrestamoId { get; set; }
        public EstadoPrestamo Accion { get; set; }
        public DateTime Fecha { get; set; }
        public string ResponsableNombreApellido { get; set; } = string.Empty;
        public int PrestamoId { get; set; }
    }
}
