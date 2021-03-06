﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.Backend.Data.Entities
{
    public class Vehiculos
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(10, ErrorMessage = "El campo {0} solo permite un maximo de {1} caracteres")]
        public string Placa { get; set; }
    }
}
