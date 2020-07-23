using RWGame.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SkiaSharp;
using SkiaSharp.Views.Forms;
using RWGame.Classes.ResponseClases;
//using System.Reflection;
//using System.IO;
//using FFImageLoading;
//using System.Drawing;
//using AssetsLibrary;
//using System.Runtime.CompilerServices;

namespace RWGame
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GameField : ContentPage
	{
        private readonly Color backgroundColor = Color.Transparent;//Color.FromHex("#39bafa");
        private readonly SKColor backgroundSKColor = SKColors.Transparent;//SKColor.Parse("#39bafa");
        //private readonly Color backgroundColor = Color.FromHex("#15c1ff03");//Color.FromHex("#39bafa");
        //private readonly SKColor backgroundSKColor = SKColor.Parse("#15c1ff03");//SKColor.Parse("#39bafa");
        //private StackLayout stackLayout;
        Label InfoTurnLabel;
        Label GameInfoLabel;
        Label GameScoreLabel;
        Label GoalLabel;
        StackLayout labelLayout;
        //Grid ControlsGrid;
        //Image[,] ControlsImages = new Image[2, 2];
        SKCanvasView canvasView;
        GameControls gameControls;

        readonly Game game;
        readonly SystemSettings systemSettings;
        readonly ServerWorker serverWorker;
        private readonly Color coloredColor = Color.FromHex("#6ecbfa");
        private readonly Color defaultColor = Color.FromHex("#39bafa");
        private int gridSize;
        private readonly int marginX = 60;
        private readonly int marginY = 30;
        private readonly int pointRadius = 30;
        private float centerRadius = 300;
        private float cellSize;
        private float shiftX = 0;
        private int idTurn = 0;
        private bool needCheckState = true;
        private List<SKPoint> gameTrajectory = new List<SKPoint> { };
        //new SKPoint(5, 5), new SKPoint(6, 5), new SKPoint( 6, 6 ), new SKPoint( 7, 6 ), new SKPoint( 6, 6 ), new SKPoint( 6, 7 )


        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            gridSize = game.GameSettings.FieldWidth;
            cellSize = (float)(Math.Min(info.Width - marginX, info.Height - marginY)) / gridSize;
            centerRadius = (float)info.Width / 5;
            shiftX = (info.Width - marginX - cellSize * gridSize) / 2;

            canvas.Clear(backgroundSKColor);
            DrawField(canvas);
            DrawTrajectory(canvas);
        }


        SKPoint GetGridPoint(SKPoint p)
        {
            return GetGridPoint(p.X, p.Y);
        }

        SKPoint GetGridPoint(float x, float y)
        {
            return new SKPoint(marginX / 2 + cellSize * x + shiftX, marginY / 2 + cellSize * y);
        }

        void DrawField(SKCanvas canvas)
        {
            SKPoint gridCenter = GetGridPoint(gridSize / 2, gridSize / 2);

            SKPaint paint = new SKPaint
            {
                Shader = SKShader.CreateRadialGradient(
                            gridCenter,
                            centerRadius,
                            new SKColor[] { SKColor.Parse("#3949AB").WithAlpha(127), backgroundSKColor },
                            null,
                            0),
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 10)
            };

            canvas.DrawCircle(gridCenter, centerRadius, paint);

            paint = new SKPaint
            {
                Color = SKColors.White,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1,
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 1)
            };
            for (int i = 0; i <= gridSize; i++)
            {
                canvas.DrawLine(GetGridPoint(i, 0), GetGridPoint(i, gridSize), paint);
                canvas.DrawLine(GetGridPoint(0, i), GetGridPoint(gridSize, i), paint);
            }
            paint.StrokeWidth = 4;
            paint.Color = SKColor.Parse("#3949AB");
            SKPoint p1 = GetGridPoint(0, 0), p2 = GetGridPoint(gridSize, gridSize);
            canvas.DrawRect(p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y, paint);

            paint = new SKPaint
            {
                Color = SKColor.Parse("#3949AB").WithAlpha(48),
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
                Color = SKColor.Parse("#3949AB"),
                Style = SKPaintStyle.StrokeAndFill,
                StrokeWidth = 5,
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 1)
            };
            for (int i = 1; i < gameTrajectory.Count; i++)
            {
                SKPoint curGridPoint = gameTrajectory[i], prvGridPoint = gameTrajectory[i - 1];
                SKPoint start = GetGridPoint(prvGridPoint), end = GetGridPoint(curGridPoint);
                float t_start = ((float)(i - 1)) / gameTrajectory.Count, t_end = ((float)i) / gameTrajectory.Count;
                //paint.Shader = SKShader.CreateLinearGradient(
                //                start, end,
                //                new SKColor[] { SKColors.Blue, SKColor.Parse("#ee423e").WithAlpha(127) },
                //                new float[] { t_start, t_end },
                //                SKShaderTileMode.Repeat);
                canvas.DrawLine(start, end, paint);
            }
            paint.Color = SKColors.Red;
            paint.Shader = null;
            paint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 2);
            //canvas.DrawCircle(GetGridPoint(gameTrajectory.Last()), pointRadius, paint);
            //Console.WriteLine(names[0]);
            //string resourceID = "RWGame.Droid.Images.star.png";
