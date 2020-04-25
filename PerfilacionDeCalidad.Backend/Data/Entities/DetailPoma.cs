using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.Backend.Data.Entities
{
    public class DetailPoma
    {
        public Frutas Frutas { get; set; }
        public List<Pallets> Palets { get; set; }
    }
}
