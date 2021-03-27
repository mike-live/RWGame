﻿using System;
using RWGame.Classes.ResponseClases;
using Xamarin.Forms;
using RWGame.Models;
using System.Collections.Generic;
using SkiaSharp;
using System.Threading.Tasks;
using System.ComponentModel;
using RWGame.Helpers;

namespace RWGame.ViewModels
{
    public class GameControlsViewModel : INotifyPropertyChanged
    {
        public GameControlsModel GameControlsModel;
        public Action<int> MakeTurnView;
        public Func<Task> MakeTurnAndWait;
        public Action FadeChosenTurn;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool ChooseRow
        {
            get { return GameControlsModel.ChooseRow; }
            set { GameControlsModel.ChooseRow = value; }
        }
        public bool CanAnimate
        {
            get { return GameControlsModel.CanAnimate; }
            set { GameControlsModel.CanAnimate = value; }
        }
        public bool CanMakeTurn
        {
            get { return GameControlsModel.CanMakeTurn; }
            set { GameControlsModel.CanMakeTurn = value; }
        }
        public int ChosenTurn
        {
            get { return GameControlsModel.ChosenTurn; }
            set { GameControlsModel.ChosenTurn = value; }
        }
        public Game Game
        {
            get { return GameControlsModel.Game; }
            set { GameControlsModel.Game = value; }
        }
        public GameStateInfo GameStateInfo
        {
            get { return GameControlsModel.GameStateInfo; }
            set { GameControlsModel.GameStateInfo = value; }
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

        public GameControlsViewModel(Game game, GameStateInfo gameStateInfo, Action<int> MakeTurnView, Func<Task> MakeTurnAndWait, Action FadeChosenTurn)
        {
            GameControlsModel = new GameControlsModel();
            Game = game;
            GameStateInfo = gameStateInfo;
            this.MakeTurnView = MakeTurnView;
            this.MakeTurnAndWait = MakeTurnAndWait;
            this.FadeChosenTurn = FadeChosenTurn;
        }

        public async Task MakeTurn(int id)
        {
            if (!CanMakeTurn)
            {
                return;
            }
            CanMakeTurn = false;
            ChosenTurn = id;
            MakeTurnView(id);
            await Task.Delay(1000);

            await MakeTurnAndWait();

            FadeChosenTurn();

            ChosenTurn = -1;
            CanMakeTurn = true;
        }

    }
    public class GameFieldViewModel : INotifyPropertyChanged
    {
        public GameFieldModel GameFieldModel { get; set; }
        Action UpdateField;
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
        public string GoalLabelText { get; set; } = "";
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
            get { return NumTurns.ToString(); }
        }
        public string InfoTurnLabelText
        {
            get { return GameFieldModel.InfoTurnLabelText.ToDescriptionString(); }
        }
        public Game Game 
        {
            get { return GameFieldModel.Game; }
            set { GameFieldModel.Game = value; }
        }
        public GameStateInfo GameStateInfo 
        { 
            get { return GameFieldModel.GameStateInfo; } 
            set { GameFieldModel.GameStateInfo = value; }
        }
        INavigation Navigation { get; set; }
        public bool NeedsCheckState
        {
            get { return GameFieldModel.NeedsCheckState; }
            set { GameFieldModel.NeedsCheckState = value; }
        }
        public List<SKPoint> GameTrajectory
        {
            get { return GameFieldModel.GameTrajectory; }
            set { GameFieldModel.GameTrajectory = value; }
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
        public bool IsFinished
        {
            get { return GameFieldModel.IsFinished; }
        }
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
        public GameFieldViewModel(Game game, GameStateInfo gameStateInfo, INavigation navigation, Action UpdateField)
        {
            GameFieldModel = new GameFieldModel();
            Game = game;
            GameStateInfo = gameStateInfo;
            Navigation = navigation;          
            this.UpdateField = UpdateField;
            GameFieldModel.FillGameTrajectory();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void AdjustSurface(SKImageInfo info)
        {
            GridSize = Game.GameSettings.FieldWidth;
            CellSize = (float)(Math.Min(info.Width - MarginX, info.Height - MarginY)) / GridSize;
            CenterRadius = (float)info.Width / 5;
            ShiftX = (info.Width - MarginX - CellSize * GridSize) / 2;
        }

        public async void CheckEnd()
        {
            if (IsFinished)
            {
                await App.Current.MainPage.DisplayAlert("Game finished", "You made " + NumTurns.ToString() + " turns!" + "\n" + "Thanks for playing ;)", "OK");
                await Navigation.PopAsync();
                return;
            }           
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

        public async Task MakeTurnAndWait(int ChosenTurn)
        {
            await GameFieldModel.MakeTurn(ChosenTurn);
            UpdateState();
        }    

        public void UpdateState()
        {
            UpdateField();
            CheckEnd();
        }
    }
}
