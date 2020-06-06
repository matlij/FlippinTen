using FlippinTen.Core.Entities;
using FlippinTen.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlippinTen.ConsoleApp
{
    class GameMenuConsole
    {
        private readonly ICardGameService _gameService;

        public GameMenuConsole(ICardGameService gameService)
        {
            _gameService = gameService;
        }

        public async Task<Core.Entities.GameFlippinTen> PickGame(string playerName)
        {
            var onGoingGames = await _gameService.GetByPlayer(playerName);

            do
            {
                Console.WriteLine("Anslut till pågående spel (tryck enter för att välja 1):");

                var i = 1;
                foreach (var game in onGoingGames)
                {
                    Console.WriteLine($"{i++}. {game.Name}");
                }
                Console.WriteLine($"{i}. Skapa nytt spel");

                var input = Console.ReadLine();
                if (input.ToUpper() == "Q")
                    return null;
                else if (string.IsNullOrWhiteSpace(input))
                    input = 1.ToString();

                if (!int.TryParse(input, out var index) ||
                    index > onGoingGames.Count + 1 ||
                    index < 0)
                {
                    Console.WriteLine($"Input måste vara en siffra mellan 1 - {onGoingGames.Count + 1}");
                    continue;
                }

                return index == i ?
                    await CreateGame(playerName) :
                    onGoingGames[index - 1];

            } while (true);
        }

        private async Task<Core.Entities.GameFlippinTen> CreateGame(string playerName)
        {
            do
            {
                Console.WriteLine("Namn på spel: ");
                //var gameName = Console.ReadLine();
                var gameName = "test";
                if (string.IsNullOrEmpty(gameName))
                {
                    Console.WriteLine("Ogiltigt namn på spel. Prova igen.");
                    continue;
                }

                Console.WriteLine("Utmanare: ");
                //var opponent = Console.ReadLine();
                var opponent = "kalle";
                if (string.IsNullOrEmpty(opponent))
                {
                    Console.WriteLine("Ogiltigt namn på utmanare. Prova igen.");
                }

                var opponents = new List<string> { opponent };
                var addedGame = await _gameService.Add(gameName, playerName, opponents);
                return addedGame;
            } while (true);
        }
    }
}
