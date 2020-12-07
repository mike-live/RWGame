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

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsGameStarted { get; set; } = false;
        public Game Game { get; set; }
        public GameStateInfo GameStateInfo { get; set; }
        public List<Game> GamesList { get; set; }
        public GameHistoryModel()
        {
            serverWorker = ServerWorker.GetServerWorker();
        }

        public async Task UpdateGameList()
        {
            GamesList = await serverWorker.TaskGetGamesList();
        }
        public async Task GetSelectedGameData(int gameId)
        {
            if (IsGameStarted) return;
            IsGameStarted = true;
            Game = await GameProcesses.MakeSavedGame(serverWorker, gameId);
            GameStateInfo = await serverWorker.TaskGetGameState(Game.IdGame);            
        }
    }
}
