using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.Backend.Data.Entities
{
    public class CreatePomas
    {
        public int ID { get; set; }
        public int Numero { get; set; }
        public string Placa { get; set; }
        public bool Estado { get; set; }
        public Frutas Frutas { get; set; }
        public Cajas Cajas { get; set; }
        public Fincas Finca { get; set; }
        public Puertos Puerto { get; set; }
        public Buques Buque { get; set; }
        public Destinos Destino { get; set; }
        public Exportadores Exportador { get; set; }
        public List<Palets> Palets { get; set; }
    }
}
