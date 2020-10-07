using System;
using System.ComponentModel;
using RWGame.Models;
using Xamarin.Forms;
using System.Threading.Tasks;
using RWGame.Classes;
using System.Collections.ObjectModel;
using RWGame.Classes.ResponseClases;
using RWGame.Helpers;

namespace RWGame.ViewModels
{
    public class StandingViewCellElement
    {
        public StandingViewCellElement(PlayerStanding playerStanding, int playerRank, bool isMe)
        {
            this.PlayerStanding = playerStanding;
            this.playerRank = playerRank;
            this.isMe = isMe;
        }

        #region StandingViewCellElementProperties
        public PlayerStanding PlayerStanding { get; }
        public int playerRank { get; set; }
        public bool isMe { get; set; }
        public string PlayerRank
        {
            get
            {
                if (playerRank <= 3)
                {
                    return getEmojiStringForRank();
                }
                return "#" + playerRank;
            }
        }
        public string UserName { get { return PlayerStanding.UserName; } }
        public string Rating { get { return Math.Round(PlayerStanding.Rating).ToString(); } }
        public string PerformanceCenter { get { return Math.Round(PlayerStanding.PerformanceCenter).ToString(); } }
        public string PerformanceBorder { get { return Math.Round(PlayerStanding.PerformanceBorder).ToString(); } }
        public Color CellBackgroundColor
        {
            get
            {
                if (playerRank % 2 == 0)
                {
                    return (isMe ? Color.FromHex("#95f7f9a0") : Color.FromHex("#1039bafa"));
                }
                else
                {
                    return (isMe ? Color.FromHex("#95f7f9a0") : Color.Transparent);
                }
            }
        }
        #endregion
        #region Emoji
        public string Emoji { get { return getEmojiStringForRank(); } }
        private string getEmojiStringForRank()
        {
            Emojis emoji = (playerRank < 4) ? (Emojis)playerRank : 0;
            return emoji.ToDescriptionString();
        }
        #endregion
    }
    class StandingsDisplayData : INotifyPropertyChanged
    {
        private StandingsModel standingsModel { get; set; }
        public StandingsDisplayData(ServerWorker serverWorker)
        {
            standingsModel = new StandingsModel(serverWorker);
            RefreshList();
        }

        #region DataProperties
        public Standings standings
        {
            get { return standingsModel.standings; }
        }
        private string UserLogin { get { return standingsModel.UserLogin; } }

        public ObservableCollection<StandingViewCellElement> standingsListViewRecords { get; } = 
            new ObservableCollection<StandingViewCellElement>();
        public bool ListViewIsRefreshing { get; set; }
        public StandingViewCellElement ManVsBot { get; set; }
        public string manPerformanceCenterLabelText {
            get 
            {
                return ManVsBot?.PerformanceCenter ?? "";
            }
        }
        public string manPerformanceBorderLabelText 
        { 
            get 
            {
                return ManVsBot?.PerformanceBorder ?? "";
            }
        }
        public string manRatingLabelText 
        { 
            get 
            {
                return ManVsBot?.Rating ?? "";
            }
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
            await standingsModel.UpdateModelStandings();
            standingsListViewRecords.Clear();
            for (int i = 0; i < standings.StandingsVsBot.Count; i++)
            {
                bool isMe = standings.StandingsVsBot[i].UserName == UserLogin;
                standingsListViewRecords.Add(new StandingViewCellElement(standings.StandingsVsBot[i], i + 1, isMe));
            }
            ManVsBot = new StandingViewCellElement(standings.ManVsBot, -1, false);
        }
        #endregion
        public event PropertyChangedEventHandler PropertyChanged;
    }
    class StandingsPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public StandingsDisplayData standingsDisplayData { get; set; }
        private readonly ServerWorker serverWorker;
        public StandingsPageViewModel(ServerWorker serverWorker)
        {
            this.serverWorker = serverWorker;
            standingsDisplayData = new StandingsDisplayData(serverWorker);
            RefreshListCommand = new Command(standingsDisplayData.RefreshList);
        }
        public Command RefreshListCommand { get; set; }

    }
}
