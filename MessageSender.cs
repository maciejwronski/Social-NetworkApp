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
    class MessageSender
    {
        public async System.Threading.Tasks.Task<int> SendBroadcastMessage(Context context, string text)
        {
            using (var client = new UdpClient())
            {
                client.EnableBroadcast = true;
                var endpoint = new IPEndPoint(IPAddress.Broadcast, Utils.Port);
                var message = Encoding.ASCII.GetBytes("Hello World - " + DateTime.Now.ToString());
                return await client.SendAsync(message, message.Length, endpoint);
            }
        }
    }
}