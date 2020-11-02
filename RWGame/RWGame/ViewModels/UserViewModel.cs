using System;
using System.Collections.Generic;
using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using RWGame.Models;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using RWGame.GameChoicePages;

namespace RWGame.ViewModels
{
    public class ElementsOfViewCell
    {
        private Game Game { get; set; }
        public int IdGame { get { return Game.IdGame; } }
        public string GameId { get { return "#" + IdGame.ToString(); } }
        public string Date { get { return Game.Start.ToString(); } }
        public string PlayerName1 { get { return Game.PlayerUserName1; } }
        public string PlayerName2 { get { return Game.PlayerUserName2; } }
        public string GameStateImage
        {
            get
            {
                if (new[] 
                { 
                    GameStateEnum.NEW, 
                    GameStateEnum.CONNECT, 
                    GameStateEnum.START 
                }.Contains(Game.GameState))
                {
                    return "state_star_gray.png";
                }
                else
                {
                    return "state_star_" + Game.GameSettings.Goals[Game.IdPlayer] + ".png";
                }
            }
        }
        public string Score { get { return Convert.ToString(Game.Score); } }
        public ElementsOfViewCell(Game Game)
        {
            this.Game = Game;
        }
    }
    public class UserDisplayData : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        ServerWorker serverWorker;
        SystemSettings systemSettings;
        public UserDisplayData(ServerWorker serverWorker, SystemSettings systemSettings, INavigation Navigation)
        {
            this.serverWorker = serverWorker;
            this.systemSettings = systemSettings;
            this.Navigation = Navigation;
            UserModel = new UserModel(serverWorker, systemSettings);
        }

        #region MainProperties
        public RealPlayerChoicePage RealPlayerChoicePage { get; set; }
        public StandingsPage StandingsPage { get; set; }
        private UserModel UserModel { get; set; }
        public INavigation Navigation { get; set; }
        public ObservableCollection<ElementsOfViewCell> CustomListViewRecords { get; } = new ObservableCollection<ElementsOfViewCell>();
        #endregion
        public void CreateRealPlayerChoicePage()
        {
            RealPlayerChoicePage = new RealPlayerChoicePage(serverWorker, systemSettings);
        }
        public void CreateStandingsPage()
        {
            StandingsPage = new StandingsPage(serverWorker, systemSettings);
        }
        public bool CancelGame
        {
            get { return UserModel.CancelGame; }
            set { UserModel.CancelGame = value; }
        }
        public bool IsGameStarted
        {
            get { return UserModel.IsGameStarted; }
            set { UserModel.IsGameStarted = value; }
        }

        #region ViewProperties
        public string Title { get { return "Started Games"; } }

        public string GameListViewEmptyMessageText { get { return "Here we place your current games.\nTo play tap bot or friend."; } }

        public bool IsGameListViewVisible { get; set; } = false;
        public bool IsListViewEmptyMessageVisible { get; set; } = true;
        public bool CustomListViewRecordsIsRefreshing { get; set; }

        public string PlayWithAnotherPlayerButtonImage { get { return "pvp.png"; } }
        public string PlayWithAnotherPlayerLabelText { get { return "Friend"; } }

        public string PlayWithBotButtonImage { get { return "bot.png"; } }
        public string PlayWithBotLabelText { get { return "Bot"; } }


        public string HelpButtonImage { get { return "help.png"; } }
        public string HelpLabelText { get { return "Help"; } }

        public string UserNameText { get; set; }
        public string RatingInfoLabelText { get { return "Rating"; } }
        public string PerformanceCenterLabelText { get; set; }
        public string PerformanceBorderLabelText { get; set; }
        public string RatingLabelText { get; set; }
        public string StatisticsInfoLabelText { get { return "Statistics"; } }
        #endregion
        
        #region ButtonMethods
        public async void PlayWithBot()
        {
            if (IsGameStarted) return;
            IsGameStarted = true;
            await UserModel.CreateGameWithBot();
            await Navigation.PushAsync(UserModel.GameField);
        }
        public async void PlayWithAnotherPlayer()
        {
            if (IsGameStarted) return;
            IsGameStarted = true;
            CreateRealPlayerChoicePage();
            await Navigation.PushAsync(RealPlayerChoicePage);
        }
        #endregion
        
        #region UpdateMethods
        public async void UpdatePersonalInfo()
        {
            await UserModel.TaskUpdatePersonalInfo();
            UserModel.UpdateStats();
            UserNameText = "Hi, " + UserModel.UserName;
            PerformanceCenterLabelText = UserModel.PerformanceCenter.ToString();
            PerformanceBorderLabelText = UserModel.PerformanceBorder.ToString();
            RatingLabelText = UserModel.Rating.ToString();
        }
        public async void UpdateGameList()
        {
            await UserModel.TaskUpdateGameList();       
            CustomListViewRecords.Clear();
            if (UserModel.GamesList == null) return;
            for (int i = 0; i < UserModel.GamesList.Count; i++)
            {
                if (UserModel.GamesList[i].GameState != GameStateEnum.END)
                {
                    CustomListViewRecords.Add(new ElementsOfViewCell(UserModel.GamesList[i]));
                }
            }
            if (CustomListViewRecords.Count == 0)
            {
                IsGameListViewVisible = false;
                IsListViewEmptyMessageVisible = true;
            }
            else
            {
                IsGameListViewVisible = true;
                IsListViewEmptyMessageVisible = false;
            }
        }
        #endregion

        public async void LoadSelectedGame(ElementsOfViewCell selectedItem)
        {
            if (IsGameStarted)
            {
                return;
            }
            await UserModel.GetSelectedGameData(selectedItem.IdGame);
            if (!CancelGame)
            {
                await Navigation.PushAsync(UserModel.GameField);
                IsGameStarted = true;
            }
            UpdateGameList();
        }

        #region ActionTriggeredMethods
        public void OnPullUpdateGameList()
        {
            UpdateGameList();
            CustomListViewRecordsIsRefreshing = false;
        }
        public void OnUserPageAppearing()
        {
            UpdatePersonalInfo();
            UpdateGameList();
            IsGameStarted = false;
        }
        public async void GridPlayerScoreTapped()
        {
            CreateStandingsPage();
            await Navigation.PushAsync(StandingsPage);
        }
        #endregion
    }
    public class UserViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public UserDisplayData UserDisplayData { get; set; }
        public UserViewModel(ServerWorker ServerWorker, SystemSettings SystemSettings, INavigation Navigation)
        {
            UserDisplayData = new UserDisplayData(ServerWorker, SystemSettings, Navigation);
            RefreshGamesListCommand = new Command(UserDisplayData.OnPullUpdateGameList);
            PlayWithBotCommand = new Command(UserDisplayData.PlayWithBot);
            PlayWithAnotherPlayerCommand = new Command(UserDisplayData.PlayWithAnotherPlayer);
            OnUserPageAppearingCommand = new Command(UserDisplayData.OnUserPageAppearing);
            GridPlayerScoreTappedCommand = new Command(UserDisplayData.GridPlayerScoreTapped);
        }
        #region Commands
        public Command RefreshGamesListCommand { get; set; }
        public Command PlayWithBotCommand { get; set; }
        public Command PlayWithAnotherPlayerCommand { get; set; }
        public Command HelpCommand { get; set; }
        public Command OnUserPageAppearingCommand { get; set; }
        public Command GridPlayerScoreTappedCommand { get; set; }
        #endregion
    }
}
