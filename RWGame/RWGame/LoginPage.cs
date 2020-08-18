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
    public class LoginPage : ContentPage
    {
        ServerWorker serverWorker;
        SystemSettings systemSettings;
        string login;
        string password;

        public async void saveCredentials(string s, string p)
        {
            try
            {
                await SecureStorage.SetAsync("login", s);
                //await SecureStorage.SetAsync("password", p);
            }
            catch (Exception)
            {

            }
        }

        public async void getCredentials()
        {
            try
            {
                login = await SecureStorage.GetAsync("login");
                //password = await SecureStorage.GetAsync("password");
            }
            catch (Exception)
            {

            }
        }


        public LoginPage(SystemSettings _systemSettings)
        {
            getCredentials();
            if (login == null) login = "";
            if (password == null) password = "";
            systemSettings = _systemSettings;
            serverWorker = new ServerWorker();

            NavigationPage.SetHasNavigationBar(this, false);
            this.BackgroundColor = Color.FromHex("#39bafa");
            StackLayout stackLayout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.FromHex("#39bafa")
            };
            Image logoImage = new Image()
            {
                Margin = new Thickness(10, 30, 10, 10),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = systemSettings.ScreenWidth / 3,
                HeightRequest = systemSettings.ScreenWidth / 3,
                Source = "HeadLogo.png"
            };
            Label enterLabel = new Label()
            {
                Text = "Random Walk",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            StackLayout informationLayout = new StackLayout()
            {
                Margin = new Thickness(20, 10, 20, 10)
            };
            Entry loginEntry = new Entry()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End,
                Placeholder = "Enter login",
                Text = login,
                TextColor = Color.White,
                PlaceholderColor = Color.White,
            };
            Entry passwordEntry = new Entry()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                Placeholder = "Enter password",
                Text = password,
                TextColor = Color.White,
                IsPassword = true,
                PlaceholderColor = Color.White,
            };

            Button loginButton = new Button()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Text = "Login",
                BackgroundColor = Color.FromHex("#7ad3ff"),
                //WidthRequest = systemSettings.ScreenWidth / 2,
                HeightRequest = 40,
                Padding = new Thickness(5, 0, 5, 0),
                TextColor = Color.White,
            };
            loginButton.Clicked += async delegate {
                saveCredentials(loginEntry.Text, passwordEntry.Text);
                await Auth(loginEntry.Text, passwordEntry.Text);
            }; 
            Label registrationButton = new Label()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Text = "No account yet? Create one",
                //BackgroundColor = Color.FromHex("#7ad3ff"),
                //WidthRequest = systemSettings.ScreenWidth/2,
                //BackgroundColor = this.BackgroundColor,
                TextColor = Color.LightBlue,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label))
            };
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async delegate {
                await Navigation.PushAsync(new RegistrationPage(serverWorker, systemSettings));
            };
            registrationButton.GestureRecognizers.Add(tapGestureRecognizer);
            StackLayout buttonStack = new StackLayout()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Fill,
                Margin = new Thickness(20, 10, 20, 10),
                Orientation = StackOrientation.Horizontal
            };

            informationLayout.Children.Add(loginEntry);
            informationLayout.Children.Add(passwordEntry);
            informationLayout.Children.Add(loginButton);
            informationLayout.Children.Add(registrationButton);

            stackLayout.Children.Add(logoImage);
            stackLayout.Children.Add(enterLabel);
            stackLayout.Children.Add(informationLayout);
            //stackLayout.Children.Add(loginButton);
            //stackLayout.Children.Add(registrationButton);
            //stackLayout.Children.Add(buttonStack);

            //com.google.android.gms.common.SignInButton
            //var GoogleButton = new Android.Gms.Common.SignInButton(Forms.Context);
            //stackLayout.Children.Add(GoogleButton);
            

            Content = stackLayout;

            if (loginEntry.Text.Length > 0 && passwordEntry.Text.Length > 0)
            {
                //Task.Run(async () => await Auth(loginEntry.Text, passwordEntry.Text));
            }
        }
        
        public async Task<bool> Auth(string login, string password)
        {
            if (login != null && password != null)
            {
                LoginResponse loginResponse = await serverWorker.TaskLogin(login, password);
                if (loginResponse.IsAuthenticationSuccessful)
                {
                    serverWorker.UserLogin = login;
                    await Navigation.PushAsync(new TabbedUserPage(serverWorker, systemSettings));
                }
                else
                {
                    await DisplayAlert("Error", "Incorrect login/password pair", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "Fields cannot be empty", "OK");
            }
            return true;
        }
    }
}