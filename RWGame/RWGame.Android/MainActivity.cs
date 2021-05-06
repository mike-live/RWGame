
using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.Gms.Common.Apis.Internal;
using Android.OS;
using Newtonsoft.Json;
using Plugin.GoogleClient;
using RWGame.Classes;
using System.Collections.Generic;
using System.Net;

namespace RWGame.Droid
{
    [Activity(Label = "RWGame.Android", Theme = "@style/MyTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            Xamarin.Forms.Forms.SetFlags("CarouselView_Experimental");
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            RequestedOrientation = ScreenOrientation.Portrait;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            global::Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            UserDialogs.Init(this);

            string client_id = Resources.GetString(Resource.String.Google_ClientID);            
            GoogleClientManager.Initialize(this, client_id, client_id);
            LoadApplication(new App());
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Android.Content.Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            GoogleClientManager.OnAuthCompleted(requestCode, resultCode, data);
        }
    }
}