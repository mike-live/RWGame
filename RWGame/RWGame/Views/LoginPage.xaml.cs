using RWGame.Classes;
using RWGame.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RWGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage(SystemSettings systemSettings)
        {
            InitializeComponent();
            BindingContext = new LoginPageViewModel(systemSettings, Navigation);

            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}