//            var names = assembly.GetManifestResourceNames();
            //var filestream = new SKManagedStream();
            var bitmap = SKBitmap.Decode(Helper.getResourceStream("Images.star.png"));
            var scaled = bitmap.Resize(new SKImageInfo(pointRadius * 2, pointRadius * 2), SKFilterQuality.High);
            SKImage image = SKImage.FromBitmap(scaled);
            SKRect rect = new SKRect
            {
                Location = GetGridPoint(gameTrajectory.Last()) - new SKPoint(pointRadius, pointRadius),
                Size = new SKSize(pointRadius * 2, pointRadius * 2)
            };
            canvas.DrawImage(image, rect);


            //var image = new Image { Source = "star.png" };
            //var b = new Bitmap;
            //using (var stream = )
            //{
            //    var bitmap = SKBitmap.Decode(stream);
            //    var scaled = bitmap.Resize(new SKImageInfo(pointRadius * 2, pointRadius * 2), SKBitmapResizeMethod.Lanczos3);
            //    SKImage image = SKImage.FromBitmap(scaled);
            //    SKRect rect = new SKRect
            //    {
            //        Location = GetGridPoint(gameTrajectory.Last()),
            //        Size = new SKSize(pointRadius * 2, pointRadius * 2)
            //    };
            //    canvas.DrawImage(image, rect);
            //}



            //ImageService.Instance.LoadCompiledResource("star").

            //using (var stream = ImageService.Instance.LoadCompiledResource("star").AsPNGStreamAsync().GetAwaiter().GetResult())
            //{
            //    var bitmap = SKBitmap.Decode(stream);
            //    var scaled = bitmap.Resize(new SKImageInfo(pointRadius * 2, pointRadius * 2), SKBitmapResizeMethod.Lanczos3);
            //    SKImage image = SKImage.FromBitmap(scaled);
            //    SKRect rect = new SKRect
            //    {
            //        Location = GetGridPoint(gameTrajectory.Last()),
            //        Size = new SKSize(pointRadius * 2, pointRadius * 2)
            //    };
            //    canvas.DrawImage(image, rect);
            //}

            //ImageSource ims = ImageSource.FromFile("star.png");
            
            //using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            //{
            //    var bitmap = SKBitmap.Decode(stream);
            //    var scaled = bitmap.Resize(new SKImageInfo(pointRadius * 2, pointRadius * 2), SKBitmapResizeMethod.Lanczos3);
            //    SKImage image = SKImage.FromBitmap(scaled);
            //    SKRect rect = new SKRect
            //    {
            //        Location = GetGridPoint(gameTrajectory.Last()),
            //        Size = new SKSize(pointRadius * 2, pointRadius * 2)
            //    };
            //    canvas.DrawImage(image, rect);
            //}
            //var bitmap = SKBitmap.Decode("Resources/drawable/star.bmp");
        }

        public SKImage MergeBitmaps(SKBitmap bitmap1, SKBitmap bitmap2, bool vertical = true)
        {
            SKImage result;
            int width, height;
            if (vertical)
            {
                width = Math.Max(bitmap1.Width, bitmap2.Width);
                height = bitmap1.Height + bitmap2.Height;
            } else
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
                else {
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
            GameStateInfo gameStateInfo = await serverWorker.TaskMakeTurn(game.IdGame, gameControls.chosenTurn);
            while (gameStateInfo.GameState == GameStateEnum.WAIT)
            {
                if (!needCheckState)
                {
                    return;
                }
                await Task.Delay(1000);
                gameStateInfo = await serverWorker.TaskGetGameState(game.IdGame);
            }
            UpdateState(gameStateInfo);
        }

        public async void UpdateState(GameStateInfo gameStateInfo)
        {
            
            gameTrajectory.Add(new SKPoint(gameStateInfo.PointState[0], gameStateInfo.PointState[1]));
            await gameControls.canvasView[gameControls.chosenTurn].FadeTo(0.75, 25);
            /*if (chooseRow)
            {
                //await Task.WhenAny(ControlsImages[gameControls.chosenTurn, 0].FadeTo(0.75, 25), ControlsImages[gameControls.chosenTurn, 1].FadeTo(0.75, 25));
                
            }
            else
            {
                //await Task.WhenAny(ControlsImages[0, gameControls.chosenTurn].FadeTo(0.75, 25), ControlsImages[1, gameControls.chosenTurn].FadeTo(0.75, 25));
            }*/
            gameControls.chosenTurn = -1;
            idTurn = gameStateInfo.LastIdTurn;
            GameInfoLabel.Text = "Score: " + idTurn;

            canvasView.InvalidateSurface();
            if (gameStateInfo.GameState == GameStateEnum.END)
            {
                await DisplayAlert("Game finished", "You made " + idTurn.ToString() + " turns!" + "\n" + "Thanks for playing ;-)", "OK");
                await Navigation.PopAsync();
                return;
            }
            InfoTurnLabel.Text = "Make turn!";
            gameControls.canMakeTurn = true;
        }

        public GameField(ServerWorker _serverWorker, SystemSettings _systemSettings,
                         Game _game, GameStateInfo gameStateInfo)
        {
            game = _game;
            systemSettings = _systemSettings;
            serverWorker = _serverWorker;


            needCheckState = true;
            NavigationPage.SetHasNavigationBar(this, false);

            this.BackgroundColor = backgroundColor;
            this.BackgroundImageSource = ImageSource.FromFile("background.png");
            foreach (TurnInfo turn in game.Turns)
            {
                gameTrajectory.Add(new SKPoint(turn.State[0], turn.State[1]));
            }

            // Create SKCanvasView to view result 
            canvasView = new SKCanvasView
            {
                //HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Margin = new Thickness(10, 0, 10, 0),
                
            };

            canvasView.PaintSurface += OnCanvasViewPaintSurface;

            stackLayout = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Spacing = 0,
            };
            labelLayout = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Fill,
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(10, 0, 10, 0),
                Spacing = 0,
            };
            InfoTurnLabel = new Label()
            {
                Text = "",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromHex("#15c1ff"),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = backgroundColor
            };

            GameInfoLabel = new Label()
            {
                Text = "Score: " + gameStateInfo.LastIdTurn,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.Start,
                BackgroundColor = backgroundColor,
                Margin = new Thickness(10, 0, 10, 0),
            };

            GameScoreLabel = new Label()
            {
                Text = "",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.End,
                BackgroundColor = backgroundColor,
                Margin = new Thickness(10, 0, 10, 0),
            };

            GoalLabel = new Label()
            {
                Text = "Your goal: " + game.GameSettings.Goals[game.IdPlayer],
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromHex("#15c1ff"),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = backgroundColor
            };

            if (game.GameSettings.Goals[game.IdPlayer] == "center")
            {
                GameScoreLabel.Text = "Top score: 546";
            }
            else if(game.GameSettings.Goals[game.IdPlayer] == "border")
            {
                GameScoreLabel.Text = "Top score: 29";
            }

            labelLayout.Children.Add(GameInfoLabel);
            labelLayout.Children.Add(GameScoreLabel);

            stackLayout.Children.Add(labelLayout);
            stackLayout.Children.Add(canvasView);
            stackLayout.Children.Add(InfoTurnLabel);

            gameControls = new GameControls(MakeTurnAndWait, InfoTurnLabel, game, gameStateInfo, systemSettings, backgroundColor);

            stackLayout.Children.Add(gameControls.ControlsGrid);
            stackLayout.Children.Add(GoalLabel);


            Content = stackLayout;
        }

        public async void CallPopAsync()
        {
            await Navigation.PopAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            needCheckState = false;
            CallPopAsync();
            return true;
        }

    }

    class GameControls
    {
        private readonly Color backgroundColor = Color.Transparent;
        readonly SystemSettings systemSettings;
        public Grid ControlsGrid;
        SKBitmap[,] ControlsImages = new SKBitmap[2, 2];
        public SKCanvasView[] canvasView = new SKCanvasView[2];
        Label InfoTurnLabel;
        Action MakeTurnAndWait;

        bool chooseRow;
        readonly Game game;
        private bool canAnimate = true;
        public bool canMakeTurn = true;

        //public int chosenControl = 0;
        public int chosenTurn = -1;

        private readonly Dictionary<string, string> ControlsImagesNames = new Dictionary<string, string> {
            { "U", "up" }, { "L", "left" }, { "D", "down" }, { "R", "right" }
        };

        public GameControls(Action MakeTurnAndWait, Label InfoTurnLabel, Game game, GameStateInfo gameStateInfo, SystemSettings systemSettings, 
            Color backgroundColor)
        {
            this.MakeTurnAndWait = MakeTurnAndWait;
            this.systemSettings = systemSettings;
            this.backgroundColor = backgroundColor;
            this.InfoTurnLabel = InfoTurnLabel;
            this.game = game;
            chooseRow = game.GameSettings.TurnControls[game.IdPlayer] == "row";
            
            MakeGameControl();
            canvasView[0].PaintSurface += (sender, args) => OnCanvasViewPaintSurface(sender, args, 0);
            canvasView[1].PaintSurface += (sender, args) => OnCanvasViewPaintSurface(sender, args, 1);

            canvasView[0].InvalidateSurface();
            canvasView[1].InvalidateSurface();

            if (gameStateInfo.GameState == GameStateEnum.WAIT)
            {
                int idTurn = gameStateInfo.Turn[game.IdPlayer];
                if (idTurn != -1)
                {
                    MakeTurn(idTurn);
                }
            }
        }

        public void MergeBitmaps(SKCanvas canvas, SKBitmap bitmap1, SKBitmap bitmap2, int width, int height, 
            bool vertical = true)
        {
            //int width, height;
            /*if (vertical)
            {
                width = Math.Max(bitmap1.Width, bitmap2.Width);
                height = bitmap1.Height + bitmap2.Height;
            }
            else
            {
                height = Math.Max(bitmap1.Height, bitmap2.Height);
                width = bitmap1.Width + bitmap2.Width;
            }*/
            
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
        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args, int id)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            //canvas.Clear(backgroundSKColor);
            int controlSize = chooseRow ? info.Height : info.Width;
            SKRect rect = chooseRow ? SKRect.Create(controlSize / 2, 0, info.Width - controlSize, info.Height) 
                                    : SKRect.Create(0, controlSize / 2, info.Width, info.Height - controlSize);
            SKPaint paint = new SKPaint { Color = SKColors.White, Style = SKPaintStyle.Fill };
            canvas.DrawRect(rect, paint);
            canvas.DrawCircle(new SKPoint(controlSize / 2, controlSize / 2), controlSize / 2, paint);
            if (chooseRow)
            {
                canvas.DrawCircle(new SKPoint(info.Width - controlSize / 2, controlSize / 2), controlSize / 2, paint);
            }
            else 
            {
                canvas.DrawCircle(new SKPoint(controlSize / 2, info.Height - controlSize / 2), controlSize / 2, paint);
            }
            if (chooseRow)
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

        async void MakeTurn(int id)
        {
            SKCanvasView curCanvas = canvasView[id];
            if (!canMakeTurn)
            {
                return;
            }
            canMakeTurn = false;
            if (canAnimate)
            {
                canAnimate = false;

                await curCanvas.FadeTo(0, 25);//, nghImage.FadeTo(0, 25));
                await curCanvas.FadeTo(1, 100);//, nghImage.FadeTo(1, 100));
                canAnimate = true;
            }
            string turnName;
            chosenTurn = id;
            if (chooseRow)
            {
                turnName = chosenTurn == 0 ? "upper row" : "bottom row";
            }
            else
            {
                turnName = chosenTurn == 0 ? "left column" : "right column";
            }
            InfoTurnLabel.Text = turnName + ". Wait...";
            await Task.Delay(1000);
            MakeTurnAndWait();
        }

        void MakeGameControl()
        {
            ControlsGrid = new Grid
            {
                Margin = new Thickness(10, 0, 10, 0),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = backgroundColor,
                HeightRequest = systemSettings.ScreenWidth / 2.5,
                WidthRequest = systemSettings.ScreenWidth / 2.5,
                RowSpacing = 6,
                ColumnSpacing = 6,
            };

            //if (chooseRow)
            //{
                ControlsGrid.RowDefinitions = new RowDefinitionCollection {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    //new RowDefinition { Height = new GridLength(0.05, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                };
                ControlsGrid.ColumnDefinitions = new ColumnDefinitionCollection {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    //new ColumnDefinition { Width = new GridLength(0.05, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                };
            /*}
            else
            {
                ControlsGrid.RowDefinitions = new RowDefinitionCollection {
                    new RowDefinition { Height = new GridLength(2, GridUnitType.Star) }
                };
                ControlsGrid.ColumnDefinitions = new ColumnDefinitionCollection {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                };
            }*/
            for (int i = 0; i < 4; i++)
            {
                string dir = game.GameSettings.Controls[i / 2][i % 2];
                ControlsImages[i / 2, i % 2] = SKBitmap.Decode(Helper.getResourceStream("Images." + ControlsImagesNames[dir] + ".png"));
            }
            for (int i = 0; i < 2; i++)
            {
                SKCanvasView curCanvas = new SKCanvasView();
                canvasView[i] = curCanvas;
                int id = i;

                var actionTap = new TapGestureRecognizer();
                actionTap.Tapped += (s, e) =>
                {
                    MakeTurn(id);
                };
                curCanvas.GestureRecognizers.Add(actionTap);
                if (chooseRow)
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


            /*SKCanvasView borderCanvas = new SKCanvasView();
            borderCanvas.PaintSurface += (sender, args) =>
            {
                SKImageInfo info = args.Info;
                SKSurface surface = args.Surface;
                SKCanvas canvas = surface.Canvas;
                SKRect rect = SKRect.Create(0, 0, info.Width, info.Height);
                SKPaint paint = new SKPaint() { Color = SKColor.Parse("#15c1ff"), StrokeWidth = 2 };
                if (chooseRow)
                {
                    canvas.DrawLine(0, info.Height / 2, info.Width, info.Height / 2, paint);
                }
                else
                {
                    canvas.DrawLine(info.Width / 2, 0, info.Width / 2, info.Height, paint);
                }
            };
            if (chooseRow)
            {
                ControlsGrid.Children.Add(borderCanvas, 0, 1);
                Grid.SetColumnSpan(borderCanvas, 3);
                Grid.SetRowSpan(borderCanvas, 1);
            }
            else
            {
                ControlsGrid.Children.Add(borderCanvas, 1, 0);
                Grid.SetColumnSpan(borderCanvas, 1);
                Grid.SetRowSpan(borderCanvas, 3);
            }*/
        }

    }

}
