using FlippinTen.Core.Entities;
using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Utilities;
using System.Linq;

namespace FlippinTen.Core.Factories
{
    public class ComputerPlayerFactory
    {
        public static ComputerPlayer Create(ICardGameService gameService, IServerHubConnection hubConnection, GameFlippinTen game)
        {
            var opponent = game.PlayerInformation
                .Single(p => p.Identifier != game.Player.UserIdentifier);
            var opponentGame = new CardGame(gameService, hubConnection, game.Identifier, opponent.Identifier);
            return new ComputerPlayer(opponentGame);
        }
    }
}
