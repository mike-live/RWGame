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

namespace RWGame.ViewModels
{
    public class ElementsOfViewCell
    {
        private readonly List<string> GameStateImages = new List<string> {
                //"new.png",           "connect.png",         "start.png",           "active.png",     "end.png",        "pause.svg",      "wait.png"
                "state_star_gray.png", "state_star_gray.png", "state_star_gray.png", "state_star.png", "state_star.png", "state_star.png", "state_star.png"
            };
        public Game game { get; set; }
        public string IdGame { get { return "#" + game.IdGame.ToString(); } }
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
    class UserDisplayData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        UserModel UserModel { get; set; }
        public INavigation Navigation { get; set; }
        List<Game> GamesList { get { return UserModel.GamesList; } }
        public string GameListViewEmptyMessageText { get { return "Here we place your current games.\nTo play tap bot or friend."; } }
        public bool IsGameListViewVisible { get; set; }
        public bool IsListViewEmptyMessageVisible { get; set; }
        public bool CustomListViewRecordsIsRefreshing { get; set; }
        public string Title { get { return "Sarted Games"; } }
        public UserDisplayData(ServerWorker ServerWorker, SystemSettings SystemSettings)
        {
            UserModel = new UserModel(ServerWorker, SystemSettings);
        }
        public List<ElementsOfViewCell> CustomListViewRecords { get; set; }
        public async void UpdateGameList()
        {
            await SubUpdateGameList();
            CustomListViewRecordsIsRefreshing = false;
        }
        public async Task SubUpdateGameList()
        {
            await UserModel.UpdateGameList();
            Device.BeginInvokeOnMainThread(() =>
            {
                CustomListViewRecords = new List<ElementsOfViewCell>();

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
            });
        }

        private ElementsOfViewCell _selectedItem;
        public ElementsOfViewCell SelectedItem 
        { 
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                CallGamesListItemSelected();
            }
        }
        public async void CallGamesListItemSelected()
        {
            await GamesListItemSelected();
        }
        public async Task GamesListItemSelected()
        {
            if (SelectedItem == null) return;
            if (UserModel.IsGameStarted) return;
            await UserModel.ExecuteItemSelectedLogic(Navigation, SelectedItem.game.IdGame);
            SelectedItem = null;
        }
    }
    class UserViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly ServerWorker ServerWorker;
        private readonly SystemSettings SystemSettings; 
        UserDisplayData UserDisplayData { get; set; }
        public UserViewModel(ServerWorker ServerWorker, SystemSettings SystemSettings)
        {
            this.ServerWorker = ServerWorker;
            this.SystemSettings = SystemSettings;
            UserDisplayData = new UserDisplayData(ServerWorker, SystemSettings);
            RefreshGamesListCommand = new Command(UserDisplayData.UpdateGameList);
        }
        public Command RefreshGamesListCommand { get; set; }
        public Command GamesListItemSelectedCommand { get; set; }
    }
}
