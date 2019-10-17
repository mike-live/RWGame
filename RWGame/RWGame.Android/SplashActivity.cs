using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace RWGame.Droid
{
    [Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        //Блокируем Freez экран и выводим логотип
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            //Thread.Sleep(3000); // Simulate a long loading process on app startup.
            StartActivity(typeof(MainActivity)); // Тут указать нашу главную Activity
        }
    }
}