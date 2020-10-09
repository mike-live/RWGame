using RWGame.Classes;
using RWGame.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RWGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserPage : ContentPage
    {
        public UserPage(ServerWorker serverWorker, SystemSettings systemSettings)
        {
            InitializeComponent();
            BindingContext = new UserViewModel(serverWorker, systemSettings);
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
