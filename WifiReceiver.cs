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
namespace Social_Network_App
{
    class WifiReceiver : BroadcastReceiver
    {
        WifiManager wifiManager;
        StringBuilder sb;
        ListView wifiDeviceList;
        public WifiReceiver(WifiManager wifiManager, ListView wifiDeviceList)
        {
            this.wifiManager = wifiManager;
            this.wifiDeviceList = wifiDeviceList;
        }
        public override void OnReceive(Context context, Intent intent)
        {
            String action = intent.Action;
            
            if (WifiManager.ScanResultsAvailableAction == action)
            {
                sb = new StringBuilder();
                var wifiList = wifiManager.ScanResults;
                ArrayList deviceList = new ArrayList();
                foreach (ScanResult scanResult in wifiList)
                {
                    sb.Append("\n").Append(scanResult.Ssid).Append(" - ").Append(scanResult.Capabilities);
                    deviceList.Add(scanResult.Ssid + " - " + scanResult.Capabilities);
                }
                String concatenatedText = sb.ToString();
                ArrayAdapter arrayAdapter = new ArrayAdapter(context, Android.Resource.Layout.SimpleExpandableListItem1, deviceList.ToArray());
                wifiDeviceList.Adapter = arrayAdapter;
            }
        }
    }
}