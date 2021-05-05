using System;
using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using RWGame.Models;
using Xamarin.Forms;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using RWGame.Views;


namespace RWGame.ViewModels
{
    public class GameListElement
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
        public GameListElement(Game Game)
        {
            this.Game = Game;
        }
    }
    public class UserDisplayData : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public UserDisplayData(INavigation Navigation)
        {
            this.Navigation = Navigation;
            UserModel = new UserModel();
            RealPlayerChoicePage = new RealPlayerChoicePage(Navigation);
            StandingsPage = new Views.StandingsPage();
        }
        #region MainProperties
        public RealPlayerChoicePage RealPlayerChoicePage { get; set; }
        public Views.StandingsPage StandingsPage { get; set; } 
        private UserModel UserModel { get; set; }
        public INavigation Navigation { get; set; }
        public Views.GameField GameField { get; set; }
        public Game Game
        {
            get { return UserModel.Game; }
            set { UserModel.Game = value; }
        }
        public GameStateInfo GameStateInfo
        {
            get { return UserModel.GameStateInfo; }
            set { UserModel.GameStateInfo = value; }
        }
        public ObservableCollection<GameListElement> CustomListViewRecords { get; } = new ObservableCollection<GameListElement>();
        #endregion
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
        public string Title { get; } = "HOME";

        public string GameListViewEmptyMessageText { get; } = "Here we place your current games.\nTo play tap bot or friend.";

        public bool IsGameListViewVisible { get; set; } = false;
        public bool IsListViewEmptyMessageVisible { get; set; } = true;
        public bool CustomListViewRecordsIsRefreshing { get; set; }

        public string PlayWithAnotherPlayerButtonImage { get; } = "pvp.png";
        public string PlayWithAnotherPlayerLabelText { get; } = "Friend";

        public string PlayWithBotButtonImage { get; } = "bot.png";
        public string PlayWithBotLabelText { get; } = "Bot";


        public string HelpButtonImage { get; } = "help.png";
        public string HelpLabelText { get; } = "Help";

        public string UserNameText { get; set; }
        public string RatingInfoLabelText { get; } = "Rating";
        public string PerformanceCenterLabelText { get; set; }
        public string PerformanceBorderLabelText { get; set; }
        public string RatingLabelText { get; set; }
        public string StatisticsInfoLabelText { get; } = "Statistics";

        public int SelectionMode { get; set; } = 1;
        #endregion
        
        #region ButtonMethods
        public async void PlayWithBot()
        {
            if (IsGameStarted) return;
            IsGameStarted = true;
            await UserModel.CreateGameWithBot();
            GameField = new GameField(Game, GameStateInfo, Navigation);
            await Navigation.PushAsync(GameField);
        }
        public async void PlayWithAnotherPlayer()
        {
            if (IsGameStarted) return;
            IsGameStarted = true;
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
            if (UserModel.GamesList == null)
            {
                return;
            }
            for (int i = 0; i < UserModel.GamesList.Count; i++)
            {
                if (UserModel.GamesList[i].GameState != GameStateEnum.END)
                {
                    CustomListViewRecords.Add(new GameListElement(UserModel.GamesList[i]));
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

        public async void LoadSelectedGame(GameListElement selectedItem)
        {
            if (SelectionMode == 1)
            {
                SelectionMode = 0;
                if (IsGameStarted)
                {
                    return;
                }

                try
                {
                    IsGameStarted = true;
                    await UserModel.GetSelectedGameData(selectedItem.IdGame);
                    GameField = new GameField(Game, GameStateInfo, Navigation);
                    await Navigation.PushAsync(GameField);
                }
                catch (Exception)
                {              
                    await App.Current.MainPage.DisplayAlert("Error", "The game was cancelled", "OK");
                    SelectionMode = 1;
                }
                finally 
                {   
                    IsGameStarted = false;                   
                    UpdateGameList();
                }
            }
        }

        #region ActionTriggeredMethods
        public void OnPullUpdateGameList()
        {
            UpdateGameList();
            CustomListViewRecordsIsRefreshing = false;
        }
        public void OnAppearance()
        {
            UpdatePersonalInfo();
            UpdateGameList();
            IsGameStarted = false;
            SelectionMode = 1;
        }
        public async void GridPlayerScoreTapped()
        {
            await Navigation.PushAsync(StandingsPage);
        }
        #endregion
    }
    public class UserViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public UserDisplayData UserDisplayData { get; set; }
        private INavigation Navigation { get; set; }
        public UserViewModel(INavigation Navigation)
        {
            UserDisplayData = new UserDisplayData(Navigation);
            this.Navigation = Navigation;
            RefreshGamesListCommand = new Command(UserDisplayData.OnPullUpdateGameList);
            PlayWithBotCommand = new Command(UserDisplayData.PlayWithBot);
            PlayWithAnotherPlayerCommand = new Command(UserDisplayData.PlayWithAnotherPlayer);
            OnAppearanceCommand = new Command(UserDisplayData.OnAppearance);
            GridPlayerScoreTappedCommand = new Command(UserDisplayData.GridPlayerScoreTapped);
        }
        #region Commands
        public Command RefreshGamesListCommand { get; set; }
        public Command PlayWithBotCommand { get; set; }
        public Command PlayWithAnotherPlayerCommand { get; set; }
        public Command HelpCommand { get; set; }
        public Command OnAppearanceCommand { get; set; }
        public Command GridPlayerScoreTappedCommand { get; set; }
        #endregion
    }
}
