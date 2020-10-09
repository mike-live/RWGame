using RWGame.Classes;
using RWGame.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RWGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GameHistoryPage : ContentPage
    {
        public GameHistoryPage(ServerWorker ServerWorker, SystemSettings SystemSettings)
        {
            InitializeComponent();
            BindingContext = new GameHistoryViewModel(ServerWorker, SystemSettings);
            NavigationPage.SetHasNavigationBar(this, false);
        }
        public void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var SelectedItem = e.SelectedItem as ElementsOfViewCell;
        }
    }
}
