using System;
using System.Collections.Generic;
using System.Text;
using Plugin.Connectivity;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using System.Net;
using RWGame.Classes.ResponseClases;

namespace RWGame.Classes
{
    public class ServerWorker
    {

        public string URLServer { get; private set; }
        private bool isDeviceConnect;
        public bool IsDeviceConnect {
            get { return isDeviceConnect; }
            private set
            {
                isDeviceConnect = value;
                if (ChangeConnectionStateEvent != null)
                    ChangeConnectionStateEvent();
            }
        }
        public delegate void ChangeConnectionStateHandler();
        public event ChangeConnectionStateHandler ChangeConnectionStateEvent;

        private HttpClient client;
        private HttpClientHandler clientHandler;
        private CookieContainer cookieContainer;

        public string CurrentLogin = "";
        public string CurrentPassword = "";
        public string UserLogin = "";
        public int UserID = -1;

        /// <summary>+Команда логирования</summary>
        string LoginCommand = "game_actions/login";
        /// <summary>Команда разлогирования</summary>
        string LogoutCommand = "game_actions/logout";
        /// <summary>+Команда проверки ранее зарегистрированного пользователя с аналогичным логином</summary>
        string CheckLoginCommand = "game_actions/check_login";
        /// <summary>+Команда проверки ранее зарегистрированного пользователя с аналогичным почтовым ящиком</summary>
        string CheckEmailCommand = "game_actions/check_email";
        /// <summary>+Команда регистрации</summary>
        string RegistrationCommand = "game_actions/join";
        /// <summary>+Команда начала игры</summary>
        string PlayGameCommand = "game_actions/play";
        /// <summary>+Команда получения статуса игры</summary>
        string GameStateCommand = "game_actions/game_state";
        /// <summary>+Команда совершения хода в игре</summary>
        string MakeTurnCommand = "game_actions/make_turn";
        /// <summary>+Команда получения списка игр</summary>
        string MyGamesCommand = "game_actions/my_games";
        /// <summary>+Команда получения игры</summary>
        string GetGameCommand = "game_actions/game_info";
        /// <summary>+Команда получения ходов игры</summary>
        string GameTurnsCommand = "game_actions/game_turns";
        /// <summary>Команда standings</summary>
        string StandingsCommand = "game_actions/standings";
        /// <summary>+Команда поиска игрока</summary>
        string FindPlayerCommand = "game_actions/find_player";

        public ServerWorker()
        {
            URLServer = "https://scigames.ru/";
            CrossConnectivity.Current.ConnectivityChanged += delegate {
                bool hasServerConnect = CrossConnectivity.Current.IsConnected;
                if (!hasServerConnect)
                {
                    IsDeviceConnect = hasServerConnect;
                }
            };
            isDeviceConnect = false;
            clientHandler = new HttpClientHandler();
            client = new HttpClient(clientHandler);
        }

