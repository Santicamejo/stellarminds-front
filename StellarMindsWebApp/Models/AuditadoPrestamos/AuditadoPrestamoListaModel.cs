using StellarMindsWebApp.Enums;

namespace StellarMindsWebApp.Models.AuditadoPrestamos
{
    public class AuditadoPrestamoListaModel
    {
        public int IdAuditoria { get; set; }

        public int IdPrestamo { get; set; }

        public string SocioNombreApellido { get; set; } = string.Empty;

        public DateTime FechaAuditoria { get; set; }

        public EstadoPrestamo Accion { get; set; }
    }
}
