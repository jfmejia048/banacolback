using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.Backend.Data.Entities
{
    public class DetailPoma
    {
        public Frutas Frutas { get; set; }
        public Destinos Destino { get; set; }
        public Buques Buque { get; set; }
        public Exportadores Exportador { get; set; }
        public List<Palets> Palets { get; set; }
    }
}
