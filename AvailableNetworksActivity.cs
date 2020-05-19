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

    public class AvailableNetworksActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        private enum Permission
        {
            LocationPermission,
            WifiPermission
        }
        private TextView textMessage;
        private ListView wifiList;
        private WifiManager wifiManager;
        private WifiReceiver wifiReceiver;
        private Button buttonScan;
        private BottomNavigationView navigation;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Activity = this;
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_availableNetworks);

            GetIDs();

            navigation.SetOnNavigationItemSelectedListener(this);

            wifiManager = (WifiManager)ApplicationContext.GetSystemService(Context.WifiService);
            wifiReceiver = new WifiReceiver(wifiManager, wifiList);

            if (!HasPermission(Permission.WifiPermission))
            {
                Toast.MakeText(ApplicationContext, "Application requires permission to Wi-Fi.", ToastLength.Long).Show();
                RequestPermissions(Utils.Permissions, 0);
            }
            else
            {
                if (!wifiManager.IsWifiEnabled)
                {
                    Toast.MakeText(ApplicationContext, "Turning WiFi ON...", ToastLength.Long).Show();
                    wifiManager.SetWifiEnabled(true);
                }
                buttonScan.Click += delegate
                {
                    if (!HasPermission(Permission.LocationPermission))
                    {
                        var GetPermissions = Utils.GetPermissions();
                    }
                    else
                    {
                        Toast.MakeText(ApplicationContext, "Searching for Wi-fi...", ToastLength.Long).Show();
                        RegisterReceiver(wifiReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction));
                        wifiManager.StartScan();
                    }
                };
            }
        }

        private void GetIDs()
        {
            textMessage = FindViewById<TextView>(Resource.Id.message);
            wifiList = FindViewById<ListView>(Resource.Id.wifiList);
            buttonScan = FindViewById<Button>(Resource.Id.scanBtn);
            navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
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
                    return true;
                case Resource.Id.navigation_notifications:
                    Finish();
                    StartActivity(typeof(CreateOwnNetworkActivity));
                    return true;
            }
            return false;
        }
        bool HasPermission(Permission permission)
        {
            switch (permission)
            {
                case Permission.WifiPermission:
                    return (ApplicationContext.CheckSelfPermission(Android.Manifest.Permission.ChangeWifiMulticastState) == Android.Content.PM.Permission.Granted) &&
                           (ApplicationContext.CheckSelfPermission(Android.Manifest.Permission.AccessWifiState) == Android.Content.PM.Permission.Granted) &&
                           (ApplicationContext.CheckSelfPermission(Android.Manifest.Permission.ChangeWifiState) == Android.Content.PM.Permission.Granted);
                case Permission.LocationPermission:
                    return ApplicationContext.CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) == Android.Content.PM.Permission.Granted;
            }
            return false;
        }
    }
}

