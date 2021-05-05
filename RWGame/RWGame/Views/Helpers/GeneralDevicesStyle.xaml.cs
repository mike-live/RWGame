using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RWGame.Views.Helpers
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GeneralDevicesStyle : ResourceDictionary
    {
        public static GeneralDevicesStyle SharedInstance { get; } = new GeneralDevicesStyle();
        public GeneralDevicesStyle()
        {
            InitializeComponent();
        }
    }
}
