using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace StellarMindsWebApp.Models.Equipo
{
    public class EditTelescopioModel
    {
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int CantidadDisponible { get; set; }
        public decimal Apertura { get; set; }
        public string RelacionFocal { get; set; }
        public decimal DistanciaFocal { get; set; }
        public decimal Peso { get; set; }
    }
}