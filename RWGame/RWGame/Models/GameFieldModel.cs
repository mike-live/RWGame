using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using RWGame.Helpers;
using PropertyChanged;
using System;

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

    public class Point
    {
        private int x;
        private int y;

        public int X
        {
            get { return x; }
            set { x = value; }
        }
        public int Y
        {
            get { return y; }
            set { y = value; }
        }
        
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    public class GameFieldModel : INotifyPropertyChanged
    {
        private readonly ServerWorker serverWorker;

        public event PropertyChangedEventHandler PropertyChanged;

        Action UpdateFieldState;

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
        public List<Point> GameTrajectory { get; set; } = new List<Point> { };
        public string GameTopScoreLabelText
        {
            get { return (GameGoal == "center") ? "546" : "20"; }
        }
        public string GameGoal
        {
            get { return Game.GameSettings.Goals[Game.IdPlayer]; }
        }
        public bool CanMakeTurn { get; set; } = true;
        public bool IsFinished
        {
            get { return GameStateInfo.GameState == GameStateEnum.END; }
        }
        public InfoStringsEnum TurnState
        {
            get
            {
                if (NumTurns == 0)
                {
                    return InfoStringsEnum.FIRST_TURN;
                }
                else if (GameStateInfo.GameState == GameStateEnum.WAIT || !CanMakeTurn)
                {
                    return InfoStringsEnum.WAIT;
                }
                else if (NumTurns > 0 && GameStateInfo.GameState != GameStateEnum.WAIT && GameStateInfo.GameState != GameStateEnum.END)
                {
                    return InfoStringsEnum.TURN;
                }
                else if (GameStateInfo.GameState == GameStateEnum.END)
                {
                    return InfoStringsEnum.END;
                }
                else
                {
                    return InfoStringsEnum.NONE;
                }
            }
            set { }
        }
        public GameFieldModel(Action UpdateFieldState, Game game, GameStateInfo gameStateInfo)
        {
            serverWorker = ServerWorker.GetServerWorker();
            this.Game = game;
            this.GameStateInfo = gameStateInfo;
            this.UpdateFieldState = UpdateFieldState;

            FillGameTrajectory();
        }
        public async Task MakeTurn(int chosenTurn)
        {
            GameStateInfo = await serverWorker.TaskMakeTurn(Game.IdGame, chosenTurn);
            while (GameStateInfo.GameState == GameStateEnum.WAIT)
            {
                if (!NeedsCheckState)
                {
                    return;
                }
                await Task.Delay(1000);
                await UpdateGameState();
            }
            UpdateFieldState();
            AddToTrajectory();
        }
        public async Task UpdateGameState()
        {
            GameStateInfo = await serverWorker.TaskGetGameState(Game.IdGame);
        }
        public void FillGameTrajectory()
        {
            foreach (TurnInfo turn in Game.Turns)
            {
                GameTrajectory.Add(new Point(turn.State[0], turn.State[1]));
            }
        }
        public void AddToTrajectory()
        {
            GameTrajectory.Add(new Point(GameStateInfo.PointState[0], GameStateInfo.PointState[1]));
        }
    }
}
