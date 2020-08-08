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

namespace Social_Network_App
{
    public class WifiConfig
    {
        string _ssid;
        string _password;
        bool _isOpenNetwork;
        public WifiConfig(bool isOpenNetwork, string ssid, string password)
        {
            Ssid = ssid;
            Password = password;
            IsOpenNetwork = isOpenNetwork;
        }

        public bool IsOpenNetwork { get => _isOpenNetwork; set => _isOpenNetwork = value; }
        public string Password { get => _password; set => _password = value; }
        public string Ssid { get => _ssid; set => _ssid = value; }
    }
}