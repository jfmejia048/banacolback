using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.Backend.Models
{
    public class Parameters
    {
        public string Palet { get; set; }

        public string Finca { get; set; }

        public string Puerto { get; set; }

        public string Buque { get; set; }

        public string Destino { get; set; }

        public string Exportador { get; set; }

        public string Caja { get; set; }

        public string Fruta { get; set; }

        public string Poma { get; set; }

        public List<DateTime> RangoFechas { get; set; }
    }
}
