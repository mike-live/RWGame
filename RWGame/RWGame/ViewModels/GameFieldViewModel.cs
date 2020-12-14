using System;
using System.Globalization;
using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using Xamarin.Forms;
using RWGame.Models;
using System.Collections.Generic;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Threading.Tasks;
using System.Linq;

namespace RWGame.ViewModels
{
    public partial class GameControlsViewModel
    {
        public int ChosenTurn { get; set; }
        public bool ChooseRow { get; set; }
        public bool CanMakeTurn { get; set; }
        public bool CanAnimate { get; set; }
        public Game Game { get; set; }
        public GameStateInfo GameStateInfo { get; set; }
        public string Direction { get; set; }
        public GameControlsViewModel(Game game, GameStateInfo gameStateInfo)
        {
            Game = game;
            GameStateInfo = gameStateInfo;
        }
        public void EvaluateChooseRow()
        {
            ChooseRow = Game.GameSettings.TurnControls[Game.IdPlayer] == "row";
        }
        public void MergeBitmaps(SKCanvas canvas, SKBitmap bitmap1, SKBitmap bitmap2, int width, int height,
            bool vertical = true)
        {
            SKRect rect1, rect2;
            if (vertical)
            {
                rect1 = SKRect.Create(0, 0, width, width);
                rect2 = SKRect.Create(0, height - width, width, width);
            }
            else
            {
                rect1 = SKRect.Create(0, 0, height, height);
                rect2 = SKRect.Create(width - height, 0, height, height);
            }

            canvas.DrawBitmap(bitmap1, rect1);
            canvas.DrawBitmap(bitmap2, rect2);
        }
        public void GetDirection(int i)
        {
           Direction = Game.GameSettings.Controls[i / 2][i % 2];
        }
    }
    public class GameFieldViewModel
    {
        GameFieldModel GameFieldModel { get; set; }
        Game Game { get; set; }
        GameStateInfo GameStateInfo 
        { 
            get { return GameFieldModel.GameStateInfo; } 
            set { GameFieldModel.GameStateInfo = value; }
        }
        public GameControlsViewModel GameControlsViewModel { get; set; }
        INavigation Navigation { get; set; }
        public bool NeedsCheckState { get; set; } = true;
        public List<SKPoint> GameTrajectory { get; set; } = new List<SKPoint> { };
        public int NumTurns { get; set; }
        public int GridSize { get; set; }
        public float CellSize { get; set; }
        public int MarginX { get; set; } = 100;
        public int MarginY { get; set; } = 100;
        public float CenterRadius { get; set; } = 300;
        public float ShiftX { get; set; } = 0;
        public string Direction1 { get; set; }
        public string Direction2 { get; set; }
        public string GameScoreLabelText { get; set; }
        public string InfoTurnLabelText { get; set; }
        public SKColor BorderColor
        {
            get
            {
                return (Game.GameSettings.Goals[Game.IdPlayer] == "center") ?
                    SKColor.Parse("#99897d") : // Color for center player
                    SKColor.Parse("#9f18ff");  // Color for border player
            }
        }
        public SKPoint GridCenter
        {
            get { return GetGridPoint(GridSize / 2, GridSize / 2); }
        }
        public GameFieldViewModel(Game game, GameStateInfo gameStateInfo, INavigation navigation)
        {
            GameFieldModel = new GameFieldModel();
            Game = game;
            GameStateInfo = gameStateInfo;
            Navigation = navigation;
            NumTurns = Game.Turns.Count - 1;
        }

        public void FillGameTrajectory()
        {
            foreach (TurnInfo turn in Game.Turns)
            {
                GameTrajectory.Add(new SKPoint(turn.State[0], turn.State[1]));
            }
        }

        public void AdjustSurface(SKImageInfo info)
        {
            GridSize = Game.GameSettings.FieldWidth;
            CellSize = (float)(Math.Min(info.Width - MarginX, info.Height - MarginY)) / GridSize;
            CenterRadius = (float)info.Width / 5;
            ShiftX = (info.Width - MarginX - CellSize * GridSize) / 2;
        }
        public SKPoint GetGridPoint(float x, float y)
        {
            return new SKPoint(MarginX / 2 + CellSize * x + ShiftX, MarginY / 2 + CellSize * y);
        }
        public SKPoint GetTrajectoryPoint(int turn)
        {
            return GameTrajectory[turn];
        }
        public string[] GetDirections()
        {
            if (GameControlsViewModel.ChooseRow)
            {
                Direction1 = Game.GameSettings.Controls[GameControlsViewModel.ChosenTurn][0];
                Direction2 = Game.GameSettings.Controls[GameControlsViewModel.ChosenTurn][1];
            }
            else
            {
                Direction1 = Game.GameSettings.Controls[0][GameControlsViewModel.ChosenTurn];
                Direction2 = Game.GameSettings.Controls[1][GameControlsViewModel.ChosenTurn];
            }
            return new string[] { Direction1, Direction2 };
        }
        public SKImage MergeBitmaps(SKBitmap bitmap1, SKBitmap bitmap2, bool vertical = true)
        {
            SKImage result;
            int width, height;
            if (vertical)
            {
                width = Math.Max(bitmap1.Width, bitmap2.Width);
                height = bitmap1.Height + bitmap2.Height;
            }
            else
            {
                height = Math.Max(bitmap1.Height, bitmap2.Height);
                width = bitmap1.Width + bitmap2.Width;
            }
            using (var tempSurface = SKSurface.Create(new SKImageInfo(width, height)))
            {
                var canvas = tempSurface.Canvas;
                SKRect rect1, rect2;
                if (vertical)
                {
                    rect1 = SKRect.Create(0, 0, bitmap1.Width, bitmap1.Height);
                    rect2 = SKRect.Create(0, bitmap1.Height, bitmap2.Width, bitmap2.Height);
                }
                else
                {
                    rect1 = SKRect.Create(0, 0, bitmap1.Width, bitmap1.Height);
                    rect2 = SKRect.Create(bitmap1.Width, 0, bitmap2.Width, bitmap2.Height);
                }

                canvas.DrawBitmap(bitmap1, rect1);
                canvas.DrawBitmap(bitmap2, rect2);
                result = tempSurface.Snapshot();
            }
            return result;
        }
        public async void MakeTurnAndWait()
        {
            GameFieldModel.MakeTurn(GameControlsViewModel.ChosenTurn);
            while (GameStateInfo.GameState == GameStateEnum.WAIT)
            {
                if (!NeedsCheckState)
                {
                    return;
                }
                await Task.Delay(1000);
                GameFieldModel.UpdateGameState();
            }
            UpdateState();
        }
        public async void UpdateState()
        {
            GameTrajectory.Add(new SKPoint(GameStateInfo.PointState[0], GameStateInfo.PointState[1]));
            //await GameControls.canvasView[GameControls.chosenTurn].FadeTo(0.75, 25);
            GameControlsViewModel.ChosenTurn = -1;   
            NumTurns = GameStateInfo.LastIdTurn;
            GameScoreLabelText = NumTurns.ToString();

            //canvasView.InvalidateSurface();
            if (GameStateInfo.GameState == GameStateEnum.END)
            {
                await App.Current.MainPage.DisplayAlert("Game finished", "You made " + NumTurns.ToString() + " turns!" + "\n" + "Thanks for playing ;)", "OK");
                await Navigation.PopAsync();
                return;
            }
            InfoTurnLabelText = "Make turn!";
            GameControlsViewModel.CanMakeTurn = true;
        }
    }
}
