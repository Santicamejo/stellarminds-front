using StellarMinds.Enums;

namespace StellarMindsWebApp.Models.Equipo
{
    public class AltaMonturaModel : AltaEquipoModel
    {
        public TipoMontura TipoMontura { get; set; }
        public decimal CargaUtil { get; set; }
        public bool EsComputarizada { get; set; }

    }
}