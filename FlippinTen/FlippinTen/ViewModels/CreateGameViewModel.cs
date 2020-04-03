using FlippinTen.Core.Entities;
using FlippinTen.Core.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FlippinTen.ViewModels
{
    public class CreateGameViewModel : BaseViewModel
    {
        private readonly IGameMenuService _gameMenuService;

        //public Command CreateGameCommand { get; set; }
        //public string GameName { get; set; }
        //public string Opponent { get; set; }
        public string PlayerName { get; set; }

        public CreateGameViewModel(IGameMenuService gameMenuService, string playerName)
        {
            _gameMenuService = gameMenuService;
            PlayerName = playerName;

            //CreateGameCommand = new Command(async () => await OnCreateGameClicked());
        }

        public async Task<CardGame> CreateGame(string gameName, string opponent) 
        {
            try
            {
                return await _gameMenuService.CreateGame(PlayerName, gameName, opponent);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Create game failed: " + ex.Message);
                throw;
            }
        }
    }
}
