using RWGame.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SkiaSharp;
using SkiaSharp.Views.Forms;
using RWGame.Classes.ResponseClases;
using System.Reflection;
using System.IO;
using FFImageLoading;

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
        Label GoalLabel;
        Grid ControlsGrid;
        Image[,] ControlsImages = new Image[2, 2];
        SKCanvasView canvasView;      

        readonly Game game;
        readonly SystemSettings systemSettings;
        readonly ServerWorker serverWorker;
        private readonly Color coloredColor = Color.FromHex("#6ecbfa");
        private readonly Color defaultColor = Color.FromHex("#39bafa");
        private bool canAnimate = true;
        private bool canMakeTurn = true;
        private int gridSize;
        private readonly int marginX = 60;
        private readonly int marginY = 30;
        private readonly int pointRadius = 30;
        private float centerRadius = 300;
        private float cellSize;
        private float shiftX = 0;
        private bool chooseRow = false;
        private int idTurn = 0;
        private List<SKPoint> gameTrajectory = new List<SKPoint> { };
        //new SKPoint(5, 5), new SKPoint(6, 5), new SKPoint( 6, 6 ), new SKPoint( 7, 6 ), new SKPoint( 6, 6 ), new SKPoint( 6, 7 )

        private readonly Dictionary<string, string> ControlsImagesNames = new Dictionary<string, string> {
            { "U", "up" }, { "L", "left" }, { "D", "down" }, { "R", "right" }
        };
        private int chosenControl = 0;
        private int chosenTurn = -1;

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
                Color = (SKColors.White),
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
        }

        Stream getResourceStream(String filename)
        {
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            string resourceID = assembly.GetManifestResourceNames()
                                .Where(name => name.Contains(filename))
                                .FirstOrDefault();
            return assembly.GetManifestResourceStream(resourceID);
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
            var bitmap = SKBitmap.Decode(getResourceStream("Images.star.png"));
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

        void MakeGameControl()
        {
            ControlsGrid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                Margin = new Thickness(10, 0, 10, 0),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = backgroundColor,
                HeightRequest = systemSettings.ScreenWidth / 2.5,
                WidthRequest = systemSettings.ScreenWidth / 2.5,
                RowSpacing = 2,
                ColumnSpacing = 2,
            };
            for (int i = 0; i < 4; i++)
            {
                string dir = game.GameSettings.Controls[i / 2][i % 2];
                ControlsImages[i / 2, i % 2] = new Image()
                {
                    BackgroundColor = backgroundColor,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    Source = ControlsImagesNames[dir] + ".png"
                };
            }
            for (int i = 0; i < 4; i++)
            {
                int id = i;
                Image curImage = ControlsImages[i / 2, i % 2];
                curImage.Opacity = 0.75;
                Image nghImage;
                if (chooseRow)
                {
                    nghImage = ControlsImages[i / 2, (i + 1) % 2];
                }
                else
                {
                    nghImage = ControlsImages[(i / 2 + 1) % 2, i % 2];
                }
                var actionTap = new TapGestureRecognizer();
                actionTap.Tapped += async (s, e) =>
                {
                    if (!canMakeTurn)
                    {
                        return;
                    }
                    canMakeTurn = false;
                    if (canAnimate)
                    {
                        canAnimate = false;
                        
                        await Task.WhenAny(curImage.FadeTo(0, 25), nghImage.FadeTo(0, 25));
                        await Task.WhenAny(curImage.FadeTo(1, 100), nghImage.FadeTo(1, 100));
                        canAnimate = true;
                    }
                    chosenControl = id;
                    string turnName;
                    if (chooseRow)
                    {
                        chosenTurn = id / 2;
                        turnName = chosenTurn == 0 ? "upper row" : "bottom row";
                    }
                    else
                    {
                        chosenTurn = id % 2;
                        turnName = chosenTurn == 0 ? "left column" : "right column";
                    }
                    InfoTurnLabel.Text = turnName + ". Wait...";
                    await Task.Delay(1000);
                    MakeTurnAndWait();
                };
                curImage.GestureRecognizers.Add(actionTap);
                ControlsGrid.Children.Add(curImage, i % 2, i / 2);
                Grid.SetColumnSpan(curImage, 1);
                Grid.SetRowSpan(curImage, 1);
            }
        }

        public async void MakeTurnAndWait()
        {
            GameStateInfo gameStateInfo = await serverWorker.TaskMakeTurn(game.IdGame, chosenTurn);
            while (gameStateInfo.GameState == GameStateEnum.WAIT)
            {
                await Task.Delay(1000);
                gameStateInfo = await serverWorker.TaskGetGameState(game.IdGame);
            }
            UpdateState(gameStateInfo);
        }

        public async void UpdateState(GameStateInfo gameStateInfo)
        {
            gameTrajectory.Add(new SKPoint(gameStateInfo.PointState[0], gameStateInfo.PointState[1]));
            if (chooseRow)
            {
                await Task.WhenAny(ControlsImages[chosenTurn, 0].FadeTo(0.75, 25), ControlsImages[chosenTurn, 1].FadeTo(0.75, 25));
            }
            else
            {
                await Task.WhenAny(ControlsImages[0, chosenTurn].FadeTo(0.75, 25), ControlsImages[1, chosenTurn].FadeTo(0.75, 25));
            }
            chosenTurn = -1;
            idTurn = gameStateInfo.LastIdTurn;
            GameInfoLabel.Text = "Time: " + idTurn;

            canvasView.InvalidateSurface();
            if (gameStateInfo.GameState == GameStateEnum.END)
            {
                await DisplayAlert("Game finshed", "You made " + idTurn.ToString() + " turns!" + "\n" + "Thanks for playing ;-)", "OK");
                await Navigation.PopAsync();
                return;
            }
            InfoTurnLabel.Text = "Make turn!";
            canMakeTurn = true;
        }

        public GameField(ServerWorker _serverWorker, SystemSettings _systemSettings,
                         Game _game)
        {
            game = _game;
            systemSettings = _systemSettings;
            serverWorker = _serverWorker;
            NavigationPage.SetHasNavigationBar(this, false);

            this.BackgroundColor = backgroundColor;
            this.BackgroundImageSource = ImageSource.FromFile("background.png");
            chooseRow = game.GameSettings.TurnControls[game.IdPlayer] == "row";
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
                Text = "Time: " + game.Turns.Count,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
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


            stackLayout.Children.Add(GameInfoLabel);
            stackLayout.Children.Add(canvasView);
            stackLayout.Children.Add(InfoTurnLabel);

            MakeGameControl();

            stackLayout.Children.Add(ControlsGrid);
            stackLayout.Children.Add(GoalLabel);


            Content = stackLayout;
        }

        public async void CallPopAsync()
        {
            await Navigation.PopAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            CallPopAsync();
            return true;
        }

    }
}
