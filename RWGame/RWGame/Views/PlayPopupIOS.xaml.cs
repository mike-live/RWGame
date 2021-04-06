using RWGame.Classes;
using RWGame.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RWGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlayPopupIOS : Rg.Plugins.Popup.Pages.PopupPage
    {
        public PlayPopupIOS(UserViewModel userViewModel)
        {
            InitializeComponent();
            BindingContext = userViewModel;
        }


    }
}
