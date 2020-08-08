using System;
using Android.App;
using Android.Net.Wifi;
using Android.Runtime;

public class HotSpotCallback : Android.Net.Wifi.WifiManager.LocalOnlyHotspotCallback
{
    Social_Network_App.LocalHotspot _hotSpot;
    public HotSpotCallback(Social_Network_App.LocalHotspot hotSpot)
    {
        _hotSpot = hotSpot;
    }
    public override void OnStarted(WifiManager.LocalOnlyHotspotReservation reservation)
    {
        base.OnStarted(reservation);
        _hotSpot.HotSpotState = Social_Network_App.HotSpotState.Enabled;
        Console.WriteLine("Hotspot on!" + reservation.WifiConfiguration.Ssid + " " + reservation.WifiConfiguration.PreSharedKey + " ");
    }
    public override void OnStopped()
    {
        base.OnStopped();
        _hotSpot.HotSpotState = Social_Network_App.HotSpotState.Disabled;
        Console.WriteLine("Hotspot off");
      
    }
    public override void OnFailed([GeneratedEnum] LocalOnlyHotspotCallbackErrorCode reason)
    {
        base.OnFailed(reason);
        _hotSpot.HotSpotState = Social_Network_App.HotSpotState.Disabled;
        Console.WriteLine("Hotspot failed with error " + reason.ToString());
    }
}
