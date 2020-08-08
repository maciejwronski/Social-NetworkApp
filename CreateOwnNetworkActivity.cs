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

            textMessage = FindViewById<TextView>(Resource.Id.message);

            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
            var menu = navigation.Menu;
            var menuItem = menu.GetItem(2);
            menuItem.SetChecked(true);
            wifiManager = (WifiManager)ApplicationContext.GetSystemService(Context.WifiService);
            buttonCreateHotspot = FindViewById<Button>(Resource.Id.hotspotBtn);
            buttonSendMessage = FindViewById<Button>(Resource.Id.SendMessageButton);
            buttonCreateHotspot.Click += OnCreateNetworkButtonClick;
            buttonSendMessage.Click += OnMessageSendButtonClick;
            LocalHotspot.StateChange += OnHotspotStateChange;
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
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

        private void OnHotspotStateChange(object sender, HotSpotState e)
        {
            if(localHotspot != null)
            {
                switch (localHotspot.GetHotSpotState())
                {
                    case HotSpotState.Disabled:
                        buttonSendMessage.Visibility = ViewStates.Invisible;
                        buttonCreateHotspot.Visibility = ViewStates.Visible;
                        break;
                    case HotSpotState.Enabled:
                        buttonSendMessage.Visibility = ViewStates.Visible;
                        buttonCreateHotspot.Visibility = ViewStates.Invisible;
                        break;
                }
            }
        }

        public void OnMessageSendButtonClick(object sender, System.EventArgs e)
        {
            if(localHotspot != null && localHotspot.GetHotSpotState() == HotSpotState.Enabled)
            {
                MessageSender messageSender = new MessageSender();
                string testMessage = "Test";
                Task<int> task = Task.Run<int>(async () => await messageSender.SendBroadcastMessage(ApplicationContext, testMessage));
                Console.WriteLine(task.Result);
            }
            else
            {
                Console.WriteLine((localHotspot == null) + " " + (localHotspot.GetHotSpotState()));
                Toast.MakeText(ApplicationContext, "Enable Hot-Spot first!", ToastLength.Long);
            }
        }
        
    }
}

