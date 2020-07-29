using Acr.UserDialogs;
using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using System;
using System.Collections.Generic;
using System.Threading;
using Xamarin.Forms;

namespace RWGame.PagesGameChoise
{
    public class ChoiseRealPlayerPage : ContentPage
    {
        ListView listPlayer;
        ServerWorker serverWorker;
        private int selectedIdPlayer = -1;
        List<Player> playersList;
        private bool cancelGame = false;
        public async void CallPopAsync()
        {
            await Navigation.PopAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            cancelGame = true;
            CallPopAsync();
            return true;
        }

        public ChoiseRealPlayerPage(ServerWorker _serverWorker, SystemSettings systemSettings)
        {
            serverWorker = _serverWorker;
            this.BackgroundColor = Color.FromHex("#39bafa");
            NavigationPage.SetHasNavigationBar(this, false);
            Grid grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                Margin = new Thickness(5, 15, 5, 15),
            };

            Label headLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Text = "Select player",
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
            };

            Entry entryLogin = new Entry()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Placeholder = "Enter player login",
                TextColor = Color.White,
                BackgroundColor = Color.FromHex("#39bafa"),
                PlaceholderColor = Color.LightGray,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
            };
            //Подумай над удалением после выбора
            entryLogin.TextChanged += async delegate
            {
                try
                {
                    if (entryLogin != null && entryLogin.Text != null && entryLogin.Text.Length > 0 && !hasLoginSelected)
                    {
                        playersList = await serverWorker.TaskGetPlayerList(entryLogin.Text);
                        if (playersList != null)
                        {
                            List<ElementsOfViewCell> currentElementsList = new List<ElementsOfViewCell>();
                            for (int i = 0; i < playersList.Count; i++)
                            {
                                currentElementsList.Add(new ElementsOfViewCell(playersList[i].Login, playersList[i].IdPlayer));
                            }
                            listPlayer.ItemsSource = currentElementsList;
                            listPlayer.SelectedItem = null;
                        }
                        else
                        {
                            listPlayer.ItemsSource = null;
                        }
                    }
                    else
                    {
                        listPlayer.ItemsSource = null;
                        if (hasLoginSelected) hasLoginSelected = false;
                    }
                }
                catch (Exception ex)
                {
                    string exceptionStackTrace = ex.StackTrace;
                }

            };

            listPlayer = new ListView();
            listPlayer.ItemTemplate = new DataTemplate(typeof(DateCellView));
            listPlayer.ItemSelected += delegate
            {
                hasLoginSelected = true;
                if ((ElementsOfViewCell)listPlayer.SelectedItem == null) return;
                string selectedLogin = ((ElementsOfViewCell)listPlayer.SelectedItem).Login;
                selectedIdPlayer = ((ElementsOfViewCell)listPlayer.SelectedItem).IdPlayer;
                entryLogin.Text = selectedLogin;
                listPlayer.ItemsSource = null;
                listPlayer.SelectedItem = null;
            };
            //listPlayer.ItemsSource = 
            Button backButton = new Button()
            {
                Text = "Back",
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.FromHex("#69c6f5")
            };
            backButton.Clicked += async delegate { await Navigation.PopAsync(); };
            Button playButton = new Button()
            {
                Text = "Play!",
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.FromHex("#69c6f5")
            };
            playButton.Clicked += async delegate
            {
                //проверим введённый ник
                if (playersList != null)
                {
                    selectedIdPlayer = -1;
                    foreach (var player in playersList)
                    {
                        if (player.Login == entryLogin.Text)
                        {
                            selectedIdPlayer = player.IdPlayer;
                        }
                    }
                }

                if (entryLogin.Text == "" || selectedIdPlayer != -1) // !(await serverWorker.TaskCheckLogin(entryLogin.Text))
                {
                    string alertMessage;
                    if (entryLogin.Text == "")
                    {
                        alertMessage = "Try to find player...";
                    }
                    else
                    {
                        alertMessage = "Wait for " + entryLogin.Text;
                    }
                    Game game = await GameProcesses.MakeGameWithPlayer(serverWorker, selectedIdPlayer);

                    var cancelSrc = new CancellationTokenSource();
                    var config = new ProgressDialogConfig()
                        .SetTitle(alertMessage)
                        .SetIsDeterministic(false)
                        .SetMaskType(MaskType.Clear)
                        .SetCancel(onCancel: cancelSrc.Cancel);

                    using (UserDialogs.Instance.Progress(config))
                    {
                        await GameProcesses.StartGame(serverWorker, game, () => cancelSrc.Token.IsCancellationRequested);
                    }

                    //UserDialogs.Instance.ShowLoading(alertMessage);
                    cancelGame = cancelSrc.IsCancellationRequested;
                    //UserDialogs.Instance.HideLoading();

                    if (!cancelGame)
                    {
                        GameStateInfo gameStateInfo = await serverWorker.TaskGetGameState(game.IdGame);
                        await Navigation.PushAsync(new GameField(serverWorker, systemSettings, game, gameStateInfo));
                    }
                    else
                    {
                        await serverWorker.TaskCancelGame(game.IdGame);
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Enter player doesn't exist", "OK");
                }
            };

            grid.Children.Add(headLabel, 0, 0);
            Grid.SetColumnSpan(headLabel, 3);
            Grid.SetRowSpan(headLabel, 1);

            grid.Children.Add(entryLogin, 0, 1);
            Grid.SetColumnSpan(entryLogin, 3);
            Grid.SetRowSpan(entryLogin, 2);

            grid.Children.Add(listPlayer, 0, 3);
            Grid.SetColumnSpan(listPlayer, 3);
            Grid.SetRowSpan(listPlayer, 7);

            /*grid.Children.Add(backButton, 0, 11);
            Grid.SetColumnSpan(backButton, 1);
            Grid.SetRowSpan(backButton, 2);*/

            grid.Children.Add(playButton, 0, 11);
            Grid.SetColumnSpan(playButton, 3);
            Grid.SetRowSpan(playButton, 1);

            Content = grid;

        }

        bool hasLoginSelected = false;

        class DateCellView : ViewCell
        {
            public DateCellView()
            {
                //instantiate each of our views
                var loginLabel = new Label()
                {
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    BackgroundColor = Color.Transparent,
                    TextColor = Color.White
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
                    VerticalOptions = LayoutOptions.FillAndExpand
                };

                loginLabel.SetBinding(Label.TextProperty, new Binding("Login"));
                loginLabel.FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label));
                loginLabel.FontAttributes = FontAttributes.Bold;


                grid.Children.Add(loginLabel, 0, 0);
                Grid.SetColumnSpan(loginLabel, 2);
                Grid.SetRowSpan(loginLabel, 2);


                View = grid;
            }

        }

        class ElementsOfViewCell
        {
            public string Login { get; set; }
            public int IdPlayer { get; set; }

            public ElementsOfViewCell(string _Login, int _IdPlayer)
            {
                Login = _Login;
                IdPlayer = _IdPlayer;
            }
        }
    }
}