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

    [Activity(Label = "HomeActivity", ParentActivity = typeof(HomeActivity))]
    public class HomeActivity : AppCompatActivity
    {
        EditText txtCodebar;
        private InputMethodManager imm;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.home);

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
                Intent intent = new Intent(Application.Context, typeof(InfoPalletActivity));
                intent.PutExtra("Codigo", this.txtCodebar.Text);
                this.txtCodebar.Text = "";
                StartActivity(intent);
            }
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
    }
}