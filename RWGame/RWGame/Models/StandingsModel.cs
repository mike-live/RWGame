using RWGame.Classes.ResponseClases;
using RWGame.Classes;
using System.Threading.Tasks;
using System.ComponentModel;

namespace RWGame.Models
{
    class StandingsModel : INotifyPropertyChanged
    {
        private readonly ServerWorker serverWorker;
        public Standings Standings { get; set; }
        public string UserLogin { get { return serverWorker.UserLogin; } }
        public StandingsModel(ServerWorker serverWorker)
        {
            this.serverWorker = serverWorker;
            _ = UpdateModelStandings();
        }
        public async Task UpdateModelStandings()
        {
            Standings = await serverWorker.TaskGetStandings();
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
