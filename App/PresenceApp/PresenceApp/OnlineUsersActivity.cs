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
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using PresenceApp.Core.Entities;
using PresenceApp.Core.SignalR;
using Newtonsoft.Json.Linq;

namespace PresenceApp
{
    [Activity(Label = "Presence", Icon = "@drawable/icon")]
    public class OnlineUsersActivity : AppCompatActivity
    {
        DrawerLayout drawerLayout;
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        OnlineUsersAdapter mAdapter;
        ChatClientHub hub;
        List<Users> usersList;
        int chatId;
        Users user;
        string signalRURL;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.OnlineUsersList);
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.onlineUsersDrawer);
            
            this.chatId = Intent.GetIntExtra("chatId", -1);
            string[] userData = Intent.GetStringArrayExtra("user");
            this.signalRURL = Intent.GetStringExtra("signalr");
            this.user = new Users() { Username = userData[0], Admin = Convert.ToBoolean(userData[1]) };
            this.hub = new ChatClientHub(this.signalRURL);
            this.usersList = new List<Users>();
            this.mAdapter = new OnlineUsersAdapter(this.usersList.ToArray(), this);

            this.hub.refreshUsers((object retVal) =>
            {
                var jContainer = ((JContainer)retVal);
                var usersList = new List<Users>();
                foreach (JToken token in jContainer)
                {
                    usersList.Add(new Users()
                    {
                        Username = token.Values().ElementAt(1).ToString(),
                        Admin = false
                    });
                }
                this.mAdapter.users = usersList.ToArray();
                this.mAdapter.NotifyDataSetChanged();
            }, this.chatId);

            this.setUpLayout();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            this.Finish();
            return base.OnOptionsItemSelected(item);
        }

        private void setUpLayout()
        {
            //Get our RecyclerView layout
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.lstOnlineUsers);

            //Use the built-in linear layout manager
            mLayoutManager = new LinearLayoutManager(this);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.InflateMenu(Resource.Menu.nav_menu);
            //Toolbar will now take on default actionbar characteristics
            SetSupportActionBar(toolbar);
            //Enable support action bar to display hamburger
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Title = this.Resources.GetString(Resource.String.RefreshOnlineUsersTitle);

            mRecyclerView.SetLayoutManager(mLayoutManager);

            //Plug the adapter into the RecyclerView
            mRecyclerView.SetAdapter(mAdapter);

            Button btnBack = FindViewById<Button>(Resource.Id.btnBack);
            btnBack.Click += onBackClick;

        }

        protected void onBackClick(object sender, EventArgs e)
        {
            this.Finish();
        }

        public class OnlineUsersViewHolder : RecyclerView.ViewHolder
        {
            public TextView Username { get; private set; }
            public ImageView Admin { get; private set; }

            public OnlineUsersViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                //Locate and cache view references
                this.Username = itemView.FindViewById<TextView>(Resource.Id.txtOnlineUser);
                this.Admin = itemView.FindViewById<ImageView>(Resource.Id.imgAdmin);

                // Detect user clicks on the item view and report which item
                // was clicked (by position) to the listener:
                itemView.Click += (sender, e) => listener(Position);
            }
        }


        public class OnlineUsersAdapter : RecyclerView.Adapter
        {
            // Event handler for item clicks:
            public event EventHandler<int> ItemClick;
            private OnlineUsersActivity context;
            public Users[] users;

            public OnlineUsersAdapter(Users[] users, OnlineUsersActivity context)
            {
                this.users = users;
                this.context = context;
            }

            // Create a new photo CardView (invoked by the layout manager):
            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                // Inflate the CardView for the photo:
                View itemView = LayoutInflater.From(parent.Context).
                            Inflate(Resource.Layout.OnlineUsersListView, parent, false);

                // Create a ViewHolder to find and hold these view references, and 
                // register OnClick with the view holder:
                OnlineUsersViewHolder vh = new OnlineUsersViewHolder(itemView, OnClick);
                return vh;
            }

            // Fill in the contents of the photo card (invoked by the layout manager):
            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                OnlineUsersViewHolder vh = holder as OnlineUsersViewHolder;

                // Set the ImageView and TextView in this ViewHolder's CardView 
                // from this position in the photo album:
                vh.Username.Text = this.users[position].Username;
                vh.Admin.Visibility = (this.users[position].Admin) ? ViewStates.Visible : ViewStates.Invisible;
            }

            // Return the number of messages available in the newsletters
            public override int ItemCount
            {
                get { return this.users.Length; }
            }

            // Raise an event when the item-click takes place:
            void OnClick(int position)
            {
                if (this.context.user.Admin && this.context.user.Username != this.users[position].Username)
                    this.context.RunOnUiThread(() =>
                    {
                        Android.Support.V7.App.AlertDialog.Builder builder;
                        builder = new Android.Support.V7.App.AlertDialog.Builder(this.context);
                        builder.SetTitle("Atenção");
                        builder.SetMessage("Deseja mesmo expulsar o usuário " + this.users[position].Username + "?");
                        builder.SetCancelable(true);
                        builder.SetPositiveButton("Sim", delegate 
                            {
                                this.context.hub.kickUser(this.users[position].Username, this.context.chatId, this.context.user.Username);
                            });
                        builder.SetNegativeButton("Não", delegate { });
                        builder.Show();
                    });
               
                    
            }
        }
    }
}