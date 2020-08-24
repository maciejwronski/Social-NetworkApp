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
using Java.Net;

namespace Social_Network_App
{
    public enum HotSpotState
    {
        Enabled,
        Disabled        
    }
    public class LocalHotspot
    {
        WifiManager wifiManager;
        Context context;
        HotSpotCallback callback;
        private HotSpotState hotSpotState = HotSpotState.Disabled;
        private string _wifiPassword;
        private string _wifiSSID;
        public static event EventHandler<HotSpotState> StateChange;
        public HotSpotState HotSpotState {
            get {
                return hotSpotState;
            }
            set {
                hotSpotState = value;
                OnStateChanged(hotSpotState);
            }
        }

        public WifiManager.LocalOnlyHotspotReservation mReservation { get; set; }

        public LocalHotspot(Context context)
        {
            this.context = context;
        }
        public bool SetNetworkState(bool enabled)
        {
            wifiManager = (WifiManager)context.GetSystemService(Context.WifiService);
            try
            {
                if (enabled)
                {
                    callback = new HotSpotCallback(this);
                    wifiManager.StartLocalOnlyHotspot(callback, new Handler());
                }
                else
                {
                    if (mReservation != null)
                    {
                        HotSpotState = HotSpotState.Disabled;
                        mReservation.Close();
                    }
                }
            }
            catch(System.Exception s)
            {
                Console.WriteLine("Exception occured: " + s.Message);
                return false;
            }
            return true;
        }
        public HotSpotState GetHotSpotState()
        {
            Console.WriteLine("[Connection info] " + wifiManager.ConnectionInfo + " " + wifiManager.DhcpInfo);
            return HotSpotState;
        }
        protected virtual void OnStateChanged(HotSpotState hotSpotState)
        {
            Console.WriteLine("[State] changed to: " + hotSpotState);
            StateChange?.Invoke(this, hotSpotState);
        }
        public Tuple<string, string> GetWifiConfig()
        {
            return new Tuple<string, string>(_wifiSSID, _wifiPassword);
        }
        public void SetWifiConfig(string ssid, string pass)
        {
            _wifiSSID = ssid;
            _wifiPassword = pass;
        }
    }
}