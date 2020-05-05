using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.Backend.Data.Entities
{
    public class Pallets
    {
        [Key]
        public int ID { get; set; } 
        public DetailTransportGuide DetailTransportGuide { get; set; }
        [MaxLength(50, ErrorMessage = "El campo {0} solo permite un maximo de {1} caracteres")]
        public string CodigoPalet { get; set; }
        public string UsuarioLectura { get; set; }
        public DateTime LecturaPalet { get; set; }
        [MaxLength(50, ErrorMessage = "El campo {0} solo permite un maximo de {1} caracteres")]
        public string UsuarioInspeccion { get; set; }
        public DateTime InspeccionPalet { get; set; }
        public int CaraPalet { get; set; }
        public int NumeroCajas { get; set; }
        [MaxLength(100, ErrorMessage = "El campo {0} solo permite un maximo de {1} caracteres")]
        public string Carga { get; set; }
        public string Tipo { get; set; }
        public bool Perfilar { get; set; }
    }
}
