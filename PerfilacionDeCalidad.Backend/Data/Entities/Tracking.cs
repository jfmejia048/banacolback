using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.Backend.Data.Entities
{
    public class Tracking
    {
        [Key]
        public int ID { get; set; }

        public int Codigo { get; set; }

        public Palets Palet { get; set; }

        public DateTime LecturaPalet { get; set; }

        [MaxLength(100, ErrorMessage = "El campo {0} solo permite un maximo de {1} caracteres")]
        public string Punto { get; set; }

        [MaxLength(100, ErrorMessage = "El campo {0} solo permite un maximo de {1} caracteres")]
        public string Localizacion { get; set; }

        [MaxLength(100, ErrorMessage = "El campo {0} solo permite un maximo de {1} caracteres")]
        public string Evento { get; set; }
    }
}
