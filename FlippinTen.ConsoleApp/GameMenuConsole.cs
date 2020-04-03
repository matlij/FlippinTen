using FlippinTen.Core.Entities;
using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Repository;
using FlippinTen.Core.Services;
using FlippinTen.Core.Utilities;
using Microsoft.AspNetCore.SignalR.Client;
using Models;
using Models.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlippinTen.ConsoleApp
{
    class GameMenuConsole
    {
        private readonly CardGameService _gameService;
        private readonly ICardGameUtilities _gameUtilities;

        public GameMenuConsole(CardGameService gameService, ICardGameUtilities gameUtilities)
        {
            //_hubConnection = new ServerHubConnection(new HubConnectionBuilder(), $"{UriConstants.BaseUri}{UriConstants.GameHub}");
            _gameService = gameService;
            _gameUtilities = gameUtilities;
        }

        public async Task<CardGame> PickGame(string playerName)
        {
            var onGoingGames = await _gameService.Get(playerName);

            do
            {
                Console.WriteLine("Anslut till pågående spel:");

                var i = 1;
                foreach (var game in onGoingGames)
                {
                    Console.WriteLine($"{i++}. {game.Name}");
                }
                Console.WriteLine($"{i}. Skapa nytt spel");

                var input = Console.ReadLine();
                if (input.ToUpper() == "Q")
                    return null;

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

        private async Task<CardGame> CreateGame(string playerName)
        {
            do
            {
                Console.WriteLine("Namn på spel: ");
                var gameName = Console.ReadLine();
                if (string.IsNullOrEmpty(gameName))
                {
                    Console.WriteLine("Ogiltigt namn på spel. Prova igen.");
                }

                Console.WriteLine("Utmanare: ");
                var opponent = Console.ReadLine();
                if (string.IsNullOrEmpty(opponent))
                {
                    Console.WriteLine("Ogiltigt namn på utmanare. Prova igen.");
                }

                var players = new List<string> { playerName, opponent };
                var game = _gameUtilities.CreateGame(gameName, players);

                return await _gameService.Add(game);
            } while (true);
        }
    }
}
