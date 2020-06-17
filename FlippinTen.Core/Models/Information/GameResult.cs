using FlippinTen.Core.Entities;
using FlippinTen.Core.Entities.Enums;
using System.Collections.Generic;
using System.ComponentModel;

namespace FlippinTen.Core.Models.Information
{
    public class GameResult
    {
        private readonly string _errorMessage;

        public GameResult(string errorMessage) : this(null, null, CardPlayResult.Invalid, new Card[0])
        {
            _errorMessage = errorMessage;
        }

        public GameResult(string gameIdentifier, string userIdentifier, CardPlayResult result, IEnumerable<Card> cards)
        {
            GameIdentifier = gameIdentifier;
            UserIdentifier = userIdentifier;
            Result = result;
            Cards = cards;
        }

        public string UserIdentifier { get; }
        public string GameIdentifier { get; }
        public CardPlayResult Result { get; }
        public IEnumerable<Card> Cards { get; }

        public bool Invalid()
        {
            return
                Result == CardPlayResult.Unknown ||
                Result == CardPlayResult.Invalid;
        }

        public string GetResultInfo(string requestingUser = null)
        {
            var player = UserIdentifier == requestingUser ? "Du" : UserIdentifier;

            switch (Result)
            {
                case CardPlayResult.Succeded:
                case CardPlayResult.CardTwoPlayed:
                    return $"{player} har lagt {string.Join(", ", Cards)}";
                case CardPlayResult.ChanceFailed:
                    return $"{player} chansade och misslyckades...";
                case CardPlayResult.CardsFlipped:
                    return $"{player} vände kort på bord!";
                case CardPlayResult.CardsOnTablePickedUp:
                    return $"{player} tog upp kort på bord...";
                case CardPlayResult.Unknown:
                case CardPlayResult.Invalid:
                    return "Ogilitigt drag. " + _errorMessage;
                default:
                    throw new InvalidEnumArgumentException(nameof(Result), (int)Result, typeof(CardPlayResult));
            }
        }
    }
}
