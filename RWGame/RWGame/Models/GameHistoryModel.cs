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
        ServerWorker ServerWorker;
        SystemSettings SystemSettings;
        bool isGameStarted = false;
        public List<Game> GamesList { get; set; }
        public GameHistoryModel(ServerWorker ServerWorker, SystemSettings SystemSettings)
        {
            this.ServerWorker = ServerWorker;
            this.SystemSettings = SystemSettings;
        }

        public async Task UpdateGameList()
        {
            GamesList = await ServerWorker.TaskGetGamesList();
        }
        public async Task ExecuteItemSelectedLogic(INavigation Navigation, int GameId)
        {
            if (isGameStarted) return;
            isGameStarted = true;
            Game game = await GameProcesses.MakeSavedGame(ServerWorker, GameId);

            GameStateInfo gameStateInfo = await ServerWorker.TaskGetGameState(game.IdGame);
            await Navigation.PushAsync(new GameField(ServerWorker, SystemSettings, game, gameStateInfo));
            await UpdateGameList();
        }
    }
}
