using System;
using RWGame.Views;
using Xamarin.Forms;
using RWGame.iOS.Renderers;
using Xamarin.Forms.Platform.iOS;
using UIKit;

[assembly: ExportRenderer(typeof(UserPage), typeof(Page_iOS))]
[assembly: ExportRenderer(typeof(RealPlayerChoicePage), typeof(Page_iOS))]
[assembly: ExportRenderer(typeof(GameHistoryPage), typeof(Page_iOS))]
[assembly: ExportRenderer(typeof(StandingsPage), typeof(Page_iOS))]
[assembly: ExportRenderer(typeof(RegistrationPage), typeof(Page_iOS))]
namespace RWGame.iOS.Renderers
{
    public class Page_iOS : PageRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(false);

            UIKit.UIGraphics.BeginImageContext(this.View.Frame.Size);
            UIKit.UIImage i = UIKit.UIImage.FromBundle("background_2_art_1.png");

            i = i.Scale(this.View.Frame.Size);

            this.View.BackgroundColor = UIKit.UIColor.FromPatternImage(i);
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
                OverrideUserInterfaceStyle = UIKit.UIUserInterfaceStyle.Light;
        }
    }
}
