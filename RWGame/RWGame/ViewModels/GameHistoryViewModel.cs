using System;
using System.Collections.Generic;
using System.Text;
using RWGame.Classes.ResponseClases;
using RWGame.Models;
using System.Threading.Tasks;
using Xamarin.Forms;
using RWGame.Classes;
using System.Collections.ObjectModel;

namespace RWGame.ViewModels
{
    public class GameHistoryDisplayData
    {
        public GameHistoryDisplayData(ServerWorker ServerWorker, SystemSettings SystemSettings)
        {
            GameHistoryModel = new GameHistoryModel(ServerWorker, SystemSettings);
        }
        public string Title { get { return "Games History"; } }
        public string GameLsitViewEmptyMessageText { get { return "Here we place your finished games.\nThanks for playing =)"; } }
        private GameHistoryModel GameHistoryModel { get; set; }
        public INavigation Navigation { get; set; }
        public ObservableCollection<ElementsOfViewCell> CustomListViewRecords { get; set; }
        List<Game> gamesList { get { return GameHistoryModel.GamesList; } }
        public bool IsGamesListViewVisible { get; set; }
        public bool IsGameListViewEmptyMessageVisible { get; set; }
        public bool IsGamesListViewRefreshing { get; set; }
        public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;
        public ElementsOfViewCell _selectedItem;
        public ElementsOfViewCell SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
            }
        }
        
        public async void UpdateGameList()
        {
            await SubUpdateGameList();
            IsGamesListViewRefreshing = false;
        }
        public async Task SubUpdateGameList()
        {
            await GameHistoryModel.UpdateGameList();
            Device.BeginInvokeOnMainThread(() =>
            {
                CustomListViewRecords = new ObservableCollection<ElementsOfViewCell>();
                if (gamesList != null && gamesList.Count > 0)
                {
                    for (int i = 0; i < gamesList.Count; i++)
                    {
                        if (gamesList[i].GameState == GameStateEnum.END)
                        {
                            CustomListViewRecords.Add(new ElementsOfViewCell(gamesList[i]));
                        }
                    }
                }
                if (CustomListViewRecords.Count == 0)
                {
                    IsGamesListViewVisible = false;
                    IsGameListViewEmptyMessageVisible = true;
                }
                else
                {
                    IsGamesListViewVisible = true;
                    IsGameListViewEmptyMessageVisible = false;
                }
            });
        }
        public async void CallGamesListItemSelected(ElementsOfViewCell SelectedItem)
        {
            await GameHistoryModel.ExecuteItemSelectedLogic(Navigation, SelectedItem.game.IdGame);
        }
        public void OnGamesListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var SelectedItem = e.SelectedItem as ElementsOfViewCell;
            if (SelectedItem == null) return;
            CallGamesListItemSelected(SelectedItem);
            SelectedItem = null;
        }
    }
    public class GameHistoryViewModel
    {
        GameHistoryDisplayData GameHistoryDisplayData { get; set; }
        
        public GameHistoryViewModel(ServerWorker ServerWorker, SystemSettings SystemSettings)
        {
            GameHistoryDisplayData = new GameHistoryDisplayData(ServerWorker, SystemSettings);
            RefreshListCommand = new Command(GameHistoryDisplayData.UpdateGameList);
        }
        
        
        public Command RefreshListCommand { get; set; }
    }
}
