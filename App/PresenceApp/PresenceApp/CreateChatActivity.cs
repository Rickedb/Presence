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
using PresenceApp.Core.SignalR;
using PresenceApp.Core.Entities;

namespace PresenceApp
{
    [Activity(Label = "CreateChatActivity", Icon = "@drawable/icon")]
    public class CreateChatActivity : AppCompatActivity
    {
        private EditText _txtChatName;
        private EditText _txtCapacity;

        private Users currentUser;
        private string signalRURL;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.currentUser = new Users() { Username = Intent.GetStringExtra("user") };
            this.signalRURL = Intent.GetStringExtra("signalr");
            this.setUpLayout();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            this.Finish();
            return base.OnOptionsItemSelected(item);
        }

        /// <summary>
        /// Set up references from layout components and load up view
        /// </summary>
        private void setUpLayout()
        {
            SetContentView(Resource.Layout.CreateChat);
            this.Title = this.Resources.GetString(Resource.String.CreateChatTitle);

            FindViewById<Button>(Resource.Id.btnCreate).Click += onCreateClick;

            this._txtChatName = FindViewById<EditText>(Resource.Id.txtChatName);
            this._txtCapacity = FindViewById<EditText>(Resource.Id.txtCapacity);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.InflateMenu(Resource.Menu.nav_menu);
            //Toolbar will now take on default actionbar characteristics
            SetSupportActionBar(toolbar);
            //Enable support action bar to display hamburger
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Title = this.Resources.GetString(Resource.String.CreateChatTitle);

            //SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        protected void onCreateClick(object sender, EventArgs e)
        {
            ChatClientHub chatHub = new ChatClientHub(this.signalRURL);
            chatHub.CreateChat((bool ok) => {
                if(ok)
                    this.Finish();
                else
                    RunOnUiThread(() =>
                    {
                        Android.Support.V7.App.AlertDialog.Builder builder;
                        builder = new Android.Support.V7.App.AlertDialog.Builder(this);
                        builder.SetTitle("Erro");
                        builder.SetMessage("Nome de Chat já existente!");
                        builder.SetCancelable(false);
                        builder.SetPositiveButton("OK", delegate { });
                        builder.Show();
                    });
            } ,this.currentUser.Username, this._txtChatName.Text, Convert.ToInt32(this._txtCapacity.Text));
        }
    }
}