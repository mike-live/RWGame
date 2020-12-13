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

namespace RWGame.ViewModels
{
    public class GameControls
    {
        SKBitmap[,] ControlsImages { get; set; } = new SKBitmap[2, 2];
        //public SKCanvasView[] canvasView { get; set; } = new SKCanvasView[2];
        Action MakeTurnAndWait { get; set; }
        public string InfoTurnLabelText { get; set; }
        public bool ChooseRow { get; set; }
        private Game Game { get; set; }
        private bool CanAnimate { get; set; } = true;
        public bool CanMakeTurn { get; set; } = true;
        SKCanvasView CanvasViewField { get; set; }
        public int ChosenTurn { get; set; } = -1;
        public int CurCanvasRow { get; set; }
        public int CurCanvasRowSpan { get; set; }
        public int CurCanvasColumn { get; set; }
        public int CurCanvasColumnSpan { get; set; }
        public float CurCanvasOpacity { get; set; }
        public Command OnCurCanvasTappedCommand { get; set; }
        public Command OnGameControlsAppearingCommand { get; set; }
        private int Id { get; set; }

        private Dictionary<string, string> ControlsImagesNames { get; set; } = new Dictionary<string, string> {
            { "U", "up" }, { "L", "left" }, { "D", "down" }, { "R", "right" }
        };

