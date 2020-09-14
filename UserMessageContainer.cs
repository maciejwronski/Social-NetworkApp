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
    public static class UserMessageContainer
    {
        public static List<UserMessage> userMessages = new List<UserMessage>();
        public static List<UserMessage> decryptedUserMessages = new List<UserMessage>();
        public static bool CleanMessagesOnSend = true;
        public static bool ContainsMessages()
        {
            return userMessages.Count > 0;
        }
        public static UserMessage GetLastUserMessage()
        {
            if (userMessages.Count > 0)
                return userMessages[userMessages.Count - 1];
            else return null;
        }
        public static int Count()
        {
            return userMessages.Count();
        }
        public static void PrintAllMessages()
        {
            Console.WriteLine("Message count: " + userMessages.Count);
            for(int i=0; i<Count(); i++)
            {
                Console.WriteLine(userMessages[i].ToString());
            }
        }
        public static void DecryptAllMessages()
        {
            decryptedUserMessages = new List<UserMessage>();
            foreach(UserMessage um in userMessages)
            {
                decryptedUserMessages.Add(new UserMessage(um.GetSenderIP(), Crypto.VigenereCrypt.Decode(um.GetCurrentMessage())));
            }
        }
    }
}