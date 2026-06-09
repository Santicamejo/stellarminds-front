using StellarMindsWebApp.Models.Prestamo;
using StellarMindsWebApp.Models.Usuario;

namespace StellarMindsWebApp.Models.AuditadoPrestamos
{
    public class PrestamosPorCoordinadorModel
    {
        public int? CoordinadorIdSeleccionado { get; set; }

        public IEnumerable<UsuarioModel> Coordinadores { get; set; } = new List<UsuarioModel>();

        public IEnumerable<AuditadoPrestamoListaModel> PrestamosAuditados { get; set; } = new List<AuditadoPrestamoListaModel>();
    }
}
