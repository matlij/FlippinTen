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
        public string PlayerName { get; set; }

        public CreateGameViewModel(ICardGameService cardGameService, string playerName)
        {
            _cardGameService = cardGameService;
            PlayerName = playerName;
        }

        public async Task<GameFlippinTen> CreateGame(string gameName, string opponent)
        {
            return await _cardGameService.Add(gameName, PlayerName, new List<string> { opponent });
        }
    }
}
