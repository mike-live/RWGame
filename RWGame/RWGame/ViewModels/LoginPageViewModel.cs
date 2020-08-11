using Plugin.GoogleClient;
using Plugin.GoogleClient.Shared;
using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RWGame.ViewModels
{
    public class LoginPageViewModel : INotifyPropertyChanged
    {
        ServerWorker serverWorker;
        SystemSettings systemSettings;

        public string login { get; set; }
        public string password { get; set; }
        public double logoSize { get { return 100; } } // DeviceDisplay.MainDisplayInfo.Width / 3
        public class UserProfile : INotifyPropertyChanged
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public Uri Picture { get; set; }
            public event PropertyChangedEventHandler PropertyChanged;
        }
        public UserProfile User { get; set; } = new UserProfile();
        public string Name
        {
            get => User.Name;
            set => User.Name = value;
        }

        public string Email
        {
            get => User.Email;
            set => User.Email = value;
        }

        public Uri Picture
        {
            get => User.Picture;
            set => User.Picture = value;
        }

        public bool IsLoggedIn { get; set; }
        private bool isLoggingIn;
        public bool IsLoggingIn {
            get { return isLoggingIn;  }
            set {
                if (isLoggingIn != value)
                {
                    isLoggingIn = value;
                    OnPropertyChanged("IsLoggingIn");
                    OnPropertyChanged("IsNotLoggingIn");
                }
            }
        }
        public bool IsNotLoggingIn { get { return !IsLoggingIn; } }

        public string Token { get; set; }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand LoginNormalCommand { get; set; }
        public ICommand RegistrationCommand { get; set; }
        public ICommand LoginCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        
        private readonly IGoogleClientManager googleClientManager;
        public event PropertyChangedEventHandler PropertyChanged;
        public INavigation Navigation { get; set; }

        public LoginPageViewModel(SystemSettings systemSettings, INavigation navigation)
        {
            LoginNormalCommand = new Command(LoginNormal);
            RegistrationCommand = new Command(Registration);
            LoginCommand = new Command(LoginAsync);
            LogoutCommand = new Command(Logout);

            googleClientManager = CrossGoogleClient.Current;
            getCredentials();

            IsLoggedIn = false;
            IsLoggingIn = false;

            this.systemSettings = systemSettings;
            this.Navigation = navigation;
            serverWorker = new ServerWorker();
        }

        public async void LoginNormal()
        {
            saveCredentials(login, password);
            await Auth(login, password);
        }

        public async Task<bool> Auth(string login, string password, string idToken = "")
        {
            if (idToken != "")
            {
                LoginResponse loginResponse = await serverWorker.TaskLogin(login, password, idToken);
                if (loginResponse.IsAuthenticationSuccessful)
                {
                    serverWorker.UserLogin = loginResponse.Login;
                    await Navigation.PushAsync(new TabbedUserPage(serverWorker, systemSettings));
                }
                else
                {
                    if (loginResponse.ErrorId == 3)
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Please, provide additional information", "OK");
                    } else 
                    if (loginResponse.ErrorId == 1)
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Expired token", "OK");
                    } else
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Ooops. Problem with server", "OK");
                    }
                }
            } else
            if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password))
            {
                LoginResponse loginResponse = await serverWorker.TaskLogin(login, password);
                if (loginResponse.IsAuthenticationSuccessful)
                {
                    serverWorker.UserLogin = loginResponse.Login;
                    await Navigation.PushAsync(new TabbedUserPage(serverWorker, systemSettings));
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Incorrect login/password pair", "OK");
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Fields cannot be empty", "OK");
            }
            return true;
        }

        public async void Registration()
        {
            await Navigation.PushAsync(new RegistrationPage(serverWorker, systemSettings));
        }

        public async void LoginAsync()
        {
            IsLoggingIn = true;
            
            googleClientManager.OnLogin += OnLoginCompleted;
            try
            {
                await googleClientManager.LoginAsync();
            }
            catch (GoogleClientSignInNetworkErrorException e)
            {
                await App.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }
            catch (GoogleClientSignInCanceledErrorException e)
            {
                await App.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }
            catch (GoogleClientSignInInvalidAccountErrorException e)
            {
                await App.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }
            catch (GoogleClientSignInInternalErrorException e)
            {
                await App.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }
            catch (GoogleClientNotInitializedErrorException e)
            {
                await App.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }
            catch (GoogleClientBaseException e)
            {
                await App.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
            }
            IsLoggingIn = false;
        }

        private async void OnLoginCompleted(object sender, GoogleClientResultEventArgs<GoogleUser> loginEventArgs)
        {
            if (loginEventArgs.Data != null)
            {
                GoogleUser googleUser = loginEventArgs.Data;
                User.Name = googleUser.Name;
                User.Email = googleUser.Email;
                User.Picture = googleUser.Picture;
                var GivenName = googleUser.GivenName;
                var FamilyName = googleUser.FamilyName;


                // Log the current User email
                //Debug.WriteLine(User.Email);
                IsLoggedIn = true;

                var token = CrossGoogleClient.Current.AccessToken;
                var idToken = CrossGoogleClient.Current.IdToken;

                Token = token;
                await Auth("", "", idToken);
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Error", loginEventArgs.Message, "OK");
            }
            googleClientManager.OnLogin -= OnLoginCompleted;
        }

        public void Logout()
        {
            googleClientManager.OnLogout += OnLogoutCompleted;
            googleClientManager.Logout();
        }

        private void OnLogoutCompleted(object sender, EventArgs loginEventArgs)
        {
            IsLoggedIn = false;
            User.Email = "Offline";
            googleClientManager.OnLogout -= OnLogoutCompleted;
        }

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
            if (login == null) login = "";
            if (password == null) password = "";
        }
    }
}
