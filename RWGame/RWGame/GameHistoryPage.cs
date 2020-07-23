using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RWGame
{
	public class GameHistoryPage : ContentPage
	{
        ServerWorker serverWorker;
        SystemSettings systemSettings;
        ListView gamesListView;
        List<UserPage.ElementsOfViewCell> customListViewRecords;
        public GameHistoryPage (ServerWorker _serverWorker, SystemSettings _systemSettings)
		{
            serverWorker = _serverWorker;
            systemSettings = _systemSettings;
            NavigationPage.SetHasNavigationBar(this, false);
            this.Title = "Games history";
            this.BackgroundColor = Color.FromHex("#39bafa");

            gamesListView = new ListView();
            StackLayout stackLayout = new StackLayout()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Fill,
                Margin = new Thickness(10, 10, 10, 10),
            };
            stackLayout.Children.Add(gamesListView);

            gamesListView.ItemTemplate = new DataTemplate(typeof(UserPage.DateCellView));
            gamesListView.ItemSelected += async delegate {
                if ((UserPage.ElementsOfViewCell)gamesListView.SelectedItem == null) return;
                Game game = await GameProcesses.MakeSavedGame(serverWorker, ((UserPage.ElementsOfViewCell)gamesListView.SelectedItem).game.IdGame);

                //await GameProcesses.StartGame(serverWorker, game, () => false);
                GameStateInfo gameStateInfo = await serverWorker.TaskGetGameState(game.IdGame);

                await Navigation.PushAsync(new GameField(serverWorker, systemSettings, game, gameStateInfo));
                gamesListView.SelectedItem = null;
                await UpdateGameList();

                //filesList.SelectedItem = null; 
            };

            gamesListView.ItemsSource = customListViewRecords;

            _ = UpdateGameList();

            this.Content = stackLayout;
        }

        public async Task UpdateGameList()
        {
            List<Game> gamesList = await serverWorker.TaskGetGamesList();
            customListViewRecords = new List<UserPage.ElementsOfViewCell>();

            if (gamesList != null && gamesList.Count > 0)
            {
                for (int i = 0; i < gamesList.Count; i++)
                {
                    if (gamesList[i].GameState == GameStateEnum.END)
                    {
                        customListViewRecords.Add(new UserPage.ElementsOfViewCell(gamesList[i]));
                    }
                }
                gamesListView.ItemsSource = customListViewRecords;
            }
            else
            {
                gamesListView.ItemsSource = null;
            }
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

        /*class DateCellView : ViewCell
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
                    Margin = new Thickness(25, 2, 25, 2),
                    Scale = 0.5
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

                gameidLabel.SetBinding(Label.TextProperty, new Binding("GameID"));
                dateLabel.SetBinding(Label.TextProperty, new Binding("Date"));
                gamestateLabel.SetBinding(Image.SourceProperty, new Binding("StateGame"));

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
            public int GameID { get; set; }
            public string Date { get; set; }
            public string StateGame { get; set; }

            /// <summary>
            /// Элемент списка
            /// </summary>
            /// <param name="_GameID">ID игры</param>
            /// <param name="_Date">Дата игры</param>
            /// <param name="_StateGame">Картинка результата игры: победа или поражение</param>
            public ElementsOfViewCell(int _GameID, string _Date, string _StateGame)
            {
                GameID = _GameID;
                Date = _Date;
                StateGame = _StateGame;
                
            }
        }*/
    }
}