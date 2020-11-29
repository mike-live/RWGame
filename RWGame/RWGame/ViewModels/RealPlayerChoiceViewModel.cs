﻿using RWGame.Classes;
using RWGame.Models;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

namespace RWGame.ViewModels
{
    public class PlayerListElement
    {
        public string Login { get; set; }
        public int IdPlayer { get; set; }

        public PlayerListElement(string Login, int IdPlayer)
        {
            this.Login = Login;
            this.IdPlayer = IdPlayer;
        }
    }
    public class RealPlayerChoiceDisplayData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public RealPlayerChoiceDisplayData(ServerWorker serverWorker, SystemSettings systemSettings, INavigation navigation)
        {
            RealPlayerChoiceModel = new RealPlayerChoiceModel(serverWorker, systemSettings);
            Navigation = navigation;
        }
        private INavigation Navigation { get; set; }
        private RealPlayerChoiceModel RealPlayerChoiceModel { get; set; }
        public string HeadLabelText { get; } = "Play with a friend";
        public string PromptLabelText { get; } = "1. Choose your friend and ask them to open the app\n2. Enter their login and tap play";
        public string EntryLoginPlaceholder { get; } = "Enter player login";
        public string PlayButtonText { get; } = "Play!";
        private int SelectedPlayerId { get; set; } = -1;
        public List<PlayerListElement> SearchResults { get; set; } = new List<PlayerListElement>();
        List<PlayerListElement> emptyList = new List<PlayerListElement> { new PlayerListElement("", 0) };
        public bool IsPlayerListVisible { get; set; } = true;
        public string Login { get; set; }
        private List<PlayerListElement> GetSearchResults()
        {
            List<PlayerListElement> searchResults = new List<PlayerListElement>();
            if (RealPlayerChoiceModel.PlayerList == null) return emptyList;
            foreach (var player in RealPlayerChoiceModel.PlayerList)
            {
                searchResults.Add(new PlayerListElement(player.Login, player.IdPlayer));
            }
            return searchResults;
        }
        public async void PerformSearch()
        {
            IsPlayerListVisible = true;
            RealPlayerChoiceModel.Login = Login;
            await RealPlayerChoiceModel.TaskUpdatePlayerList();
            List<PlayerListElement> results = GetSearchResults();           
            if (results != null)
            {
                SearchResults = results;
            }
            else
            {
                SearchResults = emptyList;
            }
        }
        public void OnItemSelected(PlayerListElement selectedItem)
        {
            Login = selectedItem.Login;
            IsPlayerListVisible = false;
        }
        public async void CheckLogin()
        {
            if (Login != null)
            {
                SelectedPlayerId = -1;
                if (RealPlayerChoiceModel.PlayerList == null)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Enter player doesn't exist", "OK");
                    return;
                }
                foreach (var player in RealPlayerChoiceModel.PlayerList)
                {
                    if (player.Login == Login)
                    {
                        SelectedPlayerId = player.IdPlayer;
                    }
                }
                if (SelectedPlayerId == -1)
                {
                    if (SearchResults != null && SearchResults.Count == 1)
                    {
                        SelectedPlayerId = SearchResults[0].IdPlayer;
                        Login = SearchResults[0].Login;
                    }
                }
            }           
        }

        public async void StartGame()
        {
            if (Login == "" || Login is null || SelectedPlayerId != -1)
            {
                await RealPlayerChoiceModel.StartGame(SelectedPlayerId);
                if (!RealPlayerChoiceModel.CancelGame)
                {
                    Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);
                    await Navigation.PushAsync(RealPlayerChoiceModel.GameField);   
                }
                else
                {
                    await RealPlayerChoiceModel.CallCancelGame();
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Enter player doesn't exist", "OK");
            }
        }
    }
    public class RealPlayerChoiceViewModel : INotifyPropertyChanged
    {
        public RealPlayerChoiceDisplayData RealPlayerChoiceDisplayData { get; set; }
        public RealPlayerChoiceViewModel(ServerWorker serverWorker, SystemSettings systemSettings, INavigation navigation)
        {
            RealPlayerChoiceDisplayData = new RealPlayerChoiceDisplayData(serverWorker, systemSettings, navigation);
            CheckLoginCommand = new Command(RealPlayerChoiceDisplayData.CheckLogin);
            //OnRealPlayerChoicePageAppearingCommand = new Command(RealPlayerChoiceDisplayData.OnRealPlayerChoicePageAppearing);
            PerformSearchCommand = new Command(RealPlayerChoiceDisplayData.PerformSearch);
            StartGameCommand = new Command(RealPlayerChoiceDisplayData.StartGame);
        }

        public Command CheckLoginCommand { get; set; }
        public Command OnRealPlayerChoicePageAppearingCommand { get; set; }
        public Command PerformSearchCommand { get; set; }
        public Command StartGameCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
