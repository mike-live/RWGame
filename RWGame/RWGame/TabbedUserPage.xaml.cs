using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RWGame
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedUserPage : TabbedPage
    {
        ServerWorker serverWorker;
        SystemSettings systemSettings;
        List<Game> gamesList;
        public TabbedUserPage(ServerWorker _serverWorker, SystemSettings _systemSettings)
        {
            InitializeComponent();
            this.systemSettings = _systemSettings;
            this.serverWorker = _serverWorker;
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, true); //
            this.BackgroundColor = Color.FromHex("#39bafa");
            //UploadGamesList();

            Children.Add(new UserPage(_serverWorker, _systemSettings));
            Children.Add(new GameHistoryPage(_serverWorker, _systemSettings));
        }

        private async void UploadGamesList()
        {
            gamesList = await serverWorker.TaskGetGamesList();
        }
    }
}