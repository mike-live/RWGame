using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;
using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using System.Threading.Tasks;

namespace RWGame.Models
{
    class GameHistoryModel
    {
        ServerWorker serverWorker;
        SystemSettings systemSettings;
        bool isGameStarted = false;
        public List<Game> GamesList { get; set; }
        public GameHistoryModel(ServerWorker serverWorker, SystemSettings systemSettings)
        {
            this.serverWorker = serverWorker;
            this.systemSettings = systemSettings;
        }

        public async Task UpdateGameList()
        {
            GamesList = await serverWorker.TaskGetGamesList();
        }
        public async Task ExecuteItemSelectedLogic(INavigation Navigation, int gameId)
        {
            if (isGameStarted) return;
            isGameStarted = true;
            Game game = await GameProcesses.MakeSavedGame(serverWorker, gameId);

            GameStateInfo gameStateInfo = await serverWorker.TaskGetGameState(game.IdGame);
            await Navigation.PushAsync(new GameField(serverWorker, systemSettings, game, gameStateInfo));
            await UpdateGameList();
        }
    }
}
