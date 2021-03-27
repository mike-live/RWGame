using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using System.Threading.Tasks;
using SkiaSharp;
using System.Collections.Generic;
using System.ComponentModel;
using RWGame.Helpers;
using PropertyChanged;

namespace RWGame.Models
{
    public class GameControlsModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public bool ChooseRow
        {
            get { return Game.GameSettings.TurnControls[Game.IdPlayer] == "row";  }
            set { }
        }
        public bool CanAnimate { get; set; } = true;
        public bool CanMakeTurn { get; set; } = true;
        public int ChosenTurn { get; set; } = -1;
        public Game Game { get; set; }
        public GameStateInfo GameStateInfo { get; set; }

        public GameControlsModel()
        {

        }
    }
    public class GameFieldModel : INotifyPropertyChanged
    {
        private readonly ServerWorker serverWorker;

        public event PropertyChangedEventHandler PropertyChanged;

        public Game Game
        { 
            get; 
            set; 
        }
        public GameStateInfo GameStateInfo 
        {
            get;
            set;
        }
        public int NumTurns
        {
            get { return GameStateInfo.LastIdTurn; }
        } 
        public bool NeedsCheckState { get; set; } = true;             
        public List<SKPoint> GameTrajectory { get; set; } = new List<SKPoint> { };
        public string GameTopScoreLabelText
        {
            get { return (GameGoal == "center") ? "546" : "20"; }
        }
        public string GameGoal
        {
            get { return Game.GameSettings.Goals[Game.IdPlayer]; }
        }
        public bool IsFinished
        {
            get { return GameStateInfo.GameState == GameStateEnum.END; }
        }
        public InfoStringsEnum InfoTurnLabelText
        {
            get
            {
                 if (Game.Turns.Count == 1)
                 {
                     return InfoStringsEnum.MAKE_FIRST_TURN;
                 }
                 else if (GameStateInfo.GameState == GameStateEnum.WAIT)
                 {
                     return InfoStringsEnum.WAIT;
                 }
                 else if (Game.Turns.Count > 1 && GameStateInfo.GameState != GameStateEnum.WAIT && GameStateInfo.GameState != GameStateEnum.END)
                 {
                     return InfoStringsEnum.MAKE_TURN;
                 }
                 else if (GameStateInfo.GameState == GameStateEnum.END)
                 {
                     return InfoStringsEnum.MOVES_HISTORY;
                 }
                 else
                 {
                     return InfoStringsEnum.NONE;
                 }
            }
            set { }
        }
        public GameFieldModel()
        {
            serverWorker = ServerWorker.GetServerWorker();
        }
        public async Task MakeTurn(int chosenTurn)
        {
            GameStateInfo = await serverWorker.TaskMakeTurn(Game.IdGame, chosenTurn);
            AddToTrajectory();
            while (GameStateInfo.GameState == GameStateEnum.WAIT)
            {
                if (!NeedsCheckState)
                {
                    return;
                }
                await Task.Delay(1000);
                await UpdateGameState();
            }
        }
        public async Task UpdateGameState()
        {
            GameStateInfo = await serverWorker.TaskGetGameState(Game.IdGame);
        }
        public void FillGameTrajectory()
        {
            foreach (TurnInfo turn in Game.Turns)
            {
                GameTrajectory.Add(new SKPoint(turn.State[0], turn.State[1]));
            }
        }
        public void AddToTrajectory()
        {
            GameTrajectory.Add(new SKPoint(GameStateInfo.PointState[0], GameStateInfo.PointState[1]));
        }
    }
}
