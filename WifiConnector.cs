using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Social_Network_App
{
    class WifiConnector
    {
        string _wifiPassword;
        string _wifiSsid;
        WifiManager _wifiManager;

        public WifiConnector(string wifiSsid, string wifiPassword, WifiManager wifiManager)
        {
            _wifiSsid = wifiSsid;
            _wifiPassword = wifiPassword;
            _wifiManager = wifiManager;
        }
        public void ConnectToWifi()
        {
            var config = new WifiConfiguration();
            config.Ssid = '"' + _wifiSsid + '"';
            config.PreSharedKey = '"' + _wifiPassword + '"';
            int id = _wifiManager.AddNetwork(config);
            _wifiManager.Disconnect();
            _wifiManager.EnableNetwork(id, true);
            _wifiManager.Reconnect();
        }
        public void DisconnectFromWifi()
        {
            _wifiManager.DisableNetwork(_wifiManager.ConnectionInfo.NetworkId);
            _wifiManager.Disconnect();
        }
    }
}