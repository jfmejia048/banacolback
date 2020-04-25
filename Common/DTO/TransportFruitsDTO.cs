using System;
using System.Collections.Generic;
using System.Text;

namespace Common.DTO
{
    public class TransportFruitsDTO
    {
        public string CodigoDeBarras { get; set; }
        public string Fruta { get; set; }
        public int CajasPalet { get; set; }
        public string Tipo { get; set; }
        public string Carga { get; set; }
        public bool Cajas { get; set; }
        public bool Perfilar { get; set; }
        public int IdPallet { get; set; }
    }
}
