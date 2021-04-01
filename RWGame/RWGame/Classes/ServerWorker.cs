using Newtonsoft.Json;
using Plugin.Connectivity;
using RWGame.Classes.ResponseClases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace RWGame.Classes
{
    public class ServerWorker
    {
        private static ServerWorker serverWorker;
        public string URLServer { get; private set; }
        private bool isDeviceConnect;
        public bool IsDeviceConnect
        {
            get { return isDeviceConnect; }
            private set
            {
                isDeviceConnect = value;
                ChangeConnectionStateEvent?.Invoke();
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

        /// <summary>+Команда входа</summary>
        readonly string LoginCommand = "game_actions/login";

        /// <summary>+Проверка аутентификации</summary>
        readonly string IsAuthCommand = "game_actions/is_auth";

        /// <summary>Команда разлогирования</summary>
        //string LogoutCommand = "game_actions/logout";

        /// <summary>+Команда проверки ранее зарегистрированного пользователя с аналогичным логином</summary>
        readonly string CheckLoginCommand = "game_actions/check_login";

        /// <summary>+Команда проверки ранее зарегистрированного пользователя с аналогичным почтовым ящиком</summary>
        readonly string CheckEmailCommand = "game_actions/check_email";

        /// <summary>+Команда регистрации</summary>
        readonly string RegistrationCommand = "game_actions/join";

        /// <summary>+Команда начала игры</summary>
        readonly string PlayGameCommand = "game_actions/play";

        /// <summary>+Команда получения статуса игры</summary>
        readonly string GameStateCommand = "game_actions/game_state";

        /// <summary>+Команда совершения хода в игре</summary>
        readonly string MakeTurnCommand = "game_actions/make_turn";

        /// <summary>+Команда получения списка игр</summary>
        readonly string MyGamesCommand = "game_actions/my_games";

        /// <summary>+Команда получения игры</summary>
        readonly string GetGameCommand = "game_actions/game_info";

        /// <summary>+Команда получения ходов игры</summary>
        readonly string GameTurnsCommand = "game_actions/game_turns";

        /// <summary>+Команда поиска игрока</summary>
        readonly string FindPlayerCommand = "game_actions/find_player";

        /// <summary>+Команда отмены игры</summary>
        readonly string CancelGameCommand = "game_actions/cancel_game";

        /// <summary>+Получить информацию об игроке и статистику</summary>
        readonly string GetPlayerInfoCommand = "game_actions/get_player_info";

        /// <summary>+Получить таблицу рейтинга</summary>
        readonly string GetStandingsCommand = "game_actions/get_standings";

        private ServerWorker() 
        {
            URLServer = "https://scigames.ru/";
            CrossConnectivity.Current.ConnectivityChanged += delegate
            {
                bool hasServerConnect = CrossConnectivity.Current.IsConnected;
                if (!hasServerConnect)
                {
                    IsDeviceConnect = hasServerConnect;
                }
            };
            isDeviceConnect = false;
            clientHandler = new HttpClientHandler { Proxy = new WebProxy("proxy.unn.ru:8080") };
            //clientHandler.DefaultProxyCredentials = CredentialCache.DefaultCredentials;
            client = new HttpClient(clientHandler);
            cookieContainer = new CookieContainer();
            clientHandler.CookieContainer = cookieContainer;

            Task.Run(() => SetCookies()).Wait();
        }

        public static ServerWorker GetServerWorker()
        {
            if (serverWorker == null)
            {
                serverWorker = new ServerWorker();
            }
            return serverWorker;
        }
        private async Task SetCookies()
        {
            try
            {
                string cookiesHeader = await SecureStorage.GetAsync("cookies");
                cookieContainer.SetCookies(new Uri(URLServer), cookiesHeader);
            }
            catch (Exception)
            {

            }
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
            HttpResponseMessage response = null;
            while (response is null)
            {
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        response = await client.PostAsync(new Uri(URLServer + command), content);
                        break;
                    }
                    catch (HttpRequestException)
                    {
                        WaitInternetView.TryConnectStart(i + 1);
                        await Task.Delay(1000);
                    }
                }
                WaitInternetView.TryConnectFinish();
                if (response is null)
                {
                    await WaitInternetView.WaitUserReconnect();
                }
            }
            IEnumerable<string> cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
            string cookiesHeader = cookies?.FirstOrDefault();
            if (cookiesHeader != null)
            {
                try
                {
                    await SecureStorage.SetAsync("cookies", cookiesHeader);
                    Console.WriteLine($"Cookies: {cookiesHeader}");
                }
                catch { }
                
            }

            string responseJsonString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(command);
            Console.WriteLine(responseJsonString);
            return JsonConvert.DeserializeObject<TResponse>(responseJsonString);
        }

        public async Task<LoginResponse> TaskIsAuth()
        {
            return await IsAuth();
        }

        public async Task<bool> TaskRegistrateNewPlayer(string name, string family, string login, string password,
            string confirm_password, string birthday, string email, string token = "")
        {
            return await RegistrateNewPlayer(name, family, login, password, confirm_password, birthday, email, token);
        }

        public async Task<bool> TaskCheckLogin(string login)
        {
            return await CheckLogin(login);
        }

        public async Task<bool> TaskCheckEmail(string email)
        {
            return await CheckEmail(email);
        }

        public async Task<LoginResponse> TaskLogin(string login, string password, string idToken = "")
        {
            return await Login(login, password, idToken);
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

        public async Task<bool> TaskCancelGame(int idGame)
        {
            return await CancelGame(idGame);
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

        public async Task<PlayerInfo> TaskGetPlayerInfo()
        {
            return await GetPlayerInfo();
        }
        public async Task<Standings> TaskGetStandings()
        {
            return await GetStandings();
        }

        private async Task<LoginResponse> IsAuth()
        {
            try
            {
                LoginResponse currentResponse = await PostData<LoginResponse>(IsAuthCommand);
                return currentResponse;
            }
            catch (System.Net.WebException)
            {
                return null;
            }
        }

        private async Task<bool> RegistrateNewPlayer(string name, string family, string login, string password,
            string confirm_password, string birthday, string email, string token)
        {
            try
            {
                Dictionary<string, object> data = new Dictionary<string, object>() {
                    { "name", name },
                    { "family", family },
                    { "login", login },
                    { "password", password },
                    { "confirm_password", confirm_password },
                    { "birthday", birthday },
                    { "email", email },
                    { "token", token },
                };

                RegistrationResponse currentResponse = await PostData<RegistrationResponse>(RegistrationCommand, data);

                return currentResponse.IsRegistrationSuccessful;
            }
            catch (System.Net.WebException)
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
            catch (System.Net.WebException)
            {
                return false;
            }
        }

        private async Task<bool> CheckEmail(string email)
        {
            try
            {
                bool currentResponse = await PostData<bool>(CheckEmailCommand,
                    new Dictionary<string, object>() {
                        { "email", email }
                    }
                );

                return currentResponse;
            }
            catch (System.Net.WebException)
            {
                return false;
            }
        }

        private async Task<LoginResponse> Login(string login, string password, string idToken = "")
        {
            try
            {
                Dictionary<string, object> data = new Dictionary<string, object>() {
                    { "login", login },
                    { "password",  password },
                    { "id_token",  idToken },
                };

                LoginResponse currentResponse = await PostData<LoginResponse>(LoginCommand, data);

                return currentResponse;
            }
            catch (System.Net.WebException)
            {
                return null;
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
            catch (System.Net.WebException)
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
            catch (System.Net.WebException)
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
            catch (System.Net.WebException)
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
            catch (System.Net.WebException)
            {
                return null;
            }
        }
        private async Task<bool> CancelGame(int idGame)
        {
            try
            {
                await PostData<Empty>(CancelGameCommand,
                    new Dictionary<string, object>() {
                        { "id_game", idGame }
                    }
                );
                return true;
            }
            catch (System.Net.WebException)
            {
                return false;
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
            catch (System.Net.WebException)
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
            catch (System.Net.WebException)
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
            catch (System.Net.WebException)
            {
                return null;
            }
        }
        private async Task<PlayerInfo> GetPlayerInfo()
        {
            try
            {
                PlayerInfo currentResponse = await PostData<PlayerInfo>(GetPlayerInfoCommand);
                return currentResponse;
            }
            catch (System.Net.WebException)
            {
                return null;
            }
        }
        private async Task<Standings> GetStandings()
        {
            try
            {
                Standings currentResponse = await PostData<Standings>(GetStandingsCommand);
                return currentResponse;
            }
            catch (System.Net.WebException)
            {
                return null;
            }
        }
    }
}
