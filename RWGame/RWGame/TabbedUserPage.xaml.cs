using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RWGame.Views;

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

            Children.Add(new Views.UserPage(_serverWorker, _systemSettings));
            //Children.Add(new Views.GameHistoryPage(_serverWorker, _systemSettings));
        }

        private async void UploadGamesList()
        {
            gamesList = await serverWorker.TaskGetGamesList();
        }
    }
}