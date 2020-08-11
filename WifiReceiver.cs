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
using Android.Net.Wifi;
using System.Collections;
using System.Threading.Tasks;
using Android.Views.InputMethods;
using Android.Net;

namespace Social_Network_App
{
    class WifiReceiver : BroadcastReceiver
    {
        WifiManager _wifiManager;
        Context _context;
        EditText _password;
        Button _connectButton;
        Dictionary<int, string> wifiDictionary;
        ListView wifiDeviceListView;

        int currentItemID;
        public WifiReceiver(WifiManager wifiManager, Android.Widget.ListView wifiDeviceList, Context context, EditText pass, Button connectButton)
        {
            _wifiManager = wifiManager;
            wifiDeviceListView = wifiDeviceList;
            _context = context;
            _password = pass;
            _connectButton = connectButton;
        }
        public override void OnReceive(Context context, Intent intent)
        {
            String action = intent.Action;
            if (action == WifiManager.ScanResultsAvailableAction)
                FillListViewWithWifi(context);
            if (action == WifiManager.SupplicantConnectionChangeAction)
            {
                ConnectivityManager conMan = (ConnectivityManager)_context.GetSystemService(Context.ConnectivityService);
                NetworkInfo netInfo = conMan.ActiveNetworkInfo;
                if (netInfo != null)
                    Console.WriteLine((netInfo.IsAvailable) + " " + (netInfo.IsConnected));
                if ((intent.GetBooleanExtra(WifiManager.ExtraSupplicantConnected, false)))
                {
                    Console.WriteLine("Connected to wifi!");
                    ShowButtonConnectState(false);
                }
                else
                {
                    Console.WriteLine("Wifi connection lost");
                    ShowButtonConnectState(false, true);
                }
            }
        }
        private void ConnectToWifi(string networkSSID, string networkPass)
        {
            Console.WriteLine("trying to connect to wifi " + networkSSID + " " + networkPass);
            var config = new WifiConfiguration();
            config.Ssid = '"' + networkSSID + '"';
            config.PreSharedKey = '"' + networkPass + '"';
            int id = _wifiManager.AddNetwork(config);
            _wifiManager.Disconnect();
            _wifiManager.EnableNetwork(id, true);
            _wifiManager.Reconnect();
        }
        private void OnConnectClick(object sender, EventArgs e)
        {
            if (_connectButton.Text == "Connect")
            {
                InputMethodManager imm = (InputMethodManager)_context.GetSystemService(Context.InputMethodService);
                imm.HideSoftInputFromWindow(_password.WindowToken, 0);

                if (wifiDictionary.ContainsKey(currentItemID))
                {
                    ConnectToWifi(wifiDictionary[currentItemID], _password.Text);
                }
            }
            else
            {
                Console.WriteLine("Disabling network");
                _wifiManager.DisableNetwork(_wifiManager.ConnectionInfo.NetworkId);
                _wifiManager.Disconnect();
                ClearWifiListView();
                ShowButtonConnectState(false, true);

            }
        }

        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            currentItemID = (int)e.Id;
            _password.RequestFocus();
            _password.SelectAll();

            InputMethodManager inputMethodManager = _context.GetSystemService(Context.InputMethodService) as InputMethodManager;
            inputMethodManager.ShowSoftInput(_password, ShowFlags.Forced);
            inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
        }

        private void ShowButtonConnectState(bool state, bool hideAll = false)
        {
            if (hideAll)
            {
                _connectButton.Visibility = ViewStates.Invisible;
                _password.Visibility = ViewStates.Invisible;
                _connectButton.Text = "Connect";
                return;
            }
            if (!state)
            {
                _connectButton.Visibility = ViewStates.Visible;
                _password.Visibility = ViewStates.Invisible;
                _connectButton.Text = "Disconnect";
            }
            else
            {
                _connectButton.Visibility = ViewStates.Visible;
                _password.Visibility = ViewStates.Visible;
                _connectButton.Text = "Connect";
            }
        }
        private void FillListViewWithWifi(Context context)
        {
            wifiDictionary = new Dictionary<int, string>();
            StringBuilder sb = new StringBuilder();
            ArrayList deviceList = new ArrayList();
            IList<ScanResult> wifiList = _wifiManager.ScanResults;
            int index = 0;
            foreach (ScanResult scanResult in wifiList)
            {
                wifiDictionary.Add(index, scanResult.Ssid);
                sb.Append("\n").Append(scanResult.Ssid).Append(" - ").Append(scanResult.Capabilities);
                deviceList.Add(scanResult.Ssid);
            }
            ArrayAdapter arrayAdapter = new ArrayAdapter(context, Android.Resource.Layout.SimpleExpandableListItem1, deviceList.ToArray());
            wifiDeviceListView.Adapter = arrayAdapter;
            AttachCallbacks();

            if (Utils.CheckWifiOnAndConnected(_wifiManager))
            {
                ShowButtonConnectState(false);
            }
            else
                ShowButtonConnectState(true);
        }
        private void ClearWifiListView()
        {
            wifiDeviceListView.Adapter = null;
            wifiDictionary = new Dictionary<int, string>();
        }
        private void AttachCallbacks()
        {
            wifiDeviceListView.ItemClick += OnItemClick;
            _connectButton.Click += OnConnectClick;
        }
        private void DettachCallbacks()
        {
            ClearWifiListView();
            wifiDeviceListView.ItemClick -= OnItemClick;
            _connectButton.Click -= OnConnectClick;
        }
    }
}