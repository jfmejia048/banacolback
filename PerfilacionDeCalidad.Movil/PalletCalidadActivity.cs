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
using Android.Media;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.Movil
{
    [Activity(Label = "CalidadActivity")]
    public class PalletCalidadActivity : AppCompatActivity
    {
        public TextView txtEscaneoCalidadValue;
        public Button btnAceptar;
        MediaPlayer player;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.pallet_calidad);
            player = MediaPlayer.Create(this, Resource.Raw.Alerta1);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbarCalidad);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Pallet";
            this.InicializeInfo();
            // Create your application here
        }

        private void InicializeInfo()
        {
            this.btnAceptar = FindViewById<Button>(Resource.Id.btnAceptarCalidad);
            this.btnAceptar.Click += Volver;
            this.txtEscaneoCalidadValue = FindViewById<TextView>(Resource.Id.txtEscaneoCalidadValue);
            var cantidad = int.Parse(Settings.CantidadEscaneo);
            cantidad++;
            this.txtEscaneoCalidadValue.Text = cantidad.ToString();
            Settings.CantidadEscaneo = cantidad.ToString();
            if (cantidad >= 20)
            {
                Settings.CantidadEscaneo = "0";
            }
            else
            {
                Settings.CantidadEscaneo = cantidad.ToString();
            }
            player.Start();
        }

        private void Volver(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(HomeActivity)));
            Finish();
        }
    }
}