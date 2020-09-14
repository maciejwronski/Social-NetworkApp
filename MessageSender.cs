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
using static Android.Arch.Lifecycle.Lifecycle;

namespace Social_Network_App
{
    class MessageSender
    {
        Socket _socketBroadcast = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint ipEndPointBroadcast = new IPEndPoint(IPAddress.Parse("192.168.43.1"), Utils.Port);
        IAsyncResult sendResultBroadcast;
        public void BroadcastMessage(Context context, byte[] buffer)
        {
            try
            {
                BroadCastMethod(buffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in sending asynchronous message" + ex.Message);
            }
        }
        private void BroadCastMethod(byte[] bufferToSend)
        {
            try
            {
                _socketBroadcast.EnableBroadcast = true;
                _socketBroadcast.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
                _socketBroadcast.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, true);
                sendResultBroadcast = _socketBroadcast.BeginSendTo(bufferToSend, 0, bufferToSend.Length, SocketFlags.None, ipEndPointBroadcast, new AsyncCallback(SendCompletedBroadcast), _socketBroadcast);
            }
            catch(Exception s)
            {
                Console.WriteLine("[Exception in broadcast prepare]: " + s.Message);
            }
        }
        private void SendCompletedBroadcast(IAsyncResult ar)
        {
            try
            {
                Console.WriteLine("--Closing socket--" + ar.IsCompleted);
                //_socketBroadcast.EndSendTo(ar);
            }
            catch (Exception s)
            {
                Console.WriteLine("[Exception Complete Broadcast]" + s.Message + " " +s.InnerException);
            }
        }
    }
}