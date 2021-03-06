﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.Backend.Data.Entities
{
    public class TransportGuide
    {
        [Key]
        public int ID { get; set; }
        public int Numero { get; set; }
        public int Estado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Recibido { get; set; }
        public DateTime LlegadaCamion { get; set; }
        public DateTime SalidaFinca { get; set; }
        public DateTime Estimado { get; set; }
        public DateTime? LlegadaTerminal { get; set; }
        public Vehiculos Vehiculo { get; set; }
        public Fincas Finca { get; set; }
        public Puertos Puerto { get; set; }
        public Destinos Destino { get; set; }
        public Buques Buque { get; set; }
        public Exportadores Exportador { get; set; }
    }
}