        public GameControls(Action MakeTurnAndWait, Game game, GameStateInfo gameStateInfo)
        {
            this.MakeTurnAndWait = MakeTurnAndWait;
            Game = game;
            ChooseRow = game.GameSettings.TurnControls[game.IdPlayer] == "row";

            OnGameControlsAppearingCommand = new Command(OnAppearance);
            OnCurCanvasTappedCommand = new Command(OnCurCanvasTapped);

            MakeGameControls();

            if (gameStateInfo.GameState == GameStateEnum.WAIT)
            {
                int idTurn = gameStateInfo.Turn[game.IdPlayer];
                if (idTurn != -1)
                {
                    MakeTurn(idTurn);
                }
            }
        }
        public void OnAppearance()
        {

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
        public void CurCanvasPaintSurface(SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            int controlSize = ChooseRow ? info.Height : info.Width;
            SKRect rect = ChooseRow ? SKRect.Create(controlSize / 2, 0, info.Width - controlSize, info.Height)
                                    : SKRect.Create(0, controlSize / 2, info.Width, info.Height - controlSize);
            SKPaint paint = new SKPaint { Color = SKColors.White, Style = SKPaintStyle.Fill };
            canvas.DrawRect(rect, paint);
            canvas.DrawCircle(new SKPoint(controlSize / 2, controlSize / 2), controlSize / 2, paint);
            if (ChooseRow)
            {
                canvas.DrawCircle(new SKPoint(info.Width - controlSize / 2, controlSize / 2), controlSize / 2, paint);
            }
            else
            {
                canvas.DrawCircle(new SKPoint(controlSize / 2, info.Height - controlSize / 2), controlSize / 2, paint);
            }
            if (ChooseRow)
            {
                SKBitmap control1 = ControlsImages[Id, 0];
                SKBitmap control2 = ControlsImages[Id, 1];
                MergeBitmaps(canvas, control1, control2, info.Width, info.Height, false);
            }
            else
            {
                SKBitmap control1 = ControlsImages[0, Id];
                SKBitmap control2 = ControlsImages[1, Id];
                MergeBitmaps(canvas, control1, control2, info.Width, info.Height, true);
            }
        }
        async void MakeTurn(int id)
        {
            if (!CanMakeTurn)
            {
                return;
            }
            CanMakeTurn = false;
            if (CanAnimate)
            {
                CanAnimate = false;

                //await curCanvas.FadeTo(1, 100);
                CanAnimate = true;
            }
            string turnName;
            ChosenTurn = id;
            if (ChooseRow)
            {
                turnName = ChosenTurn == 0 ? "upper row" : "bottom row";
            }
            else
            {
                turnName = ChosenTurn == 0 ? "left column" : "right column";
            }
            InfoTurnLabelText = "Wait...";
            CanvasViewField.InvalidateSurface();
            await Task.Delay(1000);
            MakeTurnAndWait();
        }
        public void OnCurCanvasTapped()
        {
            MakeTurn(Id);
        }
        void MakeGameControls()
        {
            for (int i = 0; i < 4; i++)
            {
                string dir = Game.GameSettings.Controls[i / 2][i % 2];
                ControlsImages[i / 2, i % 2] = SKBitmap.Decode(Helper.getResourceStream("Images." + ControlsImagesNames[dir] + ".png"));
            }
            for (int i = 0; i < 2; i++)
            {
                SKCanvasView curCanvas = new SKCanvasView();
                canvasView[i] = curCanvas;
                Id = i;

                if (ChooseRow)
                {
                    CurCanvasRow = i;
                    CurCanvasColumn = 0;
                    CurCanvasColumnSpan = 2;
                    CurCanvasRowSpan = 1;
                }
                else
                {
                    CurCanvasRow = 0;
                    CurCanvasColumn = i;
                    CurCanvasColumnSpan = 1;
                    CurCanvasRowSpan = 2;
                }

                CurCanvasOpacity = (float) 0.75;
            }
        }
    }
    public class GameFieldDisplayData
    {
        private GameFieldModel GameFieldModel { get; set; }
        public GameControls GameControls { get; set; }
        public SKColor BackgroundSKColor { get; set; } = SKColors.Transparent;
        public Color BackgroundColor { get; set; } = Color.Transparent;
        public SKCanvasView[] canvasView { get; set; } = new SKCanvasView[2];
        private Views.GameField GameField { get; set; }
        private Game Game
        {
            get { return GameFieldModel.Game; }
            set { GameFieldModel.Game = value; }
        }
        public GameStateInfo GameStateInfo
        {
            get { return GameFieldModel.GameStateInfo; }
            set { GameFieldModel.GameStateInfo = value; }
        }
        private double ScreenWidth
        {
            get { return Application.Current.MainPage.Width; }
        }
        private double ScreenHeight
        {
            get { return Application.Current.MainPage.Height; }
        }
        public Rectangle BackImageBounds
        {
            get { return new Rectangle(0, 0, ScreenWidth, 2 * ScreenWidth); }
        }
        public Rectangle StackLayoutBounds
        {
            get { return new Rectangle(0, 0, ScreenWidth, ScreenHeight); }
        }
        public INavigation Navigation { get; set; }
        public int GridSize { get; set; }
        public int MarginX { get; set; } = 100;
        public int MarginY { get; set; } = 100;
        public float CenterRadius { get; set; } = 300;
        public float CellSize { get; set; }
        public float ShiftX { get; set; } = 0;
        public int IdTurn { get; set; } = 0;
        public int NumTurns { get; set; } = 0;
        public bool NeedCheckState { get; set; } = true;
        public string GameScoreLabelText { get; set; }
        public string InfoTurnLabelText { get; set; }
        public float LayoutWidth { get; set; }
        public float LayoutHeight { get; set; }
        public string GameScoreImageSource
        {
            get { return "state_star_" + Game.GameSettings.Goals[Game.IdPlayer] + ".png"; }
        }
        public string GoalLabelText
        {
            get { return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Game.GameSettings.Goals[Game.IdPlayer]); }
        }
        public string TopScoreImageSource
        {
            get { return "top_score_" + Game.GameSettings.Goals[Game.IdPlayer] + ".png"; }
        }
        public string TopScoreLabelText
        {
            get
            {
                if (Game.GameSettings.Goals[Game.IdPlayer] == "center")
                {
                    return "546";
                }
                else if (Game.GameSettings.Goals[Game.IdPlayer] == "border")
                {
                    return "20";
                }
                else return "";
            }
        }
        public List<SKPoint> GameTrajectory { get; set; } = new List<SKPoint> { };


        public GameFieldDisplayData(Game game, GameStateInfo gameStateInfo, INavigation navigation)
        {
            GameFieldModel = new GameFieldModel();
            Game = game;
            Navigation = navigation;
            GameStateInfo = gameStateInfo;
            GameControls = new GameControls(MakeTurnAndWait, Game, GameStateInfo);
        }

