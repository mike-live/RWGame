using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RWGame
{
    class GameProcesses
    {
        static public async Task<Game> MakeGameWithBot(ServerWorker serverWorker)
        {
            return await MakeGame(serverWorker, idPlayer: 10);
        }

        static public async Task<Game> MakeGameWithPlayer(ServerWorker serverWorker, int idPlayer = -1)
        {
            return await MakeGame(serverWorker, idPlayer: idPlayer);
        }

        static public async Task<Game> MakeSavedGame(ServerWorker serverWorker, int idGame)
        {
            return await MakeGame(serverWorker, idGame: idGame);
        }

        static public async Task<Game> MakeGame(ServerWorker serverWorker, int idPlayer = -1, int idGame = -1)
        {
            idGame = (await serverWorker.TaskPlayGame(idPlayer: idPlayer, idGame: idGame)).IdGame;
            Game game = await serverWorker.TaskGetGame(idGame: idGame);
            game.Turns = await serverWorker.TaskGetGameTurns(idGame);
            return game;
        }

        static public async Task<bool> StartGame(ServerWorker serverWorker, Game game)
        {
            GameStateEnum GameState = game.GameState;
            while (GameState != GameStateEnum.ACTIVE && GameState != GameStateEnum.WAIT)
            {
                await Task.Delay(1000);
                GameState = (await serverWorker.TaskPlayGame(idGame: game.IdGame)).GameState;
            }
            return true;
        }
    }
}
