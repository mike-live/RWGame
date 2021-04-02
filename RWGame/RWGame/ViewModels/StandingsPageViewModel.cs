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
        public StandingViewCellElement(PlayerStanding PlayerStanding, int PlayerRank, bool IsMe)
        {
            this.PlayerStanding = PlayerStanding;
            this.playerRank = PlayerRank;
            this.IsMe = IsMe;
        }

        #region StandingViewCellElementProperties
        public PlayerStanding PlayerStanding { get; }
        public int playerRank;
        public bool IsMe { get; set; }
        public string PlayerRank
        {
            get
            {
                if (playerRank <= 3)
                {
                    return GetEmojiStringForRank();
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
                    return (IsMe ? Color.FromHex("#95f7f9a0") : Color.FromHex("#1039bafa"));
                }
                else
                {
                    return (IsMe ? Color.FromHex("#95f7f9a0") : Color.Transparent);
                }
            }
        }
        #endregion
        #region Emoji
        private string GetEmojiStringForRank()
        {
            Emojis emoji = (playerRank < 4) ? (Emojis)playerRank : 0;
            return emoji.ToDescriptionString();
        }
        #endregion
    }
    class StandingsDisplayData : INotifyPropertyChanged
    {
        private StandingsModel StandingsModel { get; set; }
        public StandingsDisplayData(INavigation navigation)
        {
            StandingsModel = new StandingsModel();
            Navigation = navigation;
            RefreshList();
        }
        public INavigation Navigation { get; set; }
        #region DataProperties
        public Standings Standings
        {
            get { return StandingsModel.Standings; }
        }
        private string UserLogin { get { return StandingsModel.UserLogin; } }

        public ObservableCollection<StandingViewCellElement> StandingsListViewRecords { get; } =
            new ObservableCollection<StandingViewCellElement>();
        public bool ListViewIsRefreshing { get; set; }
        public StandingViewCellElement ManVsBot { get; set; }
        public string ManPerformanceCenterLabelText
        {
            get
            {
                return ManVsBot?.PerformanceCenter ?? "";
            }
        }
        public string ManPerformanceBorderLabelText
        {
            get
            {
                return ManVsBot?.PerformanceBorder ?? "";
            }
        }
        public string ManRatingLabelText
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
            await StandingsModel.UpdateModelStandings();
            StandingsListViewRecords.Clear();
            for (int i = 0; i < Standings.StandingsVsBot.Count; i++)
            {
                bool isMe = Standings.StandingsVsBot[i].UserName == UserLogin;
                StandingsListViewRecords.Add(new StandingViewCellElement(Standings.StandingsVsBot[i], i + 1, isMe));
            }
            ManVsBot = new StandingViewCellElement(Standings.ManVsBot, -1, false);
        }
        #endregion

        public async void OnSwipedRight()
        {
            await Navigation.PopAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
    class StandingsPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public StandingsDisplayData StandingsDisplayData { get; set; }
        public StandingsPageViewModel(INavigation navigation)
        {
            StandingsDisplayData = new StandingsDisplayData(navigation);
            RefreshListCommand = new Command(StandingsDisplayData.RefreshList);
            GoBack = new Command(StandingsDisplayData.OnSwipedRight);
        }
        public Command RefreshListCommand { get; set; }
        public Command GoBack { get; set; }

    }
}
