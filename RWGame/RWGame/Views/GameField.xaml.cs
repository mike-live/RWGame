using RWGame.Classes;
using RWGame.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RWGame.Classes.ResponseClases;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace RWGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GameControls
    {
        public GameControlsViewModel ViewModel;
        private Color BackgroundColor { get; set; } = Color.Transparent;
        SKBitmap[,] ControlsImages { get; set; } = new SKBitmap[2, 2];
        public Grid ControlsGrid { get; set; }
        public SKCanvasView[] CanvasView { get; set; } = new SKCanvasView[2];
        Label InfoTurnLabel { get; set; }
        Action MakeTurnAndWait { get; set; }
        SKCanvasView CanvasViewField { get; set; }

        public GameControls(Action MakeTurnAndWait, Label InfoTurnLabel, Game game, GameStateInfo gameStateInfo, Color backgroundColor, SKCanvasView canvasViewField)
        {
            this.MakeTurnAndWait = MakeTurnAndWait;
            this.InfoTurnLabel = InfoTurnLabel;
            ViewModel = new GameControlsViewModel(game, gameStateInfo);

            BackgroundColor = backgroundColor;
            CanvasViewField = canvasViewField;

            MakeGameControl();
            CanvasView[0].PaintSurface += (sender, args) => OnCanvasViewPaintSurface(sender, args, 0);
            CanvasView[1].PaintSurface += (sender, args) => OnCanvasViewPaintSurface(sender, args, 1);

            CanvasView[0].InvalidateSurface();
            CanvasView[1].InvalidateSurface();

            if (ViewModel.GameStateInfo.GameState == GameStateEnum.WAIT)
            {
                int idTurn = ViewModel.GameStateInfo.Turn[ViewModel.Game.IdPlayer];
                if (idTurn != -1)
                {
                    MakeTurn(idTurn);
                }
            }
        }

        void MakeGameControl()
        {
            ControlsGrid = new Grid
            {
                Margin = new Thickness(10, 10, 10, 10),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.StartAndExpand,
                BackgroundColor = BackgroundColor,
                HeightRequest = ViewModel.ScreenWidth / 2.5,
                WidthRequest = ViewModel.ScreenWidth / 2.5,
                RowSpacing = 6,
                ColumnSpacing = 6,
            };

            ControlsGrid.RowDefinitions = new RowDefinitionCollection {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                };
            ControlsGrid.ColumnDefinitions = new ColumnDefinitionCollection {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                };
            for (int i = 0; i < 4; i++)
            {
                string dir = ViewModel.Game.GameSettings.Controls[i / 2][i % 2];
                ControlsImages[i / 2, i % 2] = SKBitmap.Decode(Helper.getResourceStream("Images." + ViewModel.ControlsImagesNames[dir] + ".png"));
            }
            for (int i = 0; i < 2; i++)
            {
                SKCanvasView curCanvas = new SKCanvasView();
                CanvasView[i] = curCanvas;
                int id = i;

                var actionTap = new TapGestureRecognizer();
                actionTap.Tapped += (s, e) =>
                {
                    MakeTurn(id);
                };
                curCanvas.GestureRecognizers.Add(actionTap);
                if (ViewModel.ChooseRow)
                {
                    ControlsGrid.Children.Add(curCanvas, 0, i);
                    Grid.SetColumnSpan(curCanvas, 2);
                    Grid.SetRowSpan(curCanvas, 1);
                }
                else
                {
                    ControlsGrid.Children.Add(curCanvas, i, 0);
                    Grid.SetColumnSpan(curCanvas, 1);
                    Grid.SetRowSpan(curCanvas, 2);
                }

                curCanvas.Opacity = 0.75;
            }
        }
        async void MakeTurn(int id)
        {
            SKCanvasView curCanvas = CanvasView[id];
            if (!ViewModel.CanMakeTurn)
            {
                return;
            }
            ViewModel.CanMakeTurn = false;
            if (ViewModel.CanAnimate)
            {
                ViewModel.CanAnimate = false;

                await curCanvas.FadeTo(1, 100);
                ViewModel.CanAnimate = true;
            }
            ViewModel.ChosenTurn = id;
            InfoTurnLabel.Text = "Wait...";
            CanvasViewField.InvalidateSurface();
            await Task.Delay(1000);
            MakeTurnAndWait();
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args, int id)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            int controlSize = ViewModel.ChooseRow ? info.Height : info.Width;
            SKRect rect = ViewModel.ChooseRow ? SKRect.Create(controlSize / 2, 0, info.Width - controlSize, info.Height)
                                    : SKRect.Create(0, controlSize / 2, info.Width, info.Height - controlSize);
            SKPaint paint = new SKPaint { Color = SKColors.White, Style = SKPaintStyle.Fill };
            canvas.DrawRect(rect, paint);
            canvas.DrawCircle(new SKPoint(controlSize / 2, controlSize / 2), controlSize / 2, paint);
            if (ViewModel.ChooseRow)
            {
                canvas.DrawCircle(new SKPoint(info.Width - controlSize / 2, controlSize / 2), controlSize / 2, paint);
            }
            else
            {
                canvas.DrawCircle(new SKPoint(controlSize / 2, info.Height - controlSize / 2), controlSize / 2, paint);
            }
            if (ViewModel.ChooseRow)
            {
                SKBitmap control1 = ControlsImages[id, 0];
                SKBitmap control2 = ControlsImages[id, 1];
                MergeBitmaps(canvas, control1, control2, info.Width, info.Height, false);
            }
            else
            {
                SKBitmap control1 = ControlsImages[0, id];
                SKBitmap control2 = ControlsImages[1, id];
                MergeBitmaps(canvas, control1, control2, info.Width, info.Height, true);
            }
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

        public async Task FadeChosenTurn()
        {
            await CanvasView[ViewModel.ChosenTurn].FadeTo(0.75, 25);
        }
    }
    public partial class GameField : ContentPage
    {
        public GameFieldViewModel ViewModel;
        GameControls GameControls { get; set; }
        private Color backgroundColor { get; set; } = Color.Transparent;
        private SKColor backgroundSKColor { get; set; } = SKColors.Transparent;
        public SKColor BorderColor
        {
            get
            {
                return (ViewModel.Game.GameSettings.Goals[ViewModel.Game.IdPlayer] == "center") ?
                    SKColor.Parse("#99897d") : // Color for center player
                    SKColor.Parse("#9f18ff");  // Color for border player
            }
        }
        
        public GameField(Game game, GameStateInfo gameStateInfo, INavigation navigation)
        {
            InitializeComponent();           
            ViewModel = new GameFieldViewModel(game, gameStateInfo, navigation);
            BindingContext = ViewModel;
            canvasView.PaintSurface += OnCanvasViewPaintSurface;

            ViewModel.FillGameTrajectory();
            StartGameField();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            ViewModel.AdjustSurface(info);
            canvas.Clear(backgroundSKColor);
            DrawField(canvas);
            DrawTrajectory(canvas);
        }
        private void StartGameField()
        {
            if (ViewModel.GameStateInfo.GameState != GameStateEnum.END)
            {                        
                GameControls = new GameControls(MakeTurnAndWait, InfoTurnLabel, ViewModel.Game, ViewModel.GameStateInfo, backgroundColor, canvasView);
                stackLayout.Children.Add(GameControls.ControlsGrid);
            }
            else
            {
                InfoTurnLabel.Text = "Moves history";
                ViewModel.NumTurns--;
            }
        }

        public async void UpdateState()
        {
            ViewModel.AddToTrajectory();
            await GameControls.FadeChosenTurn();
            GameControls.ViewModel.ChosenTurn = -1;
            ViewModel.NumTurns = ViewModel.GameStateInfo.LastIdTurn;
            GameScoreLabel.Text = ViewModel.NumTurns.ToString();  // Is it okay?

            canvasView.InvalidateSurface();

            ViewModel.CheckEnd();
            InfoTurnLabel.Text = "Make turn!";  // Is it okay?
            GameControls.ViewModel.CanMakeTurn = true;
        }
        private async void MakeTurnAndWait()
        {
            await ViewModel.MakeTurnAndWait(GameControls.ViewModel.ChosenTurn);
            UpdateState();
        }
        void DrawField(SKCanvas canvas)
        {
            SKPaint paint = new SKPaint
            {
                Shader = SKShader.CreateRadialGradient(
                            ViewModel.GridCenter,
                            ViewModel.CenterRadius,
                            new SKColor[] { SKColor.Parse("#3949AB").WithAlpha(127), backgroundSKColor },
                            null,
                            0),
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 10)
            };

            canvas.DrawCircle(ViewModel.GridCenter, ViewModel.CenterRadius, paint);

            paint = new SKPaint
            {
                Color = SKColors.White,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1,
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 1)
            };
            for (int i = 0; i <= ViewModel.GridSize; i++)
            {
                canvas.DrawLine(ViewModel.GetGridPoint(new SKPoint(i, 0)), ViewModel.GetGridPoint(new SKPoint(i, ViewModel.GridSize)), paint);
                canvas.DrawLine(ViewModel.GetGridPoint(new SKPoint(0, i)), ViewModel.GetGridPoint(new SKPoint(ViewModel.GridSize, i)), paint);
            }
            paint.StrokeWidth = 5;
            paint.Color = BorderColor;

            SKPoint p1 = ViewModel.GetGridPoint(new SKPoint(0, 0)), p2 = ViewModel.GetGridPoint(new SKPoint(ViewModel.GridSize, ViewModel.GridSize));
            canvas.DrawRect(p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y, paint);

            paint = new SKPaint
            {
                Color = paint.Color.WithAlpha(32),
                Style = SKPaintStyle.Fill,
                StrokeWidth = 1,
                ImageFilter = SKImageFilter.CreateBlur(20, 20),
            };
            canvas.DrawRect(p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y, paint);
        }

        void DrawTrajectory(SKCanvas canvas)
        {
            SKPaint paint = new SKPaint
            {
                Color = SKColor.Parse("#3949AB"),
                Style = SKPaintStyle.StrokeAndFill,
                StrokeWidth = 4,
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 1)
            };
            for (int i = 1; i < ViewModel.GameTrajectory.Count(); i++)
            {
                SKPoint prvGridPoint = ViewModel.GameTrajectory[i - 1];
                SKPoint start = ViewModel.GetGridPoint(prvGridPoint);
                SKPoint curGridPoint = ViewModel.GameTrajectory[i];
                SKPoint end = ViewModel.GetGridPoint(curGridPoint);
                canvas.DrawLine(start, end, paint);
            }
            paint.Color = SKColors.Red;
            paint.Shader = null;
            paint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 2);

            DrawChoice(canvas);
            float starSize = ViewModel.CellSize * 0.75f;
            var bitmap = SKBitmap.Decode(Helper.getResourceStream("Images.star.png"));
            SKImage image = SKImage.FromBitmap(bitmap);
            SKRect rect = new SKRect
            {
                Location = ViewModel.GetGridPoint(ViewModel.GameTrajectory.Last()) - new SKPoint(starSize, starSize),
                Size = new SKSize(starSize * 2, starSize * 2)
            };
            canvas.DrawImage(image, rect);
        }
        void DrawChoice(SKCanvas canvas)
        {
            if (GameControls is null)
            {
                return;
            }
            if (GameControls.ViewModel.ChosenTurn != -1)
            {
                int numDash = 5;
                float dashLength = 2 * (numDash - 1) + numDash;
                float[] dashArray = { ViewModel.CellSize / dashLength, 2 * ViewModel.CellSize / dashLength };
                SKPaint paint = new SKPaint
                {
                    Color = SKColors.White,
                    Style = SKPaintStyle.StrokeAndFill,
                    PathEffect = SKPathEffect.CreateDash(dashArray, 20),
                    StrokeWidth = 8,
                    MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 1)
                };
                SKPoint cur = ViewModel.GameTrajectory.Last();

                string dir1, dir2;
                if (GameControls.ViewModel.ChooseRow)
                {
                    dir1 = GameControls.ViewModel.Game.GameSettings.Controls[GameControls.ViewModel.ChosenTurn][0];
                    dir2 = GameControls.ViewModel.Game.GameSettings.Controls[GameControls.ViewModel.ChosenTurn][1];
                }
                else
                {
                    dir1 = GameControls.ViewModel.Game.GameSettings.Controls[0][GameControls.ViewModel.ChosenTurn];
                    dir2 = GameControls.ViewModel.Game.GameSettings.Controls[1][GameControls.ViewModel.ChosenTurn];
                }
                canvas.DrawLine(ViewModel.GetGridPoint(cur), ViewModel.GetGridPoint(ViewModel.MovePoint(cur, dir1)), paint);
                canvas.DrawLine(ViewModel.GetGridPoint(cur), ViewModel.GetGridPoint(ViewModel.MovePoint(cur, dir2)), paint);
            }
        }
    }
}
