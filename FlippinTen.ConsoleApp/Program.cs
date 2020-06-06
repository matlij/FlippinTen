using FlippinTen.Core;
using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Repository;
using FlippinTen.Core.Services;
using FlippinTen.Utilities;
using System;
using System.Threading.Tasks;
using System.Linq;
using FlippinTen.Core.Factories;
using FlippinTen.Core.Entities;

namespace FlippinTen.ConsoleApp
{
    class Program
    {
        private static GamePlayConsole _gameConsole;

        static void Main(string[] args)
        {
            RunGame().Wait();
        }

        private static async Task RunGame()
        {
            Console.WriteLine("Ditt namn: ");
            var playerName = Console.ReadLine();
            if (string.IsNullOrEmpty(playerName))
            {
                playerName = "matte";
            }

            var playOnline = false;
            var cardGameService = CreateCardService(playOnline);
            var game = await StartGameMenu(playerName, cardGameService);

            var hubConnection = ServerHubConnectionFactory.Create(cardGameService, playOnline);
            var cardGame = new CardGame(cardGameService, hubConnection, game.Identifier, game.Player.UserIdentifier);

            _gameConsole = new GamePlayConsole(cardGame);
            if (playOnline)
            {
                await _gameConsole.StartGame();
            }
            else
            {
                var computerPlayer = ComputerPlayerFactory.Create(cardGameService, hubConnection, game);
                await _gameConsole.StartGameAgainstComputer(computerPlayer);
            }

            Console.WriteLine("Closing connection");
            _gameConsole.EndGame();
        }

        private static ICardGameService CreateCardService(bool playOnline)
        {
            var cardUtilities = new CardUtilities();
            var cardGameUtilities = new CardGameUtilities(cardUtilities);
            if (playOnline)
            {
                var genericRepository = new GenericRepository();
                return new OnlineCardGameService(genericRepository, cardGameUtilities);
            }
            else
            {
                return new OfflineCardGameService(cardGameUtilities);
            }
        }

        private static async Task<GameFlippinTen> StartGameMenu(string playerName, ICardGameService cardGameService)
        {
            var gameMenu = new GameMenuConsole(cardGameService);
            return await gameMenu.PickGame(playerName);
        }
    }
}
