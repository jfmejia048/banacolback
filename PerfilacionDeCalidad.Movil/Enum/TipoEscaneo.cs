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
    public enum TipoEscaneo
    {
        [Description("Usuario de chequo")]
        chequeo = 1,
        [Description("Usuario de calidad")]
        calidad = 2
    }
}