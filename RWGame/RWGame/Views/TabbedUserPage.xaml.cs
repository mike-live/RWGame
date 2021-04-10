using RWGame.Classes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.PlatformConfiguration;
using System;

namespace RWGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedUserPage : TabbedPage
    {
        UserPage UserPage { get; set; }
        public TabbedUserPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            UserPage = new UserPage(Navigation);
            Children.Add(UserPage);
            if (Device.RuntimePlatform == Device.iOS)
            {
                Children.Add(new RealPlayerChoicePage(Navigation));
                Children.Add(new PlayWithBotPage(UserPage.ViewModel.PlayWithBotCommand, SwitchToDefaultTab));
            }           
            Children.Add(new GameHistoryPage(Navigation));
            
        }

        public void SwitchToDefaultTab()
        {
            this.CurrentPage = UserPage;
        }
    }
}
