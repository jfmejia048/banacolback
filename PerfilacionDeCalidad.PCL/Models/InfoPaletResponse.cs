using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.PCL.Models
{
    public class InfoPaletResponse
    {
        public int idPallet { get; set; }
        public string finca { get; set; }
        public string terminalDestino { get; set; }
        public int poma { get; set; }
        public string fruta { get; set; }
        public string buque { get; set; }
        public DateTime llegada { get; set; }
        public DateTime salida { get; set; }
        public DateTime estimado { get; set; }
        public DateTime? llegadaTerminal { get; set; }
        public bool cajas { get; set; }
        public string exportador { get; set; }
        public string destino { get; set; }
        public string carga { get; set; }
        public string codigoDeBarras { get; set; }
        public int cajasPalet { get; set; }
        public bool perfilar { get; set; }
        public int caraPallet { get; set; }
        public int idTransportGuide { get; set; }
    }
}