        public void UpdateScore()
        {
            if (GameStateInfo.GameState != GameStateEnum.END)
            {
                if (Game.Turns.Count == 1)
                {
                    InfoTurnLabelText = "Make first turn!";
                }
                GameControls = new GameControls(MakeTurnAndWait, Game, GameStateInfo);
            }
            else
            {
                InfoTurnLabelText = "Moves history";
                NumTurns--;
            }
            GameScoreLabelText = NumTurns.ToString();
        }

        public void PaintSurface(SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            GridSize = Game.GameSettings.FieldWidth;
            CellSize = (float)(Math.Min(info.Width - MarginX, info.Height - MarginY)) / GridSize;
            CenterRadius = (float)info.Width / 5;
            ShiftX = (info.Width - MarginX - CellSize * GridSize) / 2;

            canvas.Clear(BackgroundSKColor);
            DrawField(canvas);
            DrawTrajectory(canvas);
        }
        SKPoint GetGridPoint(SKPoint p)
        {
            return GetGridPoint(p.X, p.Y);
        }
        SKPoint GetGridPoint(float x, float y)
        {
            return new SKPoint(MarginX / 2 + CellSize * x + ShiftX, MarginY / 2 + CellSize * y);
        }
        SKPoint MovePoint(SKPoint cur, string dir)
        {
            var dx = new Dictionary<string, int> { { "U", 0 }, { "D", 0 }, { "L", -1 }, { "R", +1 } };
            var dy = new Dictionary<string, int> { { "U", -1 }, { "D", +1 }, { "L", 0 }, { "R", 0 } };

            cur.X += dx[dir];
            cur.Y += dy[dir];
            return cur;
        }
        void DrawChoice(SKCanvas canvas)
        {
            if (GameControls is null) return;
            if (GameControls.ChosenTurn != -1)
            {
                int numDash = 5;
                float dashLength = 2 * (numDash - 1) + numDash;
                float[] dashArray = { CellSize / dashLength, 2 * CellSize / dashLength };
                SKPaint paint = new SKPaint
                {
                    Color = SKColors.White,//SKColor.Parse("#3949AB"),
                    Style = SKPaintStyle.StrokeAndFill,
                    PathEffect = SKPathEffect.CreateDash(dashArray, 20),
                    StrokeWidth = 8,
                    MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 1)
                };
                SKPoint cur = GameTrajectory[^1];

                string dir1, dir2;
                if (GameControls.ChooseRow)
                {
                    dir1 = Game.GameSettings.Controls[GameControls.ChosenTurn][0];
                    dir2 = Game.GameSettings.Controls[GameControls.ChosenTurn][1];
                }
                else
                {
                    dir1 = Game.GameSettings.Controls[0][GameControls.ChosenTurn];
                    dir2 = Game.GameSettings.Controls[1][GameControls.ChosenTurn];
                }

                canvas.DrawLine(GetGridPoint(cur), GetGridPoint(MovePoint(cur, dir1)), paint);
                canvas.DrawLine(GetGridPoint(cur), GetGridPoint(MovePoint(cur, dir2)), paint);
            }
        }
        void DrawField(SKCanvas canvas)
        {
            SKPoint gridCenter = GetGridPoint(GridSize / 2, GridSize / 2);

            SKPaint paint = new SKPaint
            {
                Shader = SKShader.CreateRadialGradient(
                            gridCenter,
                            CenterRadius,
                            new SKColor[] { SKColor.Parse("#3949AB").WithAlpha(127), BackgroundSKColor },
                            null,
                            0),
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 10)
            };

            canvas.DrawCircle(gridCenter, CenterRadius, paint);

