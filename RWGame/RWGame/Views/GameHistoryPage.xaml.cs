using RWGame.Classes;
using RWGame.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RWGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GameHistoryPage : ContentPage
    {
        public GameHistoryPage()
        {
            //InitializeComponent();
            BindingContext = new GameHistoryViewModel();
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
