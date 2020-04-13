using FlippinTen.Core.Models;
using FlippinTen.Utilities;
//using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlippinTen.Core.Entities
{
    public class PlayerInformation : IEquatable<PlayerInformation>
    {
        public PlayerInformation(string identifier)
        {
            Identifier = identifier;
        }
        public string Identifier { get; }
        public bool IsPlayersTurn { get; set; }

        public bool Equals(PlayerInformation other)
        {
            if (other == null || string.IsNullOrEmpty(other.Identifier))
            {
                return false;
            }

            return other.Identifier == Identifier;
        }
        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }
    }

    public class CardGame
    {
        private const int _cardTwo = 2;
        private const int _cardTen = 10;

        public string Identifier { get; }
        public string Name { get; }
        public Stack<Card> DeckOfCards { get; }
        public Stack<Card> CardsOnTable { get; }
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
            DeckOfCards.Clear();
            foreach (var card in deckOfCards)
            {
                DeckOfCards.Push(card);
            }

            CardsOnTable.Clear();
            foreach (var card in cardsOnTable)
            {
                CardsOnTable.Push(card);
            }

            PlayerInformation.Clear();
            PlayerInformation.AddRange(playerInformation);
        }

        public bool IsPlayersTurn()
        {
            var playerInfo = PlayerInformation.First(p => p.Identifier == Player.UserIdentifier);

            return playerInfo.IsPlayersTurn;
        }

        public bool PlayCard(int cardNr)
        {
            if (!CanPlayCard(cardNr))
            {
                return false;
            }

            if (!Player.PlayCardOnHand(cardNr, out var cardCollection))
            {
                throw new ArgumentException($"Cand find card nr {cardNr} in Player {Player.UserIdentifier}");
            }

            cardCollection.Cards.ForEach(c => CardsOnTable.Push(c));

            //CurrentPlayer.CardsOnHand.Remove(cardCollection);

            PickUpCards(minimumCardOnHands: 3);

            if (cardNr == 10)
            {
                CardsOnTable.Clear();
            }

            if (cardNr != _cardTwo && cardNr != _cardTen)
            {
                ChangeCurrentPlayer();
            }

            return true;
        }

        public bool PickUpCards()
        {
            var cardsOnTable = CardsOnTable.Select(c => c);
            Player.AddCardsToHand(cardsOnTable);

            CardsOnTable.Clear();

            ChangeCurrentPlayer();

            return true;
        }

        public GamePlayResult PlayChanceCard()
        {
            if (DeckOfCards.Count < 1)
                return GamePlayResult.InvalidPlay;

            var chanceCard = DeckOfCards.Peek();
            Player.AddCardsToHand(new List<Card> { chanceCard });

            if (!PlayCard(chanceCard.Number))
            {
                PickUpCards();
                ChangeCurrentPlayer();
                return GamePlayResult.ChanceFailed;
            }

            return GamePlayResult.ChanceSucceded;
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

        private bool CanPlayCard(int cardNr)
        {
            if (!IsPlayersTurn())
                return false;

            if (CardsOnTable.Count == 0 ||
                cardNr == _cardTwo ||
                cardNr == _cardTen)
            {
                return true;
            }

            var cardOnTable = CardsOnTable.Peek();
            return cardNr > cardOnTable.Number ||
                cardOnTable.Number == 2;
        }
    }
}
