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
                Margin = new Thickness(10, 10, 10, 10),
            };

            gamesListView = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(DateCellView)),
                IsPullToRefreshEnabled = true,
                
            };

            gamesListView.RefreshCommand = new Command(async () =>
            {
                await UpdateGameList();
                gamesListView.IsRefreshing = false;
            });

            _ = UpdateGameList();

            gamesListView.ItemSelected += async delegate {
                if ((ElementsOfViewCell)gamesListView.SelectedItem == null) return;
                Game game = await GameProcesses.MakeSavedGame(serverWorker, ((ElementsOfViewCell)gamesListView.SelectedItem).game.IdGame);
                
                await GameProcesses.StartGame(serverWorker, game, () => false);
                GameStateInfo gameStateInfo = await serverWorker.TaskGetGameState(game.IdGame);

                await Navigation.PushAsync(new GameField(serverWorker, systemSettings, game, gameStateInfo));
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
            ImageButton PlayWithAnotherPlayer = new ImageButton()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                //Text = "PVP",
                BackgroundColor = Color.FromHex("#39bafa"),//7ad3ff
                //TextColor = Color.White,
                IsEnabled = true,
                Source = "pvp.png",
                HeightRequest = 40,
                WidthRequest = 100,
                Padding = 5
            };
            PlayWithAnotherPlayer.Clicked += async delegate
            {
                await Navigation.PushAsync(new ChoiseRealPlayerPage(serverWorker, systemSettings));
            };
            ImageButton PlayWithBot = new ImageButton()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                //Text = "Bot",
                BackgroundColor = Color.FromHex("#39bafa"),
                //TextColor = Color.White,
                Source = "bot.png",
                HeightRequest = 40,
                WidthRequest = 100,
                Padding = 5
            };
            PlayWithBot.Clicked += async delegate
            {
                Game game = await GameProcesses.MakeGameWithBot(serverWorker);
                GameStateInfo gameStateInfo = await serverWorker.TaskGetGameState(game.IdGame);

                await Navigation.PushAsync(new GameField(serverWorker, systemSettings, game, gameStateInfo));
                await UpdateGameList();
            };

            buttonStack.Children.Add(PlayWithAnotherPlayer);
            buttonStack.Children.Add(PlayWithBot);
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
                
                Image vsLabel = new Image()
                {
                    HorizontalOptions = LayoutOptions.EndAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    BackgroundColor = Color.FromHex("#39bafa"),
                    Source = "lightning.png",
                    Scale = 1
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
                    HeightRequest = 150
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
            public string GameStateImage { get { return GameStateImages[(int)game.GameState]; } }

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