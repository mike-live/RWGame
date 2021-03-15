using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using RWGame.Models;
using RWGame.Classes;
using Xamarin.Essentials;

namespace RWGame.ViewModels
{
    class RegistrationPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public NewUserProfile User { get; set; }

        public bool IsEnabledNextButton
        {
            get
            {
                return (User.NameIsCorrect ?? false) && User.AcceptPrivacy && User.AcceptTerms;
            }
        }
        public bool IsEnabledSignUpButton =>
                    User.AcceptPrivacy && User.AcceptTerms
                    && (User.NameIsCorrect ?? false)
                    && (User.EmailIsCorrect ?? false)
                    && (User.LoginIsCorrect ?? false)
                    && (User.PasswordIsCorrect ?? false)
                    && (User.ConfirmPasswordIsCorrect ?? false);
        public int RegistrationStep { get; set; }
        public bool IsFirstStep { get { return RegistrationStep == 1; } }
        public bool IsSecondStep { get { return RegistrationStep == 2; } }
        public bool IsThirdStep { get { return RegistrationStep == 3; } }

        public async void OnSwipedRight()
        {
            await Navigation.PopAsync();
        }

        public ICommand ContinueCommand { get; set; }
        public ICommand SignUpCommand { get; set; }
        public ICommand SignInCommand { get; set; }
        public ICommand NameUnfocusedCommand { get; set; }
        public ICommand EmailUnfocusedCommand { get; set; }
        public ICommand LoginUnfocusedCommand { get; set; }
        public ICommand PasswordUnfocusedCommand { get; set; }
        public ICommand ConfirmPasswordUnfocusedCommand { get; set; }

        public ICommand TermsHyperlinkCommand { get; set; }
        public ICommand PolicyHyperlinkCommand { get; set; }

        public ICommand GoBackCommand { get; set; }

        /*public ICommand TermsUnfocusedCommand { get; set; }
        public ICommand PrivacyUnfocusedCommand { get; set; }*/
        public INavigation Navigation { get; set; }

        private readonly LoginPageViewModel loginPageViewModel;
        private readonly ServerWorker serverWorker;
        public RegistrationPageViewModel(INavigation navigation, ServerWorker serverWorker, LoginPageViewModel loginPageViewModel = null)
        {
            this.Navigation = navigation;
            this.loginPageViewModel = loginPageViewModel;
            this.serverWorker = serverWorker;
            RegistrationStep = 1;
            ContinueCommand = new Command(NextPage);
            SignUpCommand = new Command(SignUp);
            SignInCommand = new Command(SignIn);

            GoBackCommand = new Command(OnSwipedRight);

            User = new NewUserProfile(serverWorker);
            User.PropertyChanged += (obj, args) => {

                OnPropertyChanged("IsEnabledNextButton");
                OnPropertyChanged("IsEnabledSignUpButton");
            };
            if (loginPageViewModel != null && loginPageViewModel.IsGoogleSignIn)
            {
                User.Name = loginPageViewModel.User.Name;
                User.Email = loginPageViewModel.User.Email;
                User.CheckNameCorrectness();
                User.CheckEmailCorrectness();
            }
            
            NameUnfocusedCommand = new Command(User.CheckNameCorrectness);
            EmailUnfocusedCommand = new Command(User.CheckEmailCorrectness);
            LoginUnfocusedCommand = new Command(User.CheckLoginCorrectness);
            PasswordUnfocusedCommand = new Command(() => {
                User.CheckPasswordCorrectness();
                User.CheckConfirmPasswordCorrectness(false);
            }
            );
            ConfirmPasswordUnfocusedCommand = new Command(() => User.CheckConfirmPasswordCorrectness());
            TermsHyperlinkCommand = new Command(async () =>
            {
                Uri uri = new Uri("https://scigames.ru/terms");
                await Launcher.OpenAsync(uri);
            });
            PolicyHyperlinkCommand = new Command(async () =>
            {
                Uri uri = new Uri("https://scigames.ru/privacy_policy");
                await Launcher.OpenAsync(uri);
            });
        }

        public void NextPage()
        {
            RegistrationStep += 1;
        }

        public async void SignUp()
        {
            Console.WriteLine($"Sign Up!");
            bool isRegistrationSuccessful = await serverWorker.TaskRegistrateNewPlayer(
                        User.Name,
                        User.LastName,
                        User.Login,
                        User.Password,
                        User.ConfirmPassword,
                        String.Format("{0:dd-MM-yyyy}", User.Birthday), User.Email, loginPageViewModel.Token ?? "");
            if (isRegistrationSuccessful)
            {
                RegistrationStep += 1;
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Registration unsuccessful =( Try again later", "OK");
            }
        }

        public void SignIn()
        {
            if (loginPageViewModel != null)
            {
                loginPageViewModel.login = User.Login;
                loginPageViewModel.password = User.Password;
            }
            Navigation.PopAsync();
            if (loginPageViewModel != null)
            {
                loginPageViewModel.LoginAny();
            }
        }
    }
}
