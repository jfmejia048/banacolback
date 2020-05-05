using System;
using System.Collections.Generic;
using System.Text;

namespace Common.DTO
{
    public class TransportPalletsDTO
    {
        public int IdPallet { get; set; }
        public string Carga { get; set; }
        public string CodigoDeBarras { get; set; }
        public int CajasPalet { get; set; }
        public bool Perfilar { get; set; }
        public string UsuarioLectura { get; set; }
        public DateTime HoraLectura { get; set; }
        public string UsuarioInspeccion { get; set; }
        public DateTime HoraInspeccion { get; set; }
    }
}
