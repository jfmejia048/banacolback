using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.Backend.Data.Entities
{
    public class DetailTransportGuide
    {
        [Key]
        public int ID { get; set; }
        public TransportGuide TransportGuide { get; set; }
        public Frutas Fruta { get; set; }
    }
}
