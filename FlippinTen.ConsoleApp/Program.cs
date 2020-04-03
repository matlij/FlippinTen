using FlippinTen.Core;
using FlippinTen.Core.Entities;
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

            var game = await StartGameMenu(playerName);

            StartGameConsole(playerName, game).Wait();
        }

        private static async Task<CardGame> StartGameMenu(string playerName)
        {
            var genericRepository = new GenericRepository();
            var cardGameService = new CardGameService(genericRepository);
            var gameMenu = new GameMenuConsole(cardGameService, new CardGameUtilities(new CardUtilities()));
            return await gameMenu.PickGame(playerName);
        }

        private static async Task StartGameConsole(string playerName, CardGame game)
        {
            var hubConnection = new ServerHubConnection(new HubConnectionBuilder(), UriConstants.BaseUri + UriConstants.GameHub);
            var onlineGameService = new OnlineGameService(hubConnection);
            var gameConsole = new GamePlayConsole(game, onlineGameService, playerName);

            await gameConsole.StartGame();
        }
    }
}
