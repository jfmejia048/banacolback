using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Views;
using Android.Widget;
using PerfilacionDeCalidad.Movil.Helpers;
using Android.Support.V4.View;
using Android.Support.V4.App;
using Java.Lang;

namespace PerfilacionDeCalidad.Movil
{
    [Activity(Label = "HomeActivity", ParentActivity = typeof(HomeActivity))]
    public class HomeActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.home);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbarTabs);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Perfilación De Calidad";
            //SupportActionBar.SetDefaultDisplayHomeAsUpEnabled(true);
            //SupportActionBar.SetDisplayHomeAsUpEnabled(true);
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
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}