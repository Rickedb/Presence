using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using PresenceApp.Core.Entities;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using System.Linq;
using PresenceApp.Core.SignalR;
using Newtonsoft.Json.Linq;
using System.Timers;

namespace PresenceApp
{
    [Activity(Label = "Presence",  Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity
    {
        NavigationView navigationView;
        DrawerLayout drawerLayout;
        Users currentUser;
        Timer refreshTimer;

        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        ChatsAdapter mAdapter;
        List<Chat> chats;
        string signalRURL;

        ChatClientHub chatHub;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            string[] userData = Intent.GetStringArrayExtra("user");
            this.currentUser = new Users() {  Username = userData[0], Admin = Convert.ToBoolean(userData[1]) };
            this.signalRURL = Intent.GetStringExtra("signalr");

            SetContentView(Resource.Layout.Main);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.InflateMenu(Resource.Menu.MainActionMenu);
            //Toolbar will now take on default actionbar characteristics
            SetSupportActionBar(toolbar);
            //Enable support action bar to display hamburger
            SupportActionBar.SetDisplayHomeAsUpEnabled(false);
            SupportActionBar.Title = this.Resources.GetString(Resource.String.MainActivityTitle);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            chats = new List<Chat>();
            chats.Add(new Chat() { Name = " " });

            mAdapter = new ChatsAdapter(chats.ToArray(), (int id, string chatName) =>
            {
                var ChatActivity = new Intent(this, typeof(ChatActivity));
                ChatActivity.PutExtra("user", userData);
                ChatActivity.PutExtra("chatId", id);
                ChatActivity.PutExtra("chatName", chatName);
                ChatActivity.PutExtra("signalr", this.signalRURL);
                this.StartActivity(ChatActivity);
                this.Finish();
            });
        
            this.setUpLayout();
            this.setUpDelegates();
            this.startRefreshTimer();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MainActionMenu, menu);
            if(!this.currentUser.Admin)
                menu.FindItem(Resource.Id.menuAddChat).SetVisible(false);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var createChatActivity = new Intent(this, typeof(CreateChatActivity));
            createChatActivity.PutExtra("user", this.currentUser.Username);
            createChatActivity.PutExtra("signalr", this.signalRURL);
            this.StartActivity(createChatActivity);
            return base.OnOptionsItemSelected(item);
        }


        private void setUpLayout()
        {
            //Get our RecyclerView layout
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.lstChats);

            //Use the built-in linear layout manager
            mLayoutManager = new LinearLayoutManager(this);

            // Or use the built-in grid layout manager (two horizontal rows):
            // mLayoutManager = new GridLayoutManager
            //        (this, 2, GridLayoutManager.Horizontal, false);

            // Plug the layout manager into the RecyclerView:
            mRecyclerView.SetLayoutManager(mLayoutManager);

            //Plug the adapter into the RecyclerView
            mRecyclerView.SetAdapter(mAdapter);
        }

        public void setUpDelegates()
        {
            Action<object> receiveMessage = (object response) =>
            {
                var jContainer = ((JContainer)response);
                this.chats.Clear();
                foreach (JToken token in jContainer)
                {
                    var chat = new Chat()
                    {
                        ID = (int)token.Values().ElementAt(0),
                        Name = token.Values().ElementAt(2).ToString()
                    };
                    this.chats.Add(chat);
                }
                this.mAdapter.chats = this.chats.ToArray();
                this.mAdapter.NotifyDataSetChanged();
            };

            this.chatHub = new ChatClientHub(this.signalRURL);
            this.chatHub.ConfigureCallBacks(receiveMessage);
        }

        private void startRefreshTimer()
        {
            this.chatHub.refreshChats();
            refreshTimer = new Timer(3000);
            refreshTimer.Elapsed += (object sender, ElapsedEventArgs e) => {
                refreshTimer.Stop();
                this.chatHub.refreshChats();
                refreshTimer.Start();
            };
            refreshTimer.Start();
        }

        public class ChatViewHolder : RecyclerView.ViewHolder
        {
            public TextView ChatName { get; private set; }

            public ChatViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                //Locate and cache view references
                this.ChatName = itemView.FindViewById<TextView>(Resource.Id.txtChatName);

                // Detect user clicks on the item view and report which item
                // was clicked (by position) to the listener:
                itemView.Click += (sender, e) => listener(Position);
            }
        }


        public class ChatsAdapter : RecyclerView.Adapter
        {
            // Event handler for item clicks:
            public event EventHandler<int> ItemClick;
            private Action<int, string> callback;
            public Chat[] chats;

            public ChatsAdapter(Chat[] chats, Action<int, string> callback)
            {
                this.callback = callback;
                this.chats = chats;
            }

            // Create a new photo CardView (invoked by the layout manager):
            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                // Inflate the CardView for the photo:
                View itemView = LayoutInflater.From(parent.Context).
                            Inflate(Resource.Layout.GroupCardView, parent, false);

                // Create a ViewHolder to find and hold these view references, and 
                // register OnClick with the view holder:
                ChatViewHolder vh = new ChatViewHolder(itemView, OnClick);
                return vh;
            }

            // Fill in the contents of the photo card (invoked by the layout manager):
            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                ChatViewHolder vh = holder as ChatViewHolder;

                // Set the ImageView and TextView in this ViewHolder's CardView 
                // from this position in the photo album:
                vh.ChatName.Text = this.chats[position].Name;
            }

            // Return the number of messages available in the newsletters
            public override int ItemCount
            {
                get { return this.chats.Length; }
            }

            // Raise an event when the item-click takes place:
            void OnClick(int position)
            {
                this.callback.Invoke(chats[position].ID, chats[position].Name);
            }
        }
    }
}

