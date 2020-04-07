using System;
using System.Collections.Generic;
using System.Text;

namespace Common.DTO
{
    public class TransportFruitsDTO
    {
        public string Fruta { get; set; }
        public string Buque { get; set; }
        public DateTime Llegada { get; set; }
        public DateTime Salida { get; set; }
        public DateTime Estimado { get; set; }
        public DateTime LlegadaTerminal { get; set; }
        public bool Cajas { get; set; }
        public string Exportador { get; set; }
        public string Destino { get; set; }
        public List<TransportPalletsDTO> Pallets { get; set; }
    }
}
