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
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;
using PresenceApp.Core.ViewModel;
using PresenceApp.Core.SignalR;
using Android.Graphics.Drawables;
using Newtonsoft.Json.Linq;
using PresenceApp.Core.Entities;

namespace PresenceApp
{
    [Activity(Label = "Presence",  Icon = "@drawable/icon")]
    public class ChatActivity : AppCompatActivity
    {
        EditText _txtMessage;
        ImageView _btnSend;
        Users currentUser;
        int chatId;
        string signalRURL;

        DrawerLayout drawerLayout;
        //NavigationView navigationView;

        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        MessagesAdapter mAdapter;
        List<ChatMessages> chatMessages;

        ChatClientHub chatHub;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            string[] userData = Intent.GetStringArrayExtra("user");
            this.currentUser = new Users() { Username = userData[0], Admin = Convert.ToBoolean(userData[1]) };
            this.chatId = Intent.GetIntExtra("chatId", -1);
            this.signalRURL = Intent.GetStringExtra("signalr");

            this.chatMessages = new List<ChatMessages>();
            mAdapter = new MessagesAdapter(chatMessages.ToArray(), this, this.currentUser.Username);
            //Instantiate the adapter and pass in its data source
            this.setUpLayout(Intent.GetStringExtra("chatName"));
            this.setUpDelegates();
            this.enterChat();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ChatActionMenu, menu);
            if(!this.currentUser.Admin)
                menu.FindItem(Resource.Id.menuCloseChat).SetVisible(false);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menuOnlineUsers:
                    var onlineUsersActivity = new Intent(this, typeof(OnlineUsersActivity));
                    onlineUsersActivity.PutExtra("chatId", this.chatId);
                    onlineUsersActivity.PutExtra("user", new string[] {
                            currentUser.Username,
                            currentUser.Admin.ToString()
                            });
                    onlineUsersActivity.PutExtra("signalr", this.signalRURL);
                    this.StartActivity(onlineUsersActivity);
                    break;
                case Resource.Id.menuCloseChat:
                    Android.Support.V7.App.AlertDialog.Builder builder;
                    builder = new Android.Support.V7.App.AlertDialog.Builder(this);
                    builder.SetTitle("Atenção");
                    builder.SetMessage("Deseja mesmo fechar este chat?");
                    builder.SetCancelable(true);
                    builder.SetPositiveButton("Sim", delegate
                    {
                        this.chatHub.closeChat(this.chatId);
                    });
                    builder.SetNegativeButton("Não", delegate { });
                    builder.Show();
                    
                    break;
                default:
                    this.chatHub.leaveChat(this.chatId, this.currentUser.Username);
                    var mainActivity = new Intent(this, typeof(MainActivity));
                    mainActivity.PutExtra("user", new string[] {
                            currentUser.Username,
                            currentUser.Admin.ToString()
                            });
                    mainActivity.PutExtra("signalr", this.signalRURL);
                    this.StartActivity(mainActivity);
                    this.Finish();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        public void enterChat()
        {
            var dialog = ProgressDialog.Show(this, "", "Obtendo últimas mensagens...");
            try
            {
                this.chatHub.enterChat((bool entered) =>
                {
                    if (entered)
                    {
                        //this.chatHub.GetLast50Messages((object messages) =>
                        //{
                        //    var jContainer = ((JContainer)messages);
                        //    var newMessages = new List<ChatMessages>();
                        //    foreach (JToken token in jContainer)
                        //    {
                        //        newMessages.Add(
                        //        new ChatMessages()
                        //        {
                        //            Username = token.Values().ElementAt(5).Values().ElementAt(1).ToString(),
                        //            Message = token.Values().ElementAt(2).ToString()
                        //        });
                        //    }
                        //    this.mAdapter.chatMessages.Clear();
                        //    this.mAdapter.chatMessages.AddRange(newMessages);
                        //}, this.chatId);
                        //this.mAdapter.NotifyDataSetChanged();
                        dialog.Dismiss();
                    }
                }, this.chatId, this.currentUser.Username);
            }
            catch (Exception ex)
            {
                RunOnUiThread(() =>
                {
                    Android.Support.V7.App.AlertDialog.Builder builder;
                    builder = new Android.Support.V7.App.AlertDialog.Builder(this);
                    builder.SetTitle("Erro");
                    builder.SetMessage(ex.Message);
                    builder.SetCancelable(false);
                    builder.SetPositiveButton("OK", delegate { });
                    builder.Show();
                });
                dialog.Dismiss();
            }
        }

