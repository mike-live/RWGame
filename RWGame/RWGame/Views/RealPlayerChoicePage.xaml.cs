﻿using RWGame.Classes;
using RWGame.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RWGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RealPlayerChoicePage : ContentPage
    {
        private RealPlayerChoiceViewModel ViewModel;
        public RealPlayerChoicePage(INavigation navigation)
        {
            InitializeComponent();
            ViewModel = new RealPlayerChoiceViewModel(navigation);
            BindingContext = ViewModel;

            NavigationPage.SetHasNavigationBar(this, false);
        }
        private void OnSelection(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }
            PlayerListElement item = (PlayerListElement)e.SelectedItem;
            ViewModel.RealPlayerChoiceDisplayData.OnItemSelected(item);
        }
        private void OnTextChanged(object sender, EventArgs e)
        {
            ViewModel.PerformSearchCommand.Execute(sender);
        }
        private void OnButtonClicked(object sender, EventArgs e)
        {
            ViewModel.CheckLoginCommand.Execute(sender);
            ViewModel.StartGameCommand.Execute(sender);
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}