using System;
using System.Collections.Generic;
using RWGame.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RWGame.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlayWithBotPage : ContentPage
    {
        public Command PlayWithBotCommand { get; set; }
        public Command SwitchToDefaultTab { get; set; }

        public PlayWithBotPage(Command PlayWithBotCommand, Action SwitchToDefaultTab)
        {
            InitializeComponent();
            BindingContext = this;
            this.PlayWithBotCommand = PlayWithBotCommand;
            this.SwitchToDefaultTab = new Command(SwitchToDefaultTab);
        }
    }
}
