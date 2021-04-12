using RWGame.Classes;
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
            Device.SetFlags(new string[] { "AppTheme_Experimental" });

            InitializeComponent();
            MainPage = new NavigationPage(new Views.LoginPage());
        }

        protected override void OnStart()
        {
            //Application.Current.UserAppTheme = OSAppTheme.Light;
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
