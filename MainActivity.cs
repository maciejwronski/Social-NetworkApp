using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Plugin.CurrentActivity;

namespace Social_Network_App
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        TextView textMessage;
        Button saveKey;
        Button saveMessage;
        TextInputEditText textEditKey;
        TextInputEditText textEditMessage;
        ListView listView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Activity = this;
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            RequestPermissions(Utils.RequiredWifiPermissions, 0);
            AttachCallbacksAndGetIDs();
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
            var menu = navigation.Menu;
            var menuItem = menu.GetItem(0);
            menuItem.SetChecked(true);
            FillCurrentMessagesInListView();
        }

        private void FillCurrentMessagesInListView()
        {
            UserMessageContainer.DecryptAllMessages();
            ArrayAdapter arrayAdapter = new ArrayAdapter(Application.Context, Android.Resource.Layout.SimpleExpandableListItem1, UserMessageContainer.decryptedUserMessages.ToArray());
            listView.Adapter = arrayAdapter;
        }
        private void AttachCallbacksAndGetIDs()
        {
            textMessage = FindViewById<TextView>(Resource.Id.message);
            textEditKey = FindViewById<TextInputEditText>(Resource.Id.textInputEditText1);
            textEditKey.Text = Crypto.VigenereCrypt._vigenereCryptKey;

            textEditMessage = FindViewById<TextInputEditText>(Resource.Id.textInputEditText2);
            textEditMessage.Text = Utils.MessageToSend;

            saveKey = FindViewById<Button>(Resource.Id.button1);
            saveMessage = FindViewById<Button>(Resource.Id.button2);
            listView = FindViewById<ListView>(Resource.Id.listView1);
            saveKey.Click += OnSaveKeyClick;
            saveMessage.Click += OnSaveMessageClick;
        }

        private void OnSaveKeyClick(object sender, EventArgs e)
        {
            Console.WriteLine("Saving Key!" + textEditKey.Text);
            Crypto.VigenereCrypt._vigenereCryptKey = textEditKey.Text;

            FillCurrentMessagesInListView();
        }
        private void OnSaveMessageClick(object sender, EventArgs e)
        {
            Console.WriteLine("Saving message!" + textEditMessage.Text);
            Utils.MessageToSend = textEditMessage.Text;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    return true;
                case Resource.Id.navigation_dashboard:
                    Finish();
                    StartActivity(typeof(AvailableNetworksActivity));
                    return true;
                case Resource.Id.navigation_notifications:
                    Finish();
                    StartActivity(typeof(CreateOwnNetworkActivity));
                    return true;
            }
            return false;
        }
    }
}

