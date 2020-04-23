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
using Newtonsoft.Json;
using PerfilacionDeCalidad.PCL.Models;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using PerfilacionDeCalidad.Movil.Helpers;
using PerfilacionDeCalidad.Movil.Enum;
using Android.Support.V7.App;

namespace PerfilacionDeCalidad.Movil
{
    [Activity(Label = "SelectedRoleActivity")]
    public class SelectedRoleActivity : AppCompatActivity
    {
        public Button btnCalidad;
        public Button btnChequeo;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.selected_roles);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbarSelectedRoles);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Seleccionar un rol";
            this.InicializeInfo();
        }

        private void InicializeInfo()
        {
            this.btnCalidad = FindViewById<Button>(Resource.Id.btnCalidad);
            this.btnChequeo = FindViewById<Button>(Resource.Id.btnChequeo);
            this.btnCalidad.Click += GoToCalidad;
            this.btnChequeo.Click += GoToChequeo;
        }

        private void GoToChequeo(object sender, EventArgs e)
        {
            Settings.TypeUser = ((int)TipoEscaneo.chequeo).ToString();
            StartActivity(new Intent(Application.Context, typeof(HomeActivity)));
        }

        private void GoToCalidad(object sender, EventArgs e)
        {
            Settings.TypeUser = ((int)TipoEscaneo.calidad).ToString();
            StartActivity(new Intent(Application.Context, typeof(HomeActivity)));
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbar_items, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.item1)
            {
                Settings.AccessToken = string.Empty;
                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                Finish();
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}