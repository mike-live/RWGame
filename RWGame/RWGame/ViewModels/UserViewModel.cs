using System;
using System.Collections.Generic;
using System.Text;
using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using RWGame.Models;
using System.Runtime.CompilerServices;
using RWGame.PagesGameChoise;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using System.ComponentModel;
using Xamarin.Essentials;
using System.Collections.ObjectModel;

namespace RWGame.ViewModels
{
    public class ElementsOfViewCell
    {
        private readonly List<string> GameStateImages = new List<string> {
                //"new.png",           "connect.png",         "start.png",           "active.png",     "end.png",        "pause.svg",      "wait.png"
                "state_star_gray.png", "state_star_gray.png", "state_star_gray.png", "state_star.png", "state_star.png", "state_star.png", "state_star.png"
            };

        public Game game { get; set; }
        public string GameId { get { return "#" + game.IdGame.ToString(); } }
        public string Date { get { return game.Start.ToString(); } }
        public string PlayerName1 { get { return game.PlayerUserName1; } }
        public string PlayerName2 { get { return game.PlayerUserName2; } }
        public string GameState { get { return game.GameState.ToString(); } }
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
        public int Player1 { get { return game.Player1 == null ? -1 : (int)game.Player1; } }
        public int Player2 { get { return game.Player2 == null ? -1 : (int)game.Player2; } }
        public string Score { get { return Convert.ToString(game.Score); } }
        public GameSettings Settings { get { return game.GameSettings; } }
        public ElementsOfViewCell(Game _game)
        {
            game = _game;
        }
    }
    public class UserDisplayData : INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public UserDisplayData(ServerWorker ServerWorker, SystemSettings SystemSettings)
        {
            UserModel = new UserModel(ServerWorker, SystemSettings);
            UpdateUserPage();
        }
        private UserModel UserModel {get; set; }

        public INavigation Navigation { get; set; }
        public List<Game> GamesList { get; set; }

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

        
        public ObservableCollection<ElementsOfViewCell> CustomListViewRecords { get; } = new ObservableCollection<ElementsOfViewCell>();
        public async void UpdateUserPage()
        {
            await TaskUpdatePersonalInfo();
            await TaskUpdateGameList();
        }
        public async void UpdateGameList()
        {         
            await TaskUpdateGameList();
            CustomListViewRecordsIsRefreshing = false;
        }
        public async Task TaskUpdatePersonalInfo()
        {
            await UserModel.TaskUpdateModel();
            UserNameText = "Hi, " + UserModel.UserName;
            PerformanceCenterLabelText = UserModel.PerformanceCenter.ToString();
            PerformanceBorderLabelText = UserModel.PerformanceBorder.ToString();
            RatingLabelText = UserModel.Rating.ToString();
        }
        public async Task TaskUpdateGameList()
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
    }
    class UserViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public UserDisplayData UserDisplayData { get; set; }
        public UserViewModel(ServerWorker ServerWorker, SystemSettings SystemSettings)
        {
            UserDisplayData = new UserDisplayData(ServerWorker, SystemSettings);
            RefreshGamesListCommand = new Command(UserDisplayData.UpdateGameList);
        }
        public Command RefreshGamesListCommand { get; set; }
    }
}
