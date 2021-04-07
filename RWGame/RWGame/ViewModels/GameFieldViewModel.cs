using System;
using RWGame.Classes.ResponseClases;
using Xamarin.Forms;
using RWGame.Models;
using System.Collections.Generic;
using SkiaSharp;
using System.Threading.Tasks;
using System.ComponentModel;
using RWGame.Helpers;
using RWGame.Views;

namespace RWGame.ViewModels
{   public class GameFieldViewModel : INotifyPropertyChanged
    {
        public Command GoBackCommand { get; set; }
        public GameFieldModel GameFieldModel { get; set; }

        #region ScreenSettings
        public double ScreenWidth
        {
            get { return Application.Current.MainPage.Width; }
        }
        public double ScreenHeight
        {
            get { return Application.Current.MainPage.Height; }
        }
        #endregion
        public string BackImageSource
        {
            get { return "seashore2.png"; }
        }
        public Rectangle BackImageBounds
        {
            get { return new Rectangle(0, 0, ScreenWidth, ScreenWidth * 2); }
        }
        public Color BackgroundColor { get; set; } = Color.Transparent;
        public Rectangle StackLayoutBounds
        {
            get { return new Rectangle(0, 0, ScreenWidth, ScreenHeight); }
        }
        public string GameGoal
        {
            get { return GameFieldModel.GameGoal; }
        }
        public string GameScoreImageSource
        {
            get { return "state_star_" + GameGoal + ".png"; }
        }
        public string GoalLabelText 
        { 
            get { return GameGoal == "center" ? "Center" : "Border"; } 
        }
        public string GameTopScoreImageSource
        {
            get { return "top_score_" + GameGoal + ".png"; }
        }
        public string GameTopScoreLabelText
        {
            get { return GameFieldModel.GameTopScoreLabelText; }
        }
        public double CanvasViewHeightRequest
        {
            get { return ScreenWidth - 10; }
        }
        public double CanvasViewWidthRequest
        {
            get { return ScreenWidth; }
        }
        public string GameScoreLabelText
        {
            get { return GameFieldModel.NumTurns.ToString(); }
        }
        public string InfoTurnLabelText
        {
            get { return GameFieldModel.TurnState.ToDescriptionString(); }
        }
        public Game Game
        {
            get { return GameFieldModel.Game; }
        }
        INavigation Navigation { get; set; }
        public List<SKPoint> GameTrajectory
        {
            get
            {
                List<SKPoint> temp = new List<SKPoint>();
                foreach (Models.Point pnt in GameFieldModel.GameTrajectory)
                {
                    temp.Add(new SKPoint(pnt.X, pnt.Y));
                }
                return temp;
            }
        }
        public int NumTurns
        {
            get { return GameFieldModel.NumTurns; }
        }
        public int GridSize { get; set; }
        public float CellSize { get; set; }
        public int MarginX { get; set; } = 100;
        public int MarginY { get; set; } = 100;
        public float CenterRadius { get; set; } = 300;
        public float ShiftX { get; set; } = 0;
        public SKColor BorderColor
        {
            get
            {
                return (GameFieldModel.GameGoal == "center") ?
                    SKColor.Parse("#99897d") : // Color for center player
                    SKColor.Parse("#9f18ff");  // Color for border player
            }
        }
        public SKPoint GridCenter
        {
            get { return GetGridPoint(GridSize / 2, GridSize / 2); }
        }
        public bool IsFinished
        {
            get { return GameFieldModel.IsFinished; }
        }

        public GameFieldViewModel(Game game, GameStateInfo gameStateInfo, INavigation navigation)
        {
            GameFieldModel = new GameFieldModel(game, gameStateInfo);
            Navigation = navigation;
            GoBackCommand = new Command(GoBack);
            GameFieldModel.PropertyChanged += (obj, args) => {
                if (args.PropertyName == "NumTurns")
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NumTurns"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GameScoreLabelText"));
                } else
                if (args.PropertyName == "TurnState")
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InfoTurnLabelText"));
                } else
                if (args.PropertyName == "IsFinished")
                {
                    if (IsFinished)
                    {
                        FinishGame();
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsFinished"));
                }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async void GoBack()
        {
            await Navigation.PopAsync();
        }

        public void AdjustSurface(SKImageInfo info)
        {
            GridSize = Game.GameSettings.FieldWidth;
            CellSize = (float)(Math.Min(info.Width - MarginX, info.Height - MarginY)) / GridSize;
            CenterRadius = (float)info.Width / 5;
            ShiftX = (info.Width - MarginX - CellSize * GridSize) / 2;
        }

        #region PointMethods
        public SKPoint GetGridPoint(float x, float y)
        {
            return new SKPoint(MarginX / 2 + CellSize * x + ShiftX, MarginY / 2 + CellSize * y);
        }
        public SKPoint GetGridPoint(SKPoint p)
        {
            return GetGridPoint(p.X, p.Y);
        }
        public SKPoint MovePoint(SKPoint cur, string dir)
        {
            var dx = new Dictionary<string, int> { { "U", 0 }, { "D", 0 }, { "L", -1 }, { "R", +1 } };
            var dy = new Dictionary<string, int> { { "U", -1 }, { "D", +1 }, { "L", 0 }, { "R", 0 } };

            cur.X += dx[dir];
            cur.Y += dy[dir];
            return cur;
        }
        #endregion

        public async void FinishGame()
        {
            await App.Current.MainPage.DisplayAlert("Game finished", "You made " + NumTurns.ToString() + " turns!" + "\n" + "Thanks for playing ;)", "OK");
            await Navigation.PopAsync();
        }
    }

    public class GameControlsViewModel : INotifyPropertyChanged
    {
        private readonly GameControlsModel GameControlsModel;
        public Action StartTurn { get; set; }
        public Action FinishTurn { get; set; }

        public bool ChooseRow
        {
            get { return GameControlsModel.ChooseRow; }
        }
        public int ChosenTurn
        {
            get { return GameControlsModel.ChosenTurn; }
        }
        public Game Game
        {
            get { return GameControlsModel.Game; }
        }
        public TurnStateEnum TurnState
        {
            get { return GameControlsModel.TurnState; }
        }
        public (string, string) CurrentDirections
        {
            get { return GameControlsModel.CurrentDirections; }
        }
        public List<List<string>> Controls
        {
            get { return GameControlsModel.Controls; }
        }
        public Dictionary<string, string> ControlsImagesNames { get; set; } = new Dictionary<string, string> {
            { "U", "up" }, { "L", "left" }, { "D", "down" }, { "R", "right" }
        };
        public double ScreenWidth
        {
            get { return Application.Current.MainPage.Width; }
        }
        public double ScreenHeight
        {
            get { return Application.Current.MainPage.Height; }
        }

        public void OnTurnStateChanged()
        {
            if (TurnState == TurnStateEnum.WAIT)
            {
                StartTurn();
            }
            else if (TurnState == TurnStateEnum.TURN || TurnState == TurnStateEnum.END)
            {
                FinishTurn();
            }
        }

        public void StopWaitTurn()
        {
            GameControlsModel.StopWaitTurn();
        }

        public GameControlsViewModel(GameFieldViewModel gameFieldViewModel, Action StartTurn, Action FinishTurn)
        {
            this.StartTurn = StartTurn;
            this.FinishTurn = FinishTurn;
            GameControlsModel = new GameControlsModel(gameFieldViewModel.GameFieldModel);
            GameControlsModel.PropertyChanged += (obj, args) => {
                if (args.PropertyName == "TurnState")
                {
                    OnTurnStateChanged();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TurnState"));
                }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task MakeTurn(int ChosenTurn)
        {
            await GameControlsModel.MakeTurn(ChosenTurn);
        }
    }

}
