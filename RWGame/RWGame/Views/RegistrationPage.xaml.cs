using RWGame.Classes;
using RWGame.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RWGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationPage : ContentPage
    {
        public RegistrationPage(ServerWorker serverWorker, LoginPageViewModel loginPageViewModel)
        {
            InitializeComponent();
            BindingContext = new RegistrationPageViewModel(Navigation, serverWorker, loginPageViewModel);

            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}