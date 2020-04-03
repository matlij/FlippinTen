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
        private const int _cardTwo = 2;
        private const int _cardTen = 10;

        public string Identifier { get; }
        public string Name { get; }
        public List<Player> Players { get; }
        public Stack<Card> DeckOfCards { get; }
        public Stack<Card> CardsOnTable { get; }
        public Player CurrentPlayer { get; private set; }

        public CardGame(
            string identifier,
            string name,
            List<Player> players,
            Stack<Card> deckOfCards,
            Stack<Card> cardsOnTable,
            Player currentPlayer)
        {
            Identifier = identifier;
            Name = name;
            Players = players;
            DeckOfCards = deckOfCards;
            CardsOnTable = cardsOnTable;
            CurrentPlayer = currentPlayer;
            CurrentPlayer.IsPlayersTurn = true;
        }

        public Player GetPlayer(string userIdentifier)
        {
            var player = Players.FirstOrDefault(p => p.UserIdentifier == userIdentifier);
            if (player == null)
            {
                throw new ArgumentException($"Failed to set player '{userIdentifier}' to state connected.");
            }

            return player;
        }

        public bool AllPlayersOnline()
        {
            return Players.All(p => p.IsConnected);
        }

        public void UpdateGame(Player currentPlayer, List<Card> deckOfCards, List<Card> cardsOnTable)
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

            var playerToUpdate = Players.FirstOrDefault(p => p.UserIdentifier == currentPlayer.UserIdentifier);
            playerToUpdate = currentPlayer;
        }

        public bool PlayCard(int cardNr)
        {
            if (!CanPlayCard(cardNr))
            {
                return false;
            }

            if (!CurrentPlayer.PlayCardOnHand(cardNr, out var cardCollection))
            {
                throw new ArgumentException($"Cand find card nr {cardNr} in Player {CurrentPlayer.UserIdentifier}");
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
            CurrentPlayer.AddCardsToHand(cardsOnTable);

            CardsOnTable.Clear();

            ChangeCurrentPlayer();

            return true;
        }

        public GamePlayResult PlayChanceCard()
        {
            if (DeckOfCards.Count < 1)
                return GamePlayResult.InvalidPlay;

            var chanceCard = DeckOfCards.Peek();
            CurrentPlayer.AddCardsToHand(new List<Card> { chanceCard });

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
            var currentTurnIndex = Players.FindIndex(p => p.UserIdentifier == CurrentPlayer.UserIdentifier);
            if (currentTurnIndex < 0)
            {
                throw new InvalidOperationException($"Failed to update player turn. Cant find player with identifier: {CurrentPlayer.UserIdentifier}");
            }
            Players.ForEach(p => p.IsConnected = false);
            CurrentPlayer = Players[++currentTurnIndex % Players.Count];
            CurrentPlayer.IsPlayersTurn = true;
        }

        private void PickUpCards(int minimumCardOnHands)
        {
            var numberOfCardsToPickUp = CurrentPlayer.CardsOnHand.Count < minimumCardOnHands
                ? minimumCardOnHands - CurrentPlayer.CardsOnHand.Count
                : 0;
            var cardsToPickup = new List<Card>();
            for (var i = 0; i < numberOfCardsToPickUp; i++)
            {
                if (DeckOfCards.Count == 0)
                    break;

                cardsToPickup.Add(DeckOfCards.Pop());
            }

            CurrentPlayer.AddCardsToHand(cardsToPickup);
        }

        private bool CanPlayCard(int cardNr)
        {
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

    //public class CardGame
    //{
    //    private const int _cardTwo = 2;
    //    private const int _cardTen = 10;

    //    public string Identifier { get; }
    //    public string Name { get; }
    //    public List<Player> Players { get; }
    //    public Stack<Card> DeckOfCards { get; }
    //    public Stack<Card> CardsOnTable { get; }
    //    public Player CurrentPlayer { get; private set; }

    //    public CardGame(
    //        string identifier,
    //        string name,
    //        List<Player> players,
    //        Stack<Card> deckOfCards,
    //        Stack<Card> cardsOnTable,
    //        Player currentPlayer)
    //    {
    //        Identifier = identifier;
    //        Name = name;
    //        Players = players;
    //        DeckOfCards = deckOfCards;
    //        CardsOnTable = cardsOnTable;
    //        CurrentPlayer = currentPlayer;
    //    }

    //    public bool PlayCard(int cardNr)
    //    {
    //        if (!CanPlayCard(cardNr))
    //        {
    //            return false;
    //        }

    //        if (!CurrentPlayer.PlayCardOnHand(cardNr, out var cardCollection))
    //        {
    //            throw new ArgumentException($"Cand find card nr {cardNr} in Player {CurrentPlayer.UserIdentifier}");
    //        }

    //        cardCollection.Cards.ForEach(c => CardsOnTable.Push(c));

    //        //CurrentPlayer.CardsOnHand.Remove(cardCollection);

    //        PickUpCards(minimumCardOnHands: 3);

    //        if (cardNr == 10)
    //        {
    //            CardsOnTable.Clear();
    //        }

    //        if (cardNr != _cardTwo && cardNr != _cardTen)
    //        {
    //            ChangeCurrentPlayer();
    //        }

    //        return true;
    //    }

    //    public bool PickUpCardsFromTable()
    //    {
    //        var cardsOnTable = CardsOnTable.Select(c => c);
    //        CurrentPlayer.AddCardsToHand(cardsOnTable);

    //        CardsOnTable.Clear();

    //        ChangeCurrentPlayer();

    //        return true;
    //    }

    //    public GamePlayResult PlayChanceCard()
    //    {
    //        if (DeckOfCards.Count < 1)
    //            return GamePlayResult.InvalidPlay;

    //        var chanceCard = DeckOfCards.Peek();
    //        CurrentPlayer.AddCardsToHand(new List<Card> { chanceCard });

    //        if (!PlayCard(chanceCard.Number))
    //        {
    //            PickUpCardsFromTable();
    //            return GamePlayResult.ChanceFailed;
    //        }

    //        return GamePlayResult.ChanceSucceded;
    //    }

    //    private void PickUpCards(int minimumCardOnHands)
    //    {
    //        var numberOfCardsToPickUp = CurrentPlayer.CardsOnHand.Count < minimumCardOnHands
    //            ? minimumCardOnHands - CurrentPlayer.CardsOnHand.Count
    //            : 0;
    //        var cardsToPickup = new List<Card>();
    //        for (var i = 0; i < numberOfCardsToPickUp; i++)
    //        {
    //            if (DeckOfCards.Count == 0)
    //                break;

    //            cardsToPickup.Add(DeckOfCards.Pop());
    //        }

    //        CurrentPlayer.AddCardsToHand(cardsToPickup);
    //    }

    //    private bool CanPlayCard(int cardNr)
    //    {
    //        if (CardsOnTable.Count == 0 ||
    //            cardNr == _cardTwo ||
    //            cardNr == _cardTen)
    //        {
    //            return true;
    //        }

    //        var cardOnTable = CardsOnTable.Peek();
    //        return cardNr > cardOnTable.Number ||
    //            cardOnTable.Number == 2;
    //    }

    //    private void ChangeCurrentPlayer()
    //    {
    //        var currentTurnIndex = Players.FindIndex(p => p.UserIdentifier == CurrentPlayer.UserIdentifier);
    //        if (currentTurnIndex < 0)
    //        {
    //            throw new InvalidOperationException($"Failed to update player turn. Cant find player with identifier: {CurrentPlayer.UserIdentifier}");
    //        }

    //        CurrentPlayer = Players[++currentTurnIndex % Players.Count];
    //    }
    //}
}
