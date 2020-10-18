using Java.Security.Acl;
using RWGame.Classes;
using RWGame.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RWGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserPage : ContentPage
    {
        public UserViewModel ViewModel { get; set; }
        public UserPage(ServerWorker serverWorker, SystemSettings systemSettings, INavigation navigation)
        {
            ViewModel = new UserViewModel(serverWorker, systemSettings, navigation);
            InitializeComponent();
            BindingContext = ViewModel;
            NavigationPage.SetHasNavigationBar(this, false);
        }
        void OnSelection(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }
            ElementsOfViewCell item = (ElementsOfViewCell)e.SelectedItem;
            ViewModel.UserDisplayData.LoadSelectedGame(item);
        }
        protected override bool OnBackButtonPressed()
        {
            return false;
        }

    }
}
