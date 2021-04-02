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
    public partial class StandingsPage : ContentPage
    {
        public StandingsPage()
        {
            InitializeComponent();
            BindingContext = new StandingsPageViewModel(Navigation);
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
