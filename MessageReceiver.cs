using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Social_Network_App
{
    class MessageReceiver
    {
        private readonly UdpClient _udpClient = new UdpClient(15000);
        public async System.Threading.Tasks.Task<UdpReceiveResult> StartListening(Context context)
        {
            UdpReceiveResult result;
            while (true)
            {
                result = await _udpClient.ReceiveAsync();
                if (result.Buffer.Length > 0)
                    break;
            }
            Toast.MakeText(context, result.Buffer.Length, ToastLength.Long);
            return result;
        }
    }
}