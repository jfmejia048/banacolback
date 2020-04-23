using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace PerfilacionDeCalidad.Movil.Enum
{
    public enum CarasPallet
    {
        [Description("01 Frente 1 DERECHA")]
        Cara1 = 1,
        [Description("02 Frente 1 CENTRO")]
        Cara2 = 2,
        [Description("03 Frente 1 IZQUIERDA")]
        Cara3 = 3,
        [Description("04 Frente 2 DERECHA")]
        Cara4 = 4,
        [Description("05 Frente 2 CENTRO")]
        Cara5 = 5,
        [Description("06 Frente 2 IZQUIERDA")]
        Cara6 = 6,
    }
}