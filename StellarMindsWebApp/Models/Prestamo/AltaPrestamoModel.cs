namespace StellarMindsWebApp.Models.Prestamo
{
    public class AltaPrestamoModel
    {
        public DateTime Inicio { get; set; } = DateTime.Now;
        public DateTime Fin { get; set; } = DateTime.Now + TimeSpan.FromDays(7);

        public int EquipoVisualId { get; set; }
        public int MonturaId { get; set; }
        public int TelescopioId { get; set; }
        public int SocioId { get; set; }
    }
}
