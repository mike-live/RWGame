using RWGame.Classes;
using RWGame.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RWGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RealPlayerChoicePage : ContentPage
    {
        RealPlayerChoiceViewModel ViewModel;
        public RealPlayerChoicePage(ServerWorker serverWorker, SystemSettings systemSettings)
        {
            InitializeComponent();
            ViewModel = new RealPlayerChoiceViewModel(serverWorker, systemSettings, Navigation);
            BindingContext = ViewModel;

            NavigationPage.SetHasNavigationBar(this, false);
        }
        void OnSelection(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }
            string item = (string)e.SelectedItem;
        }
        public void RemovePage()
        {
            Navigation.RemovePage(this);
        }
        public void OnTextChanged(object sender, EventArgs e)
        {
            ViewModel.PerformSearchCommand.Execute(sender);
        }
    }
}