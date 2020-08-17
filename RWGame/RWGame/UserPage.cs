using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using RWGame.PagesGameChoise;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SkiaSharp;
using SkiaSharp.Views.Forms;
//using System.Text;
//using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using Android.Renderscripts;
using Android.Hardware.Camera2.Params;
using Android.Graphics.Text;
using Android.Animation;
//using Xamarin.Forms.Platform.Android;

namespace RWGame
{
    public class CarouselItem
    {
        public ImageSource Picture { get; set; }
        public string text { get; set; }
    }
    
    public class GuideStep
    {
        public View button { get; set; }
        public string guidePhrase { get; set; }
        public SKCanvasView canvasView { get; set; }

        public GuideStep()
        {
            button = new Button();
            guidePhrase = "";
            canvasView = new SKCanvasView();
        }
        public GuideStep(View _button, string _guidePhrase, SKCanvasView _canvasView)
        {
            button = _button;
            guidePhrase = _guidePhrase;
            canvasView = _canvasView;
        }
    }


    public class UserPage : ContentPage
    {
        ServerWorker serverWorker;
        ListView gamesListView;
        SystemSettings systemSettings;
        List<Game> gamesList;
        List<ElementsOfViewCell> customListViewRecords;
        Label gameListViewEmptyMessage;
        PlayerInfo playerInfo;
        Label userName;
        Label performanceCenterLabel;
        Label performanceBorderLabel;
        Label RatingLabel;
        String guidePhrase;
        SKCanvasView canvasView;
        View tempView;
        AbsoluteLayout absoluteLayout;
        List<GuideStep> introGuide;
        List<string> guidePhrases;
        int widening = 2;
        int dpi = (int)Android.App.Application.Context.Resources.DisplayMetrics.DensityDpi;
        bool isGuideActive = true;
        bool isGameStarted = false;
        public UserPage(ServerWorker _serverWorker, SystemSettings _systemSettings)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            this.systemSettings = _systemSettings;
            this.serverWorker = _serverWorker;
            this.Title = "Started games";
            
