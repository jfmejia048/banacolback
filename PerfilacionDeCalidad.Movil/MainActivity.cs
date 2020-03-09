namespace PerfilacionDeCalidad.Movil
{
    using Android.App;
    using Android.OS;
    using Android.Runtime;
    using Android.Widget;
    using PerfilacionDeCalidad.PCL.Services;
    using System;
    using AlertDialog = Android.App.AlertDialog;
    using Newtonsoft.Json;
    using PerfilacionDeCalidad.PCL.Models;
    using PerfilacionDeCalidad.Movil.Helpers;
    using Android.Content;


    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false, NoHistory = true)]
    public class MainActivity : Activity
    {
        public ApiService apiService;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            NavigationLoader.init(this);
            apiService = new ApiService();

            var buttoLogin = FindViewById<Button>(Resource.Id.btnLogin);
            buttoLogin.Click += Login;
        }

        private async void Login(object sender, EventArgs e)
        {
            NavigationLoader.ShowLoading();
            var user = FindViewById<EditText>(Resource.Id.txtUserLogin);
            var pass = FindViewById<EditText>(Resource.Id.txtPassLogin);
            if (string.IsNullOrEmpty(user.Text))
            {
                NavigationLoader.HideLoading();
                this.PresentAlert("Debe ingresar el nombre de usuario");
                return;
            }

            if (string.IsNullOrEmpty(pass.Text))
            {
                NavigationLoader.HideLoading();
                this.PresentAlert("Debe ingresar  la contraseña");
                return;
            }

            var data = new
            {
                Username = user.Text,
                Password = pass.Text
            };

            var response = await this.apiService.Post("Account/Login", data);
            if (response.success)
            {
                var result = JsonConvert.DeserializeObject<LoginResponse>(response.data.ToString());
                Settings.AccessToken = result.Token;
                StartActivity(new Intent(Application.Context, typeof(HomeActivity)));
                NavigationLoader.HideLoading();
            }
            else{
                NavigationLoader.HideLoading();
                this.PresentAlert("Usuario y/o contraseña incorrecta");
            }
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

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}