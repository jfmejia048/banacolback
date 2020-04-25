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
    [Activity(Label = "ChequeoActivity")]
    public class PalletChequeoActivity : AppCompatActivity
    {
        public TextView txtEscaneoChequeoValue;
        public Button btnAceptar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.pallet_chequeo);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbarChequeo);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Pallet";
            this.InicializeInfo();
            // Create your application here
        }

        private void InicializeInfo()
        {
            this.btnAceptar = FindViewById<Button>(Resource.Id.btnAceptarChequeo);
            this.btnAceptar.Click += Volver;
            this.txtEscaneoChequeoValue = FindViewById<TextView>(Resource.Id.txtEscaneoChequeoValue);
            var cantidad = int.Parse(Settings.CantidadEscaneo);
            cantidad++;
            this.txtEscaneoChequeoValue.Text = cantidad.ToString();
            if(cantidad >= 20)
            {
                Settings.CantidadEscaneo = "0";
            }
            else
            {
                Settings.CantidadEscaneo = cantidad.ToString();
            }
        }

        private void Volver(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(HomeActivity)));
            Finish();
        }
    }
}