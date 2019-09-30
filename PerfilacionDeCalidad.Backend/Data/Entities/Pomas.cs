using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.Backend.Data.Entities
{
    public class Pomas
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int Codigo { get; set; }

        public int Numero { get; set; }

        [MaxLength(10, ErrorMessage = "El campo {0} solo permite un maximo de {1} caracteres")]
        public string Placa { get; set; }

        public bool Estado { get; set; }
    }
}
