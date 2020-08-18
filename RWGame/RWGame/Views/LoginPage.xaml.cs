using RWGame.Classes;
using RWGame.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RWGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage(SystemSettings systemSettings)
        {
            InitializeComponent();
            BindingContext = new LoginPageViewModel(systemSettings, Navigation);

            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}