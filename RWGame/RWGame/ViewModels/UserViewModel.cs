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

namespace RWGame.ViewModels
{
    public class ElementsOfViewCell
    {
        public Game game { get; set; }
        public string GameId { get { return "#" + game.IdGame.ToString(); } }
        public string Date { get { return game.Start.ToString(); } }
        public string PlayerName1 { get { return game.PlayerUserName1; } }
        public string PlayerName2 { get { return game.PlayerUserName2; } }
        public string GameStateImage
        {
            get
            {
                if (new[] { GameStateEnum.NEW, GameStateEnum.CONNECT, GameStateEnum.START }.Contains(game.GameState))
                {
                    return "state_star_gray.png";
                }
                else
                {
                    return "state_star_" + game.GameSettings.Goals[game.IdPlayer] + ".png";
                }
            }
        }
        public string Score { get { return Convert.ToString(game.Score); } }
        public ElementsOfViewCell(Game _game)
        {
            game = _game;
        }
    }
    public class UserDisplayData : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public UserDisplayData(ServerWorker ServerWorker, SystemSettings SystemSettings, INavigation Navigation)
        {
            this.Navigation = Navigation;
            UserModel = new UserModel(ServerWorker, SystemSettings);
            UpdatePersonalInfo();
            UpdateGameList();
        }

        #region MainProperties
        private UserModel UserModel { get; set; }
        public INavigation Navigation { get; set; }
        public List<Game> GamesList { get; set; }
        public ObservableCollection<ElementsOfViewCell> CustomListViewRecords { get; } = new ObservableCollection<ElementsOfViewCell>();
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
        public string Title { get { return "Sarted Games"; } }

        public string GameListViewEmptyMessageText { get { return "Here we place your current games.\nTo play tap bot or friend."; } }

        public bool IsGameListViewVisible { get; set; }
        public bool IsListViewEmptyMessageVisible { get; set; }
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
            UpdateGameList();
        }
        public async void PlayWithAnotherPlayer()
        {
            if (IsGameStarted) return;
            IsGameStarted = true;
            UserModel.CreateRealPlayerChoicePage();
            await Navigation.PushAsync(UserModel.ChoiceRealPlayerPage);
        }
        #endregion
        
        #region UpdateMethods
        public async void UpdatePersonalInfo()
        {
            await UserModel.TaskUpdateModel();
            UserNameText = "Hi, " + UserModel.UserName;
            PerformanceCenterLabelText = UserModel.PerformanceCenter.ToString();
            PerformanceBorderLabelText = UserModel.PerformanceBorder.ToString();
            RatingLabelText = UserModel.Rating.ToString();
        }
        public async void UpdateGameList()
        {
            await UserModel.TaskUpdateModel();
            GamesList = UserModel.GamesList;
            CustomListViewRecords.Clear();
            for (int i = 0; i < GamesList.Count; i++)
            {
                if (GamesList[i].GameState != GameStateEnum.END)
                {
                    CustomListViewRecords.Add(new ElementsOfViewCell(GamesList[i]));
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
            await UserModel.GetSelectedGameData(selectedItem.game.IdGame);
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
            UpdateGameList();
            IsGameStarted = false;
        }
        public async void GridPlayerScoreTapped()
        {
            UserModel.CreateStandingsPage();
            await Navigation.PushAsync(UserModel.StandingsPage);
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
