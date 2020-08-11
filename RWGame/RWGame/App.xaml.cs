﻿using RWGame.Classes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace RWGame
{
    public partial class App : Application
    {
        public static int ScreenWidth;
        public static int ScreenHeight;

        public App()
        {
            InitializeComponent();
            var systemSettings = new SystemSettings()
            {
                ScreenHeight = ScreenHeight,
                ScreenWidth = ScreenWidth
            };
            MainPage = new NavigationPage(new Views.LoginPage(systemSettings));
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
