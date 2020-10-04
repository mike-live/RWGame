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

        #region StandingViewCellElementFields
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
        #endregion
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

        #region DataFields
        public Standings standings
        {
            get { return standingsModel.standings; }
        }
        private string UserLogin { get { return standingsModel.UserLogin; } }

        public ObservableCollection<StandingViewCellElement> standingsListViewRecordsExt { get; } = 
            new ObservableCollection<StandingViewCellElement>();
        public bool ListViewIsRefreshing { get; set; }
        public StandingViewCellElement ManVsBot { get { return new StandingViewCellElement(standings.ManVsBot, -1, false); } }
        public string manPerformanceCenterLabelText {
            get 
            { 
                if (standings != null && standings.StandingsVsBot.Count > 0) 
                { 
                    return ManVsBot.PerformanceCenter; 
                } 
                else
                {
                    return "";
                }
            }
        }
        public string manPerformanceBorderLabelText 
        { 
            get 
            {
                if (standings != null && standings.StandingsVsBot.Count > 0)
                {
                    return ManVsBot.PerformanceBorder;
                }
                else 
                {
                    return "";
                }
            } 
        }
        public string manRatingLabelText 
        { 
            get 
            {
                if (standings != null && standings.StandingsVsBot.Count > 0)
                {
                    return ManVsBot.Rating;
                }
                else
                {
                    return "";
                }
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
            standingsListViewRecordsExt.Clear();
            for (int i = 0; i < standings.StandingsVsBot.Count; i++)
            {
                bool isMe = standings.StandingsVsBot[i].UserName == UserLogin;
                standingsListViewRecordsExt.Add(new StandingViewCellElement(standings.StandingsVsBot[i], i + 1, isMe));
            }
            OnPropertyChanged(nameof(manPerformanceCenterLabelText));
            OnPropertyChanged(nameof(manPerformanceBorderLabelText));
            OnPropertyChanged(nameof(manRatingLabelText));
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
        public StandingsDisplayData standingsData { get; set; }
        private readonly ServerWorker serverWorker;

        public StandingsPageViewModel(ServerWorker serverWorker)
        {
            this.serverWorker = serverWorker;
            standingsData = new StandingsDisplayData(serverWorker);
            RefreshListCommand = new Command(standingsData.RefreshList);
        }

        public Command RefreshListCommand { get; set; }

    }
}
