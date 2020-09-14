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

namespace Social_Network_App
{
    [Serializable]
    public class UserMessage
    {
        string _sender = "";
        string _message = "";

        public UserMessage(string sender, string message)
        {
            _sender = sender;
            _message = message;
        }
        public void SetSender(string sender)
        {
            _sender = sender;
        }
        public string GetSenderIP()
        {
            return _sender;
        }
        public string GetCurrentMessage()
        {
            if (ContainsMessage())
                return _message;
            else
                return "";
        }
        public bool ContainsMessage()
        {
            return _message != "";
        }
        public override string ToString()
        {
            return "Sender: " +_sender + " - Message:" + _message;
        }
    }
}