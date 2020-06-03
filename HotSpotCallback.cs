using System;
using Android.Net.Wifi;
using Android.Runtime;

public class HotSpotCallback : Android.Net.Wifi.WifiManager.LocalOnlyHotspotCallback
{
    public override void OnStarted(WifiManager.LocalOnlyHotspotReservation reservation)
    {
        base.OnStarted(reservation);
        Console.WriteLine("Hotspot on!");
    }
    public override void OnStopped()
    {
        base.OnStopped();
        Console.WriteLine("Hotspot off");
    }
    public override void OnFailed([GeneratedEnum] LocalOnlyHotspotCallbackErrorCode reason)
    {
        base.OnFailed(reason);
        Console.WriteLine("Hotspot failed with error " + reason.ToString());
    }
}
