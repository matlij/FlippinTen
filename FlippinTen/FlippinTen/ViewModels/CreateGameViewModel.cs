using FlippinTen.Core.Entities;
using FlippinTen.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

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
