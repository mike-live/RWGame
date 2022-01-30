using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RWGame.UWP
{
    public class try_resize_main_window
    {
        private readonly Page page_;
        private ThreadPoolTimer m_timer;
        private bool resized_already_ = false;
        public try_resize_main_window(Page page)
        {
            page_ = page;
            page.Loaded += on_loaded;
        }
        public void OnWindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            if (m_timer != null)
            {
                m_timer.Cancel();
                m_timer = null;
            }
            TimeSpan period = TimeSpan.FromSeconds(1.0);
            m_timer = ThreadPoolTimer.CreateTimer(async (source) => {
                if (!resized_already_)
                    await page_.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                        if (!resized_already_)
                        {
                            resized_already_ = true;
                        ApplicationView.GetForCurrentView().TryResizeView(new Windows.Foundation.Size { Width = 500, Height = 840 });
                            m_timer.Cancel();
                        }
                    });
                else
                    m_timer.Cancel();
            }, period);
        }
        private void on_loaded(object sender, RoutedEventArgs e)
        {
            var result = ApplicationView.GetForCurrentView().TryResizeView(new Windows.Foundation.Size { Width = 500, Height = 840 });
            Debug.WriteLine("OnLoaded TryResizeView: " + result);
            page_.Loaded -= on_loaded;
        }
    }
}
