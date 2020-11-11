using Java.Lang;
using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using RWGame.Models;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

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
        public event PropertyChangedEventHandler PropertyChanged;
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

        private int SelectedPlayerId { get; set; } = -1;
        public List<ElementsOfViewCell> SearchResults { get; set; } = new List<ElementsOfViewCell>();
        List<ElementsOfViewCell> emptyList = new List<ElementsOfViewCell> { new ElementsOfViewCell("", 0) };
        public bool IsPlayerListVisible { get; set; } = true;
        public string Login { get; set; }
        private bool IsGameStarted
        {
            get { return RealPlayerChoiceModel.IsGameStarted; }
            set { RealPlayerChoiceModel.IsGameStarted = value; }
        }
        
        private List<ElementsOfViewCell> GetSearchResults()
        {
            List<ElementsOfViewCell> searchResults = new List<ElementsOfViewCell>();
            if (RealPlayerChoiceModel.PlayerList == null) return emptyList;
            foreach (var player in RealPlayerChoiceModel.PlayerList)
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
        public async void PerformSearch()
        {
            RealPlayerChoiceModel.Login = Login;
            await RealPlayerChoiceModel.TaskUpdatePlayerList();
            List<ElementsOfViewCell> results = GetSearchResults();           
            if (results != null)
            {
                SearchResults = results;
            }
            else
            {
                SearchResults = emptyList;
            }
        }
        public void OnItemSelected(ElementsOfViewCell selectedItem)
        {
            Login = selectedItem.Login;
            IsPlayerListVisible = false;
        }
        public async void PlayButtonClicked()
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
            }

            if (Login == "" || Login is null || SelectedPlayerId != -1)
            {
                await RealPlayerChoiceModel.CreateGame(SelectedPlayerId);
                await RealPlayerChoiceModel.GetCancelGame();
                if (!RealPlayerChoiceModel.CancelGame)
                {
                    await RealPlayerChoiceModel.GetGameStateInfo();
                    RealPlayerChoiceModel.CreateGameField();
                    await Navigation.PushAsync(RealPlayerChoiceModel.GameField);
                    IsGameStarted = true;
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
            PlayButtonClickedCommand = new Command(RealPlayerChoiceDisplayData.PlayButtonClicked);
            OnRealPlayerChoicePageAppearingCommand = new Command(RealPlayerChoiceDisplayData.OnRealPlayerChoicePageAppearing);
            PerformSearchCommand = new Command(RealPlayerChoiceDisplayData.PerformSearch);
        }

        public Command PlayButtonClickedCommand { get; set; }
        public Command OnRealPlayerChoicePageAppearingCommand { get; set; }
        public Command PerformSearchCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
