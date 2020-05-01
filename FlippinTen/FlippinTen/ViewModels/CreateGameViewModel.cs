using FlippinTen.Core.Entities;
using FlippinTen.Core.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using FlippinTen.Models;
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
        private readonly ICardGameService _cardGameService;

        //public Command CreateGameCommand { get; set; }
        //public string GameName { get; set; }
        //public string Opponent { get; set; }
        public string PlayerName { get; set; }

        public CreateGameViewModel(ICardGameService cardGameService, string playerName)
        {
            _cardGameService = cardGameService;
            PlayerName = playerName;

            //CreateGameCommand = new Command(async () => await OnCreateGameClicked());
        }

        public async Task<CardGame> CreateGame(string gameName, string opponent) 
        {
            try
            {
                return await _cardGameService.Add(gameName, PlayerName, new List<string> { opponent });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Create game failed: " + ex.Message);
                throw;
            }
        }
    }
}
