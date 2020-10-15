using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RWGame.Models
{
    public class UserModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private ServerWorker serverWorker;
        private SystemSettings systemSettings;
        public UserModel(ServerWorker serverWorker, SystemSettings systemSettings)
        {
            this.serverWorker = serverWorker;
            this.systemSettings = systemSettings;
            UpdateModel();
            IsGameStarted = false;
        }
        public PlayerInfo PlayerInfo { get; set; }
        public List<Game> GamesList { get; set; }
        public bool IsGameStarted { get; set; }
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
        public async Task ExecuteItemSelectedLogic(INavigation Navigation, int GameId)
        {
            Game game = await GameProcesses.MakeSavedGame(serverWorker, GameId);
            bool cancelGame = await GameProcesses.StartGame(serverWorker, game);
            if (!cancelGame)
            {
                GameStateInfo gameStateInfo = await serverWorker.TaskGetGameState(game.IdGame);
                await Navigation.PushAsync(new GameField(serverWorker, systemSettings, game, gameStateInfo));
                IsGameStarted = true;
            }
        }
    }
}
