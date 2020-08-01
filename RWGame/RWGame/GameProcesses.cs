using Acr.UserDialogs;
using RWGame.Classes;
using RWGame.Classes.ResponseClases;
using System;
using System.Threading;
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

        static private async Task<bool> StartGame(ServerWorker serverWorker, Game game, Func<bool> isCancel)
        {
            GameStateEnum GameState = game.GameState;
            while (GameState != GameStateEnum.ACTIVE && GameState != GameStateEnum.WAIT)
            {
                if (isCancel()) return false;
                await Task.Delay(1000);
                GameState = (await serverWorker.TaskPlayGame(idGame: game.IdGame)).GameState;
            }
            return true;
        }

        static public async Task<bool> StartGame(ServerWorker serverWorker, Game game)
        {
            string alertMessage;
            if (game.Player1 is null || game.Player2 is null)
            {
                alertMessage = "Try to find player...";
            }
            else
            {
                alertMessage = "Wait for " + (game.IdPlayer == 0 ? game.PlayerUserName2 : game.PlayerUserName1);
            }
            alertMessage += "\n\nAsk your friend to update list of started games\nAnd tap on game #" + game.IdGame.ToString() + "\n";

            var cancelSrc = new CancellationTokenSource();
            var config = new ProgressDialogConfig()
                .SetTitle(alertMessage)
                .SetIsDeterministic(false)
                .SetMaskType(MaskType.Black)
                .SetCancel(onCancel: cancelSrc.Cancel);

            if (game.GameState == GameStateEnum.CONNECT)
            {
                using (UserDialogs.Instance.Progress(config))
                {
                    await StartGame(serverWorker, game, () => cancelSrc.Token.IsCancellationRequested);
                }
            } else
            {
                await StartGame(serverWorker, game, () => false);
            }
            bool cancelGame = cancelSrc.IsCancellationRequested;
            return cancelGame;
        }
    }
}
