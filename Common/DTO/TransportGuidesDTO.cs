using System;
using System.Collections.Generic;
using System.Text;

namespace Common.DTO
{
    public class TransportGuidesDTO
    {
        public int IdPoma { get; set; }
        public string Finca { get; set; }
        public string TerminalDestino { get; set; }
        public int Poma { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string EstadoPoma { get; set; }
        public List<TransportFruitsDTO> Frutas { get; set; }
    }
}
