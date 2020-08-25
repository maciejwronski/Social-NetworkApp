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
using Xamarin.Essentials;

namespace Social_Network_App
{
    class WifiReceiver : BroadcastReceiver
    {
        WifiManager _wifiManager;
        Context _context;
        EditText _password;
        Button _connectButton;
        Dictionary<int, string> _wifiDictionary;
        ListView _wifiDeviceListView;
        WifiConnector wifiConnector;
        public event EventHandler<NetworkAccess> StateChange;
        int currentItemID;
        public WifiReceiver(WifiManager wifiManager, Android.Widget.ListView wifiDeviceList, Context context, EditText pass, Button connectButton)
        {
            _wifiManager = wifiManager;
            _wifiDeviceListView = wifiDeviceList;
            _context = context;
            _password = pass;
            _connectButton = connectButton;
            AttachCallbacks();
            ShowButtonConnectState(true, true);
        }
        ~WifiReceiver()
        {
            DettachCallbacks();
        }
        public override void OnReceive(Context context, Intent intent)
        {
            String action = intent.Action;
            if (action == WifiManager.ScanResultsAvailableAction && !Utils.CheckWifiOnAndConnected(_wifiManager))
                FillListViewWithWifi(context);
        }
        private void ConnectToWifi(string networkSSID, string networkPass)
        {
            wifiConnector = new WifiConnector(networkSSID, networkPass, _wifiManager);
            wifiConnector.ConnectToWifi();
        }
        private void OnConnectClick(object sender, EventArgs e)
        {
            if (_connectButton.Text == "Connect")
            {
                HideVirtualKeyboard();
                if (_wifiDictionary.ContainsKey(currentItemID))
                {
                    ConnectToWifi(_wifiDictionary[currentItemID], _password.Text);
                }
            }
            else
            {
                if(wifiConnector != null)
                    wifiConnector.DisconnectFromWifi();
                ClearWifiListView();
                ShowButtonConnectState(false, true);
            }
        }

        public void ShowButtonConnectState(bool connect, bool hideAll = false)
        {
            if (hideAll)
            {
                _connectButton.Visibility = ViewStates.Invisible;
                _password.Visibility = ViewStates.Invisible;
                _connectButton.Text = "Connect";
                return;
            }
            if (!connect)
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
        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            OnStateChanged(e.NetworkAccess);
            switch (e.NetworkAccess)
            {
                case NetworkAccess.Internet:
                    ShowButtonConnectState(false);
                    ClearWifiListView();
                    _password.Text = "";
                    break;
                case NetworkAccess.None:
                    ShowButtonConnectState(true);
                    break;
            }

        }
        private void FillListViewWithWifi(Context context)
        {
            _wifiDictionary = new Dictionary<int, string>();
            StringBuilder sb = new StringBuilder();
            ArrayList deviceList = new ArrayList();
            IList<ScanResult> wifiList = _wifiManager.ScanResults;
            int index = 0;
            foreach (ScanResult scanResult in wifiList)
            {
                _wifiDictionary.Add(index, scanResult.Ssid);
                sb.Append("\n").Append(scanResult.Ssid).Append(" - ").Append(scanResult.Capabilities);
                deviceList.Add(scanResult.Ssid);
                index++;
            }
            ArrayAdapter arrayAdapter = new ArrayAdapter(context, Android.Resource.Layout.SimpleExpandableListItem1, deviceList.ToArray());
            _wifiDeviceListView.Adapter = arrayAdapter;
        }
        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            ShowButtonConnectState(true);
            currentItemID = (int)e.Id;
            _password.RequestFocus();
            _password.SelectAll();

            ShowVirtualKeyboard();
        }
        private void ShowVirtualKeyboard()
        {
            InputMethodManager inputMethodManager = _context.GetSystemService(Context.InputMethodService) as InputMethodManager;
            inputMethodManager.ShowSoftInput(_password, ShowFlags.Forced);
            inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
        }
        private void HideVirtualKeyboard()
        {
            InputMethodManager imm = (InputMethodManager)_context.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(_password.WindowToken, 0);
        }
        private void ClearWifiListView()
        {
            _wifiDeviceListView.Adapter = null;
            _wifiDictionary = new Dictionary<int, string>();
        }
        private void AttachCallbacks()
        {
            _wifiDeviceListView.ItemClick += OnItemClick;
            _connectButton.Click += OnConnectClick;
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }
        private void DettachCallbacks()
        {
            ClearWifiListView(); 
            _wifiDeviceListView.ItemClick -= OnItemClick;
            _connectButton.Click -= OnConnectClick;
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        }
        protected virtual void OnStateChanged(NetworkAccess networkAccess)
        {
            StateChange?.Invoke(this, networkAccess);
        }
    }
}