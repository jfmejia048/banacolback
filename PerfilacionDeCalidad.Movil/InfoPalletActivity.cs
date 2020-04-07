namespace PerfilacionDeCalidad.Movil
{
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Support.V7.App;
    using Newtonsoft.Json;
    using PerfilacionDeCalidad.Movil.Helpers;
    using PerfilacionDeCalidad.PCL.Services;
    using Toolbar = Android.Support.V7.Widget.Toolbar;
    using AlertDialog = Android.App.AlertDialog;
    using PerfilacionDeCalidad.PCL.Models;
    using Android.Widget;
    using System;
    using System.Threading.Tasks;

    [Activity(Label = "InfoPalletActivity", ParentActivity = typeof(HomeActivity))]
    public class InfoPalletActivity : AppCompatActivity
    {
        public ApiService apiService;
        public TextView txtInfoCarga;
        public TextView txtInfoCajasPallet;
        public TextView txtInfoPerfilar;
        public TextView txtInfoDestino;
        public TextView txtInfoExportador;
        public TextView txtInfoBuque;
        public TextView txtInfoHLlegadaCamion;
        public TextView txtInfoHSalidaFinca;
        public TextView txtInfoHEstimada;
        public TextView txtInfoHLlegadaTerminal;
        public TextView txtInfoFruta;
        public TextView txtInfoPoma;
        public TextView txtInfoFinca;
        public TextView txtInfoTerminalDestino;
        public TextView txtInfoCara;
        public Button btnInfoAleatorio;
        public string codigo;
        public int idPallet;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.infoPallet);
            NavigationLoader.init(this);
            apiService = new ApiService();
            this.InicializeInfo();
            this.codigo = Intent.GetStringExtra("Codigo");
            this.GetInfoPallet(this.codigo);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbarInfoPallets);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Información Pallet";
            SupportActionBar.SetDefaultDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        private async void aleatorioCaraPallet(object sender, EventArgs e)
        {
            NavigationLoader.ShowLoading();
            Random random = new Random();
            var cara = random.Next(1, 6);
            var data = new
            {
                ID = this.idPallet,
                CaraPalet = cara
            };
            var response = await this.apiService.Post("Palet/GuardarCara", data, Settings.AccessToken);
            if (response.success)
            {
                if (response.data != null)
                {
                    this.PresentAlert("La cara seleccionada es la número: " + cara, false);
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

        private void InicializeInfo()
        {
            this.txtInfoCarga = FindViewById<TextView>(Resource.Id.txtInfoCarga);
            this.txtInfoCajasPallet = FindViewById<TextView>(Resource.Id.txtInfoCajasPallet);
            this.txtInfoPerfilar = FindViewById<TextView>(Resource.Id.txtInfoPerfilar);
            this.txtInfoDestino = FindViewById<TextView>(Resource.Id.txtInfoDestino);
            this.txtInfoExportador = FindViewById<TextView>(Resource.Id.txtInfoExportador);
            this.txtInfoBuque = FindViewById<TextView>(Resource.Id.txtInfoBuque);
            this.txtInfoHLlegadaCamion = FindViewById<TextView>(Resource.Id.txtInfoHLlegadaCamion);
            this.txtInfoHSalidaFinca = FindViewById<TextView>(Resource.Id.txtInfoHSalidaFinca);
            this.txtInfoHEstimada = FindViewById<TextView>(Resource.Id.txtInfoHEstimada);
            this.txtInfoHLlegadaTerminal = FindViewById<TextView>(Resource.Id.txtInfoHLlegadaTerminal);
            this.txtInfoFruta = FindViewById<TextView>(Resource.Id.txtInfoFruta);
            this.txtInfoPoma = FindViewById<TextView>(Resource.Id.txtInfoPoma);
            this.txtInfoFinca = FindViewById<TextView>(Resource.Id.txtInfoFinca);
            this.txtInfoTerminalDestino = FindViewById<TextView>(Resource.Id.txtInfoTerminalDestino);
            this.txtInfoCara = FindViewById<TextView>(Resource.Id.txtInfoCara);
            this.btnInfoAleatorio = FindViewById<Button>(Resource.Id.btnInfoAleatorio);
            this.btnInfoAleatorio.Click += aleatorioCaraPallet;
        }

        private async void GetInfoPallet(string codigo)
        {
            NavigationLoader.ShowLoading();
            var data = new
            {
                CodigoPalet = codigo
            };
            var response = await this.apiService.Post("Palet/GetByCodigo", data, Settings.AccessToken);
            if (response.success)
            {
                if(response.data != null)
                {
                    var result = JsonConvert.DeserializeObject<InfoPaletResponse>(response.data.ToString());
                    this.idPallet = result.idPallet;
                    this.txtInfoCarga.Text = result.carga;
                    this.txtInfoCajasPallet.Text = result.cajasPalet.ToString();
                    this.txtInfoPerfilar.Text = result.perfilar ? "SI" : "NO";
                    this.txtInfoDestino.Text = result.destino;
                    this.txtInfoExportador.Text = result.exportador;
                    this.txtInfoBuque.Text = result.buque;
                    this.txtInfoHLlegadaCamion.Text = result.llegada.ToString("h:mm tt");
                    this.txtInfoHSalidaFinca.Text = result.salida.ToString("h:mm tt");
                    this.txtInfoHEstimada.Text = result.estimado.ToString("h:mm tt");
                    this.txtInfoHLlegadaTerminal.Text = result.llegadaTerminal.ToString("h:mm tt");
                    this.txtInfoFruta.Text = result.fruta;
                    this.txtInfoPoma.Text = result.poma.ToString();
                    this.txtInfoFinca.Text = result.finca;
                    this.txtInfoCara.Text = result.caraPallet != 0 ? result.caraPallet.ToString() : "N/A";
                    this.txtInfoTerminalDestino.Text = result.terminalDestino;
                    if (result.perfilar)
                    {
                        this.txtInfoPerfilar.SetTextColor(Android.Graphics.Color.Rgb(95, 154, 53));
                    }
                    else
                    {
                        this.txtInfoPerfilar.SetTextColor(Android.Graphics.Color.Orange);
                    }
                    var visibleButton = result.caraPallet != 0 ? Android.Views.ViewStates.Invisible : result.perfilar ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Invisible;
                    this.btnInfoAleatorio.Visibility = visibleButton;
                    NavigationLoader.HideLoading();
                }
                else
                {
                    NavigationLoader.HideLoading();
                    this.PresentAlert("No hay información del pallet escaneado. Código del pallet: " + this.codigo, true);
                }
            }
            else
            {
                NavigationLoader.HideLoading();
                if(response.data == null)
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