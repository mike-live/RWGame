using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using RWGame.Models;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using RWGame.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace RWGame.ViewModels
{
    public class ElementsOfViewCell
    {
        public string Login { get; set; }
        public int IdPlayer { get; set; }

        public ElementsOfViewCell(string Login, int IdPlayer)
        {
            this.Login = Login;
            this.IdPlayer = IdPlayer;
        }
    }
    public class RealPlayerChoiceDisplayData : INotifyPropertyChanged
    {
        public RealPlayerChoiceDisplayData(ServerWorker serverWorker, SystemSettings systemSettings, INavigation navigation)
        {
            RealPlayerChoiceModel = new RealPlayerChoiceModel(serverWorker, systemSettings);
            Navigation = navigation;
        }
        private INavigation Navigation { get; set; }
        private RealPlayerChoiceModel RealPlayerChoiceModel { get; set; }
        public string HeadLabelText { get { return "Play with a friend"; } }
        public string PromptLabelText { get { return "1. Choose your friend and ask them to open the app\n2. Enter their login and tap play"; } }
        public string EntryLoginPlaceholder { get { return "Enter player login"; } }
        public string PlayButtonText { get { return "Play!"; } }
        public Game Game { get; set; }
        public GameStateInfo GameStateInfo { get; set; }
        public bool CancelGame { get; set; }
        private int SelectedPlayerId { get; set; } = -1;
        public List<ElementsOfViewCell> SearchResults { get; set; } = new List<ElementsOfViewCell>();
        public bool IsPlayerListVisible { get; set; } = false;
        public string Login { get; set; }
        public bool HasLoginSelected { get; set; } = false;
        public bool IsGameStarted
        {
            get { return RealPlayerChoiceModel.IsGameStarted; }
            set { RealPlayerChoiceModel.IsGameStarted = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public List<ElementsOfViewCell> GetSearchResults(string queryString)
        {
            var normalizedQuery = queryString?.ToLower() ?? "";
            RealPlayerChoiceModel.UpdatePlayerList();
            if (RealPlayerChoiceModel.PlayerList == null) return new List<ElementsOfViewCell> { new ElementsOfViewCell("null haha", 0) };
            List<Player> playerSearchResults = RealPlayerChoiceModel.PlayerList.Where(f => f.Login.ToLowerInvariant().Contains(normalizedQuery)).ToList();
            List<ElementsOfViewCell> searchResults = new List<ElementsOfViewCell>();
            foreach (var player in playerSearchResults)
            {
                searchResults.Add(new ElementsOfViewCell(player.Login, player.IdPlayer));
            }
            return searchResults;
        }
        public void OnRealPlayerChoicePageAppearing()
        {
            if (IsGameStarted)
            {
                IsGameStarted = false;
            }
        }

        public void PerformSearch()
        {
            SearchResults = GetSearchResults(Login);
        }

    }
    public class RealPlayerChoiceViewModel : INotifyPropertyChanged
    {
        public RealPlayerChoiceDisplayData RealPlayerChoiceDisplayData { get; set; }
        public RealPlayerChoiceViewModel(ServerWorker serverWorker, SystemSettings systemSettings, INavigation navigation)
        {
            RealPlayerChoiceDisplayData = new RealPlayerChoiceDisplayData(serverWorker, systemSettings, navigation);
            //PlayButtonClickedCommand = new Command(RealPlayerChoiceDisplayData.PlayButtonClicked);
            OnRealPlayerChoicePageAppearingCommand = new Command(RealPlayerChoiceDisplayData.OnRealPlayerChoicePageAppearing);
            PerformSearchCommand = new Command(RealPlayerChoiceDisplayData.PerformSearch);
        }

        public Command PlayButtonClickedCommand { get; set; }
        public Command OnRealPlayerChoicePageAppearingCommand { get; set; }
        public Command PerformSearchCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
