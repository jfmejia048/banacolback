using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using PerfilacionDeCalidad.Movil.Helpers;
using Android.Support.V7.App;

namespace PerfilacionDeCalidad.Movil
{
    [Activity(Label = "CalidadActivity")]
    public class PalletCalidadActivity : AppCompatActivity
    {
        public TextView txtEscaneoCalidadValue;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.pallet_calidad);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbarCalidad);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Pallet";
            this.InicializeInfo();
            // Create your application here
        }

        private void InicializeInfo()
        {
            this.txtEscaneoCalidadValue = FindViewById<TextView>(Resource.Id.txtEscaneoCalidadValue);
            var cantidad = int.Parse(Settings.CantidadEscaneo);
            cantidad++;
            this.txtEscaneoCalidadValue.Text = cantidad.ToString();
            Settings.CantidadEscaneo = cantidad.ToString();
        }
    }
}