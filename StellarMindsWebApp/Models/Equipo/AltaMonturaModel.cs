using StellarMinds.Enums;

namespace DTOs.DTOs.Equipo
{
    public class AltaMonturaModel : AltaEquipoModel
    {
        public TipoMontura TipoMontura { get; set; }
        public decimal CargaUtil { get; set; }
        public bool EsComputarizada { get; set; }

    }
}