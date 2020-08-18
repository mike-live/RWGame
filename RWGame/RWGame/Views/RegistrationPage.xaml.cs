using RWGame.Classes;
using RWGame.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RWGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationPage : ContentPage
    {
        public RegistrationPage(ServerWorker serverWorker, LoginPageViewModel loginPageViewModel)
        {
            InitializeComponent();
            BindingContext = new RegistrationPageViewModel(Navigation, serverWorker, loginPageViewModel);

            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}