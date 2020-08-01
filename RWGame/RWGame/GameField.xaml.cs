using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
//using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
        Label GameScoreLabel;
        Image GameScoreImage;
        Label GameTopScoreLabel;
        Image GameTopScoreImage;
        Label GoalLabel;
        //StackLayout labelLayout;
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
        private readonly int marginX = 100;
        private readonly int marginY = 100;
        //private readonly int pointRadius = 30;
        private float centerRadius = 300;
        private float cellSize;
        private float shiftX = 0;
        private int idTurn = 0;
        private int numTurns = 0;
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

        
        SKPoint MovePoint(SKPoint cur, string dir)
        {
            var dx = new Dictionary<string, int> { { "U", 0 }, { "D", 0 }, { "L", -1 }, { "R", +1 } };
            var dy = new Dictionary<string, int> { { "U", -1 }, { "D", +1 }, { "L", 0 }, { "R", 0 } };

            cur.X += dx[dir];
            cur.Y += dy[dir];
            return cur;
        }

        static readonly float[] dashArray = { 5, 10 };
        void DrawChoice(SKCanvas canvas)
        {
            if (gameControls is null) return;
            if (gameControls.chosenTurn != -1) {
                SKPaint paint = new SKPaint
                {
                    Color = SKColors.White,//SKColor.Parse("#3949AB"),
                    Style = SKPaintStyle.StrokeAndFill,
                    PathEffect = SKPathEffect.CreateDash(dashArray, 20),
                    StrokeWidth = 8,
                    MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 1)
                };
                SKPoint cur = gameTrajectory.Last();

                string dir1, dir2;
                if (gameControls.chooseRow) {
                    dir1 = game.GameSettings.Controls[gameControls.chosenTurn][0];
                    dir2 = game.GameSettings.Controls[gameControls.chosenTurn][1];
                } 
                else
                {
                    dir1 = game.GameSettings.Controls[0][gameControls.chosenTurn];
                    dir2 = game.GameSettings.Controls[1][gameControls.chosenTurn];
                }
                
                canvas.DrawLine(GetGridPoint(cur), GetGridPoint(MovePoint(cur, dir1)), paint);
                canvas.DrawLine(GetGridPoint(cur), GetGridPoint(MovePoint(cur, dir2)), paint);
            }
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
            paint.StrokeWidth = 5;
            

            if (game.GameSettings.Goals[game.IdPlayer] == "center")
            {
                paint.Color = SKColor.Parse("#99897d"); //ffd119 fca821 f36f24 ead3c3 d6b9a5 96755d bdaa9d
            } else if (game.GameSettings.Goals[game.IdPlayer] == "border")
            {
                paint.Color = SKColor.Parse("#9f18ff");
            }

            SKPoint p1 = GetGridPoint(0, 0), p2 = GetGridPoint(gridSize, gridSize);
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

            DrawChoice(canvas);
            float starSize = cellSize * 0.75f;
            var bitmap = SKBitmap.Decode(Helper.getResourceStream("Images.star.png"));
            //var scaled = bitmap.Resize(new SKImageInfo(starSize * 2, pointRstarSizeadius * 2), SKFilterQuality.High);
            SKImage image = SKImage.FromBitmap(bitmap);
            SKRect rect = new SKRect
            {
                Location = GetGridPoint(gameTrajectory.Last()) - new SKPoint(starSize, starSize),
                Size = new SKSize(starSize * 2, starSize * 2)
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
            //await gameControls.canvasView[gameControls.chosenTurn].(0.75, 25);
            gameControls.chosenTurn = -1;
            idTurn = gameStateInfo.LastIdTurn;
            numTurns = idTurn;
            GameScoreLabel.Text = numTurns.ToString();

            canvasView.InvalidateSurface();
            if (gameStateInfo.GameState == GameStateEnum.END)
            {
                await DisplayAlert("Game finished", "You made " + numTurns.ToString() + " turns!" + "\n" + "Thanks for playing ;)", "OK");
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
            numTurns = game.Turns.Count - 1;

            needCheckState = true;
            NavigationPage.SetHasNavigationBar(this, false);

            this.BackgroundColor = backgroundColor;
            //this.BackgroundImageSource = ImageSource.FromFile("seashore2.png"); // background.png
            
            foreach (TurnInfo turn in game.Turns)
            {
                gameTrajectory.Add(new SKPoint(turn.State[0], turn.State[1]));
            }

            AbsoluteLayout absoluteLayout = new AbsoluteLayout();
            Image back = new Image
            {
                //HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.Start,
                Aspect = Aspect.AspectFill,
                Source = ImageSource.FromFile("seashore2.png"),
            };
            absoluteLayout.Children.Add(back, new Rectangle(0, 0, systemSettings.ScreenWidth, systemSettings.ScreenWidth * 2));//systemSettings.ScreenHeight
            /*AbsoluteLayout.SetLayoutFlags(back,
                AbsoluteLayoutFlags.WidthProportional | AbsoluteLayoutFlags.PositionProportional);*/
            /*AbsoluteLayout.SetLayoutBounds(back,
                 new Rectangle(0, 0, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));*/
            //AbsoluteLayout.SetLayoutFlags(back, AbsoluteLayoutFlags.All);


            // Create SKCanvasView to view result 
            canvasView = new SKCanvasView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Fill,
                Margin = new Thickness(0, 0, 0, 0),
                HeightRequest = systemSettings.ScreenWidth - 10,
                WidthRequest = systemSettings.ScreenWidth,
            };
            //canvasView.BindingContext = canvasView;
            //canvasView.SetBinding(HeightProperty, new Binding("Width"));
            /*Grid grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                }
            };*/
            canvasView.PaintSurface += OnCanvasViewPaintSurface;
            //grid.Children.Add(canvasView, 0, 0);

            stackLayout = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Spacing = 0,
            };
            Grid labelGrid = new Grid()
            {
                //VerticalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                },
                ColumnDefinitions =
                {
                    //new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(2.2, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
                    //new ColumnDefinition { Width = new GridLength(1.5, GridUnitType.Star) },
                },
                HorizontalOptions = LayoutOptions.Fill,
                //Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(20, 0, 20, 0),
                //Spacing = 0
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
                //HorizontalTextAlignment = TextAlignment.Start,
                BackgroundColor = backgroundColor,
                
            };
            GameScoreImage = new Image()
            {
                //HorizontalOptions = LayoutOptions.Start,
                //VerticalOptions = LayoutOptions.Center,
                BackgroundColor = backgroundColor, //Color.FromHex("#39bafa"),
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
                //HorizontalTextAlignment = TextAlignment.End,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                BackgroundColor = backgroundColor,
                //Margin = new Thickness(0, 0, 10, 0),
            };
            GameTopScoreImage = new Image()
            {
                HorizontalOptions = LayoutOptions.End,
                //VerticalOptions = LayoutOptions.Center,
                BackgroundColor = backgroundColor, //Color.FromHex("#39bafa"),
                Source = "top_score_" + game.GameSettings.Goals[game.IdPlayer] + ".png",
                HeightRequest = GameTopScoreLabel.FontSize,
                Scale = 1,
                Margin = new Thickness(0, 3, 0, 3),
            };

            GoalLabel = new Label()
            {
                Text = /*"Your goal: " + */CultureInfo.CurrentCulture.TextInfo.ToTitleCase(game.GameSettings.Goals[game.IdPlayer]),
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                //TextColor = Color.FromHex("#15c1ff"),
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

            var stackLayoutScore = new StackLayout { Orientation = StackOrientation.Horizontal };
            var stackLayoutTopScore = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.EndAndExpand };
            stackLayoutScore.Children.Add(GameScoreImage);
            stackLayoutScore.Children.Add(GameScoreLabel);
            stackLayoutTopScore.Children.Add(GameTopScoreImage);
            stackLayoutTopScore.Children.Add(GameTopScoreLabel);
            /*labelLayout.Children.Add(GameScoreImage);
            labelLayout.Children.Add(GameScoreLabel);
            labelLayout.Children.Add(GoalLabel);
            labelLayout.Children.Add(GameTopScoreImage);
            labelLayout.Children.Add(GameTopScoreLabel);*/

            /*labelGrid.Children.Add(

                GameScoreImage, 0, 0);
            labelGrid.Children.Add(GameScoreLabel, 1, 0);
            labelGrid.Children.Add(GoalLabel, 2, 0);
            labelGrid.Children.Add(GameTopScoreImage, 3, 0);
            labelGrid.Children.Add(GameTopScoreLabel, 4, 0);*/

            labelGrid.Children.Add(stackLayoutScore, 0, 0);
            labelGrid.Children.Add(GoalLabel, 1, 0);
            labelGrid.Children.Add(stackLayoutTopScore, 2, 0);

            stackLayout.Children.Add(labelGrid);
            stackLayout.Children.Add(canvasView);
            stackLayout.Children.Add(InfoTurnLabel);

            if (gameStateInfo.GameState != GameStateEnum.END)
            {
                if (game.Turns.Count == 1)
                {
                    InfoTurnLabel.Text = "Make first turn!";
                }
                gameControls = new GameControls(MakeTurnAndWait, InfoTurnLabel, game, gameStateInfo, systemSettings, backgroundColor, canvasView);
                stackLayout.Children.Add(gameControls.ControlsGrid);
            } else
            {
                InfoTurnLabel.Text = "Moves history";
                numTurns--;
            }
            GameScoreLabel.Text = numTurns.ToString();

            //AbsoluteLayout.SetLayoutFlags(stackLayout, AbsoluteLayoutFlags.All);
            //stackLayout.Children.Add(GoalLabel);
            absoluteLayout.Children.Add(stackLayout, new Rectangle(0, 0, App.ScreenWidth, App.ScreenHeight));
            Content = absoluteLayout;
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
        SKCanvasView canvasViewField;

        public bool chooseRow;
        readonly Game game;
        private bool canAnimate = true;
        public bool canMakeTurn = true;

        //public int chosenControl = 0;
        public int chosenTurn = -1;

        private readonly Dictionary<string, string> ControlsImagesNames = new Dictionary<string, string> {
            { "U", "up" }, { "L", "left" }, { "D", "down" }, { "R", "right" }
        };

        public GameControls(Action MakeTurnAndWait, Label InfoTurnLabel, Game game, GameStateInfo gameStateInfo, SystemSettings systemSettings,
            Color backgroundColor, SKCanvasView canvasViewField)
        {
            this.MakeTurnAndWait = MakeTurnAndWait;
            this.systemSettings = systemSettings;
            this.backgroundColor = backgroundColor;
            this.InfoTurnLabel = InfoTurnLabel;
            this.game = game;
            this.canvasViewField = canvasViewField;
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
            //InfoTurnLabel.Text = turnName + ". Wait...";
            InfoTurnLabel.Text = "Wait...";
            canvasViewField.InvalidateSurface();
            await Task.Delay(1000);
            MakeTurnAndWait();
        }

        void MakeGameControl()
        {
            ControlsGrid = new Grid
            {
                Margin = new Thickness(10, 10, 10, 10),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.StartAndExpand,
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
