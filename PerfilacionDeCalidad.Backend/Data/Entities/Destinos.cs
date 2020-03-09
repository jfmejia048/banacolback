﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.Backend.Data.Entities
{
    public class Destinos
    {
        [Key]
        public int ID { get; set; }

        public int Codigo { get; set; }

        [MaxLength(50, ErrorMessage = "El campo {0} solo permite un maximo de {1} caracteres")]
        public string DestinoName { get; set; }

        public bool Estado { get; set; }
    }
}