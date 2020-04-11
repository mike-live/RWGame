using Android.Telephony;
using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using RWGame.PagesGameChoise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RWGame
{
    public class UserPage : ContentPage
    {
        ServerWorker serverWorker;
        ListView gamesListView;
        SystemSettings systemSettings;
        List<Game> gamesList;
        List<ElementsOfViewCell> customListViewRecords;

        
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
            Label userName = new Label()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                TextColor = Color.White,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(20, 10, 20, 10),
                Text = "Hi, " + serverWorker.UserLogin
            };
            StackLayout stackLayoutListView = new StackLayout()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Fill,
                Margin = new Thickness(10, 10, 10, 10)
            };

            gamesListView = new ListView();
            gamesListView.ItemTemplate = new DataTemplate(typeof(DateCellView));
            
            UpdateGameList();

            gamesListView.ItemSelected += async delegate {
                if ((ElementsOfViewCell)gamesListView.SelectedItem == null) return;
                Game game = await GameProcesses.MakeSavedGame(serverWorker, ((ElementsOfViewCell)gamesListView.SelectedItem).game.IdGame);
                await GameProcesses.StartGame(serverWorker, game);
                await Navigation.PushAsync(new GameField(serverWorker, systemSettings, game));
                gamesListView.SelectedItem = null;
                await UpdateGameList();
            };
            stackLayoutListView.Children.Add(gamesListView);

            StackLayout buttonStack = new StackLayout()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Fill,
                Margin = new Thickness(20, 10, 20, 10),
                Orientation = StackOrientation.Horizontal
            };
            Button PlayWithAnotherPlayer = new Button()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Text = "PVP",
                BackgroundColor = Color.FromHex("#7ad3ff"),
                TextColor = Color.White
            };
            PlayWithAnotherPlayer.Clicked += async delegate
            {
                //Вызов подокна PopUp с полями
                await Navigation.PushAsync(new ChoiseRealPlayerPage(serverWorker, systemSettings));
            };
            Button PlayWithBot = new Button()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Text = "Bot",
                BackgroundColor = Color.FromHex("#7ad3ff"),
                TextColor = Color.White
            };
            Image updateImage = new Image()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Opacity = 1,
                Source = "update3.png"
            };
            PlayWithBot.Clicked += async delegate
            {
                // Запуск игры с ботом
                Game game = await GameProcesses.MakeGameWithBot(serverWorker);
                await Navigation.PushAsync(new GameField(serverWorker, systemSettings, game));
                await UpdateGameList();
            };

            var downSwipeGesture = new SwipeGestureRecognizer { Direction = SwipeDirection.Down };
            downSwipeGesture.Swiped += async delegate
            {
                updateImage.IsVisible = true;
                await Task.WhenAll(UpdateGameList(), updateImage.RotateTo(360, 1000));
                updateImage.Rotation = 0;
                updateImage.IsVisible = false;
            };

            buttonStack.Children.Add(PlayWithAnotherPlayer);
            buttonStack.Children.Add(PlayWithBot);

            userprofilStackLayout.Children.Add(updateImage);
            updateImage.IsVisible = false;
            userprofilStackLayout.GestureRecognizers.Add(downSwipeGesture);
            userprofilStackLayout.Children.Add(userName);
            userprofilStackLayout.Children.Add(stackLayoutListView);
            userprofilStackLayout.Children.Add(buttonStack);

            Content = userprofilStackLayout;
        }        

        public async Task UpdateGameList()
        {
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
            CallUpdateGameList();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        class DateCellView : ViewCell
        {
            public DateCellView()
            {
                var gameidLabel = new Label()
                {
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    VerticalOptions = LayoutOptions.EndAndExpand,
                    BackgroundColor = Color.FromHex("#39bafa"),
                    TextColor = Color.White,
                    Margin = new Thickness(25, 0, 25, 1)
                };
                var dateLabel = new Label()
                {
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    VerticalOptions = LayoutOptions.StartAndExpand,
                    BackgroundColor = Color.FromHex("#39bafa"),
                    Margin = new Thickness(25, 1, 25, 0),
                    TextColor = Color.White,
                };
                Image gamestateLabel = new Image()
                {
                    HorizontalOptions = LayoutOptions.EndAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    BackgroundColor = Color.FromHex("#39bafa"),
                    Margin = new Thickness(25, 2, 25, 2)
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
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                    },
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.FromHex("#39bafa"),
                    //Margin = new Thickness(25, 2, 25, 2)
                };

                gameidLabel.SetBinding(Label.TextProperty, new Binding("IdGame"));
                dateLabel.SetBinding(Label.TextProperty, new Binding("Date"));
                gamestateLabel.SetBinding(Image.SourceProperty, new Binding("GameStateImage"));

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

                grid.Children.Add(gamestateLabel, 1, 0);
                Grid.SetColumnSpan(gamestateLabel, 1);
                Grid.SetRowSpan(gamestateLabel, 2);

                View = grid;
            }

        }

        class ElementsOfViewCell
        {
            private readonly List<string> GameStateImages = new List<string> {
                "new.png", "connect.svg", "start.png", "active.png", "end.png", "pause.svg", "wait.png"
            };
            public Game game { get; }
            public int IdGame { get { return game.IdGame; } }
            public string Date { get { return game.Start.ToString(); } }
            public string GameState { get { return game.GameState.ToString(); } }
            public string GameStateImage { get { return GameStateImages[(int)game.GameState]; } }

            public int Player1 { get { return game.Player1 == null ? -1 : (int)game.Player1; } }
            public int Player2 { get { return game.Player2 == null ? -1 : (int)game.Player2; } }
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