            StackLayout userprofilStackLayout = new StackLayout()
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill
            };
            StackLayout globalStackLayout = new StackLayout()
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill
            };
            absoluteLayout = new AbsoluteLayout()
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill
            };
            userName = new Label()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                TextColor = Color.White,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(20, 10, 20, 10),
            };

            var statisticsInfoLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center,
                //BackgroundColor = Color.FromHex("#39bafa"),
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                TextDecorations = TextDecorations.Underline,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Statistics",
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                Margin = new Thickness(0, 3, 0, 3),
            };

            performanceCenterLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center,
                //BackgroundColor = Color.FromHex("#39bafa"),
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
            };

            performanceBorderLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center,
                //BackgroundColor = Color.FromHex("#39bafa"),
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
            };

            RatingLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center,
                //BackgroundColor = Color.FromHex("#39bafa"),
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
            };
            var ratingInfoLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center,
                //BackgroundColor = Color.FromHex("#39bafa"),
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Rating",
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
            };

            Image performanceBorderImage = new Image()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center,
                //BackgroundColor = Color.FromHex("#39bafa"),
                Source = "top_score_border.png",
                Margin = 0,
                HeightRequest = ratingInfoLabel.FontSize
            };

            Image performanceCenterImage = new Image()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center,
                //BackgroundColor = Color.FromHex("#39bafa"),
                Source = "top_score_center.png",
                Margin = 0,
                HeightRequest = ratingInfoLabel.FontSize
            };

            Grid gridPlayerInfo = new Grid
            {
                RowDefinitions =
                    {
                        new RowDefinition { Height = GridLength.Auto },
                    },
                ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
                    },
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                ColumnSpacing = 1,
                RowSpacing = 0,
                Margin = new Thickness(0, 10, 5, 0)
            };

            Grid gridPlayerScore = new Grid
            {
                ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    },
                RowDefinitions =
                    {
                        new RowDefinition { Height = GridLength.Auto },
                        new RowDefinition { Height = GridLength.Auto },
                        new RowDefinition { Height = GridLength.Auto },
                    },
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                ColumnSpacing = 5,
                RowSpacing = 0,
                Margin = new Thickness(0, 0, 5, 0)
            };

            gridPlayerInfo.Children.Add(userName, 0, 0);
            Grid.SetColumnSpan(userName, 3);
            Grid.SetRowSpan(userName, 3);

            gridPlayerScore.Children.Add(performanceCenterLabel, 0, 2);
            Grid.SetColumnSpan(performanceCenterLabel, 1);
            Grid.SetRowSpan(performanceCenterLabel, 1);

            gridPlayerScore.Children.Add(performanceCenterImage, 0, 1);
            Grid.SetColumnSpan(performanceCenterImage, 1);
            Grid.SetRowSpan(performanceCenterImage, 1);

            gridPlayerScore.Children.Add(performanceBorderLabel, 1, 2);
            Grid.SetColumnSpan(performanceBorderLabel, 1);
            Grid.SetRowSpan(performanceBorderLabel, 1);

            gridPlayerScore.Children.Add(performanceBorderImage, 1, 1);
            Grid.SetColumnSpan(performanceBorderImage, 1);
            Grid.SetRowSpan(performanceBorderImage, 1);

            gridPlayerScore.Children.Add(RatingLabel, 2, 2);
            Grid.SetColumnSpan(RatingLabel, 1);
            Grid.SetRowSpan(RatingLabel, 1);

            gridPlayerScore.Children.Add(ratingInfoLabel, 2, 1);
            Grid.SetColumnSpan(ratingInfoLabel, 1);
            Grid.SetRowSpan(ratingInfoLabel, 1);

            gridPlayerScore.Children.Add(statisticsInfoLabel, 0, 0);
            Grid.SetColumnSpan(statisticsInfoLabel, 3);
            Grid.SetRowSpan(statisticsInfoLabel, 1);

            gridPlayerInfo.Children.Add(gridPlayerScore, 1, 0 );
            Grid.SetColumnSpan(gridPlayerScore, 3);
            Grid.SetRowSpan(gridPlayerScore, 3);


            var actionStandings = new TapGestureRecognizer();
            actionStandings.Tapped += async (s, e) =>
            {
                await Navigation.PushAsync(new StandingsPage(serverWorker, systemSettings));
            };
            gridPlayerScore.GestureRecognizers.Add(actionStandings);

            StackLayout stackLayoutListView = new StackLayout()
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.Fill,
                Margin = new Thickness(10, 0, 0, 0),
            };

            gamesListView = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(DateCellView)),
                IsPullToRefreshEnabled = true,
            };

            gameListViewEmptyMessage = new Label
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Color.White,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(20, 10, 20, 10),
                Text = "Here we place your current games.\nTo play tap bot or friend.",
                HorizontalTextAlignment = TextAlignment.Center,
                IsVisible = false
            };

            gamesListView.RefreshCommand = new Command(async () =>
            {
                await UpdateGameList();
                gamesListView.IsRefreshing = false;
            });

            _ = UpdateGameList();

            gamesListView.ItemSelected += async delegate
            {
                if ((ElementsOfViewCell)gamesListView.SelectedItem == null) return;
                if (isGameStarted) return;
                Game game = await GameProcesses.MakeSavedGame(serverWorker, ((ElementsOfViewCell)gamesListView.SelectedItem).game.IdGame);

                bool cancelGame = await GameProcesses.StartGame(serverWorker, game);

                if (!cancelGame)
                {
                    GameStateInfo gameStateInfo = await serverWorker.TaskGetGameState(game.IdGame);
                    await Navigation.PushAsync(new GameField(serverWorker, systemSettings, game, gameStateInfo));
                    isGameStarted = true;
                }
                gamesListView.SelectedItem = null;
                //await UpdateGameList();
            };
            stackLayoutListView.Children.Add(gameListViewEmptyMessage);
            stackLayoutListView.Children.Add(gamesListView);

            StackLayout buttonStack = new StackLayout()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Fill,
                Margin = new Thickness(20, 10, 20, 50),
                Orientation = StackOrientation.Horizontal
            };
            var stackLayoutPlayWithAnotherPlayer = new StackLayout
            {
                Spacing = 0,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
            };
            Label labelPlayWithAnotherPlayer = new Label()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Color.White,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                Text = "Friend"
            };

            ImageButton PlayWithAnotherPlayer = new ImageButton()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                //Text = "PVP",
                BackgroundColor = Color.FromHex("#39bafa"),//7ad3ff
                //TextColor = Color.White,
                IsEnabled = true,
                Source = "pvp.png",
                HeightRequest = 30,
                WidthRequest = 100,
                Padding = 0
            };
            PlayWithAnotherPlayer.Clicked += async delegate
            {
                if (isGameStarted) return;
                isGameStarted = true;
                await Navigation.PushAsync(new ChoiseRealPlayerPage(serverWorker, systemSettings));
            };


            var stackLayoutPlayWithBot = new StackLayout
            {
                Spacing = 0,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
            };
            Label labelPlayWithBot = new Label()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Color.White,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                Text = "Bot"
            };
            ImageButton PlayWithBot = new ImageButton()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                //Text = "Bot",
                BackgroundColor = Color.FromHex("#39bafa"),
                //TextColor = Color.White,
                Source = "bot.png",
                HeightRequest = 30,
                WidthRequest = 100,
                Padding = 0,
            };
            PlayWithBot.Clicked += async delegate
            {
                if (isGameStarted) return;
                isGameStarted = true;
                Game game = await GameProcesses.MakeGameWithBot(serverWorker);
                GameStateInfo gameStateInfo = await serverWorker.TaskGetGameState(game.IdGame);

                await Navigation.PushAsync(new GameField(serverWorker, systemSettings, game, gameStateInfo));
                await UpdateGameList();
            };  
            var guide = new CarouselView
            {
                IsVisible = false,
                IsEnabled = false,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Always
            };  
            
            var guideImages = new ObservableCollection<CarouselItem>();
            guideImages.Add(new CarouselItem { Picture = ImageSource.FromFile("guidePlayWithBot.png") });
            guideImages.Add(new CarouselItem { Picture = ImageSource.FromFile("guidePlayWithFriend.png") });
            guideImages.Add(new CarouselItem { Picture = ImageSource.FromFile("guideRating.png") });
            guideImages.Add(new CarouselItem { Picture = ImageSource.FromFile("guideGoal.png") });
            guideImages.Add(new CarouselItem { Picture = ImageSource.FromFile("guideMove.png") });
            guideImages.Add(new CarouselItem { Picture = ImageSource.FromFile("guideScore.png") });
         
            StackLayout stackLayoutHelp = new StackLayout
            {
                Spacing = 0,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
            };
            Label helpLabel = new Label
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Color.White,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                Text = "Help"
            };
            ImageButton helpButton = new ImageButton
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                BackgroundColor = Color.FromHex("#39bafa"),
                Source = "help.png",
                //IsEnabled = true,
                HeightRequest = 30,
                WidthRequest = 100,
                Padding = 0
            };
            helpButton.Clicked += delegate
            {
                StartGuide(guideImages, guide);
            };
            canvasView = new SKCanvasView
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Margin = new Thickness(0, 0, 0, 0),
                IsEnabled = false,
                IsVisible = false,
                HeightRequest = systemSettings.ScreenHeight,
                WidthRequest = systemSettings.ScreenWidth,
            };
            Label infoLabel = new Label
            {
                Text = "Press button",
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center,
            };

            introGuide = new List<GuideStep>();
            guidePhrases = new List<String>();
            guidePhrases.Add("Play with a bot!");
            guidePhrases.Add("Play with a friend!");
            guidePhrases.Add("Check out your rating!");
            guidePhrases.Add("Check out our guide!");
            introGuide.Add(new GuideStep(stackLayoutHelp, guidePhrases[3], canvasView));
            introGuide.Add(new GuideStep(stackLayoutPlayWithBot, guidePhrases[0], canvasView));
            introGuide.Add(new GuideStep(stackLayoutPlayWithAnotherPlayer, guidePhrases[1], canvasView));
            introGuide.Add(new GuideStep(gridPlayerScore, guidePhrases[2], canvasView));
            
            canvasView.PaintSurface += OnCanvasViewPaintSurface;

            if (!Application.Current.Properties.ContainsKey("FirstUse"))
            {
                Application.Current.Properties["FirstUse"] = false;
                //Do things when it IS the first use...
                isGuideActive = true;
                StartIntroGuide(introGuide);
            }
            
            stackLayoutPlayWithAnotherPlayer.Children.Add(PlayWithAnotherPlayer);
            stackLayoutPlayWithAnotherPlayer.Children.Add(labelPlayWithAnotherPlayer);

            stackLayoutPlayWithBot.Children.Add(PlayWithBot);
            stackLayoutPlayWithBot.Children.Add(labelPlayWithBot);

            stackLayoutHelp.Children.Add(helpButton);
            stackLayoutHelp.Children.Add(helpLabel);

            buttonStack.Children.Add(stackLayoutPlayWithAnotherPlayer);
            buttonStack.Children.Add(stackLayoutPlayWithBot);
            buttonStack.Children.Add(stackLayoutHelp);

            userprofilStackLayout.Children.Add(gridPlayerInfo);
            userprofilStackLayout.Children.Add(stackLayoutListView);
            userprofilStackLayout.Children.Add(buttonStack);
            

            globalStackLayout.Children.Add(userprofilStackLayout);
            //globalStackLayout.Children.Add(canvasView);

            
            absoluteLayout.Children.Add(globalStackLayout, new Rectangle(0, 0, App.ScreenWidth, App.ScreenHeight));
            absoluteLayout.Children.Add(canvasView, new Rectangle(0, 0, App.ScreenWidth, App.ScreenHeight));
            //absoluteLayout.Children.Add(buttonStack, new Rectangle(0, App.ScreenHeight - 120, App.ScreenWidth, PlayWithBot.Height));
            Content = absoluteLayout;
        }
        
        void StartIntroGuide(List<GuideStep> guide)
        {
            int countTapped = 0;
            canvasView.IsVisible = true;
            canvasView.IsEnabled = true;
            PerformGuideStep(guide[0]);
            TapGestureRecognizer localCanvasTappedRecognizer = new TapGestureRecognizer();
            canvasView.GestureRecognizers.Add(localCanvasTappedRecognizer);
            localCanvasTappedRecognizer.Tapped += delegate
            {          
                if (countTapped + 1 < guide.Count() && guide[countTapped] != null)
                {
                    countTapped++;
                    PerformGuideStep(guide[(countTapped)]);
                }
                else
                {
                    canvasView.IsVisible = false;
                    canvasView.IsEnabled = false;
                    countTapped = 0;
                    return;
                }   
            };
            countTapped = 0;
        }
        void PerformGuideStep(GuideStep step)
         {
            tempView = step.button;
            guidePhrase = step.guidePhrase;
            canvasView.InvalidateSurface();
        }
         void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
         {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear();
            //float vx = GetAbsoluteX(canvasView);
            float vy = GetAbsoluteY(canvasView);
            if (isGuideActive)
            {
                if (tempView != null)
                {
                    float x = GetAbsoluteX(tempView);
                    float y = GetAbsoluteY(tempView);
                    Rectangle rect = new Rectangle( // White rectangle around a clickable item
                        DpToPx(x - widening), 
                        DpToPx(y - vy - widening), 
                        DpToPx(tempView.Width + widening * 2), 
                        DpToPx(tempView.Height + widening));
                    Darken(canvas, rect);
                    DrawLightRect(canvas, rect);
                    DisplayText(canvas, guidePhrase, rect);
                    absoluteLayout.Children.Add(canvasView, new Rectangle(0, 0, App.ScreenWidth, App.ScreenHeight));
                }
            }
        }
        float DpToPx(double value)
        {
            return (float)(value * dpi / 160f);
        }
        float GetAbsoluteX(View view)
        {
            var parent = view.ParentView;
            double x = view.X;
            while (parent != null)
            {
                x += parent.X;
                parent = parent.ParentView;
            }
            return (float)x;
        }
        float GetAbsoluteY(View view)
        {
            var parent = view.ParentView;
            double y = view.Y;
            while (parent != null)
            {
                y += parent.Y;
                parent = parent.ParentView;
            }
            return (float)y;
        }
        void Darken(SKCanvas canvas, Rectangle rect)
        {
            SKColor hex;
            SKColor.TryParse("#66000000", out hex);
            SKPaint paint = new SKPaint
            {
                Color = hex,
                Style = SKPaintStyle.Fill,
            };
            canvas.DrawRect(0, 0, (float)rect.X, DpToPx(App.ScreenHeight), paint); // left
            canvas.DrawRect((float)(rect.X + rect.Width), 0, 
              (float)(DpToPx(App.ScreenWidth) - (rect.X + rect.Width)), DpToPx(App.ScreenHeight), paint); // right
            canvas.DrawRect((float)rect.X, 0, (float)rect.Width, (float)rect.Y, paint); // center top
            canvas.DrawRect((float)rect.X, (float)(rect.Y + rect.Height),
                (float)rect.Width, (float)(DpToPx(App.ScreenHeight) - rect.Y - rect.Height), paint); // center bottom
        }
        void DrawLightRect(SKCanvas canvas, Rectangle rect)
         {
            SKPaint paintStroke = new SKPaint
            {
                Color = SKColors.White,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 3,
            };
            canvas.DrawRect((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height, paintStroke);
        }
         void DisplayText(SKCanvas canvas, string text, Rectangle whiteRect)
         {
            SKPaint textPaint = new SKPaint
            {
                Color = SKColors.White,
                TextSize = 45.0f,
                IsLinearText = true
            };
            SKPaint strokePaint = new SKPaint
            {
                Color = SKColors.White,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 3
            };
            int textWidening = 5;
            float tlX, tlY, brX, brY; // top left X, top left Y, bottom right X, bottom right Y
            float textWidth = textPaint.MeasureText(text);
            float radius = 50.0f;
            float mgn = 60;
            //if (whiteRect.Y < DpToPx(App.ScreenHeight / 2))
            //    mgn = -mgn - (float)whiteRect.Height;
            tlX = (float)(whiteRect.X - DpToPx(textWidening));
            tlY = (float)(whiteRect.Y - DpToPx(textWidening + mgn));
            brX = (float)(whiteRect.X + textWidth + DpToPx(2 * textWidening));
            brY = (float)(whiteRect.Y + DpToPx(textPaint.TextSize) + DpToPx(2 * textWidening) - DpToPx(mgn));           
            if (tlX < 0)
            {
                tlX = 0;
                tlY = (float)(whiteRect.Y - DpToPx(textWidening));
                brX = (float)(2 * Math.Abs(whiteRect.X) + textWidth + DpToPx(2 * textWidening + mgn));
                brY = (float)(whiteRect.Y + DpToPx(textPaint.TextSize) + DpToPx(2 * textWidening) - DpToPx(mgn));
            }
            if (brX > DpToPx(App.ScreenWidth))
            {
                if (whiteRect.Y > DpToPx(App.ScreenHeight / 2))
                {
                    tlX = (DpToPx(App.ScreenWidth) - (textWidth + DpToPx(2 * textWidening)));
                    tlY = (float)(whiteRect.Y - DpToPx(textWidening + mgn));
                    brX = DpToPx(App.ScreenWidth);
                    brY = (float)(whiteRect.Y + DpToPx(textPaint.TextSize) + DpToPx(2 * textWidening) - DpToPx(mgn));
                }
                else
                {
                    tlX = (DpToPx(App.ScreenWidth) - (textWidth + DpToPx(2 * textWidening)));
                    tlY = (float)(whiteRect.Y + DpToPx(textWidening + mgn));
                    brX = DpToPx(App.ScreenWidth);
                    brY = (float)(whiteRect.Y + DpToPx(textPaint.TextSize) + DpToPx(2 * textWidening) + DpToPx(mgn));
                }
                    
            }
            SKRect rect = new SKRect(tlX, tlY, brX, brY);
            SKRoundRect roundRect = new SKRoundRect(rect, radius);
            canvas.DrawRoundRect(roundRect, strokePaint);
            canvas.DrawText(text, tlX + DpToPx(textWidening), tlY + (brY - tlY + textPaint.TextSize) / 2, textPaint);
        }      
         
        void StartGuide(ObservableCollection<CarouselItem> images, CarouselView guide)
        {
            int i = 0;
            DataTemplate template = new DataTemplate(() =>
            {
                Image image = new Image();
                TapGestureRecognizer guideImgTapped = new TapGestureRecognizer();
                guideImgTapped.Tapped += delegate
                {
                    if (i + 1 < images.Count())
                    {
                        guide.ScrollTo(++i);
                    }
                    else
                    {
                        guide.IsEnabled = false;
                        guide.IsVisible = false;
                    }
                };
                image.GestureRecognizers.Add(guideImgTapped);
                image.SetBinding(Image.SourceProperty, "Picture");

                return image;
            });

            guide = new CarouselView
            {
                ItemsSource = images,
                ItemTemplate = template,
                IsVisible = true,
                IsEnabled = true,
                IsSwipeEnabled = false
            };
            absoluteLayout.Children.Add(guide, new Rectangle(20, 20, App.ScreenWidth - 40, App.ScreenHeight - 100));
        }
        public async Task UpdatePlayerInfo()
        {
            playerInfo = await serverWorker.TaskGetPlayerInfo();
            userName.Text = "Hi, " + playerInfo.PersonalInfo.Name;
            if (playerInfo.PlayerStatistics.PerformanceBorderVsBot.HasValue)
            {
                performanceBorderLabel.Text = Math.Round(playerInfo.PlayerStatistics.PerformanceBorderVsBot.Value).ToString();
            }
            if (playerInfo.PlayerStatistics.PerformanceCenterVsBot.HasValue)
            {
                performanceCenterLabel.Text = Math.Round(playerInfo.PlayerStatistics.PerformanceCenterVsBot.Value).ToString();
            }
            if (playerInfo.PlayerStatistics.RatingVsBot.HasValue)
            {
                RatingLabel.Text = Math.Round(playerInfo.PlayerStatistics.RatingVsBot.Value).ToString();
            }
        }

        public async Task UpdateGameList()
        {
            await UpdatePlayerInfo();
            gamesList = await serverWorker.TaskGetGamesList();
            customListViewRecords = new List<ElementsOfViewCell>();

            if (gamesList != null && gamesList.Count > 0)
            {
                for (int i = 0; i < gamesList.Count; i++)
                {
                    if (gamesList[i].GameState != GameStateEnum.END)
                    {
                        customListViewRecords.Add(new ElementsOfViewCell(gamesList[i]));
                    }
                }
                gamesListView.ItemsSource = customListViewRecords;
            }
            else
            {
                gamesListView.ItemsSource = null;
            }

            if (customListViewRecords.Count == 0)
            {
                gamesListView.IsVisible = false;
                gameListViewEmptyMessage.IsVisible = true;
            }
            else
            {
                gamesListView.IsVisible = true;
                gameListViewEmptyMessage.IsVisible = false;
            }
        }

        public Game GetGame(int idGame)
        {
            for (int i = 0; i < gamesList.Count; i++)
            {
                if (gamesList[i].IdGame == idGame)
                {
                    return gamesList[i];
                }
            }
            return null;
        }

        public async void CallUpdateGameList()
        {
            await UpdateGameList();
        }

        protected override void OnAppearing()
        {
            isGameStarted = false;
            CallUpdateGameList();
            //butX = (float)tempImageButton.X;
            //butY = (float)tempImageButton.Y;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        public class DateCellView : ViewCell
        {
            public DateCellView()
            {
                var gameidLabel = new Label()
                {
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    BackgroundColor = Color.FromHex("#39bafa"),
                    TextColor = Color.FromHex("c5eeff"),
                    //Margin = new Thickness(25, 0, 25, 1)
                };
                var dateLabel = new Label()
                {
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    BackgroundColor = Color.FromHex("#39bafa"),
                    //Margin = new Thickness(25, 1, 25, 0),
                    TextColor = Color.FromHex("c5eeff"),
                };
                var player1Label = new Label()
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    BackgroundColor = Color.FromHex("#39bafa"),
                    TextColor = Color.FromHex("#FFFFFF"), //Color.FromHex("#000000"),
                    FontAttributes = FontAttributes.Bold,
                    //Margin = new Thickness(25, 0, 25, 1),

                };
                var player2Label = new Label()
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    BackgroundColor = Color.FromHex("#39bafa"),
                    //TextColor = Color.Orange,
                    TextColor = Color.FromHex("#FFFFFF"), //Color.FromHex("#000000"), // ffd966 // FFC630
                    FontAttributes = FontAttributes.Bold,
                    HorizontalTextAlignment = TextAlignment.End,
                    //Margin = new Thickness(25, 0, 25, 1)
                };


                Image vsLabel = new Image()
                {
                    HorizontalOptions = LayoutOptions.EndAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    BackgroundColor = Color.FromHex("#39bafa"),
                    Source = "lightning.png",
                    Scale = 1
                };

                var scoreLabel = new Label()
                {
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    BackgroundColor = Color.FromHex("#39bafa"),
                    //TextColor = Color.Orange,
                    TextColor = Color.White,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalTextAlignment = TextAlignment.Center,
                    //Margin = new Thickness(25, 0, 25, 1)
                };
                Image gamestateLabel = new Image()
                {
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    BackgroundColor = Color.FromHex("#39bafa"),
                    //Margin = new Thickness(25, 2, 25, 2),
                    Scale = 0.9
                };

                Grid grid = new Grid
                {
                    RowDefinitions =
                    {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                    },
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(1.8, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(0.4, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    },
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.FromHex("#39bafa"),
                    ColumnSpacing = 0,
                    RowSpacing = 1,
                    //Margin = new Thickness(25, 2, 25, 2)
                    Margin = new Thickness(1, 3, 1, 3),
                    //HeightRequest = 150
                };
                gameidLabel.SetBinding(Label.TextProperty, new Binding("IdGame"));
                dateLabel.SetBinding(Label.TextProperty, new Binding("Date"));
                gamestateLabel.SetBinding(Image.SourceProperty, new Binding("GameStateImage"));
                player1Label.SetBinding(Label.TextProperty, new Binding("PlayerName1"));
                player2Label.SetBinding(Label.TextProperty, new Binding("PlayerName2"));
                scoreLabel.SetBinding(Label.TextProperty, new Binding("Score"));

                gameidLabel.FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label));
                gameidLabel.FontAttributes = FontAttributes.Bold;
                dateLabel.FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label));
                dateLabel.FontAttributes = FontAttributes.Bold;

                grid.Children.Add(gameidLabel, 0, 0);
                Grid.SetColumnSpan(gameidLabel, 1);
                Grid.SetRowSpan(gameidLabel, 1);

                grid.Children.Add(dateLabel, 0, 1);
                Grid.SetColumnSpan(dateLabel, 1);
                Grid.SetRowSpan(dateLabel, 1);

                grid.Children.Add(gamestateLabel, 4, 0);
                Grid.SetColumnSpan(gamestateLabel, 1);
                Grid.SetRowSpan(gamestateLabel, 1);

                grid.Children.Add(scoreLabel, 4, 1);
                Grid.SetColumnSpan(scoreLabel, 1);
                Grid.SetRowSpan(scoreLabel, 1);

                grid.Children.Add(player1Label, 1, 0);
                Grid.SetColumnSpan(player1Label, 1);
                Grid.SetRowSpan(player1Label, 2);

                grid.Children.Add(vsLabel, 2, 0);
                Grid.SetColumnSpan(vsLabel, 1);
                Grid.SetRowSpan(vsLabel, 2);

                grid.Children.Add(player2Label, 3, 0);
                Grid.SetColumnSpan(player2Label, 1);
                Grid.SetRowSpan(player2Label, 2);

                View = grid;
            }

        }

        public class ElementsOfViewCell
        {
            private readonly List<string> GameStateImages = new List<string> {
                //"new.png",           "connect.png",         "start.png",           "active.png",     "end.png",        "pause.svg",      "wait.png"
                "state_star_gray.png", "state_star_gray.png", "state_star_gray.png", "state_star.png", "state_star.png", "state_star.png", "state_star.png"
            };
            public Game game { get; }
            public string IdGame { get { return "#" + game.IdGame.ToString(); } }
            public string Date { get { return game.Start.ToString(); } }
            public string PlayerName1 { get { return game.PlayerUserName1; } }
            public string PlayerName2 { get { return game.PlayerUserName2; } }
            public string GameState { get { return game.GameState.ToString(); } }
            public string GameStateImage { 
                get { 
                    if (new[] { GameStateEnum.NEW, GameStateEnum.CONNECT, GameStateEnum.START }.Contains(game.GameState))
                    {
                        return "state_star_gray.png";
                    }
                    else
                    {
                        return "state_star_" + game.GameSettings.Goals[game.IdPlayer] + ".png";
                    }
                } 
            }

            public int Player1 { get { return game.Player1 == null ? -1 : (int)game.Player1; } }
            public int Player2 { get { return game.Player2 == null ? -1 : (int)game.Player2; } }
            public string Score { get { return Convert.ToString(game.Score); } }
            public GameSettings Settings { get { return game.GameSettings; } }

            /// <summary>
            /// Элемент списка
            /// </summary>
            /// <param name="_GameID">ID игры</param>
            /// <param name="_Date">Дата игры</param>
            /// <param name="_StateGame">Картинка результата игры: победа или поражение</param>
            public ElementsOfViewCell(Game _game)
            {
                game = _game;
            }

        }
    }
}