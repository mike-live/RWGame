using System;
using System.Collections.Generic;
using System.ComponentModel;
using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using System.Threading.Tasks;

namespace RWGame.Models
{
    public class UserModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly ServerWorker serverWorker;
        public UserModel()
        {
            serverWorker = ServerWorker.GetServerWorker();
            IsGameStarted = false;
        }
        public PlayerInfo PlayerInfo { get; set; }
        public List<Game> GamesList { get; set; } 
        public bool IsGameStarted { get; set; }
        public bool CancelGame { get; set; }
        public Game Game { get; set; }
        public GameStateInfo GameStateInfo { get; set; }
        public string UserName { get; set; } = "";
        public double PerformanceCenter { get; set; } = 0;
        public double PerformanceBorder { get; set; } = 0;
        public double Rating { get; set; } = 0;
        public void UpdateStats()
        { 
            UserName = PlayerInfo?.PersonalInfo.Name ?? "";
            PerformanceCenter = Math.Round(PlayerInfo?.PlayerStatistics.PerformanceCenterVsBot ?? 100);
            Rating = Math.Round(PlayerInfo?.PlayerStatistics.RatingVsBot ?? 1000);
            PerformanceBorder = Math.Round(PlayerInfo?.PlayerStatistics.PerformanceBorderVsBot ?? 100);
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
            Game = await GameProcesses.MakeSavedGame(serverWorker, GameId);
            CancelGame = await GameProcesses.StartGame(serverWorker, Game);
            GameStateInfo = await serverWorker.TaskGetGameState(Game.IdGame);                       
        }
        public async Task CreateGameWithBot()
        {           
            Game = await GameProcesses.MakeGameWithBot(serverWorker);
            GameStateInfo = await serverWorker.TaskGetGameState(Game.IdGame);
        }
    }
}
