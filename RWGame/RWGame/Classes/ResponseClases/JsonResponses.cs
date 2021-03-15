using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RWGame.Classes.ResponseClases
{
    class JsonResponses
    {
    }
    class Empty : List<bool> { };

    //Классы для десериализации json ответов

    //"{"x_len":16,"y_len":16,"matrix":[["U","R"],["D","L"]]}"

    public class GamesList
    {
        [JsonProperty("games")]
        public List<Game> Games { get; set; }
    }

    public enum GameStateEnum
    {
        NEW = 0, 
        CONNECT = 1, 
        START = 2, 
        ACTIVE = 3, 
        END = 4, 
        PAUSE = 5, 
        WAIT = 6
    };

    /*public enum ControlsEnum
    {
        D = 0, U = 1, R = 2, L = 3
    };*/

    public class GetGameResponse
    {
        [JsonProperty("game")]
        public Game Game { get; set; }
    }

    public class Game
    {
        [JsonProperty("id_game")]
        public int IdGame { get; set; }

        [JsonProperty("game_settings")]
        private string game_settings { get; set; }
        public GameSettings GameSettings { get { return JsonConvert.DeserializeObject<GameSettings>(game_settings); } }

        [JsonProperty("game_state")]
        private string gameState { get; set; }
        public GameStateEnum GameState { get { Enum.TryParse(gameState, out GameStateEnum curGameState); return curGameState; } }

        [JsonProperty("player_1")]
        public int? Player1 { get; set; }

        [JsonProperty("player_2")]
        public int? Player2 { get; set; }

        [JsonProperty("id_player")]
        public int IdPlayer { get; set; }

        [JsonProperty("player_user_name_1")]
        public string PlayerUserName1 { get; set; }

        [JsonProperty("player_user_name_2")]
        public string PlayerUserName2 { get; set; }

        [JsonProperty("start")]
        public string Start { get; set; }

        [JsonProperty("finish")]
        public string Finish { get; set; }

        [JsonProperty("score")]
        public int? Score { get; set; }

        [JsonProperty("user_error")]
        public string UserError { get; set; }

        [JsonProperty("message_error")]
        public string MessageError { get; set; }

        public List<TurnInfo> Turns;
    }

    //Используй при выборе той или иной игры, для построения игрового поля
    public class GameSettings
    {
        [JsonProperty("x_len")]
        public int FieldWidth { get; set; }

        [JsonProperty("y_len")]
        public int FieldHeight { get; set; }

        [JsonProperty("matrix")]
        public List<List<string>> Controls { get; set; }

        [JsonProperty("goals")]
        public List<string> Goals { get; set; }

        [JsonProperty("turn_controls")]
        public List<string> TurnControls { get; set; }
    }


    public class ListPlayers
    {
        [JsonProperty("players")]
        public List<Player> Players { get; set; }
    }

    public class Player
    {
        [JsonProperty("id_player")]
        public int IdPlayer { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }
    }

    public class RegistrationResponse
    {
        [JsonProperty("ok_reg")]
        public bool IsRegistrationSuccessful { get; set; }

        [JsonProperty("user_error")]
        public string UserError { get; set; }

        [JsonProperty("message_error")]
        public string MessageError { get; set; }
    }

    public class CheckLoginResponse
    {
        [JsonProperty("ok_login")]
        public bool IsLoginExist { get; set; }

        [JsonProperty("user_error")]
        public string UserError { get; set; }

        [JsonProperty("message_error")]
        public string MessageError { get; set; }
    }

    public class CheckEmailResponse
    {
        [JsonProperty("ok_email")]
        public bool IsEmailExist { get; set; }

        [JsonProperty("user_error")]
        public string UserError { get; set; }

        [JsonProperty("message_error")]
        public string MessageError { get; set; }
    }

    public class LoginResponse
    {
        [JsonProperty("ok_auth")]
        public bool IsAuthenticationSuccessful { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("user_error")]
        public string UserError { get; set; }

        [JsonProperty("message_error")]
        public string MessageError { get; set; }
        
        [JsonProperty("error_id")]
        public int ErrorId { get; set; }
    }

    public class PlayGameResponse
    {
        [JsonProperty("id_game")]
        public int IdGame { get; set; }

        private string game_settings { get; set; }
        public GameSettings GameSettings { get { return JsonConvert.DeserializeObject<GameSettings>(game_settings); } }

        [JsonProperty("game_state")]
        private string gameState { get; set; }
        public GameStateEnum GameState { get { Enum.TryParse(gameState, out GameStateEnum curGameState); return curGameState; } }

        [JsonProperty("user_error")]
        public string UserError { get; set; }

        [JsonProperty("message_error")]
        public string MessageError { get; set; }
    }

    public class GameStateInfo
    {
        [JsonProperty("id_turn")]
        public int LastIdTurn { get; set; }

        [JsonProperty("turn")]
        public List<int> Turn { get; set; }

        [JsonProperty("state")]
        public List<int> PointState { get; set; }

        [JsonProperty("game_state")]
        private string gameState { get; set; }
        public GameStateEnum GameState { get { Enum.TryParse(gameState, out GameStateEnum curGameState); return curGameState; } }

        [JsonProperty("user_error")]
        public string UserError { get; set; }

        [JsonProperty("message_error")]
        public string MessageError { get; set; }
    }

    public class GameTurns
    {
        [JsonProperty("turns")]
        public List<TurnInfo> Turns { get; set; }
    }

    public class TurnInfo
    {
        [JsonProperty("id_turn")]
        public int IdTurn { get; set; }

        [JsonProperty("turn")]
        public List<int> Turn { get; set; }

        [JsonProperty("state")]
        public List<int> State { get; set; }
    }

    public class PersonalInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("family")]
        public string Surname { get; set; }

        [JsonProperty("birthday")]
        public DateTime Birthday { get; set; }

        [JsonProperty("age")]
        public int Age { get; set; }
    }

    public class PlayerStatistics
    {
        [JsonProperty("rating_bot")]
        public float? RatingVsBot { get; set; }

        [JsonProperty("performance_1_bot")]
        public float? PerformanceCenterVsBot { get; set; }

        [JsonProperty("performance_2_bot")]
        public float? PerformanceBorderVsBot { get; set; }

        [JsonProperty("count_games_1")]
        public int CountGamesCenter { get; set; }

        [JsonProperty("count_games_2")]
        public int CountGamesBorder { get; set; }
    }

    public class PlayerInfo
    {
        [JsonProperty("personal_info")]
        public PersonalInfo PersonalInfo { get; set; }

        [JsonProperty("player_statistics")]
        public PlayerStatistics PlayerStatistics { get; set; }
    }

    public class PlayerStanding
    {
        [JsonProperty("id_player")]
        public int IdPlayer { get; set; }

        [JsonProperty("login")]
        public string UserName { get; set; }

        [JsonProperty("rating")]
        public float Rating { get; set; }

        [JsonProperty("performance_1")]
        public float PerformanceCenter { get; set; }

        [JsonProperty("performance_2")]
        public float PerformanceBorder { get; set; }
    }

    public class Standings
    {
        [JsonProperty("standings_vs_bot")]
        public List<PlayerStanding> StandingsVsBot { get; set; }

        [JsonProperty("man_vs_bot")]
        public PlayerStanding ManVsBot { get; set; }
    }
}
