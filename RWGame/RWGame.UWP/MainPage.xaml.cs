using RWGame.UWP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

[assembly: ExportRenderer(typeof(Xamarin.Forms.Entry), typeof(MyEntryRenderer))]
[assembly: ExportRenderer(typeof(Xamarin.Forms.Button), typeof(MyButtonRenderer))]
[assembly: ExportRenderer(typeof(Xamarin.Forms.CheckBox), typeof(MyCheckBoxRenderer))]
[assembly: ExportRenderer(typeof(Xamarin.Forms.SearchBar), typeof(MySearchBarRenderer))]
[assembly: ExportRenderer(typeof(Xamarin.Forms.TabbedPage), typeof(MyTabbedPageRenderer))]
[assembly: ExportRenderer(typeof(Xamarin.Forms.Image), typeof(CustomImageRenderer))]
namespace RWGame.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            try_resize_ = new try_resize_main_window(this);
            this.LoadApplication(new RWGame.App());
        }
        private try_resize_main_window try_resize_;
        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Window.Current.SizeChanged += try_resize_.OnWindowSizeChanged;
        }
        protected override void OnNavigatedFrom(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Window.Current.SizeChanged -= try_resize_.OnWindowSizeChanged;
        }
    }
    // https://docs.microsoft.com/en-us/windows/apps/design/style/xaml-styles
    class MyEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Style = App.Current.Resources["DefaultTextBoxStyle"] as Windows.UI.Xaml.Style;
            }
        }
    }

    class MyButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Style = App.Current.Resources["DefaultButtonStyle"] as Windows.UI.Xaml.Style;
            }
        }
    }

    class MyCheckBoxRenderer : CheckBoxRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.CheckBox> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Style = App.Current.Resources["DefaultCheckBoxStyle"] as Windows.UI.Xaml.Style;
            }
        }
    }

    class MySearchBarRenderer : SearchBarRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 0, 0));
                //Control.Style = App.Current.Resources["DefaultAutoSuggestBoxStyle"] as Windows.UI.Xaml.Style;

                /*Windows.UI.Xaml.ResourceDictionary lightTheme = new Windows.UI.Xaml.ResourceDictionary();
                lightTheme["TextControlForeground"] = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Red);
                Control.Resources.ThemeDictionaries.Add("Light", lightTheme);
                Control.TextBoxStyle.Setters.Add(new Windows.UI.Xaml.Setter(AutoSuggestBox.for, lightTheme["TextControlForeground"]));*/

            }
        }
    }

    class MyTabbedPageRenderer : TabbedPageRenderer
    {
        public MyTabbedPageRenderer()
        {
            this.ElementChanged += CustomTabRenderer_ElementChanged;

        }

        private void CustomTabRenderer_ElementChanged(object sender, VisualElementChangedEventArgs e)
        {
            if (Control != null)
            {
                /*Control.HeaderTemplate = (Windows.UI.Xaml.DataTemplate)
                    ((Windows.UI.Xaml.Setter)((Windows.UI.Xaml.Style)App.Current.Resources["DefaultPivotHeaderItemStyle"]).Setters[12]).Value;*/
                //Control.Style = (Windows.UI.Xaml.Style)Windows.UI.Xaml.Application.Current.Resources["DefaultPivotHeaderItemStyle"];

                Windows.UI.Xaml.Style pivotStyle = (Windows.UI.Xaml.Style)App.Current.Resources["MyPivotStyle"];
                Control.Template = (Windows.UI.Xaml.Controls.ControlTemplate)
                    ((Windows.UI.Xaml.Setter)(pivotStyle).Setters[5]).Value;
                
                //Background = "#F2FAFE"

                Control.HeaderTemplate = (Windows.UI.Xaml.DataTemplate)App.Current.Resources["PivotHeaderTemplate"];
                
                //GetStyledTitleTemplate();

                //((Windows.UI.Xaml.Controls.ItemsControl) Control.Items[0]).
                //Control.Template = (Windows.UI.Xaml.Controls.ControlTemplate)App.Current.Resources["DefaultPivotStyle"];
                //Control.Style = (Windows.UI.Xaml.Style)App.Current.Resources["DefaultPivotStyle"];
                //Control.Template = (Windows.UI.Xaml.Controls.ControlTemplate)App.Current.Resources["Windows.UI.Xaml.Controls.Pivot"];
                //Control.ItemContainerStyle = ((Windows.UI.Xaml.Style)App.Current.Resources["DefaultPivotItemStyle"]);
                /*foreach (var x in App.Current.Resources) {
                    Debug.WriteLine(x.Key);
                }*/

                //Control.Style = (Windows.UI.Xaml.Style)App.Current.Resources["DefaultPivotHeaderItemStyle"];
            }
        }


        private Windows.UI.Xaml.DataTemplate GetStyledTitleTemplate()
        {
            string dataTemplateXaml = @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
            xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                <Grid Height=""50"" Width=""100"">

                <Image Source=""Assets\\bot.svg"" Width=""30""/>
                <TextBlock
                    Text=""{Binding Title}""
                    FontFamily=""/Assets/Fonts/museosans-500.ttf#museosans-500""
                    FontSize =""14"" />
                </Grid>
                  </DataTemplate>";

            return (Windows.UI.Xaml.DataTemplate)XamlReader.Load(dataTemplateXaml);
        }
    }
    
    public class CustomImageRenderer : ImageRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Image> e)
        {
            if (e.NewElement != null)
            {
                var source = e.NewElement.Source;
                if (source is FileImageSource fileImageSource)
                {
                    fileImageSource.File = $"Assets/{fileImageSource.File}";
                }
            }
            base.OnElementChanged(e);
        }
    }

}

