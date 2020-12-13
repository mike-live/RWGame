using RWGame.Classes;
using RWGame.Classes.ResponseClases;

namespace RWGame.Models
{
    class GameFieldModel
    {
        private readonly ServerWorker serverWorker;
        public Game Game { get; set; }
        public GameStateInfo GameStateInfo { get; set;}
        public GameFieldModel()
        {
            serverWorker = ServerWorker.GetServerWorker();
        }
        public async void MakeTurn(int chosenTurn)
        {
            GameStateInfo = await serverWorker.TaskMakeTurn(Game.IdGame, chosenTurn); 
        }
        public async void GetGameState()
        {
            GameStateInfo = await serverWorker.TaskGetGameState(Game.IdGame);
        }
    }
}
