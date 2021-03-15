﻿using RWGame.Classes;
using RWGame.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RWGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserPage : ContentPage
    {
        public UserViewModel ViewModel { get; set; }
        public UserPage(INavigation navigation)
        {
            ViewModel = new UserViewModel(navigation);
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
            ViewModel.UserDisplayData.LoadSelectedGame(item);
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

    }
}
