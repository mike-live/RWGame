using RWGame.Classes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.PlatformConfiguration;
using System;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace RWGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedUserPage : Xamarin.Forms.TabbedPage
    {
        UserPage UserPage { get; set; }
        public TabbedUserPage()
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            NavigationPage.SetHasNavigationBar(this, false);
            UserPage = new UserPage(Navigation);
            Children.Add(UserPage);
            Children.Add(new RealPlayerChoicePage(Navigation));
            Children.Add(new PlayWithBotPage(UserPage.ViewModel.PlayWithBotCommand, SwitchToDefaultTab));         
            Children.Add(new GameHistoryPage(Navigation));         
        }

        public void SwitchToDefaultTab()
        {
            this.CurrentPage = UserPage;
        }
    }
}
