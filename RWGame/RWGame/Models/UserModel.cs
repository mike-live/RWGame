using System;
using System.Collections.Generic;
using System.ComponentModel;
using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using System.Threading.Tasks;
using RWGame.GameChoicePages;

namespace RWGame.Models
{
    public class UserModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly ServerWorker serverWorker;
        private readonly SystemSettings systemSettings;
        public UserModel(ServerWorker serverWorker, SystemSettings systemSettings)
        {
            this.serverWorker = serverWorker;
            this.systemSettings = systemSettings;
            IsGameStarted = false;
        }
        public GameField GameField { get; set; }
        public PlayerInfo PlayerInfo { get; set; }
        public List<Game> GamesList { get; set; } 
        public bool IsGameStarted { get; set; }
        public bool CancelGame { get; set; }
        public string UserName { get; set; } = "";
        public double PerformanceCenter { get; set; } = 0;
        public double PerformanceBorder { get; set; } = 0;
        public double Rating { get; set; } = 0;
        public void UpdateStats()
        { 
            UserName = PlayerInfo?.PersonalInfo.Name ?? "";
            PerformanceCenter = Math.Round(PlayerInfo?.PlayerStatistics.PerformanceCenterVsBot ?? 0);
            Rating = Math.Round(PlayerInfo?.PlayerStatistics.RatingVsBot ?? 0);
            PerformanceBorder = Math.Round(PlayerInfo?.PlayerStatistics.PerformanceBorderVsBot ?? 0);
        }
        public async Task TaskUpdatePersonalInfo()
        {
            PlayerInfo = await serverWorker.TaskGetPlayerInfo();
        }
        public async Task TaskUpdateGameList()
        {
            GamesList = await serverWorker.TaskGetGamesList();
        }
        public async Task GetSelectedGameData(int GameId)
        {
            Game game = await GameProcesses.MakeSavedGame(serverWorker, GameId);
            CancelGame = await GameProcesses.StartGame(serverWorker, game);
            GameStateInfo gameStateInfo = await serverWorker.TaskGetGameState(game.IdGame);
            GameField = new GameField(serverWorker, systemSettings, game, gameStateInfo);             
        }
        public async Task CreateGameWithBot()
        {           
            Game game = await GameProcesses.MakeGameWithBot(serverWorker);
            GameStateInfo gameStateInfo = await serverWorker.TaskGetGameState(game.IdGame);
            GameField = new GameField(serverWorker, systemSettings, game, gameStateInfo);
        }
    }
}
