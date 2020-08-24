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
using System.Threading.Tasks;

namespace Social_Network_App
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class CreateOwnNetworkActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        TextView textMessage;
        WifiManager wifiManager;
        LocalHotspot localHotspot;
        private Button buttonCreateHotspot;
        private Button buttonSendMessage;
        protected override void OnCreate(Bundle savedInstanceState)
        {

            CrossCurrentActivity.Current.Activity = this;
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_OwnNetworkActivity);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
            var menu = navigation.Menu;
            var menuItem = menu.GetItem(2);
            menuItem.SetChecked(true);
            AttachCallbacksAndIDs();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            DettachCallbacks();
        }
        private void AttachCallbacksAndIDs()
        {
            textMessage = FindViewById<TextView>(Resource.Id.message);
            wifiManager = (WifiManager)ApplicationContext.GetSystemService(Context.WifiService);
            buttonCreateHotspot = FindViewById<Button>(Resource.Id.hotspotBtn);
            buttonSendMessage = FindViewById<Button>(Resource.Id.SendMessageButton);
            buttonCreateHotspot.Click += OnCreateNetworkButtonClick;
            buttonSendMessage.Click += OnMessageSendButtonClick;
            LocalHotspot.StateChange += OnHotspotStateChange;
        }
        private void DettachCallbacks()
        {
            buttonCreateHotspot.Click -= OnCreateNetworkButtonClick;
            buttonSendMessage.Click -= OnMessageSendButtonClick;
            LocalHotspot.StateChange -= OnHotspotStateChange;
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
        public void OnCreateNetworkButtonClick(object sender, System.EventArgs e)
        {
            if (localHotspot == null)
            {
                if (!Utils.HasPermission(ApplicationContext, Utils.ePermission.HotSpotPermission))
                {
                    Toast.MakeText(ApplicationContext, "Application requires some HotSpotPermissions!.", ToastLength.Long).Show();
                    RequestPermissions(Utils.RequiredHotSpotPermissions, 0);
                    return;
                }
                if (!Utils.HasPermission(ApplicationContext, Utils.ePermission.LocationPermission))
                {
                    var GetPermissions = Utils.GetPermissions();
                }
                else
                {
                    if (wifiManager.IsWifiEnabled)
                        wifiManager.SetWifiEnabled(false);
                    localHotspot = new LocalHotspot(ApplicationContext);
                    localHotspot.SetNetworkState(true);
                }
            }
            else
            {
                Console.WriteLine("Disabling hotspot");
                localHotspot.SetNetworkState(false);
                localHotspot = null;
            }
        }

        private void OnHotspotStateChange(object sender, HotSpotState e)
        {
            if(localHotspot != null)
            {
                switch (localHotspot.GetHotSpotState())
                {
                    case HotSpotState.Disabled:
                        buttonSendMessage.Visibility = ViewStates.Invisible;
                        buttonCreateHotspot.Text = Utils.TurnHotspotOnMessage;
                        ShowWifiParameters(false);
                        break;
                    case HotSpotState.Enabled:
                        buttonSendMessage.Visibility = ViewStates.Visible;
                        buttonCreateHotspot.Text = Utils.TurnHotspotOffMessage;
                        ShowWifiParameters(true);
                        break;
                }
            }
        }

        private void OnMessageSendButtonClick(object sender, System.EventArgs e)
        {
            if(localHotspot != null && localHotspot.GetHotSpotState() == HotSpotState.Enabled)
            {
                MessageSender messageSender = new MessageSender();
                string testMessage = "Test";
                Task<int> task = Task.Run<int>(async () => await messageSender.SendBroadcastMessage(ApplicationContext, testMessage));
                task.ContinueWith(t =>
                {
                    Console.WriteLine("[SendResult]" + t.Result);
                });
            }
            else
            {
                Console.WriteLine("[OwnNetworkActivity]" + (localHotspot == null) + " " + (localHotspot.GetHotSpotState()));
                Toast.MakeText(ApplicationContext, "Enable Hot-Spot first!", ToastLength.Long);
            }
        }
        private void ShowWifiParameters(bool state)
        {
            if (!state)
            {
                textMessage.Text = "Press the button to create own network activity";
            }
            else
            {
                textMessage.Text = ("SSID: " + localHotspot.GetWifiConfig().Item1 + "\n" + "Password: " + localHotspot.GetWifiConfig().Item2);
            }
        }
        
    }
}

