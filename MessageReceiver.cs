using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.CurrentActivity;
using Xamarin.Essentials;

namespace Social_Network_App
{
    class MessageReceiver
    {
        Socket _socketb = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private IPEndPoint ipEndPointBroadcast = new IPEndPoint(IPAddress.Any, Utils.Port);
        private class StateObject
        {
            public Socket workSocket = null;
            public const int BufferSize = 1024;
            public byte[] buffer = new byte[BufferSize];

        }
        StateObject state = new StateObject();
        Context context;
        IAsyncResult receiveResultBroadcast;
        public void StartListening(Context context)
        {
            try
            {
                this.context = context;
                ReceiveBroadcastMethod();
            }
            catch (Exception s)
            {
                Console.WriteLine("Start listening " + s.Message);
            }
        }
        void ReceiveBroadcastMethod()
        {
            try
            {
                EndPoint epb = (EndPoint)ipEndPointBroadcast;
                _socketb.Bind(epb);
                _socketb.EnableBroadcast = true;
                receiveResultBroadcast = _socketb.BeginReceiveFrom(state.buffer, 0, 1024, SocketFlags.None, ref epb, new AsyncCallback(ReceiveCompleteBroadcast), state);
            }
            catch (Exception s)
            {
                Console.WriteLine("[Preparing broadcast] " + s.Message);
            }
        }
        void ReceiveCompleteBroadcast(IAsyncResult ar)
        {
            try
            {
                StateObject so = (StateObject)ar.AsyncState;
                EndPoint ep = (EndPoint)ipEndPointBroadcast;
                int bytes = _socketb.EndReceiveFrom(receiveResultBroadcast, ref ep);
                Console.WriteLine("RECV: {0}: {1}, {2}", ep.ToString(), bytes, Encoding.ASCII.GetString(so.buffer, 0, bytes));
                ArrivedMessageHandler(ep.ToString(), so.buffer);
                _socketb.Close();
            }
            catch (Exception s)
            {
                Console.WriteLine("Error in broadcast listening " + s.Message);
            }
        }
        void ArrivedMessageHandler(string sender, byte[] buffer)
        {
            UserMessageContainer.userMessages.AddRange(DeserializeMessageArray(buffer));
            if (UserMessageContainer.GetLastUserMessage() != null && UserMessageContainer.GetLastUserMessage().GetSenderIP() == "")
            {
                if (!UserMessageContainer.GetLastUserMessage().ContainsMessage())
                    UserMessageContainer.userMessages.RemoveAt(UserMessageContainer.Count() - 1); // removing - its empty ( someone is just a sender-extender)
                else
                    UserMessageContainer.GetLastUserMessage().SetSender(sender.ToString()); // someone send message - lets write save his ip.
            }
            UserMessageContainer.PrintAllMessages();
        }
        List<UserMessage> DeserializeMessageArray(byte[] array)
        {
            var mStream = new MemoryStream();
            var binFormatter = new BinaryFormatter();

            mStream.Write(array, 0, array.Length);
            mStream.Position = 0;

            return binFormatter.Deserialize(mStream) as List<UserMessage>;
        }
    }
}