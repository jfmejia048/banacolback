using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.Backend.Enum
{
    public enum EstadosPoma
    {
        [Description("No Chequeado")]
        NoChequeado = 0,
        [Description("Chequeado")]
        Chequeado = 1,
        [Description("Perfilado")]
        Perfilado = 2
    }
}
