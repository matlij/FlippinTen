using FlippinTen.Core;
using FlippinTen.Core.Repository;
using FlippinTen.Core.Services;
using FlippinTen.Core.Utilities;
using FlippinTen.Utilities;
using Microsoft.AspNetCore.SignalR.Client;
using Models;
using Models.Constants;
using System;
using System.Linq;
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

        private static async Task<GamePlay> StartGameMenu(string playerName)
        {
            var genericRepository = new GenericRepository();
            var gameMenu = new GameMenuConsole(new GameMenuService(genericRepository));
            return await gameMenu.PickGame(playerName);
        }

        private static async Task StartGameConsole(string playerName, GamePlay game)
        {
            var hubConnection = new ServerHubConnection(new HubConnectionBuilder(), UriConstants.BaseUri + UriConstants.GameHub);
            var gameConsole = new GamePlayConsole(new GamePlayService(hubConnection, new CardGameEngine(game, playerName)));

            await gameConsole.StartGame();
        }
    }
}
