using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Models;
using Models;
using Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlippinTen.Core
{
    public class CardGameEngine : ICardGameEngine
    {
        private readonly string _playerName;
        private const int _cardTwo = 2;
        private const int _cardTen = 10;

        public GamePlay Game { get; set; }

        public Player Player
        {
            get
            {
                return Game.Players.FirstOrDefault(p => p.Name == _playerName);
            }
        }

        public CardGameEngine(GamePlay game, string playerName)
        {
            if (game is null) throw new ArgumentNullException(nameof(game));
            if (string.IsNullOrEmpty(playerName)) throw new ArgumentNullException(nameof(playerName));

            Game = game;
            _playerName = playerName;
            if (Player is null)
                throw new ArgumentException($"Player '{playerName}' is not included in game: '{Game.Name}'");
        }

        public bool PlayCard(int cardNr)
        {
            if (!IsPlayersTurn())
                return false;

            var cardCollection = Player.CardsOnHand.FirstOrDefault(c => c.CardNr == cardNr);
            if (cardCollection == null)
                throw new ArgumentException($"Cand find card nr {cardNr} in Player {Player.Name}");

            if (!CanPlayCard(cardCollection.CardNr))
                return false;

            cardCollection.Cards.ForEach(c => Game.CardsOnTable.Push(c));

            Player.CardsOnHand.Remove(cardCollection);

            var minimumCardOnHands = 3;
            AddCardsToHand(minimumCardOnHands);

            if (cardNr == 10)
            {
                Game.CardsOnTable.Clear();
            }

            if (cardNr != _cardTwo && cardNr != _cardTen)
            {
                UpdateTurnIndex();
            }

            return true;
        }

        public bool PickUpCardsFromTable()
        {
            if (!IsPlayersTurn())
                return false;

            var cardsOnTable = Game.CardsOnTable.Select(c => c);
            Player.AddCardsToHand(cardsOnTable);

            Game.CardsOnTable.Clear();

            UpdateTurnIndex();

            return true;
        }

        public GamePlayResult PlayChanceCard()
        {
            if (!IsPlayersTurn() || Game.DeckOfCards.Count < 1)
                return GamePlayResult.InvalidPlay;

            var chanceCard = Game.DeckOfCards.Peek();
            Player.AddCardsToHand(new List<Card> { chanceCard });

            if (!PlayCard(chanceCard.Number))
            {
                PickUpCardsFromTable();
                return GamePlayResult.ChanceFailed;
            }

            return GamePlayResult.ChanceSucceded;
        }

        private bool IsPlayersTurn()
        {
            return Game.PlayerTurnIdentifier == Player.Identifier;
        }

        private void AddCardsToHand(int minimumCardOnHands)
        {
            var numberOfCardsToPickUp = Player.CardsOnHand.Count < minimumCardOnHands
                ? minimumCardOnHands - Player.CardsOnHand.Count
                : 0;
            var cardsToPickup = new List<Card>();
            for (var i = 0; i < numberOfCardsToPickUp; i++)
            {
                if (Game.DeckOfCards.Count == 0)
                    break;

                cardsToPickup.Add(Game.DeckOfCards.Pop());
            }

            Player.AddCardsToHand(cardsToPickup);
        }

        private bool CanPlayCard(int cardNr)
        {
            if (Game.CardsOnTable.Count == 0 ||
                cardNr == _cardTwo ||
                cardNr == _cardTen)
            { 
                return true;
            }

            var cardOnTable = Game.CardsOnTable.Peek();
            return cardNr > cardOnTable.Number ||
                cardOnTable.Number == 2;
        }

        private void UpdateTurnIndex()
        {
            var currentTurnIndex = Game.Players.FindIndex(p => p.Identifier == Game.PlayerTurnIdentifier);
            if (currentTurnIndex < 0)
            {
                throw new InvalidOperationException($"Failed to update player turn. Cant find player with identifier: {Game.PlayerTurnIdentifier}");
            }

            Game.PlayerTurnIdentifier = Game.Players[++currentTurnIndex % Game.Players.Count].Identifier;
        }
    }
}
