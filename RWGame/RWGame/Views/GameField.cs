using RWGame.ViewModels;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using RWGame.Classes;
using System.Globalization;
using RWGame.Classes.ResponseClases;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace RWGame.Views
{
    public class GameControls
    {
        #region Properties
        private Color BackgroundColor { get; set; } = Color.Transparent;
        public Grid ControlsGrid { get; set; }
        SKBitmap[,] ControlsImages { get; set; } = new SKBitmap[2, 2];
        public SKCanvasView[] CanvasView { get; set; } = new SKCanvasView[2];
        Label InfoTurnLabel { get; set; }
        Action MakeTurnAndWait { get; set; }
        SKCanvasView CanvasViewField { get; set; }

        public bool ChooseRow { get; set; }
        public Game Game { get; set; }
        public GameStateInfo GameStateInfo { get; set; } // LINK TO ViewModel.GameStateInfo, cuz this one doesn't update.
        private bool CanAnimate { get; set; } = true;
        public bool CanMakeTurn { get; set; } = true;
        public int ChosenTurn { get; set; } = -1;
        private Dictionary<string, string> ControlsImagesNames { get; set; } = new Dictionary<string, string> {
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
        #endregion

        public GameControls(Action MakeTurnAndWait, Label InfoTurnLabel, Game game, GameStateInfo gameStateInfo, Color backgroundColor, SKCanvasView canvasViewField)
        {
            this.MakeTurnAndWait = MakeTurnAndWait;
            this.InfoTurnLabel = InfoTurnLabel;
            Game = game;
            GameStateInfo = gameStateInfo;
            BackgroundColor = backgroundColor;
            CanvasViewField = canvasViewField;
            ChooseRow = Game.GameSettings.TurnControls[Game.IdPlayer] == "row";

            MakeGameControl();
            CanvasView[0].PaintSurface += (sender, args) => OnCanvasViewPaintSurface(sender, args, 0);
            CanvasView[1].PaintSurface += (sender, args) => OnCanvasViewPaintSurface(sender, args, 1);

            CanvasView[0].InvalidateSurface();
            CanvasView[1].InvalidateSurface();

            if (GameStateInfo.GameState == GameStateEnum.WAIT)
            {
                int idTurn = GameStateInfo.Turn[Game.IdPlayer];
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
                HeightRequest = ScreenWidth / 2.5,
                WidthRequest = ScreenWidth / 2.5,
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
                string dir = Game.GameSettings.Controls[i / 2][i % 2];
                ControlsImages[i / 2, i % 2] = SKBitmap.Decode(Helper.getResourceStream("Images." + ControlsImagesNames[dir] + ".png"));
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
                if (ChooseRow)
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
            if (!CanMakeTurn)
            {
                return;
            }
            CanMakeTurn = false;
            if (CanAnimate)
            {
                CanAnimate = false;

                await curCanvas.FadeTo(1, 100);
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
    }
    public partial class GameField : ContentPage
    {
        private GameFieldViewModel ViewModel { get; set; }
        #region ViewModelProperties
        private GameControls GameControls { get; set; }
        private float CenterRadius
        {
            get { return ViewModel.CenterRadius; }
        }
        private SKPoint GridCenter
        {
            get { return ViewModel.GridCenter; }
        }
        private int GridSize
        {
            get { return ViewModel.GridSize; }
        }
        private SKColor BorderColor
        {
            get { return ViewModel.BorderColor; }
        }
        private float CellSize
        {
            get { return ViewModel.CellSize; }
        }
        private int TrajectoryLength
        {
            get { return ViewModel.GameTrajectory.Count; }
        }
        #endregion
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
        #region ViewProperties
        private Color backgroundColor { get; set; } = Color.Transparent;
        private SKColor backgroundSKColor { get; set; } = SKColors.Transparent;
        private StackLayout stackLayout { get; set; }
        private SKCanvasView canvasView { get; set; }
        private Label InfoTurnLabel { get; set; }
        private Label GameScoreLabel { get; set; }
        private Image GameScoreImage { get; set; }
        private Label GameTopScoreLabel { get; set; }
        private Image GameTopScoreImage { get; set; }
        private Label GoalLabel { get; set; }
        #endregion
        #region Constructor
        public GameField(Game game, GameStateInfo gameStateInfo, INavigation navigation)
        {
            ViewModel = new GameFieldViewModel(game, gameStateInfo, navigation);
            ViewModel.NeedsCheckState = true;
            NavigationPage.SetHasNavigationBar(this, false);

            this.BackgroundColor = backgroundColor;

            ViewModel.FillGameTrajectory();

            AbsoluteLayout absoluteLayout = new AbsoluteLayout();
            Image back = new Image
            {
                VerticalOptions = LayoutOptions.Start,
                Aspect = Aspect.AspectFill,
                Source = ImageSource.FromFile("seashore2.png"),
            };

            absoluteLayout.Children.Add(back, new Rectangle(0, 0, ScreenWidth, ScreenWidth * 2));

            canvasView = new SKCanvasView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Fill,
                Margin = new Thickness(0, 0, 0, 0),
                HeightRequest = ScreenWidth - 10,
                WidthRequest = ScreenWidth,
            };
            canvasView.PaintSurface += OnCanvasViewPaintSurface;

            stackLayout = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Spacing = 0,
            };
            Grid labelGrid = new Grid()
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(2.2, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
                },
                HorizontalOptions = LayoutOptions.Fill,
                Margin = new Thickness(20, 0, 20, 0),
                BackgroundColor = Color.FromHex("#6bbaff").MultiplyAlpha(0.15),
            };
            InfoTurnLabel = new Label()
            {
                Text = "",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromHex("#15c1ff"),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                BackgroundColor = Color.FromHex("#f9ce6f").MultiplyAlpha(0.5)
            };

            GameScoreLabel = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                BackgroundColor = backgroundColor,

            };
            GameScoreImage = new Image()
            {
                BackgroundColor = backgroundColor,
                Source = "state_star_" + game.GameSettings.Goals[game.IdPlayer] + ".png",
                Aspect = Aspect.AspectFit,
                HeightRequest = GameScoreLabel.FontSize,
                Scale = 1,
                Margin = new Thickness(0, 3, 0, 3),
            };
            GameTopScoreLabel = new Label()
            {
                Text = "",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                BackgroundColor = backgroundColor,
            };
            GameTopScoreImage = new Image()
            {
                HorizontalOptions = LayoutOptions.End,
                BackgroundColor = backgroundColor,
                Source = "top_score_" + game.GameSettings.Goals[game.IdPlayer] + ".png",
                HeightRequest = GameTopScoreLabel.FontSize,
                Scale = 1,
                Margin = new Thickness(0, 3, 0, 3),
            };

            GoalLabel = new Label()
            {
                Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(game.GameSettings.Goals[game.IdPlayer]),
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = backgroundColor
            };

            if (game.GameSettings.Goals[game.IdPlayer] == "center")
            {
                GameTopScoreLabel.Text = "546";
            }
            else if (game.GameSettings.Goals[game.IdPlayer] == "border")
            {
                GameTopScoreLabel.Text = "20";
            }

            var stackLayoutScore = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Start };
            var stackLayoutTopScore = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.EndAndExpand };
            stackLayoutScore.Children.Add(GameScoreImage);
            stackLayoutScore.Children.Add(GameScoreLabel);
            stackLayoutTopScore.Children.Add(GameTopScoreImage);
            stackLayoutTopScore.Children.Add(GameTopScoreLabel);

            labelGrid.Children.Add(stackLayoutScore, 0, 0);
            labelGrid.Children.Add(GoalLabel, 1, 0);
            labelGrid.Children.Add(stackLayoutTopScore, 2, 0);

            stackLayout.Children.Add(labelGrid);
            stackLayout.Children.Add(canvasView);
            stackLayout.Children.Add(InfoTurnLabel);

            if (ViewModel.GameStateInfo.GameState != GameStateEnum.END)
            {
                if (game.Turns.Count == 1)
                {
                    InfoTurnLabel.Text = "Make first turn!";
                }
                GameControls = new GameControls(MakeTurnAndWait, InfoTurnLabel, ViewModel.Game, ViewModel.GameStateInfo, backgroundColor, canvasView);
                stackLayout.Children.Add(GameControls.ControlsGrid);
            }
            else
            {
                InfoTurnLabel.Text = "Moves history";
                ViewModel.NumTurns--;
            }
            GameScoreLabel.Text = ViewModel.NumTurns.ToString();

            absoluteLayout.Children.Add(stackLayout, new Rectangle(0, 0, App.ScreenWidth, App.ScreenHeight));
            Content = absoluteLayout;
        }
        #endregion
        async void MakeTurnAndWait()
        {
            await ViewModel.MakeTurnAndWait(GameControls.ChosenTurn); // doesn't finish before adding to trajectory
            ViewModel.AddToTrajectory();
            FinishTurn();
        }
        public async void FinishTurn()
        {
            await GameControls.CanvasView[GameControls.ChosenTurn].FadeTo(0.75, 25);

            GameControls.ChosenTurn = -1;
            ViewModel.NumTurns = ViewModel.GameStateInfo.LastIdTurn;
            ViewModel.GameScoreLabelText = ViewModel.NumTurns.ToString();
            GameScoreLabel.Text = ViewModel.GameScoreLabelText;
            GameControls.GameStateInfo = ViewModel.GameStateInfo;
            canvasView.InvalidateSurface();
            if (GameControls.GameStateInfo.GameState == GameStateEnum.END) // GameStateInfo is outdated
            {
                await App.Current.MainPage.DisplayAlert("Game finished", "You made " + ViewModel.NumTurns.ToString() + " turns!" + "\n" + "Thanks for playing ;)", "OK");
                await Navigation.PopAsync();
                return;
            }
            ViewModel.InfoTurnLabelText = "Make turn!";
            GameControls.CanMakeTurn = true;
        }
        #region BackButtonBehaviour
        public async void CallPopAsync()
        {
            await Navigation.PopAsync();
        }
        protected override bool OnBackButtonPressed()
        {
            ViewModel.NeedsCheckState = false;
            CallPopAsync();
            return true;
        }
        #endregion
        #region PointMethods
        public SKPoint GetGridPoint(SKPoint p)
        {
            return ViewModel.GetGridPoint(p.X, p.Y);
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
        #region DrawMethods
        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            ViewModel.AdjustSurface(info);
            canvas.Clear(backgroundSKColor);
            DrawField(canvas);
            DrawTrajectory(canvas);
        }
        void DrawChoice(SKCanvas canvas)
        {
            if (GameControls is null)
            {
                return;
            }
            if (GameControls.ChosenTurn != -1)
            {
                int numDash = 5;
                float dashLength = 2 * (numDash - 1) + numDash;
                float[] dashArray = { CellSize / dashLength, 2 * CellSize / dashLength };
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
                if (GameControls.ChooseRow)
                {
                    dir1 = GameControls.Game.GameSettings.Controls[GameControls.ChosenTurn][0];
                    dir2 = GameControls.Game.GameSettings.Controls[GameControls.ChosenTurn][1];
                }
                else
                {
                    dir1 = GameControls.Game.GameSettings.Controls[0][GameControls.ChosenTurn];
                    dir2 = GameControls.Game.GameSettings.Controls[1][GameControls.ChosenTurn];
                }
                canvas.DrawLine(GetGridPoint(cur), GetGridPoint(MovePoint(cur, dir1)), paint);
                canvas.DrawLine(GetGridPoint(cur), GetGridPoint(MovePoint(cur, dir2)), paint);
            }
        }
        void DrawField(SKCanvas canvas)
        {
            SKPaint paint = new SKPaint
            {
                Shader = SKShader.CreateRadialGradient(
                            GridCenter,
                            CenterRadius,
                            new SKColor[] { SKColor.Parse("#3949AB").WithAlpha(127), backgroundSKColor },
                            null,
                            0),
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 10)
            };

            canvas.DrawCircle(GridCenter, CenterRadius, paint);

            paint = new SKPaint
            {
                Color = SKColors.White,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1,
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 1)
            };
            for (int i = 0; i <= GridSize; i++)
            {
                canvas.DrawLine(GetGridPoint(new SKPoint(i, 0)), GetGridPoint(new SKPoint(i, GridSize)), paint);
                canvas.DrawLine(GetGridPoint(new SKPoint(0, i)), GetGridPoint(new SKPoint(GridSize, i)), paint);
            }
            paint.StrokeWidth = 5;
            paint.Color = BorderColor;

            SKPoint p1 = GetGridPoint(new SKPoint(0, 0)), p2 = GetGridPoint(new SKPoint(GridSize, GridSize));
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
            for (int i = 1; i < TrajectoryLength; i++)
            {
                SKPoint prvGridPoint = ViewModel.GameTrajectory[i - 1];
                SKPoint start = GetGridPoint(prvGridPoint);
                SKPoint curGridPoint = ViewModel.GameTrajectory[i];
                SKPoint end = GetGridPoint(curGridPoint);
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
                Location = GetGridPoint(ViewModel.GameTrajectory.Last()) - new SKPoint(starSize, starSize),
                Size = new SKSize(starSize * 2, starSize * 2)
            };
            canvas.DrawImage(image, rect);
        }
        #endregion
    }
}

