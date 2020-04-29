using FlippinTen.Core.Models;
using FlippinTen.Utilities;
//using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlippinTen.Core.Entities
{
    public class CardGame
    {
        private const int _cardTwoNumber = 2;
        private const int _cardTenNumber = 10;

        public string Identifier { get; }
        public string Name { get; }
        public Stack<Card> DeckOfCards { get; private set; }
        public Stack<Card> CardsOnTable { get; private set; }
        public Player Player { get; }
        public List<PlayerInformation> PlayerInformation { get; }
        public bool AllPlayersOnline { get; set; }

        public CardGame(
            string identifier,
            string name,
            Stack<Card> deckOfCards,
            Stack<Card> cardsOnTable,
            Player currentPlayer,
            List<PlayerInformation> playerInformation)
        {
            Identifier = identifier;
            Name = name;
            DeckOfCards = deckOfCards;
            CardsOnTable = cardsOnTable;
            Player = currentPlayer;
            PlayerInformation = playerInformation;
        }

        public void UpdateGame(IEnumerable<Card> deckOfCards, IEnumerable<Card> cardsOnTable, List<PlayerInformation> playerInformation)
        {
            DeckOfCards = new Stack<Card>(new Stack<Card>(deckOfCards));
            CardsOnTable = new Stack<Card>(new Stack<Card>(cardsOnTable));

            PlayerInformation.Clear();
            PlayerInformation.AddRange(playerInformation);
        }

        public bool IsPlayersTurn()
        {
            var playerInfo = PlayerInformation.First(p => p.Identifier == Player.UserIdentifier);

            return playerInfo.IsPlayersTurn;
        }

        public GamePlayResult PlayOrSelectCard(int cardId)
        {
            var card = Player.CardsOnHand.FirstOrDefault(c => c.ID == cardId);
            if (card == null)
                throw new ArgumentException($"{cardId} not found in player {Player.UserIdentifier}");

            var selectedCards = Player.CardsOnHand
                .Where(s => s.Selected)
                .ToList();

            var hasSelectedCardWithDifferentNumber = selectedCards.Count > 0 && selectedCards.First().Number != card.Number;
            if (hasSelectedCardWithDifferentNumber)
                return GamePlayResult.Invalid;

            if (!card.Selected)
            {
                selectedCards.Add(card);
                if (PlayerHasMoreCardsWithSameNumber(selectedCards))
                {
                    card.Selected = true;
                    return GamePlayResult.CardSelected;
                }
            }

            var result = PlayCard(selectedCards);
            return result
                ? GamePlayResult.Succeded
                : GamePlayResult.Invalid;
        }

        public GamePlayResult PlayChanceCard()
        {
            if (DeckOfCards.Count < 1)
                return GamePlayResult.Invalid;

            var chanceCard = DeckOfCards.Peek();
            var chanceCardList = new List<Card> { chanceCard };
            Player.AddCardsToHand(chanceCardList);

            if (!PlayCard(chanceCardList))
            {
                PickUpCards();
                ChangeCurrentPlayer();
                return GamePlayResult.Failed;
            }

            return GamePlayResult.Succeded;
        }

        public GamePlayResult PickUpCards()
        {
            if (CardsOnTable.Count == 0)
                return GamePlayResult.Invalid;

            Player.AddCardsToHand(CardsOnTable.ToList());

            CardsOnTable.Clear();

            ChangeCurrentPlayer();

            return GamePlayResult.Succeded;
        }

        private bool PlayCard(List<Card> cards)
        {
            if (!CanPlayCard(cards.First()))
                return false;

            foreach (var card in cards)
            {
                if (!Player.CardsOnHand.Remove(card))
                    throw new ArgumentException($"Player '{Player.UserIdentifier}' cannot play '{card}' since it doesn't exist in players CardOnHand list.");

                CardsOnTable.Push(card);
            }

            PickUpCards(minimumCardOnHands: 3);

            var number = cards.First().Number;
            if (number == _cardTenNumber)
            {
                CardsOnTable.Clear();
            }
            if (number != _cardTwoNumber && number != _cardTenNumber)
            {
                ChangeCurrentPlayer();
            }

            return true;
        }

        private void ChangeCurrentPlayer()
        {
            var currentPlayer = PlayerInformation.FirstOrDefault(p => p.IsPlayersTurn);
            if (currentPlayer == null)
            {
                throw new InvalidOperationException($"Failed to update player turn. Cant find player with identifier: {Player.UserIdentifier}");
            }
            currentPlayer.IsPlayersTurn = false;

            var currentPlayerIndex = PlayerInformation.IndexOf(currentPlayer);
            var newPlayersTurn = PlayerInformation[++currentPlayerIndex % PlayerInformation.Count];
            newPlayersTurn.IsPlayersTurn = true;
        }

        private void PickUpCards(int minimumCardOnHands)
        {
            var numberOfCardsToPickUp = Player.CardsOnHand.Count < minimumCardOnHands
                ? minimumCardOnHands - Player.CardsOnHand.Count
                : 0;
            var cardsToPickup = new List<Card>();
            for (var i = 0; i < numberOfCardsToPickUp; i++)
            {
                if (DeckOfCards.Count == 0)
                    break;

                cardsToPickup.Add(DeckOfCards.Pop());
            }

            Player.AddCardsToHand(cardsToPickup);
        }

        private bool CanPlayCard(Card card)
        {
            if (!IsPlayersTurn())
                return false;

            if (CardsOnTable.Count == 0 ||
                card.Number == _cardTwoNumber ||
                card.Number == _cardTenNumber)
            {
                return true;
            }

            var cardOnTable = CardsOnTable.Peek();
            return card.Number >= cardOnTable.Number;
        }

        private bool PlayerHasMoreCardsWithSameNumber(List<Card> cards)
        {
            var otherCardsOnHand = Player.CardsOnHand
                .Except(cards)
                .ToList();

            var number = cards.First().Number;
            var result = otherCardsOnHand.Any(c => c.Number == number);

            return result;
        }

        private static bool CardsHasSameNumber(List<Card> cards, out int number)
        {
            var firstCardNumber = cards.First().Number;
            var result = cards.All(c => c.Number == firstCardNumber);

            number = result ? firstCardNumber : default;
            return result;
        }
    }
}
