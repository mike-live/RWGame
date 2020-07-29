using Android.App;
using Android.OS;

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