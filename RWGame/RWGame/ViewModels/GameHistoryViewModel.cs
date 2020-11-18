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
        public GameHistoryDisplayData(ServerWorker serverWorker, SystemSettings systemSettings, INavigation Navigation)
        {
            this.Navigation = Navigation;
            GameHistoryModel = new GameHistoryModel(serverWorker, systemSettings);
        }
        #region MainProperties
        private GameHistoryModel GameHistoryModel { get; set; }
        public INavigation Navigation { get; set; }
        public ObservableCollection<ElementsOfViewCell> CustomListViewRecords { get; } = new ObservableCollection<ElementsOfViewCell>();
        #endregion

        #region ViewProperties
        public string Title { get; } = "Games History";
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
                        CustomListViewRecords.Add(new ElementsOfViewCell(GameHistoryModel.GamesList[i]));
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
        public async void LoadSelectedGame(ElementsOfViewCell selectedItem)
        {
            if (SelectionMode == 1)
            {
                SelectionMode = 0;
                await GameHistoryModel.GetSelectedGameData(selectedItem.IdGame);
                await Navigation.PushAsync(GameHistoryModel.GameField);
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

        public GameHistoryViewModel(ServerWorker serverWorker, SystemSettings systemSettings, INavigation Navigation)
        {
            GameHistoryDisplayData = new GameHistoryDisplayData(serverWorker, systemSettings, Navigation);
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