        public void setUpDelegates()
        {
            Action<int, string, string> receiveMessage = (int chatId, string user, string msg) =>
            {
                this.mAdapter.chatMessages.Add(new ChatMessages()
                {
                    Username = user,
                    Message = msg
                });
                
                this.mAdapter.NotifyItemRangeChanged(this.chatMessages.Count - 1, 1);
            };

            Action<int, string> kickedUser = (int chatId, string user) =>
            {
                if (user == this.currentUser.Username)
                {
                    var mainActivity = new Intent(this, typeof(MainActivity));
                    mainActivity.PutExtra("user", new string[] {
                            currentUser.Username,
                            currentUser.Admin.ToString()
                            });
                    mainActivity.PutExtra("signalr", this.signalRURL);
                    this.StartActivity(mainActivity);
                    this.Finish();
                }
            };

            this.chatHub = new ChatClientHub(this.signalRURL);
            this.chatHub.ConfigureCallBacks(receiveMessage, kickedUser);
        }


        /// <summary>
        /// Set up references from layout components and load up view
        /// </summary>
        private void setUpLayout(string chatName)
        {
            SetContentView(Resource.Layout.Chat);

            //Get our RecyclerView layout
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.lstMessages);

            //Use the built-in linear layout manager
            mLayoutManager = new LinearLayoutManager(this);

            // Or use the built-in grid layout manager (two horizontal rows):
            // mLayoutManager = new GridLayoutManager
            //        (this, 2, GridLayoutManager.Horizontal, false);

            // Plug the layout manager into the RecyclerView:
            mRecyclerView.SetLayoutManager(mLayoutManager);

            //Plug the adapter into the RecyclerView
            mRecyclerView.SetAdapter(mAdapter);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.InflateMenu(Resource.Menu.ChatActionMenu);
            //Toolbar will now take on default actionbar characteristics
            SetSupportActionBar(toolbar);
            //Enable support action bar to display hamburger
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            //SupportActionBar.SetIcon(GetDrawable(Resource.Drawable.Icon));
            SupportActionBar.Title = chatName;


            this._txtMessage = FindViewById<EditText>(Resource.Id.txtMessage);
            this._txtMessage.Hint = "Escreva uma mensagem";

            this._btnSend = FindViewById<ImageView>(Resource.Id.btnSend);
            this._btnSend.Click += _btnSend_Click;
        }

        private void _btnSend_Click(object sender, EventArgs e)
        {
            this.chatHub.sendMessage(this.chatId, this.currentUser.Username, this._txtMessage.Text);
            this._txtMessage.Text = string.Empty;
        }

    }

    public class MessageViewHolder : RecyclerView.ViewHolder
    {
        public TextView Username { get; private set; }
        public TextView Message { get; private set; }
        public RelativeLayout Layout { get; set; }

        public MessageViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            //Locate and cache view references
            Username = itemView.FindViewById<TextView>(Resource.Id.usernameTextView);
            Message = itemView.FindViewById<TextView>(Resource.Id.messageTextView);
            Layout = itemView.FindViewById<RelativeLayout>(Resource.Id.MessageLinearLayout);

            // Detect user clicks on the item view and report which item
            // was clicked (by position) to the listener:
            itemView.Click += (sender, e) => listener(Position);
        }
    }


    public class MessagesAdapter : RecyclerView.Adapter
    {
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;
        public List<ChatMessages> chatMessages;
        private string user;
        private Activity context;

        public MessagesAdapter(ChatMessages[] chatMessages, Activity context, string user)
        {
            this.chatMessages = chatMessages.ToList();
            this.context = context;
            this.user = user;
        }

        // Create a new photo CardView (invoked by the layout manager):
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.MessageCardView, parent, false);

            // Create a ViewHolder to find and hold these view references, and 
            // register OnClick with the view holder:
            MessageViewHolder vh = new MessageViewHolder(itemView, OnClick);
            return vh;
        }

        // Fill in the contents of the photo card (invoked by the layout manager):
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                MessageViewHolder vh = holder as MessageViewHolder;

                // Set the ImageView and TextView in this ViewHolder's CardView 
                // from this position in the photo album:
                vh.Username.Text = this.chatMessages[position].Username;
                vh.Message.Text = this.chatMessages[position].Message;

                vh.Layout.SetGravity(GravityFlags.Left);
                vh.Layout.Background = this.context.GetDrawable(Resource.Drawable.friendBubbleChat);

                if (vh.Username.Text == this.user)
                {
                    vh.Layout.Background = this.context.GetDrawable(Resource.Drawable.selfBubbleChat);
                    vh.Username.Gravity = GravityFlags.Right;
                    vh.Message.Gravity = GravityFlags.Right;
                    vh.Username.SetTextColor(Android.Graphics.Color.White);
                    vh.Message.SetTextColor(Android.Graphics.Color.White);
                }
            }
            catch { }
        }

        // Return the number of messages available in the newsletters
        public override int ItemCount
        {
            get { return this.chatMessages.Count; }
        }

        // Raise an event when the item-click takes place:
        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }
    }
}