using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace PerfilacionDeCalidad.Movil.Helpers
{
    public class NavigationLoader
    {
        static bool InLoading = false;

        public static void ShowLoading(string Message = "Cargando")
        {
            if (!InLoading)
            {

                UserDialogs.Instance.ShowLoading(Message, MaskType.Gradient);


                InLoading = true;
            }
        }

        public static void HideLoading()
        {
            UserDialogs.Instance.HideLoading();
            InLoading = false;
        }

        public static void init(Activity activity)
        {
            UserDialogs.Init(activity);
        }
    }
}