using FlippinTen.Core;
using FlippinTen.Core.Entities;
using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Repository;
using FlippinTen.Core.Services;
using FlippinTen.Core.Utilities;
using FlippinTen.Utilities;
using Microsoft.AspNetCore.SignalR.Client;
using Models.Constants;
using System;
using System.Threading.Tasks;

namespace FlippinTen.ConsoleApp
{
    class Program
    {
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

            StartGameConsole(game, cardGameService).Wait();
        }

        private static async Task<CardGame> StartGameMenu(string playerName, ICardGameService cardGameService)
        {
            var gameMenu = new GameMenuConsole(cardGameService, new CardGameUtilities(new CardUtilities()));
            return await gameMenu.PickGame(playerName);
        }

        private static async Task StartGameConsole(CardGame game, ICardGameService cardGameService)
        {
            var hubConnection = new ServerHubConnection(new HubConnectionBuilder(), UriConstants.BaseUri + UriConstants.GameHub);
            var onlineGameService = new OnlineGameService(cardGameService, hubConnection, game);
            var gameConsole = new GamePlayConsole(onlineGameService);

            await gameConsole.StartGame();
        }
    }
}
