namespace PerfilacionDeCalidad.Movil
{
    using System;
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Support.V7.App;
    using Toolbar = Android.Support.V7.Widget.Toolbar;
    using Android.Views;
    using Android.Widget;
    using PerfilacionDeCalidad.Movil.Helpers;
    using Android.Text;
    using AlertDialog = Android.App.AlertDialog;
    using Android.Views.InputMethods;
    using PerfilacionDeCalidad.Movil.Enum;
    using Newtonsoft.Json;
    using PerfilacionDeCalidad.PCL.Models;
    using PerfilacionDeCalidad.PCL.Services;
    using System.Text.RegularExpressions;

    [Activity(Label = "HomeActivity", ParentActivity = typeof(HomeActivity))]
    public class HomeActivity : AppCompatActivity
    {
        EditText txtCodebar;
        private InputMethodManager imm;
        public ApiService apiService;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.home);
            NavigationLoader.init(this);
            this.apiService = new ApiService();
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbarTabs);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Perfilación De Calidad";
            this.txtCodebar = FindViewById<EditText>(Resource.Id.txtcodeBar);
            txtCodebar.AfterTextChanged += changeCodebar;
            imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            txtCodebar.Touch += onTouchEvent;
        }

        private void onTouchEvent(object sender, View.TouchEventArgs e)
        {
            imm.HideSoftInputFromWindow(txtCodebar.WindowToken, 0);
        }

        private void changeCodebar(object sender, AfterTextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtCodebar.Text))
            {
                this.GetInfoPallet(this.txtCodebar.Text);
                //Intent intent = new Intent(Application.Context, typeof(InfoPalletActivity));
                //intent.PutExtra("Codigo", this.txtCodebar.Text);
                //this.txtCodebar.Text = "";
                //StartActivity(intent);
                //if(int.Parse(Settings.TypeUser) == (int)TipoEscaneo.calidad)
                //{
                //    StartActivity(new Intent(Application.Context, typeof(CalidadActivity)));
                //}
                //else
                //{
                //    StartActivity(new Intent(Application.Context, typeof(ChequeoActivity)));
                //}
            }
        }

        private async void GetInfoPallet(string codigo)
        {
            string[] cadena = Regex.Split(codigo, @"\r\n?|\n");
            if(!(cadena.Length > 1))
            {
                NavigationLoader.ShowLoading();
                var data = new
                {
                    CodigoPalet = codigo
                };
                var response = await this.apiService.Post("Palet/GetByCodigo", data, Settings.AccessToken);
                if (response.success)
                {
                    if (response.data != null)
                    {
                        var result = JsonConvert.DeserializeObject<InfoPaletResponse>(response.data.ToString());
                        if (int.Parse(Settings.TypeUser) == (int)TipoEscaneo.chequeo)
                        {
                            if (result.perfilar)
                            {
                                StartActivity(new Intent(Application.Context, typeof(PalletCalidadActivity)));
                            }
                            else
                            {
                                StartActivity(new Intent(Application.Context, typeof(PalletChequeoActivity)));
                            }
                        }
                        else
                        {
                            if (result.perfilar)
                            {
                                Intent intent = new Intent(Application.Context, typeof(SeleccionCaraActivity));
                                intent.PutExtra("IdPallet", result.idPallet.ToString());
                                intent.PutExtra("CaraPallet", result.caraPallet);
                                StartActivity(intent);
                            }
                            else
                            {
                                this.PresentAlert("El pallet escaneado no es para calidad.");
                            }
                        }
                        NavigationLoader.HideLoading();
                    }
                    else
                    {
                        NavigationLoader.HideLoading();
                        this.PresentAlert("No hay información del pallet escaneado. Código del pallet: " + codigo);
                    }
                }
                else
                {
                    NavigationLoader.HideLoading();
                    if (response.data == null)
                    {
                        this.PresentAlert("Se ha presentado un problema interno");
                    }
                    else
                    {
                        this.PresentAlert(response.data.ToString());
                    }
                }
            }
            this.txtCodebar.Text = "";
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbar_items, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if(item.ItemId == Resource.Id.item1)
            {
                Settings.AccessToken = string.Empty;
                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                Finish();
            }
            return base.OnOptionsItemSelected(item);
        }

        public void PresentAlert(string content)
        {
            var dialogVal = new AlertDialog.Builder(this, Resource.Style.AlertDialog);
            AlertDialog alertVal = dialogVal.Create();
            alertVal.SetTitle("Información");
            alertVal.SetMessage(content);
            alertVal.SetButton("Aceptar", (c, ev) =>
            {
                alertVal.Hide();
            });
            alertVal.Show();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            if (int.Parse(Settings.TypeUser) == (int)TipoEscaneo.chequeo || int.Parse(Settings.TypeUser) == (int)TipoEscaneo.calidad)
            {
                this.Finish();
            }
            else
            {
                StartActivity(new Intent(Application.Context, typeof(SelectedRoleActivity)));
            }
        }
    }
}