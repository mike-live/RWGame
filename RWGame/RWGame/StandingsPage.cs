using RWGame.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using RWGame.Classes.ResponseClases;

namespace RWGame
{
    public class StandingsPage : ContentPage
    {
        ServerWorker serverWorker;
        SystemSettings systemSettings;
        Standings standings;
        List<StandingViewCellElement> standingsListViewRecords;
        ListView standingsListView;
        Label manPerformanceCenterLabel;
        Label manPerformanceBorderLabel;
        Label manRatingLabel;
        public StandingsPage(ServerWorker serverWorker, SystemSettings systemSettings)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            this.systemSettings = systemSettings;
            this.serverWorker = serverWorker;
            this.BackgroundColor = Color.FromHex("#39bafa");

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

            manRatingLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
            };

            manPerformanceCenterLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
            };

            manPerformanceBorderLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
            };

            var manLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                Text = "Humanity",
            };

            var standingsHeaderView = new StackLayout
            {
                Children =
                {
                    new Grid
                    {
                        RowDefinitions =
                        {
                            new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                            new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        },
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = new GridLength(0.75, GridUnitType.Star) },
                            new ColumnDefinition { Width = new GridLength(2.7, GridUnitType.Star) },
                            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                            new ColumnDefinition { Width = new GridLength(1.2, GridUnitType.Star) },
                        },
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        ColumnSpacing = 0,
                        RowSpacing = 1,
                        Margin = new Thickness(1, 3, 1, 3),
                        Children =
                        {
                            {new BoxView{Color = Color.FromHex("#3949AB").MultiplyAlpha(0.2)}, 0, 5, 0, 1 },
                            { performanceCenterImage, 2, 1 },
                            { performanceBorderImage, 3, 1 },
                            { ratingInfoLabel, 4, 1 },
                            { manLabel, 1, 0 },
                            { manPerformanceCenterLabel, 2, 0 },
                            { manPerformanceBorderLabel, 3, 0 },
                            { manRatingLabel, 4, 0 },
                        }
                    }
                }
            };

            standingsListView = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(StandingCellView)),
                IsPullToRefreshEnabled = true,
                Header = standingsHeaderView
            };

            standingsListView.RefreshCommand = new Command(async () =>
            {
                await UpdateStandings();
                standingsListView.IsRefreshing = false;
            });

            var stackLayout = new StackLayout
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                Margin = new Thickness(0, 0, 0, 0),
            };
            Label standingsLabel = new Label
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                TextColor = Color.White,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(20, 10, 20, 10),
                Text = "Humanity vs. bot"
            };
            stackLayout.Children.Add(standingsLabel);
            stackLayout.Children.Add(standingsListView);

            _ = UpdateStandings();

            Content = stackLayout;
        }

        public async Task UpdateStandings()
        {
            standings = await serverWorker.TaskGetStandings();
            standingsListViewRecords = new List<StandingViewCellElement>();
            if (standings != null && standings.StandingsVsBot.Count > 0)
            {
                for (int i = 0; i < standings.StandingsVsBot.Count; i++)
                {
                    bool isMe = standings.StandingsVsBot[i].UserName == serverWorker.UserLogin;
                    standingsListViewRecords.Add(new StandingViewCellElement(standings.StandingsVsBot[i], i + 1, isMe));
                }
                standingsListView.ItemsSource = standingsListViewRecords;

                var ManVsBot = new StandingViewCellElement(standings.ManVsBot, -1, false);

                manPerformanceCenterLabel.Text = ManVsBot.PerformanceCenter;
                manPerformanceBorderLabel.Text = ManVsBot.PerformanceBorder;
                manRatingLabel.Text = ManVsBot.Rating;
            }
            else
            {
                standingsListView.ItemsSource = null;
            }
        }

        public class StandingCellView : ViewCell
        {
            public StandingCellView()
            {
                var playerRankLabel = new Label()
                {
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    VerticalOptions = LayoutOptions.Center,
                    //BackgroundColor = Color.FromHex("#39bafa"),
                    TextColor = Color.FromHex("c5eeff"),
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(10, 0, 0, 0)
                };
                var playerUserNameLabel = new Label()
                {
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    VerticalOptions = LayoutOptions.Center,
                    //BackgroundColor = Color.FromHex("#39bafa"),
                    //Margin = new Thickness(25, 1, 25, 0),
                    TextColor = Color.White,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalTextAlignment = TextAlignment.Start,
                };
                var playerPerformanceCenterLabel = new Label()
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.Center,
                    //BackgroundColor = Color.FromHex("#39bafa"),
                    TextColor = Color.White, //Color.FromHex("#000000"),
                    FontAttributes = FontAttributes.Bold,
                    //Margin = new Thickness(25, 0, 25, 1),
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    HorizontalTextAlignment = TextAlignment.Center,
                };
                var playerPerformanceBorderLabel = new Label()
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.Center,
                    //BackgroundColor = Color.FromHex("#39bafa"),
                    //TextColor = Color.Orange,
                    TextColor = Color.White, //Color.FromHex("#000000"), // ffd966 // FFC630
                    FontAttributes = FontAttributes.Bold,
                    HorizontalTextAlignment = TextAlignment.Center,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    //Margin = new Thickness(25, 0, 25, 1)
                };

                var playerRatingLabel = new Label()
                {
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.Center,
                    //BackgroundColor = Color.FromHex("#39bafa"),
                    //TextColor = Color.Orange,
                    TextColor = Color.White,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalTextAlignment = TextAlignment.Center,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    //Margin = new Thickness(25, 0, 25, 1)
                };

                Grid grid = new Grid
                {
                    RowDefinitions =
                    {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    },
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(0.75, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(2.7, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1.2, GridUnitType.Star) },
                    },
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    //BackgroundColor = Color.FromHex("#39bafa"),
                    ColumnSpacing = 0,
                    RowSpacing = 1,
                    //Margin = new Thickness(25, 2, 25, 2)
                    Margin = new Thickness(1, 3, 1, 3),
                    //HeightRequest = 150
                };
                playerRankLabel.SetBinding(Label.TextProperty, new Binding("PlayerRank"));
                playerUserNameLabel.SetBinding(Label.TextProperty, new Binding("UserName"));
                playerPerformanceCenterLabel.SetBinding(Label.TextProperty, new Binding("PerformanceCenter"));
                playerPerformanceBorderLabel.SetBinding(Label.TextProperty, new Binding("PerformanceBorder"));
                playerRatingLabel.SetBinding(Label.TextProperty, new Binding("Rating"));
                grid.SetBinding(Grid.BackgroundColorProperty, new Binding("Color"));

                grid.Children.Add(playerRankLabel, 0, 0);
                Grid.SetColumnSpan(playerRankLabel, 1);
                Grid.SetRowSpan(playerRankLabel, 1);

                grid.Children.Add(playerUserNameLabel, 1, 0);
                Grid.SetColumnSpan(playerUserNameLabel, 1);
                Grid.SetRowSpan(playerUserNameLabel, 1);

                grid.Children.Add(playerPerformanceCenterLabel, 2, 0);
                Grid.SetColumnSpan(playerPerformanceCenterLabel, 1);
                Grid.SetRowSpan(playerPerformanceCenterLabel, 1);

                grid.Children.Add(playerPerformanceBorderLabel, 3, 0);
                Grid.SetColumnSpan(playerPerformanceBorderLabel, 1);
                Grid.SetRowSpan(playerPerformanceBorderLabel, 1);

                grid.Children.Add(playerRatingLabel, 4, 0);
                Grid.SetColumnSpan(playerRatingLabel, 1);
                Grid.SetRowSpan(playerRatingLabel, 1);

                View = grid;
            }

        }

        public class StandingViewCellElement
        {
            public PlayerStanding PlayerStanding { get; }
            readonly int playerRank;
            readonly bool isMe;
            public string PlayerRank { get { return "#" + playerRank.ToString(); } }
            public string UserName { get { return PlayerStanding.UserName; } }
            public string Rating { get { return Math.Round(PlayerStanding.Rating).ToString(); } }
            public string PerformanceCenter { get { return Math.Round(PlayerStanding.PerformanceCenter).ToString(); } }
            public string PerformanceBorder { get { return Math.Round(PlayerStanding.PerformanceBorder).ToString(); } }
            public Color Color { get { return isMe ? Color.FromHex("#3949AB").MultiplyAlpha(0.15) : Color.Transparent; } }
            public StandingViewCellElement(PlayerStanding playerStanding, int playerRank, bool isMe)
            {
                this.PlayerStanding = playerStanding;
                this.playerRank = playerRank;
                this.isMe = isMe;
            }
        }
    }
}