using Presence.Core;
using Presence.Core.Context;
using Presence.Core.Interfaces.Services;
using Presence.Core.Services;
using Presence.Core.Tasks.Chats;
using System;
using System.Windows.Forms;

namespace Presence.Service.Tester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (this.btnStart.Text == "START")
            {
                var container = new SimpleInjector.Container();

                container.Register<IChatServices, ChatServices>();
                container.Register<IMessagesServices, MessagesServices>();
                container.Register<IUsersByChatServices, UsersByChatServices>();
                container.Register<IUsersServices, UsersServices>();
                container.RegisterSingleton<PresenceContext>();

                container.Verify();


                //PresenceServiceInitializer.configure(container);

                this.btnStart.Text = "STOP";
            }
            else
            {
                //PresenceServiceInitializer.getInstance().Stop();
                this.btnStart.Text = "START";
            }
        }
    }
}
