using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Android.Content;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Android.Net.Wifi;
using Xamarin.Essentials;
using Plugin.Permissions;
using Plugin.CurrentActivity;

namespace Social_Network_App
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class CreateOwnNetworkActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        TextView textMessage;
        WifiManager wifiManager;
        private Button buttonHotspot;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Activity = this;
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_OwnNetworkActivity);

            textMessage = FindViewById<TextView>(Resource.Id.message);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
            wifiManager = (WifiManager)ApplicationContext.GetSystemService(Context.WifiService);
            buttonHotspot = FindViewById<Button>(Resource.Id.hotspotBtn);
            if (!Utils.HasPermission(ApplicationContext, Utils.ePermission.WifiPermission))
            {
                Toast.MakeText(ApplicationContext, "Application requires permission to Wi-Fi.", ToastLength.Long).Show();
                RequestPermissions(Utils.RequiredWifiPermissions, 0);
            }
            else
            {
                if (!wifiManager.IsWifiEnabled)
                {
                    Toast.MakeText(ApplicationContext, "Turning WiFi ON...", ToastLength.Long).Show();
                    wifiManager.SetWifiEnabled(true);
                }
                buttonHotspot.Click += delegate
                {
                    if (!Utils.HasPermission(ApplicationContext, Utils.ePermission.LocationPermission))
                    {
                        var GetPermissions = Utils.GetPermissions();
                    }
                    else
                    {
                        LocalHotspot localHotspot = new LocalHotspot(ApplicationContext);
                        localHotspot.SetConfig("Nowe wifi");
                        localHotspot.SetNetworkState(true);
                    }
                };
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    Finish();
                    StartActivity(typeof(MainActivity));
                    return true;
                case Resource.Id.navigation_dashboard:
                    Finish();
                    StartActivity(typeof(AvailableNetworksActivity));
                    return true;
                case Resource.Id.navigation_notifications:
                    return true;
            }
            return false;
        }
    }
}

