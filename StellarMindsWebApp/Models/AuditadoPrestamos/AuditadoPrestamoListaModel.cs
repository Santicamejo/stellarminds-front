namespace StellarMindsWebApp.Models.AuditadoPrestamos
{
    public class AuditadoPrestamoListaModel
    {
        public int IdAuditoria { get; set; }

        public int PrestamoId { get; set; }

        public string SocioNombreApellido { get; set; } = string.Empty;

        public DateTime FechaAuditoria { get; set; }

        public string Accion { get; set; } = string.Empty;
    }
}
