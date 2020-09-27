using RWGame.Classes.ResponseClases;
using RWGame.Classes;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace RWGame.Models
{
    public class StandingViewCellElement
    {
        public PlayerStanding PlayerStanding { get; }
        public readonly int playerRank;
        public readonly bool isMe;
        public string PlayerRank { get { return "#" + playerRank.ToString(); } }
        public string UserName { get { return PlayerStanding.UserName; } }
        public string Rating { get { return Math.Round(PlayerStanding.Rating).ToString(); } }
        public string PerformanceCenter { get { return Math.Round(PlayerStanding.PerformanceCenter).ToString(); } }
        public string PerformanceBorder { get { return Math.Round(PlayerStanding.PerformanceBorder).ToString(); } }
        
        public StandingViewCellElement(PlayerStanding playerStanding, int playerRank, bool isMe)
        {
            this.PlayerStanding = playerStanding;
            this.playerRank = playerRank;
            this.isMe = isMe;
        }
    }
    class StandingsModel : INotifyPropertyChanged
    {
        private ServerWorker serverWorker;
        public Standings standings { get; set; }
        public ObservableCollection<StandingViewCellElement> standingsListViewRecords { get; set; }
        public StandingViewCellElement ManVsBot { get; set; }
        public StandingsModel(ServerWorker serverWorker)
        {
            this.serverWorker = serverWorker;
            _ = UpdateModelStandigns();
        }

        public async Task UpdateModelStandigns()
        {
            standings = await serverWorker.TaskGetStandings();
            standingsListViewRecords = new ObservableCollection<StandingViewCellElement>();
            ManVsBot = new StandingViewCellElement(standings.ManVsBot, -1, false);
            for (int i = 0; i < standings.StandingsVsBot.Count; i++)
            {
                bool isMe = standings.StandingsVsBot[i].UserName == serverWorker.UserLogin;
                standingsListViewRecords.Add(new StandingViewCellElement(standings.StandingsVsBot[i], i + 1, isMe));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
