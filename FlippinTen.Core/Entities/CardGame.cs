using FlippinTen.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public bool GameOver { get; set; }
        public string Winner { get; set; }

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

        public void UpdateGame(IEnumerable<Card> deckOfCards, IEnumerable<Card> cardsOnTable, List<PlayerInformation> playerInformation, bool gameOver, string winner)
        {
            DeckOfCards = new Stack<Card>(new Stack<Card>(deckOfCards));
            CardsOnTable = new Stack<Card>(new Stack<Card>(cardsOnTable));

            PlayerInformation.Clear();
            PlayerInformation.AddRange(playerInformation);

            GameOver = gameOver;
            Winner = winner;
        }

        public bool IsPlayersTurn()
        {
            var playerInfo = PlayerInformation.First(p => p.Identifier == Player.UserIdentifier);

            return playerInfo.IsPlayersTurn;
        }

        public GamePlayResult SelectCard(int cardId)
        {
            return Play(() =>
            {
                var card = Player.CardsOnHand.FirstOrDefault(c => c.ID == cardId);
                if (card == null)
                    throw new ArgumentException($"{cardId} not found in player {Player.UserIdentifier}");

                var selectedCards = GetSelectedCards();
                var hasSelectedCardWithDifferentNumber = selectedCards.Count > 0 && selectedCards.First().Number != card.Number;
                if (hasSelectedCardWithDifferentNumber)
                    return GamePlayResult.Invalid;

                card.Selected = !card.Selected;
                return GamePlayResult.CardSelected;
            });
        }

        public GamePlayResult PlayChanceCard()
        {
            return Play(() =>
            {
                if (DeckOfCards.Count == 0)
                    return GamePlayResult.Invalid;

                var chanceCard = DeckOfCards.Peek();
                var chanceCardList = new List<Card> { chanceCard };
                Player.AddCardsToHand(chanceCardList);

                if (!PlayCards(chanceCardList))
                {
                    PickUpCards();
                    ChangeCurrentPlayer();
                    return GamePlayResult.Failed;
                }

                return GamePlayResult.Succeded;
            });
        }

        public GamePlayResult PickUpCards()
        {
            return Play(() =>
            {
                if (CardsOnTable.Count == 0)
                    return GamePlayResult.Invalid;

                Player.AddCardsToHand(CardsOnTable.ToList());
                CardsOnTable.Clear();
                ChangeCurrentPlayer();

                return GamePlayResult.Succeded;
            });
        }

        public GamePlayResult PlaySelectedCards()
        {
            return Play(() =>
            {
                var cards = GetSelectedCards();
                if (cards.Count == 0)
                    return GamePlayResult.Invalid;

                var result = PlayCards(cards);
                return result
                    ? GamePlayResult.Succeded
                    : GamePlayResult.Invalid;
            });
        }

        private GamePlayResult Play(Func<GamePlayResult> play)
        {
            if (GameOver || !IsPlayersTurn())
                return GamePlayResult.Invalid;

            try
            {
                return play();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Play failed: " + e);
                throw;
            }
        }

        private bool PlayCards(List<Card> cards)
        {
            var cardFirst = cards.First();
            if (!CanPlayCard(cards.First()))
                return false;

            foreach (var card in cards)
            {
                if (!Player.CardsOnHand.Remove(card))
                    throw new ArgumentException($"Player '{Player.UserIdentifier}' cannot play '{card}' since it doesn't exist in players CardOnHand list.");

                CardsOnTable.Push(card);
            }

            PickUpCards(minimumCardOnHands: 3);

            var shouldFlip = ShouldFlipCardsOnTable(cardFirst.Number);
            if (shouldFlip)
            {
                CardsOnTable.Clear();
            }
            if (cardFirst.Number != _cardTwoNumber && !shouldFlip)
            {
                ChangeCurrentPlayer();
            }

            CheckIfGameOver();

            return true;
        }

        private bool ShouldFlipCardsOnTable(int number)
        {
            var count = 0;
            foreach (var card in CardsOnTable)
            {
                if (card.Number != number || count == 4)
                    break;

                count++;
            }

            return number == _cardTenNumber || count == 4
                ? true
                : false;
                }

        private void CheckIfGameOver()
        {
            if (Player.CardsOnHand.Count == 0)
            {
                GameOver = true;
                Winner = Player.UserIdentifier;
            }
        }

        private List<Card> GetSelectedCards()
        {
            return Player.CardsOnHand
                .Where(s => s.Selected)
                .ToList();
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
            if (CardsOnTable.Count == 0 ||
                card.Number == _cardTwoNumber ||
                card.Number == _cardTenNumber)
            {
                return true;
            }

            var cardOnTable = CardsOnTable.Peek();
            return card.Number >= cardOnTable.Number;
        }
    }
}
