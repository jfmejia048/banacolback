using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.Backend.Data.Entities
{
    public class Palets
    {
        [Key]
        public int ID { get; set; } 

        public Fincas Finca { get; set; }

        public Puertos Puerto { get; set; }

        public Buques Buque { get; set; }

        public Destinos Destino { get; set; }

        public Exportadores Exportador { get; set; }

        public Cajas Caja { get; set; }

        public int Codigo { get; set; }

        [MaxLength(20, ErrorMessage = "El campo {0} solo permite un maximo de {1} caracteres")]
        public string CodigoPalet { get; set; }

        public TimeSpan LlegadaCamion { get; set; }

        public TimeSpan SalidaFinca { get; set; }

        public TimeSpan Estimado { get; set; }

        public TimeSpan LlegadaTerminal { get; set; }

        public TimeSpan LecturaPalet { get; set; }

        [MaxLength(50, ErrorMessage = "El campo {0} solo permite un maximo de {1} caracteres")]
        public string UsuarioLectura { get; set; }

        public TimeSpan InspeccionPalet { get; set; }

        public int CaraPalet { get; set; }
    
        public int NumeroCajas { get; set; }

        [MaxLength(100, ErrorMessage = "El campo {0} solo permite un maximo de {1} caracteres")]
        public string Carga { get; set; }

        public bool Perfilar { get; set; }
    }
}
