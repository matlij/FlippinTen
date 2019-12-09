using FlippinTen.Core.Repository;
using FlippinTen.Core.Services;
using FlippinTen.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlippinTen.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var repo = new GenericRepository();
            var gameService = new GameMenuService(repo);
            var hubConnection = new ServerHubConnection(new HubConnectionBuilder);

            var game = new GameView(gameService);
        }
    }
}
