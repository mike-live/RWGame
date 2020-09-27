using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using RWGame.Models;
using Xamarin.Forms;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;
using System.Threading.Tasks;
using RWGame.Classes;
using System.Collections.ObjectModel;
using RWGame.Classes.ResponseClases;

namespace RWGame.ViewModels
{
    class StandingsData : INotifyPropertyChanged
    {
        public StandingsModel standingsModel { get; set; }
        bool k = true;
        public StandingsData(ServerWorker serverWorker)
        {
            standingsModel = new StandingsModel(serverWorker);
            RefreshList();
            k = false;
        }

        #region RepeatedFields
        public Standings standings { 
            get { return standingsModel.standings; }
            set { standingsModel.standings = value; } 
        }
        public ObservableCollection<StandingViewCellElement> standingsListViewRecords { 
            get { return standingsModel.standingsListViewRecords; } 
            set { standingsModel.standingsListViewRecords = value; } 
        }
        public bool ListViewIsRefreshing { get; set; }
        public string manPerformanceCenterLabelText { get; set; }
        public string manPerformanceBorderLabelText { get; set; }
        public string manRatingLabelText { get; set; }
        public StandingViewCellElement ManVsBot { get { return standingsModel.ManVsBot; } }
        public Color myColor
        {
            get { return Color.FromHex("#153949AB"); }
        }
        public Color notMyColor
        {
            get { return Color.Transparent; }
        }

        #endregion
        #region RefreshMethods
        public async void RefreshList()
        {
            await UpdateStandings();
            ListViewIsRefreshing = false;
        }

        public async Task UpdateStandings()
        {
            await standingsModel.UpdateModelStandigns();
            if (standings != null && standings.StandingsVsBot.Count > 0)
            {
                manPerformanceCenterLabelText = ManVsBot.PerformanceCenter;
                manPerformanceBorderLabelText = ManVsBot.PerformanceBorder;
                manRatingLabelText = ManVsBot.Rating;
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
    class StandingsPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public StandingsData standingsData { get; set; }
        private readonly ServerWorker serverWorker;
        void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public StandingsPageViewModel(ServerWorker serverWorker)
        {
            this.serverWorker = serverWorker;
            standingsData = new StandingsData(serverWorker);
            RefreshListCommand = new Command(standingsData.RefreshList);
        }

        public Command RefreshListCommand { get; set; }

    }
}
