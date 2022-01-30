using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace RWGame.UWP
{
    public sealed partial class HeaderTemplate : ResourceDictionary
    {
        public HeaderTemplate()
        {
            this.InitializeComponent();
        }

    }
    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null && value is Xamarin.Forms.FileImageSource)
                return "Assets/" + ((Xamarin.Forms.FileImageSource)value).File + ".svg";

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
