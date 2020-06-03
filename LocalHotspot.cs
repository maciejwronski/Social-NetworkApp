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
using Xamarin.Essentials;
using Plugin.Permissions;
using Plugin.CurrentActivity;

namespace Social_Network_App
{
    class LocalHotspot
    {
        WifiManager wifiManager;
        Context context;
        WifiConfiguration wifiConfiguration;
        HotSpotCallback callback;
        WifiManager.LocalOnlyHotspotReservation mReservation { get; set; }
        public LocalHotspot(Context context)
        {
            this.context = context;
        }
        public void SetConfig(string ssid = "DefaultWifi")
        {
            wifiManager = (WifiManager)context.GetSystemService(Context.WifiService);
            wifiConfiguration = new WifiConfiguration();
            wifiConfiguration.Ssid = ssid;
        }
        public bool SetNetworkState(bool enabled)
        {
            try
            {
                if (enabled)
                {
                    callback = new HotSpotCallback();
                    wifiManager.StartLocalOnlyHotspot(callback, new Handler());
                    callback.OnStarted(mReservation);
                }
                else
                {
                    if (mReservation != null)
                    {
                        mReservation.Close();
                    }
                }
            }
            catch(Exception s)
            {
                return false;
            }
            return true;
        }
    }
}