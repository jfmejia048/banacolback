namespace PerfilacionDeCalidad.Movil
{
    using System.Threading.Tasks;
    using Android.App;
    using Android.Content;
    using Android.OS;
    using PerfilacionDeCalidad.Movil.Helpers;

    [Activity(Theme = "@style/MyTheme.Splash", Label = "@string/app_name", MainLauncher = true)]
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
            if (string.IsNullOrEmpty(Settings.AccessToken))
            {
                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            }
            else
            {
                StartActivity(new Intent(Application.Context, typeof(HomeActivity)));
            }
        }
    }
}