            paint = new SKPaint
            {
                Color = SKColors.White,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1,
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 1)
            };
            for (int i = 0; i <= GridSize; i++)
            {
                canvas.DrawLine(GetGridPoint(i, 0), GetGridPoint(i, GridSize), paint);
                canvas.DrawLine(GetGridPoint(0, i), GetGridPoint(GridSize, i), paint);
            }
            paint.StrokeWidth = 5;


            if (Game.GameSettings.Goals[Game.IdPlayer] == "center")
            {
                paint.Color = SKColor.Parse("#99897d"); //ffd119 fca821 f36f24 ead3c3 d6b9a5 96755d bdaa9d
            }
            else if (Game.GameSettings.Goals[Game.IdPlayer] == "border")
            {
                paint.Color = SKColor.Parse("#9f18ff");
            }

            SKPoint p1 = GetGridPoint(0, 0), p2 = GetGridPoint(GridSize, GridSize);
            canvas.DrawRect(p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y, paint);

            paint = new SKPaint
            {
                Color = paint.Color.WithAlpha(32), // SKColor.Parse("#3949AB").WithAlpha(48)
                Style = SKPaintStyle.Fill,
                StrokeWidth = 1,
                //MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 5),
                ImageFilter = SKImageFilter.CreateBlur(20, 20),
            };
            canvas.DrawRect(p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y, paint);
        }
        void DrawTrajectory(SKCanvas canvas)
        {
            SKPaint paint = new SKPaint
            {
                Color = SKColor.Parse("#3949AB"), //.WithAlpha(128) 3949AB 84add1 3d7eb8
                Style = SKPaintStyle.StrokeAndFill,
                StrokeWidth = 4,
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 1)
            };
            for (int i = 1; i < GameTrajectory.Count; i++)
            {
                SKPoint curGridPoint = GameTrajectory[i], prvGridPoint = GameTrajectory[i - 1];
                SKPoint start = GetGridPoint(prvGridPoint), end = GetGridPoint(curGridPoint);
                float t_start = ((float)(i - 1)) / GameTrajectory.Count, t_end = ((float)i) / GameTrajectory.Count;
                canvas.DrawLine(start, end, paint);
            }
            paint.Color = SKColors.Red;
            paint.Shader = null;
            paint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 2);

            DrawChoice(canvas);
            float starSize = CellSize * 0.75f;
            var bitmap = SKBitmap.Decode(Helper.getResourceStream("Images.star.png"));
            SKImage image = SKImage.FromBitmap(bitmap);
            SKRect rect = new SKRect
            {
                Location = GetGridPoint(GameTrajectory[^1]) - new SKPoint(starSize, starSize),
                Size = new SKSize(starSize * 2, starSize * 2)
            };
            canvas.DrawImage(image, rect);
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
            GameFieldModel.MakeTurn(GameControls.ChosenTurn);
            while (GameStateInfo.GameState == GameStateEnum.WAIT)
            {
                if (!NeedCheckState)
                {
                    return;
                }
                await Task.Delay(1000);
                GameFieldModel.GetGameState();
            }
            UpdateState(GameStateInfo);
        }
        public async void UpdateState(GameStateInfo gameStateInfo)
        {
            GameTrajectory.Add(new SKPoint(gameStateInfo.PointState[0], gameStateInfo.PointState[1]));
            await GameControls.canvasView[GameControls.ChosenTurn].FadeTo(0.75, 25);
            GameControls.ChosenTurn = -1;
            IdTurn = gameStateInfo.LastIdTurn;
            NumTurns = IdTurn;
            GameScoreLabelText = NumTurns.ToString();

            //CanvasView.InvalidateSurface();
            if (gameStateInfo.GameState == GameStateEnum.END)
            {
                await App.Current.MainPage.DisplayAlert("Game finished", "You made " + NumTurns.ToString() + " turns!" + "\n" + "Thanks for playing ;)", "OK");
                await Navigation.PopAsync();
                return;
            }
            InfoTurnLabelText = "Make turn!";
            GameControls.CanMakeTurn = true;
        }

    }
    public class GameFieldViewModel
    {
        public GameFieldDisplayData GameFieldDisplayData { get; set; }
        public GameFieldViewModel(Game game, GameStateInfo gameStateInfo, INavigation navigation)
        {
            GameFieldDisplayData = new GameFieldDisplayData(game, gameStateInfo, navigation);
        }
    }
}
