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
        _hotSpot.mReservation = reservation;
        _hotSpot.SetWifiConfig(reservation.WifiConfiguration.Ssid, reservation.WifiConfiguration.PreSharedKey);
        _hotSpot.HotSpotState = Social_Network_App.HotSpotState.Enabled;
    }
    public override void OnStopped()
    {
        base.OnStopped();
        Console.WriteLine("Hotspot off");
        _hotSpot.SetWifiConfig(null,null);
        _hotSpot.HotSpotState = Social_Network_App.HotSpotState.Enabled;
    }
    public override void OnFailed([GeneratedEnum] LocalOnlyHotspotCallbackErrorCode reason)
    {
        base.OnFailed(reason);
        Console.WriteLine("Hotspot failed with error " + reason.ToString());
        _hotSpot.SetWifiConfig(null, null);
        _hotSpot.HotSpotState = Social_Network_App.HotSpotState.Disabled;
    }
}
