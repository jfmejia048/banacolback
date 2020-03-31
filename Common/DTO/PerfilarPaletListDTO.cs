using System;
using System.Collections.Generic;
using System.Text;

namespace Common.DTO
{
    public class PerfilarPaletListDTO
    {
        public List<PerfilarPaletDTO> Perfilar { get; set; }
        public List<PerfilarPaletDTO> NoPerfilar { get; set; }
        public int IdPoma { get; set; }
        public int Action { get; set; }

    }
}
