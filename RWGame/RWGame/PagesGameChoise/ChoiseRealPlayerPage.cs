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
        private bool isGameStarted = false;
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
            /*Grid grid = new Grid
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
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                },
                Margin = new Thickness(5, 15, 5, 15),
            };*/

            var stackLayout = new StackLayout
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                Margin = new Thickness(10, 0, 10, 0),
                Spacing = 0,
            };

            Label headLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Text = "Play with friend",
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                Margin = new Thickness(0, 10),
            };

            Entry entryLogin = new Entry()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Placeholder = "Enter player login",
                TextColor = Color.White,
                BackgroundColor = Color.FromHex("#39bafa"),
                PlaceholderColor = Color.LightGray,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                Margin = new Thickness(0, 5),
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

            listPlayer = new ListView 
            { 
                VerticalOptions = LayoutOptions.FillAndExpand
            };
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
                BackgroundColor = Color.FromHex("#69c6f5"),
                HeightRequest = 40,
                VerticalOptions = LayoutOptions.Center,
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
                    alertMessage += "\n\nAsk your friend to update page - upper item in the list should be your game\n";
                    Game game = await GameProcesses.MakeGameWithPlayer(serverWorker, selectedIdPlayer);

                    var cancelSrc = new CancellationTokenSource();
                    var config = new ProgressDialogConfig()
                        .SetTitle(alertMessage)
                        .SetIsDeterministic(false)
                        .SetMaskType(MaskType.Gradient)
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
                        Navigation.RemovePage(this);
                        await Navigation.PushAsync(new GameField(serverWorker, systemSettings, game, gameStateInfo));
                        isGameStarted = true;
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

            Label promptLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Text = "1. Choose your friend and ask them to open the app\n2. Enter their login and tap play",
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                Margin = new Thickness(0, 10),
            };

            var stackLayoutPlay = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    entryLogin,
                    playButton
                }
            };

            stackLayout.Children.Add(headLabel);
            stackLayout.Children.Add(promptLabel);
            stackLayout.Children.Add(stackLayoutPlay);
            stackLayout.Children.Add(listPlayer);

            /*grid.Children.Add(headLabel, 0, 0);
            Grid.SetColumnSpan(headLabel, 3);
            Grid.SetRowSpan(headLabel, 1);

            grid.Children.Add(promptLabel, 0, 1);
            Grid.SetColumnSpan(promptLabel, 3);
            Grid.SetRowSpan(promptLabel, 2);

            grid.Children.Add(entryLogin, 0, 3);
            Grid.SetColumnSpan(entryLogin, 3);
            Grid.SetRowSpan(entryLogin, 1);

            grid.Children.Add(listPlayer, 0, 4);
            Grid.SetColumnSpan(listPlayer, 3);
            Grid.SetRowSpan(listPlayer, 7);

            //grid.Children.Add(backButton, 0, 11);
            //Grid.SetColumnSpan(backButton, 1);
            //Grid.SetRowSpan(backButton, 2);

            grid.Children.Add(playButton, 0, 12);
            Grid.SetColumnSpan(playButton, 3);
            Grid.SetRowSpan(playButton, 1);*/

            Content = stackLayout;
        }

        protected override void OnAppearing()
        {
            if (isGameStarted)
            {
                isGameStarted = false;
                //Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
            }
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
                    VerticalOptions = LayoutOptions.Center,
                    BackgroundColor = Color.Transparent,
                    TextColor = Color.White,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                };


                Grid grid = new Grid
                {
                    RowDefinitions =
                    {
                        new RowDefinition { Height = GridLength.Auto }
                    },
                    ColumnDefinitions =

                    {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                    },
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.Fill
                };

                loginLabel.SetBinding(Label.TextProperty, new Binding("Login"));
                
                grid.Children.Add(loginLabel, 0, 0);
                Grid.SetColumnSpan(loginLabel, 1);
                Grid.SetRowSpan(loginLabel, 1);

                View = loginLabel;
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