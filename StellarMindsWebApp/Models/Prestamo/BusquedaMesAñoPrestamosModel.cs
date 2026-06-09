namespace StellarMindsWebApp.Models.Prestamo
{
    public class BusquedaMesAnioPrestamosModel
    {
        public DateTime MesAnio { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        public IEnumerable<PrestamoModel> Prestamos { get; set; } = new List<PrestamoModel>();
    }
}
