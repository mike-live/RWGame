using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using RWGame.Helpers;

namespace RWGame.Models
{
    public class GameControlsModel : INotifyPropertyChanged
    {
        private readonly ServerWorker serverWorker;
        public readonly GameFieldModel gameFieldModel;

        public event PropertyChangedEventHandler PropertyChanged;
        public bool ChooseRow
        {
            get { return Game.GameSettings.TurnControls[Game.IdPlayer] == "row"; }
        }
        public int TurnDelayTime { get; set; } = 1000;
        public bool CanMakeTurn { get; set; } = true;
        public int ChosenTurn { get; set; } = -1;
        public Game Game { get { return gameFieldModel.Game; } }
        public GameStateInfo GameStateInfo { get { return gameFieldModel.GameStateInfo; } }
        public GameStateEnum GameState { get { return gameFieldModel.GameState; } }
        private bool NeedsCheckState { get; set; } = true;

        public (string, string) CurrentDirections {
            get
            {
                string dir1 = "", dir2 = "";
                if (ChosenTurn != -1)
                {
                    if (ChooseRow)
                    {
                        dir1 = Game.GameSettings.Controls[ChosenTurn][0];
                        dir2 = Game.GameSettings.Controls[ChosenTurn][1];
                    }
                    else
                    {
                        dir1 = Game.GameSettings.Controls[0][ChosenTurn];
                        dir2 = Game.GameSettings.Controls[1][ChosenTurn];
                    }
                }
                return (dir1, dir2);
            }
        }
        public List<List<string>> Controls { get { return Game.GameSettings.Controls; } }

        public GameControlsModel(GameFieldModel gameFieldModel)
        {
            this.gameFieldModel = gameFieldModel;
            serverWorker = ServerWorker.GetServerWorker();
            _ = Prepare();
            gameFieldModel.PropertyChanged += (obj, args) => {
                if (args.PropertyName == "GameState")
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GameState"));
                }
            };
        }

        public async Task Prepare()
        {
            if (GameState == GameStateEnum.WAIT)
            {
                int idTurn = GameStateInfo.Turn[Game.IdPlayer];
                if (idTurn != -1)
                {
                    await MakeTurn(idTurn);
                }
            }
        }
        public async Task MakeTurn(int chosenTurn)
        {
            if (!CanMakeTurn)
            {
                return;
            }
            CanMakeTurn = false;
            ChosenTurn = chosenTurn;
            gameFieldModel.GameState = GameStateEnum.WAIT;
            await Task.Delay(TurnDelayTime);
            await MakeTurnAndWait();
            ChosenTurn = -1;
            CanMakeTurn = true;
        }
        public async Task MakeTurnAndWait()
        {
            gameFieldModel.GameStateInfo = await serverWorker.TaskMakeTurn(Game.IdGame, ChosenTurn);
            while (GameState == GameStateEnum.WAIT)
            {
                if (!NeedsCheckState)
                {
                    return;
                }
                await Task.Delay(1000);
                await gameFieldModel.UpdateGameState();
            }
            gameFieldModel.AddToTrajectory();
        }
        public void StopWaitTurn()
        {
            NeedsCheckState = false;
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
        public GameStateEnum GameState { 
            get { return GameStateInfo.GameState; } 
            set { GameStateInfo.GameState = value; } 
        }
        public int NumTurns
        {
            get { return GameStateInfo.Score ?? 0; }
        } 
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
            get { return GameState == GameStateEnum.END; }
        }
        public TurnStateEnum TurnState
        {
            get
            {
                if (GameStateInfo.LastIdTurn == 0)
                {
                    return TurnStateEnum.FIRST_TURN;
                }
                else if (GameState == GameStateEnum.WAIT || !CanMakeTurn)
                {
                    return TurnStateEnum.WAIT;
                }
                else if (NumTurns > 0 && GameState != GameStateEnum.WAIT && GameState != GameStateEnum.END)
                {
                    return TurnStateEnum.TURN;
                }
                else if (GameState == GameStateEnum.END)
                {
                    return TurnStateEnum.END;
                }
                else
                {
                    return TurnStateEnum.NONE;
                }
            }
        }
        public GameFieldModel(Game game, GameStateInfo gameStateInfo)
        {
            serverWorker = ServerWorker.GetServerWorker();
            this.Game = game;
            this.GameStateInfo = gameStateInfo;

            FillGameTrajectory();
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
