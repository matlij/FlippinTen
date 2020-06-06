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

        public GameResult(string gameIdentifier, string userIdentifier, CardPlayResult result, Card card) 
            : this(gameIdentifier, userIdentifier, result, new Card[] { card })
        {
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

        public bool ShouldUpdateGame()
        {
            return !Invalid() && Result != CardPlayResult.CardSelected;
        }

        public bool Invalid()
        {
            return
                Result == CardPlayResult.Unknown ||
                Result == CardPlayResult.Invalid;
        }

        public bool ShouldSwitchTurn()
        {
            return
                Result == CardPlayResult.ChanceFailed ||
                Result == CardPlayResult.ChanceSucceded ||
                Result == CardPlayResult.Succeded ||
                Result == CardPlayResult.CardsOnTablePickedUp;
        }

        public string GetResultInfo(string requestingUser = null)
        {
            var player = UserIdentifier == requestingUser ? "Du" : UserIdentifier;

            switch (Result)
            {
                case CardPlayResult.Succeded:
                case CardPlayResult.CardTwoPlayed:
                    return $"{player} har lagt {Join(Cards)}";
                case CardPlayResult.ChanceFailed:
                case CardPlayResult.ChanceSucceded:
                    return $"{player} chansade och {(Result == CardPlayResult.ChanceSucceded ? "lyckades!" : "misslyckades...")}";
                case CardPlayResult.CardSelected:
                    return $"{player} har markerat {Join(Cards)}";
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

        private static string Join(IEnumerable<Card> cards)
        {
            return string.Join(", ", cards);
        }
    }
}
