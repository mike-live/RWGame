using RWGame.Classes;
using RWGame.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RWGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GameHistoryPage : ContentPage
    {
        GameHistoryViewModel ViewModel;
        public GameHistoryPage(ServerWorker ServerWorker, SystemSettings SystemSettings, INavigation Navigation)
        {
            ViewModel = new GameHistoryViewModel(ServerWorker, SystemSettings, Navigation);
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
            GameListElement item = (GameListElement)e.SelectedItem;
            ViewModel.GameHistoryDisplayData.LoadSelectedGame(item);
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
