using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.Backend.Data.Entities
{
    public class Cajas
    {
        [Key]
        public int ID { get; set; }
    
        public int Codigo { get; set; }

        public Pomas Pomas { get; set; }

        public Frutas Frutas { get; set; }

        public int Cantidad { get; set; }

        public bool Estado { get; set; }
    }
}
