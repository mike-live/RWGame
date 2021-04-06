using RWGame.Classes.ResponseClases;
using RWGame.Models;
using System.Threading.Tasks;
using Xamarin.Forms;
using RWGame.Classes;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RWGame.ViewModels
{
    public class GameHistoryDisplayData : INotifyPropertyChanged
    {
        public GameHistoryDisplayData(INavigation Navigation)
        {
            this.Navigation = Navigation;
            GameHistoryModel = new GameHistoryModel();
        }
        #region MainProperties
        private GameHistoryModel GameHistoryModel { get; set; }
        public INavigation Navigation { get; set; }
        private Game Game
        {
            get { return GameHistoryModel.Game; }
        }
        private GameStateInfo GameStateInfo
        {
            get { return GameHistoryModel.GameStateInfo; }
        }
        private Views.GameField GameField { get; set; }
        public ObservableCollection<GameListElement> CustomListViewRecords { get; } = new ObservableCollection<GameListElement>();
        #endregion

        #region ViewProperties
        public string Title { get; } = "GAMES HISTORY";
        public string GameListViewEmptyMessageText { get; } = "Here we place your finished games.\nThanks for playing =)";

        public bool IsCustomListViewVisible { get; set; } = false;
        public bool IsGameListViewEmptyMessageVisible { get; set; } = true;
        public bool IsCustomListViewRefreshing { get; set; }
        public int SelectionMode { get; set; }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        #region UpdateMethods
        public async void UpdateGameList()
        {
            await TaskUpdateGameList();
            IsCustomListViewRefreshing = false;
        }
        public async Task TaskUpdateGameList()
        {
            await GameHistoryModel.UpdateGameList();
            CustomListViewRecords.Clear();
            if (GameHistoryModel.GamesList != null && GameHistoryModel.GamesList.Count > 0)
            {
                for (int i = 0; i < GameHistoryModel.GamesList.Count; i++)
                {
                    if (GameHistoryModel.GamesList[i].GameState == GameStateEnum.END)
                    {
                        CustomListViewRecords.Add(new GameListElement(GameHistoryModel.GamesList[i]));
                    }
                }
            }
            if (CustomListViewRecords.Count == 0)
            {
                IsCustomListViewVisible = false;
                IsGameListViewEmptyMessageVisible = true;
            }
            else
            {
                IsCustomListViewVisible = true;
                IsGameListViewEmptyMessageVisible = false;
            }
        }
        #endregion
        public async void LoadSelectedGame(GameListElement selectedItem)
        {
            if (SelectionMode == 1)
            {
                SelectionMode = 0;
                await GameHistoryModel.GetSelectedGameData(selectedItem.IdGame);
                GameField = new Views.GameField(Game, GameStateInfo, Navigation);
                await Navigation.PushAsync(GameField);
                GameHistoryModel.IsGameStarted = false;
                UpdateGameList();
            }
            
        }

        #region ActionTriggeredMethods
        public void OnGameHistoryPageAppearing()
        {
            SelectionMode = 1;
            _ = TaskUpdateGameList();            
        }
        #endregion
    }

    public class GameHistoryViewModel : INotifyPropertyChanged
    {
        public GameHistoryDisplayData GameHistoryDisplayData { get; set; }

        public GameHistoryViewModel(INavigation Navigation)
        {
            GameHistoryDisplayData = new GameHistoryDisplayData(Navigation);
            RefreshGamesListCommand = new Command(GameHistoryDisplayData.UpdateGameList);
            OnGameHistoryPageAppearingCommand = new Command(GameHistoryDisplayData.OnGameHistoryPageAppearing);
        }

        #region Commands
        public Command RefreshGamesListCommand { get; set; }
        public Command OnGameHistoryPageAppearingCommand { get; set; }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
