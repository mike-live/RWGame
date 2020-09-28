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
using RWGame.Enums;

namespace RWGame.ViewModels
{
    public class StandingViewCellElementExtended
    {
        public StandingViewCellElement standingViewCellElement;
        public StandingViewCellElementExtended(StandingViewCellElement standingViewCellElement)
        {
            this.standingViewCellElement = standingViewCellElement;
        }
        public bool isMe { get { return standingViewCellElement.isMe; } }
        public string PlayerRank { get { return standingViewCellElement.PlayerRank; } }
        public string UserName { get { return standingViewCellElement.UserName; } }
        public string Rating { get { return standingViewCellElement.Rating; } }
        public string PerformanceCenter { get { return standingViewCellElement.PerformanceCenter; } }
        public string PerformanceBorder { get { return standingViewCellElement.PerformanceBorder; } }
        public Color CellBackgroundColor
        {
            get
            {
                if (standingViewCellElement.playerRank % 2 == 0)
                    return Color.FromHex("#1039bafa");
                else
                    return Color.Transparent;
            }
        }
        public string Emoji
        {
            get 
            { 
                return getPlaceEmojiString(standingViewCellElement.playerRank); 
            }
        }
        private string getPlaceEmojiString(int position)
        {
            string res;
            Emojis emoji;
            switch (position)
            {
                case 1:
                    emoji = Emojis.medalFirstPlace;
                    break;
                case 2:
                    emoji = Emojis.medalSecondPlace;
                    break;
                case 3:
                    emoji = Emojis.medalThirdPlace;
                    break;
                default:
                    emoji = Emojis.none;
                    break;
            }
            res =  emoji.ToDescriptionString();
            return res;
        }
    }
    class StandingsData : INotifyPropertyChanged
    {
        public StandingsModel standingsModel { get; set; }
        public StandingsData(ServerWorker serverWorker)
        {
            standingsModel = new StandingsModel(serverWorker);
            RefreshList();
        }

        #region DataFields
        public Standings standings { 
            get { return standingsModel.standings; }
            set { standingsModel.standings = value; } 
        }
        public ObservableCollection<StandingViewCellElementExtended> standingsListViewRecordsExt
        { 
            get 
            {
                var temp = new ObservableCollection<StandingViewCellElementExtended>();
                if (standingsModel.standingsListViewRecords != null)
                    for (int i = 0; i < standingsModel.standingsListViewRecords.Count; ++i)
                    {
                        temp.Add(new StandingViewCellElementExtended(standingsModel.standingsListViewRecords[i]));
                    }
                return temp;
            } 
            set 
            {
                var temp = new ObservableCollection<StandingViewCellElement>();
                for (int i = 0; i < value.Count; ++i)
                {
                    temp.Add(value[i].standingViewCellElement);
                }
                standingsModel.standingsListViewRecords = temp;
            } 
        }
        public bool ListViewIsRefreshing { get; set; }
        public string manPerformanceCenterLabelText { get; set; }
        public string manPerformanceBorderLabelText { get; set; }
        public string manRatingLabelText { get; set; }
        public StandingViewCellElement ManVsBot { get { return standingsModel.ManVsBot; } }
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
            OnPropertyChanged();
            OnPropertyChanged(nameof(standings));
            OnPropertyChanged(nameof(standingsListViewRecordsExt));
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
