using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RWGame.Views.Helpers
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SmallDevicesStyle : ResourceDictionary
    {
        public static SmallDevicesStyle SharedInstance { get; } = new SmallDevicesStyle();
        public SmallDevicesStyle()
        {
            InitializeComponent();
        }
    }
}
