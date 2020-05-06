using FlippinTen.Core;
using FlippinTen.Core.Entities;
using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Repository;
using FlippinTen.Core.Services;
using FlippinTen.Core.Utilities;
using FlippinTen.Utilities;
using Microsoft.AspNetCore.SignalR.Client;
using FlippinTen.Models.Constants;
using System;
using System.Threading.Tasks;

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

            var genericRepository = new GenericRepository();
            var cardUtilities = new CardUtilities();
            var cardGameUtilities = new CardGameUtilities(cardUtilities);
            var cardGameService = new CardGameService(genericRepository, cardGameUtilities);
            var game = await StartGameMenu(playerName, cardGameService);

            await StartGameConsole(game, cardGameService);

            Console.WriteLine("Closing connection");
            await _gameConsole.EndGame();
        }

        private static async Task<CardGame> StartGameMenu(string playerName, ICardGameService cardGameService)
        {
            var gameMenu = new GameMenuConsole(cardGameService);
            return await gameMenu.PickGame(playerName);
        }

        private static async Task StartGameConsole(CardGame game, ICardGameService cardGameService)
        {
            var hubConnection = new ServerHubConnection(new HubConnectionBuilder(), UriConstants.BaseUri + UriConstants.GameHub);
            var onlineGameService = new OnlineGameService(cardGameService, hubConnection, game);
            _gameConsole = new GamePlayConsole(onlineGameService);

            await _gameConsole.StartGame();
        }
    }
}