        private async Task<TResponse> PostData<TResponse>(string command, Dictionary<string, object> data = null)
        {
            if (data == null)
            {
                data = new Dictionary<string, object>();
            }
            string contentString = "";
            foreach (KeyValuePair<string, object> pair in data)
            {
                contentString += pair.Key + "=" + Uri.EscapeDataString(pair.Value.ToString()) + "&";
            }
            contentString = contentString.TrimEnd('&');
            var content = new StringContent(contentString, Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await client.PostAsync(new Uri(URLServer + command), content);

            string responseJsonString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(command);
            Console.WriteLine(responseJsonString);
            return JsonConvert.DeserializeObject<TResponse>(responseJsonString);
        }

        public async Task<bool> TaskRegistrateNewPlayer(string name, string family, string login, string password, string confirm_password, string birthday, string email)
        {
            return await RegistrateNewPlayer(name, family, login, password, confirm_password, birthday, email);
        }

        public async Task<bool> TaskCheckLogin(string login)
        {
            return await CheckLogin(login);
        }

        public async Task<bool> TaskCheckEmail(string email)
        {
            return await CheckEmail(email);
        }

        public async Task<bool> TaskLogin(string login, string password)
        {
            return await Login(login, password);
        }

        public async Task<List<Player>> TaskGetPlayerList(string loginPart)
        {
            return await GetPlayerList(loginPart);
        }

        public async Task<List<Game>> TaskGetGamesList()
        {
            return await GetGamesList();
        }

        public async Task<Game> TaskGetGame(int idGame)
        {
            return await GetGame(idGame);
        }

        public async Task<PlayGameResponse> TaskPlayGame(int idGame = -1, int idPlayer = -1)
        {
            return await PlayGame(idGame, idPlayer);
        }

        public async Task<GameStateInfo> TaskGetGameState(int idGame)
        {
            return await GetGameState(idGame);
        }

        public async Task<GameStateInfo> TaskMakeTurn(int idGame, int turn)
        {
            return await MakeTurn(idGame, turn);
        }

        public async Task<List<TurnInfo>> TaskGetGameTurns(int idGame)
        {
            return await GetGameTurns(idGame);
        }



        private async Task<bool> RegistrateNewPlayer(string name, string family, string login, string password, string confirm_password, string birthday, string email)
        {
            try
            {
                Dictionary<string, object> data = new Dictionary<string, object>() {
                    {"name", name}, 
                    {"family", family}, 
                    { "login", login }, 
                    { "password", password}, 
                    { "confirm_password", confirm_password}, 
                    { "birthday", birthday}, 
                    { "email", email }
                };

                RegistrationResponse currentResponse = await PostData<RegistrationResponse>(RegistrationCommand, data);

                return currentResponse.IsRegistrationSuccessful;
            }
            catch (System.Net.WebException e)
            {
                return false;
            }
        }

        private async Task<bool> CheckLogin(string login)
        {
            try
            {
                bool currentResponse = await PostData<bool>(CheckLoginCommand, 
                    new Dictionary<string, object>() {
                        { "login", login }
                    }
                );

                return currentResponse;
            }
            catch (System.Net.WebException e)
            {
                return false;
            }
        }

        private async Task<bool> CheckEmail(string login)
        {
            try
            {
                bool currentResponse = await PostData<bool>(CheckEmailCommand,
                    new Dictionary<string, object>() {
                        { "login", login }
                    }
                );

                return currentResponse;
            }
            catch (System.Net.WebException e)
            {
                return false;
            }
        }

        private async Task<bool> Login(string login, string password)
        {
            try
            {
                Dictionary<string, object> data = new Dictionary<string, object>() {
                    { "login", login },
                    { "password",  password }
                };

                LoginResponse currentResponse = await PostData<LoginResponse>(LoginCommand, data);

                return currentResponse.IsAuthenticationSuccessful;
            }
            catch (System.Net.WebException e)
            {
                return false;
            }
        }

        private async Task<List<Player>> GetPlayerList(string loginPart)
        {
            try
            {
                Dictionary<string, object> data = new Dictionary<string, object>() {
                    { "player_name", loginPart }
                };

                ListPlayers currentResponse = await PostData<ListPlayers>(FindPlayerCommand, data);
                
                if (currentResponse != null && currentResponse.Players != null && currentResponse.Players.Count > 0)
                {
                    return currentResponse.Players;
                }
                else
                {
                    return null;
                }
            }
            catch (System.Net.WebException e)
            {
                return null;
            }
        }


        private async Task<List<Game>> GetGamesList()
        {
            try
            {
                GamesList currentResponse = await PostData<GamesList>(MyGamesCommand);

                if (currentResponse != null && currentResponse.Games != null && currentResponse.Games.Count > 0)
                {
                    return currentResponse.Games;
                }
                else
                {
                    return null;
                }
            }
            catch (System.Net.WebException e)
            {
                return null;
            }
        }

        private async Task<Game> GetGame(int idGame)
        {
            try
            {
                GetGameResponse currentResponse = await PostData<GetGameResponse>(GetGameCommand, new Dictionary<string, object>() {
                    { "id_game", idGame }
                });

                return currentResponse.Game;
            }
            catch (System.Net.WebException e)
            {
                return null;
            }
        }

        private async Task<PlayGameResponse> PlayGame(int idGame, int idPlayer)
        {
            try
            {
                PlayGameResponse currentResponse = await PostData<PlayGameResponse>(PlayGameCommand,
                    new Dictionary<string, object>() {
                        { "id_game", idGame },
                        { "id_player", idPlayer }
                    }
                );
                return currentResponse;
            }
            catch (System.Net.WebException e)
            {
                return null;
            }
        }

        private async Task<GameStateInfo> GetGameState(int idGame)
        {
            try
            {
                GameStateInfo currentResponse = await PostData<GameStateInfo>(GameStateCommand,
                    new Dictionary<string, object>() {
                        { "id_game", idGame }
                    }
                );
                return currentResponse;
            }
            catch (System.Net.WebException e)
            {
                return null;
            }
        }

        private async Task<GameStateInfo> MakeTurn(int idGame, int turn)
        {
            try
            {
                GameStateInfo currentResponse = await PostData<GameStateInfo>(MakeTurnCommand,
                    new Dictionary<string, object>() {
                        { "id_game", idGame },
                        { "turn", turn }
                    }
                );
                return currentResponse;
            }
            catch (System.Net.WebException e)
            {
                return null;
            }
        }

        private async Task<List<TurnInfo>> GetGameTurns(int idGame)
        {
            try
            {
                GameTurns currentResponse = await PostData<GameTurns>(GameTurnsCommand,
                    new Dictionary<string, object>() {
                        { "id_game", idGame }
                    }
                );
                return currentResponse.Turns;
            }
            catch (System.Net.WebException e)
            {
                return null;
            }
        }
    }
}
