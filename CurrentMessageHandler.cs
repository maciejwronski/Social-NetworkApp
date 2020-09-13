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
    public static class CurrentMessageHandler
    {
        static string MessageSender = "";
        static string currentString = "";

        public static void SetMessage(string sender, string message)
        {
            MessageSender = sender;
            currentString = message;
        }
        public static string GetSenderIP()
        {
            if (ContainsMessage())
                return MessageSender;
            else
                return "";
        }
        public static string GetCurrentMessage()
        {
            if (ContainsMessage())
                return currentString;
            else
                return "";
        }
        public static bool ContainsMessage()
        {
            return currentString != "";
        }
    }
}