using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RWGame.Models
{
    partial class ElementsOfViewCell 
    {
        public Game game { get; set; }
    }
    class UserModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ServerWorker ServerWorker;
        private SystemSettings SystemSettings;
        public UserModel(ServerWorker ServerWorker, SystemSettings SystemSettings)
        {
            this.ServerWorker = ServerWorker;
            this.SystemSettings = SystemSettings;
            IsGameStarted = false;
        }
        public Game Game { get; set; }
        public PlayerInfo PlayerInfo { get; set; }
        public List<Game> GamesList { get; set; }
        public bool IsGameStarted { get; set; }
        public async Task UpdateGameList()
        {
            GamesList = await ServerWorker.TaskGetGamesList();
        }
        public async Task UpdatePlayerInfo()
        {
            PlayerInfo = await ServerWorker.TaskGetPlayerInfo();
        }
        public async Task ExecuteItemSelectedLogic(INavigation Navigation, int GameId)
        {
            Game game = await GameProcesses.MakeSavedGame(ServerWorker, GameId);

            bool cancelGame = await GameProcesses.StartGame(ServerWorker, game);

            if (!cancelGame)
            {
                GameStateInfo gameStateInfo = await ServerWorker.TaskGetGameState(game.IdGame);
                await Navigation.PushAsync(new GameField(ServerWorker, SystemSettings, game, gameStateInfo));
                IsGameStarted = true;
            }
            
        }
    }
}
