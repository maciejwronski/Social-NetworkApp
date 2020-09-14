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
using Java.Lang;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Social_Network_App
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]

    public class AvailableNetworksActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        private TextView textMessage;
        private EditText passwordText;
        private Button connectButton;
        private ListView wifiList;
        private WifiManager wifiManager;
        private WifiReceiver wifiReceiver;
        private Button buttonWifiScanner;
        private Button buttonReceiveMessage;
        private BottomNavigationView navigation;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Activity = this;
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_availableNetworks);

            AttachCallbacksAndGetIDs();
            ClearConnections();

            navigation.SetOnNavigationItemSelectedListener(this);
            var menu = navigation.Menu;
            var menuItem = menu.GetItem(1);

            menuItem.SetChecked(true);
        }
        protected override void OnDestroy()
        {
            DettachCallbacks();
            base.OnDestroy();
        }
        void DettachCallbacks()
        {
            if(buttonWifiScanner != null)
                buttonWifiScanner.Click -= OnAvailableWifiButtonClick;
            if(buttonReceiveMessage != null)
                buttonReceiveMessage.Click -= OnStartListeningButtonClick;
            if (wifiReceiver != null)
                wifiReceiver.StateChange -= OnConnectionStateChanged;
            try
            {
                UnregisterReceiver(wifiReceiver);
                wifiReceiver = null;
            }
            catch (IllegalArgumentException ex)
            {
                Console.WriteLine("Most likely receiver was null" + ex.Message);
            }
        }
        void AttachCallbacksAndGetIDs()
        {
            passwordText = FindViewById<EditText>(Resource.Id.passwordText);
            connectButton = FindViewById<Button>(Resource.Id.wifiConnect);
            textMessage = FindViewById<TextView>(Resource.Id.message);
            wifiList = FindViewById<ListView>(Resource.Id.wifiList);
            buttonWifiScanner = FindViewById<Button>(Resource.Id.scanBtn);
            buttonReceiveMessage = FindViewById<Button>(Resource.Id.ReceiveMessageButton);
            navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            wifiManager = (WifiManager)ApplicationContext.GetSystemService(Context.WifiService);
            wifiReceiver = new WifiReceiver(wifiManager, wifiList, ApplicationContext, passwordText, connectButton);
            wifiReceiver.StateChange += OnConnectionStateChanged;
            buttonWifiScanner.Click += OnAvailableWifiButtonClick;
            buttonReceiveMessage.Click += OnStartListeningButtonClick;
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
        public void OnAvailableWifiButtonClick(object sender, System.EventArgs e)
        {
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
                if (!Utils.HasPermission(ApplicationContext, Utils.ePermission.LocationPermission))
                {
                    var GetPermissions = Utils.GetPermissions();
                }
                else
                {
                    if(wifiReceiver == null)
                        wifiReceiver = new WifiReceiver(wifiManager, wifiList, ApplicationContext, passwordText, connectButton);
                    textMessage.Visibility = ViewStates.Invisible;
                    Toast.MakeText(ApplicationContext, "Searching for Wi-fi...", ToastLength.Long).Show();
                    RegisterReceiver(wifiReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction));

                    wifiManager.StartScan();
                }
            }
        }
        public void OnStartListeningButtonClick(object sender, System.EventArgs e)
        {
            if (Utils.CheckWifiOnAndConnected(wifiManager))
            {
                Console.WriteLine("Sending a message...");
                var messageSender = new MessageSender();

                UserMessageContainer.userMessages.Add(new UserMessage("", Crypto.VigenereCrypt.Code(Utils.MessageToSend)));

                messageSender.BroadcastMessage(ApplicationContext, SerializeMessageArray(UserMessageContainer.userMessages));
                if (UserMessageContainer.CleanMessagesOnSend)
                    UserMessageContainer.userMessages.Clear();
            }
            else
            {
                Toast.MakeText(ApplicationContext, "Connect to any wi-fi first!", ToastLength.Long);
            }
        }
        private void OnConnectionStateChanged(object sender, NetworkAccess e)
        {
            Console.WriteLine("network state" + e);
            switch (e)
            {
                case NetworkAccess.Internet:
                    buttonReceiveMessage.Visibility = ViewStates.Visible;
                    buttonWifiScanner.Visibility = ViewStates.Invisible;
                    connectButton.Visibility = ViewStates.Invisible;
                    try
                    {
                        UnregisterReceiver(wifiReceiver);
                        wifiReceiver = null;
                    }
                    catch(IllegalArgumentException ex)
                    {
                        Console.WriteLine("Most like receiver was null" + ex.Message);
                    }
                    break;
                default:
                    buttonReceiveMessage.Visibility = ViewStates.Invisible;
                    buttonWifiScanner.Visibility = ViewStates.Visible;
                    connectButton.Visibility = ViewStates.Invisible;
                    break;
            }
        }
        byte[] SerializeMessageArray(List<UserMessage> list)
        {
            var binFormatter = new BinaryFormatter();
            var mStream = new MemoryStream();
            binFormatter.Serialize(mStream, list);
            return mStream.ToArray();
        }
        void ClearConnections()
        {
            // lets assume that you are not connected to any wi-fi at start
            wifiManager.SetWifiEnabled(false);
            buttonReceiveMessage.Visibility = ViewStates.Invisible;
        }
    }
}

