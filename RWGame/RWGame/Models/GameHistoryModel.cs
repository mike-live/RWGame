using System.Collections.Generic;
using System.ComponentModel;
using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using System.Threading.Tasks;

namespace RWGame.Models
{
    class GameHistoryModel : INotifyPropertyChanged
    {
        private readonly ServerWorker serverWorker;
        private readonly SystemSettings systemSettings;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsGameStarted { get; set; } = false;
        public List<Game> GamesList { get; set; }
        public GameField GameField { get; set; }
        public GameHistoryModel(SystemSettings systemSettings)
        {
            serverWorker = ServerWorker.GetServerWorker();
            this.systemSettings = systemSettings;
        }

        public async Task UpdateGameList()
        {
            GamesList = await serverWorker.TaskGetGamesList();
        }
        public async Task GetSelectedGameData(int gameId)
        {
            if (IsGameStarted) return;
            IsGameStarted = true;
            Game game = await GameProcesses.MakeSavedGame(serverWorker, gameId);

            GameStateInfo gameStateInfo = await serverWorker.TaskGetGameState(game.IdGame);
            GameField = new GameField(serverWorker, systemSettings, game, gameStateInfo);
        }
    }
}
