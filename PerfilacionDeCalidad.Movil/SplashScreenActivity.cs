namespace PerfilacionDeCalidad.Movil
{
    using System.Threading.Tasks;
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Newtonsoft.Json;
    using PerfilacionDeCalidad.Movil.Enum;
    using PerfilacionDeCalidad.Movil.Helpers;
    using PerfilacionDeCalidad.PCL.Models;

    [Activity(Theme = "@style/MyTheme.Splash", Label = "@string/app_name", MainLauncher = true, NoHistory = true)]
    public class SplashScreenActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => { SimulateStartup(); });
            startupWork.Start();
        }

        // Simulates background work that happens behind the splash screen
        async void SimulateStartup()
        {
            await Task.Delay(2000); // Simulate a bit of startup work.
            var user = JsonConvert.DeserializeObject<LoginResponse>(Settings.User);
            if (string.IsNullOrEmpty(Settings.AccessToken))
            {
                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            }
            else
            {
                if (user.TipoUsuario.ToLower() == "calidad" || user.TipoUsuario.ToLower() == "chequeo")
                {
                    if (user.TipoUsuario.ToLower() == "calidad")
                        Settings.TypeUser = ((int)TipoEscaneo.calidad).ToString();
                    else
                        Settings.TypeUser = ((int)TipoEscaneo.chequeo).ToString();
                    StartActivity(new Intent(Application.Context, typeof(HomeActivity)));
                }
                else
                {
                    StartActivity(new Intent(Application.Context, typeof(SelectedRoleActivity)));
                }
            }
        }
    }
}