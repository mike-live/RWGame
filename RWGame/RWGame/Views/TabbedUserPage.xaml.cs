using RWGame.Classes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RWGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedUserPage : TabbedPage
    {
        public TabbedUserPage(ServerWorker serverWorker, SystemSettings systemSettings)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            Children.Add(new UserPage(serverWorker, systemSettings, Navigation));
            Children.Add(new GameHistoryPage(serverWorker, systemSettings, Navigation));           
        }
    }
}
