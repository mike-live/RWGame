using System;
using System.Collections.Generic;
using System.Text;
using RWGame.Classes;
using RWGame.Classes.ResponseClases;

namespace RWGame.Models
{
    class GameHistoryModel
    {
        ServerWorker serverWorker;
        bool isGameStarted = false;
        private List<Game> gamesList { get; set; }
        public GameHistoryModel(ServerWorker serverWorker)
        {
            this.serverWorker = serverWorker;
        }
        public async void UpdateGameList(ServerWorker serverWorker)
        {
            gamesList = await serverWorker.TaskGetGamesList();
        }
    }
}
