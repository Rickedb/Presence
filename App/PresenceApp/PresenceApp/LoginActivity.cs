using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using PresenceApp.Core.SignalR;

namespace PresenceApp
{
    [Activity(Label = "Presence", MainLauncher = true, Icon = "@drawable/icon")]
    public class LoginActivity : AppCompatActivity
    {
        EditText _txtUsername;
        EditText _txtPassword;
        EditText _txtIP;
        CheckBox _chkAdmnistrator;

            
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.setUpLayout();

        }

        /// <summary>
        /// Set up references from layout components and load up view
        /// </summary>
        private void setUpLayout()
        {
            SetContentView(Resource.Layout.Login);
            this.Title = this.Resources.GetString(Resource.String.LoginActivityTitle);

            FindViewById<LinearLayout>(Resource.Id.llLogin).SetGravity(GravityFlags.CenterVertical);
            FindViewById<ImageView>(Resource.Id.imgvLogin).SetScaleType(ImageView.ScaleType.FitXy);
            FindViewById<Button>(Resource.Id.btnLogin).Click += onLoginClick;

            this._txtUsername = FindViewById<EditText>(Resource.Id.txtUsername);

            this._txtIP = FindViewById<EditText>(Resource.Id.txtIp);

            this._txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);
            this._txtPassword.Visibility = ViewStates.Gone;

            this._chkAdmnistrator = FindViewById<CheckBox>(Resource.Id.chkAdministrator);
            this._chkAdmnistrator.Click += onAdministratorCheckedChanged;
        }

        protected void onAdministratorCheckedChanged(object sender, EventArgs e)
        {
            this._txtPassword.Visibility = (this._chkAdmnistrator.Checked) ? ViewStates.Visible : ViewStates.Gone;
        }

        protected async void onLoginClick(object sender, EventArgs e)
        {
            var dialog = ProgressDialog.Show(this, "", "Validando informações...");
            var user = new Core.Entities.Users()
            {
                Username = this._txtUsername.Text,
                Password = this._txtPassword.Text,
                Admin = this._chkAdmnistrator.Checked
            };
            try
            {
                string signalr = "http://" + this._txtIP.Text;
                await new UsersClientHub(signalr).login(user,
                (bool authenticated) =>
                {
                    dialog.Dismiss();
                    if (authenticated)
                    {
                        var mainActivity = new Intent(this, typeof(MainActivity));
                        mainActivity.PutExtra("user", new string[] {
                            user.Username,
                            user.Admin.ToString()
                            });
                        mainActivity.PutExtra("signalr", signalr);
                        this.StartActivity(mainActivity);
                        this.Finish();
                    }
                    else
                        this.invalidAuthentication();
                }
                );
            }catch(TimeoutException)
            {
                this.couldntConnect();
            }
            catch(Exception ex)
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
            }
        }

        private void invalidAuthentication()
        {
            //set alert for executing the task
            Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);

            alert.SetTitle("Erro");

            if (!this._chkAdmnistrator.Checked)
                alert.SetMessage("Usuário já esta sendo usado!");
            else
                alert.SetMessage("Usuário ou senha incorretos!");

            alert.SetNeutralButton("OK!",(senderAlert, args) => { this._txtPassword.Text = string.Empty; });

            //run the alert in UI thread to display in the screen
            RunOnUiThread(() => {
                alert.Show();
            });
        }

        private void couldntConnect()
        {
            //set alert for executing the task
            Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);

            alert.SetTitle("Erro");

            alert.SetMessage(@"Não conseguimos connectar com o servidor, verifique com o administrador da rede se o serviço Presence está ativo!");
            alert.SetNeutralButton("OK!", (senderAlert, args) => { });
            //run the alert in UI thread to display in the screen
            RunOnUiThread(() => {
                alert.Show();
            });
        }
    }
}