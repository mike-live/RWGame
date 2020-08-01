using Acr.UserDialogs;
using System.Threading.Tasks;

namespace RWGame
{
    static class WaitInternetView
    {
        //static Mutex isExecuted = new Mutex();
        static Task<bool> task = null;
        static IProgressDialog progress = null;
        public static bool TryConnectStart(int id_attempt)
        {
            var config = new ProgressDialogConfig()
                .SetTitle("Trying connect to internet...\n" + "Attempt: " + id_attempt)
                .SetIsDeterministic(false)
                .SetMaskType(MaskType.Clear);
            progress = UserDialogs.Instance.Progress(config);
            progress.Show();
            return true;
        }

        public static bool TryConnectFinish()
        {
            if (progress != null)
            {
                progress.Dispose();
                progress = null;
            }
            return true;
        }
        public static async Task<bool> WaitUserReconnect()
        {
            if (task != null)
            {
                await task;
                return true;
            }
            var config = new ConfirmConfig()
                .SetMessage("No internet connection").SetOkText("Reconnect");
            task = UserDialogs.Instance.ConfirmAsync(config);
            await task;
            task = null;
            return true;
        }


    }
}
