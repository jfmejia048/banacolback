using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using AlertDialog = Android.App.AlertDialog;
using PerfilacionDeCalidad.PCL.Services;
using PerfilacionDeCalidad.Movil.Helpers;
using PerfilacionDeCalidad.PCL.Models;
using Newtonsoft.Json;
using PerfilacionDeCalidad.Movil.Enum;

namespace PerfilacionDeCalidad.Movil
{
    [Activity(Label = "SeleccionCaraActivity")]
    public class SeleccionCaraActivity : AppCompatActivity
    {
        public ApiService apiService;
        public Button btnSeleccionCara;
        public string idPallet;
        public int CaraPallet;
        public LoginResponse user;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.seleccion_cara);
            this.apiService = new ApiService();
            NavigationLoader.init(this);
            this.idPallet = Intent.GetStringExtra("IdPallet");
            this.CaraPallet = Intent.GetIntExtra("CaraPallet", 0);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbarSeleccionCara);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Inspeción Pallet";
            this.user = JsonConvert.DeserializeObject<LoginResponse>(Settings.User);
            this.btnSeleccionCara = FindViewById<Button>(Resource.Id.btnSeleccionCara);
            this.btnSeleccionCara.Click += SelecionarCara;
            if(this.CaraPallet > 0)
            {
                this.PresentAlert("Ya se seleccionó una cara para la inspección de este pallet, la cara seleccionada es: " + EnumHelper.GetEnumDescription((CarasPallet)this.CaraPallet), true);
            }
            // Create your application here
        }

        private async void SelecionarCara(object sender, EventArgs e)
        {
            NavigationLoader.ShowLoading();
            Random random = new Random();
            var cara = random.Next(1, 6);
            var data = new
            {
                Id = this.idPallet,
                Cara = cara,
                Usuario = this.user.NombreCompleto
            };
            var response = await this.apiService.Post("Palet/GuardarCara", data, Settings.AccessToken);
            if (response.success)
            {
                if (response.data != null)
                {
                    this.PresentAlert("La cara seleccionada es la número: " + EnumHelper.GetEnumDescription((CarasPallet)cara), true);
                    NavigationLoader.HideLoading();
                }
                else
                {
                    NavigationLoader.HideLoading();
                    this.PresentAlert("Se presentó un error, por favor intente mas tarde", true);
                }
            }
            else
            {
                NavigationLoader.HideLoading();
                if (response.data == null)
                {
                    this.PresentAlert("Se ha presentado un problema interno", true);
                }
                else
                {
                    this.PresentAlert(response.data.ToString(), true);
                }
            }
        }

        public void PresentAlert(string content, bool backActivity)
        {
            var dialogVal = new AlertDialog.Builder(this, Resource.Style.AlertDialog);
            AlertDialog alertVal = dialogVal.Create();
            alertVal.SetCanceledOnTouchOutside(false);
            alertVal.SetTitle("Información");
            alertVal.SetMessage(content);
            alertVal.SetButton("Aceptar", (c, ev) =>
            {
                alertVal.Hide();
                if (backActivity)
                {
                    StartActivity(new Intent(Application.Context, typeof(HomeActivity)));
                    Finish();
                }
            });
            alertVal.Show();
        }
    }
}