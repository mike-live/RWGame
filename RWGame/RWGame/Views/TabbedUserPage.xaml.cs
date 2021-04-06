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
        public TabbedUserPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            UserPage userPage = new UserPage(Navigation);
            Children.Add(userPage);

            if (Device.RuntimePlatform == Device.iOS)
            {
                Children.Add(new PlayPopupIOS(userPage.ViewModel));
            }            
            Children.Add(new GameHistoryPage(Navigation));
            
        }
    }
}
