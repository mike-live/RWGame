using RWGame.Classes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RWGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedUserPage : TabbedPage
    {
        public TabbedUserPage(SystemSettings systemSettings)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            NavigationPage userPage = new NavigationPage(new UserPage(systemSettings, Navigation));
            userPage.Title = "Started Games";

            NavigationPage historyPage = new NavigationPage(new GameHistoryPage(systemSettings, Navigation));
            historyPage.Title = "Game History";

            Children.Add(userPage);
            Children.Add(historyPage);           
        }
    }
}
