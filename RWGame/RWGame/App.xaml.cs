using RWGame;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace RWGame
{
    public partial class App : Application
    {
        const int smallWidthResolution = 768;
        const int smallHeightResolution = 1280;

        public App()
        {
            Device.SetFlags(new string[] { "AppTheme_Experimental" });

            InitializeComponent();
            LoadStyles();

            MainPage = new NavigationPage(new Views.LoginPage());
        }

        void LoadStyles()
        {
            if (IsASmallDevice())
            {
                MainDictionary.MergedDictionaries.Add(Views.Helpers.SmallDevicesStyle.SharedInstance);
            }
            else
            {
                MainDictionary.MergedDictionaries.Add(Views.Helpers.GeneralDevicesStyle.SharedInstance);
            }
        }

        public static bool IsASmallDevice()
        {
            // Get Metrics
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;

            // Width (in pixels)
            var width = mainDisplayInfo.Width;

            // Height (in pixels)
            var height = mainDisplayInfo.Height;
            return (width <= smallWidthResolution && height <= smallHeightResolution);
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
