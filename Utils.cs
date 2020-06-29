using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace Social_Network_App
{
    public static class Utils
    {
        public enum ePermission
        {
            LocationPermission,
            WifiPermission,
            HotSpotPermission
        }
        public static readonly string[] RequiredHotSpotPermissions =    {
            Android.Manifest.Permission.AccessFineLocation,
            Android.Manifest.Permission.ChangeWifiState,
            Android.Manifest.Permission.ChangeNetworkState,
            Android.Manifest.Permission.AccessNetworkState
        };
        public static readonly string[] RequiredWifiPermissions = {
            Android.Manifest.Permission.ChangeWifiMulticastState,
            Android.Manifest.Permission.AccessWifiState,
            Android.Manifest.Permission.ChangeWifiState
        };
        public static async Task<bool> GetPermissions()
        {
            bool permissionsGranted = true;

            var permissionsStartList = new List<Permission>()
        {
            Permission.LocationAlways,
            Permission.LocationWhenInUse,
            Permission.Location,
        };
            var permissionsNeededList = new List<Permission>();
            try
            {
                foreach (var permission in permissionsStartList)
                {
                    var status = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);
                    if (status != PermissionStatus.Granted)
                    {
                        permissionsNeededList.Add(permission);
                    }
                }
            }
            catch (Exception ex)
            {
            }

            var results = await CrossPermissions.Current.RequestPermissionsAsync(permissionsNeededList.ToArray());

            try
            {
                foreach (var permission in permissionsNeededList)
                {
                    var status = PermissionStatus.Unknown;
                    if (results.ContainsKey(permission))
                        status = results[permission];
                    if (status == PermissionStatus.Granted || status == PermissionStatus.Unknown)
                    {
                        permissionsGranted = true;
                    }
                    else
                    {
                        permissionsGranted = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return permissionsGranted;
        }
        public static bool HasPermission(Context context, ePermission permission)
        {
            switch (permission)
            {
                case ePermission.WifiPermission:
                    return (context.CheckSelfPermission(Android.Manifest.Permission.ChangeWifiMulticastState) == Android.Content.PM.Permission.Granted) &&
                           (context.CheckSelfPermission(Android.Manifest.Permission.AccessWifiState) == Android.Content.PM.Permission.Granted) &&
                           (context.CheckSelfPermission(Android.Manifest.Permission.ChangeWifiState) == Android.Content.PM.Permission.Granted);
                case ePermission.LocationPermission:
                    return context.CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) == Android.Content.PM.Permission.Granted;
                case ePermission.HotSpotPermission:
                    return (context.CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) == Android.Content.PM.Permission.Granted) &&
       (context.CheckSelfPermission(Android.Manifest.Permission.ChangeWifiState) == Android.Content.PM.Permission.Granted) &&
              (context.CheckSelfPermission(Android.Manifest.Permission.ChangeNetworkState) == Android.Content.PM.Permission.Granted) &&
                            (context.CheckSelfPermission(Android.Manifest.Permission.AccessNetworkState) == Android.Content.PM.Permission.Granted);
            }
            return false;
        }
    }
}