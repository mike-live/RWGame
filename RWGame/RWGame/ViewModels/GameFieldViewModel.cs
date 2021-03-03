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

    public class GameFieldViewModel
    {
        GameFieldModel GameFieldModel { get; set; }
        Game Game 
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
        public bool NeedsCheckState { get; set; } = true;
        public List<SKPoint> GameTrajectory { get; set; } = new List<SKPoint> { };
        public int NumTurns { get; set; }
        public int GridSize { get; set; }
        public float CellSize { get; set; }
        public int MarginX { get; set; } = 100;
        public int MarginY { get; set; } = 100;
        public float CenterRadius { get; set; } = 300;
        public float ShiftX { get; set; } = 0;      
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
        
        public void AddToTrajectory()
        {
            GameTrajectory.Add(new SKPoint(GameStateInfo.PointState[0], GameStateInfo.PointState[1]));
        }

        public async Task MakeTurnAndWait(int ChosenTurn)
        {
            await GameFieldModel.MakeTurn(ChosenTurn);
            while (GameStateInfo.GameState == GameStateEnum.WAIT)
            {
                if (!NeedsCheckState)
                {
                    return;
                }
                await Task.Delay(1000);
                GameFieldModel.UpdateGameState();
            }            
        }    
    }
}
