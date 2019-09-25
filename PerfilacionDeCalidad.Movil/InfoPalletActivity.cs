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

namespace PerfilacionDeCalidad.Movil
{
    [Activity(Label = "InfoPalletActivity", ParentActivity = typeof(HomeActivity))]
    public class InfoPalletActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.home);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbarTabs);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Información Pallet";
            SupportActionBar.SetDefaultDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }
    }
}