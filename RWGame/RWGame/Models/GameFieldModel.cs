using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using System.Threading.Tasks;

namespace RWGame.Models
{
    class GameFieldModel
    {
        private readonly ServerWorker serverWorker;
        public Game Game { get; set; }
        public GameStateInfo GameStateInfo { get; set; }
        public GameFieldModel()
        {
            serverWorker = ServerWorker.GetServerWorker();
        }
        public async Task MakeTurn(int chosenTurn)
        {
            GameStateInfo = await serverWorker.TaskMakeTurn(Game.IdGame, chosenTurn);
            GameStateInfo = await serverWorker.TaskGetGameState(Game.IdGame);
        }
        public async Task UpdateGameState()
        {
            GameStateInfo = await serverWorker.TaskGetGameState(Game.IdGame);
        }
    }
}
