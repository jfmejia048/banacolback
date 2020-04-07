using System;
using System.Collections.Generic;
using System.Text;

namespace Common.DTO
{
    public class TransportPalletsDTO
    {
        public int IdPallet { get; set; }
        public int CodigoPalet { get; set; }
        public string Carga { get; set; }
        public string CodigoDeBarras { get; set; }
        public int CajasPalet { get; set; }
        public bool Perfilar { get; set; }
    }
}
