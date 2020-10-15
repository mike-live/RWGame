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
        public GameHistoryDisplayData(ServerWorker serverWorker, SystemSettings systemSettings)
        {
            GameHistoryModel = new GameHistoryModel(serverWorker, systemSettings);
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
    }

    public class GameHistoryViewModel
    {
        public GameHistoryDisplayData GameHistoryDisplayData { get; set; }

        public GameHistoryViewModel(ServerWorker serverWorker, SystemSettings systemSettings)
        {
            GameHistoryDisplayData = new GameHistoryDisplayData(serverWorker, systemSettings);
            RefreshListCommand = new Command(GameHistoryDisplayData.UpdateGameList);
        }

        public Command RefreshListCommand { get; set; }
    }
}
