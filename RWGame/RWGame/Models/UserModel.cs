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
        private ServerWorker serverWorker;
        private SystemSettings systemSettings;
        public UserModel(ServerWorker serverWorker, SystemSettings systemSettings)
        {
            this.serverWorker = serverWorker;
            this.systemSettings = systemSettings;
            UpdateModel();
            IsGameStarted = false;
        }
        public GameField GameField { get; set; }
        public PlayerInfo PlayerInfo { get; set; }
        public ChoiceRealPlayerPage ChoiceRealPlayerPage { get; set; }
        public StandingsPage StandingsPage { get; set; }
        public List<Game> GamesList { get; set; }
        public bool IsGameStarted { get; set; }
        public bool CancelGame { get; set; }
        public string UserName { get; set; }
        public double PerformanceCenter { get; set; }
        public double PerformanceBorder {get; set; }
        public double Rating { get; set; }
        public async void UpdateModel()
        {
            await TaskUpdateModel();
        }
        public async Task TaskUpdateModel()
        {
            PlayerInfo = await serverWorker.TaskGetPlayerInfo();
            GamesList = await serverWorker.TaskGetGamesList();
            UserName = PlayerInfo?.PersonalInfo.Name ?? "";
            PerformanceCenter = Math.Round(PlayerInfo?.PlayerStatistics.PerformanceCenterVsBot.Value ?? 0);
            Rating = Math.Round(PlayerInfo?.PlayerStatistics.RatingVsBot.Value ?? 0);
            PerformanceBorder = Math.Round(PlayerInfo?.PlayerStatistics.PerformanceBorderVsBot.Value ?? 0);
        }
        public async Task GetSelectedGameData(int GameId)
        {
            Game game = await GameProcesses.MakeSavedGame(serverWorker, GameId);
            CancelGame = await GameProcesses.StartGame(serverWorker, game);
            if (!CancelGame)
            {
                GameStateInfo gameStateInfo = await serverWorker.TaskGetGameState(game.IdGame);
                GameField = new GameField(serverWorker, systemSettings, game, gameStateInfo);             
            }
        }
        public async Task CreateGameWithBot()
        {           
            Game game = await GameProcesses.MakeGameWithBot(serverWorker);
            GameStateInfo gameStateInfo = await serverWorker.TaskGetGameState(game.IdGame);
            GameField = new GameField(serverWorker, systemSettings, game, gameStateInfo);
        }
        public void CreateRealPlayerChoicePage()
        {
            ChoiceRealPlayerPage = new ChoiceRealPlayerPage(serverWorker, systemSettings);
        }
        public void CreateStandingsPage()
        {
            StandingsPage = new StandingsPage(serverWorker, systemSettings);
        }
    }
}