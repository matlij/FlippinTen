using FlippinTen.Core.Entities.Enums;
using FlippinTen.Core.Models.Information;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FlippinTen.Core.Entities
{
    public class GameFlippinTen
    {
        private const int _cardTwoNumber = 2;
        private const int _cardTenNumber = 10;

        public string Identifier { get; }
        public string Name { get; }
        public Stack<Card> DeckOfCards { get; private set; }
        public Stack<Card> CardsOnTable { get; private set; }
        public Player Player { get; }
        public List<PlayerInformation> PlayerInformation { get; }
        public bool AllPlayersOnline { get => PlayerInformation.TrueForAll(p => p.IsConnected); }
        public bool GameOver { get; set; }
        public string Winner { get; set; }

        public GameFlippinTen(
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

        public GameResult PlayCards(List<Card> cards)
        {
            if (cards is null)
            {
                throw new ArgumentNullException(nameof(cards));
            }

            if (cards.Count == 0)
            {
                return new GameResult("No cards selected to play.");
            }

            return Play(() =>
            {
                if (cards.Any(c => c.Number != cards.First().Number))
                    return new GameResult($"Must play cards of same type. Input: {string.Join(", ", cards)}");

                var result = Play(cards);
                return new GameResult(Identifier, Player.UserIdentifier, result, cards);
            });
        }

        public GameResult PlayChanceCard()
        {
            var gameResult = Play(() =>
            {
                if (DeckOfCards.Count == 0)
                    return new GameResult("Kortlek tom!");

                var chanceCard = DeckOfCards.Pop();
                var chanceCardList = new List<Card> { chanceCard };
                Player.AddCardsToHand(chanceCardList);

                var result = Play(chanceCardList);
                if (result == CardPlayResult.Invalid)
                { 
                    PickUpCards();
                    result = CardPlayResult.ChanceFailed;
                }

                Debug.WriteLine($"{DateTime.Now} - CardGame - Chance card played. Chance card '{chanceCard}', result '{result}', ");
                return new GameResult(Identifier, Player.UserIdentifier, result, chanceCardList);
            });


            return gameResult;
        }

        public GameResult PickUpCards()
        {
            return Play(() =>
            {
                if (CardsOnTable.Count == 0)
                    return new GameResult("Kortlek tom!");

                var cardsToPickUp = CardsOnTable.ToList();
                Player.AddCardsToHand(cardsToPickUp);
                CardsOnTable.Clear();
                ChangeCurrentPlayer();

                return new GameResult(Identifier, Player.UserIdentifier, CardPlayResult.CardsOnTablePickedUp, cardsToPickUp);
            });
        }

        public bool CanPlayCard(Card card)
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

        private GameResult Play(Func<GameResult> play)
        {
            if (GameOver)
                return new GameResult("Spelet är avslutat.");
            if (!IsPlayersTurn())
                return new GameResult($"Inte din tur. Väntar på motståndare '{PlayerInformation.Single(p => p.IsPlayersTurn).Identifier}'");
            if (ShouldFlipCardsOnTable())
                CardsOnTable.Clear();

            try
            {
                var result = play();
                Debug.WriteLine($"CardGame - Card played by user '{result.UserIdentifier}'. Result: {result.Result}");
                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Play failed: " + e);
                throw;
            }
        }

        private CardPlayResult Play(List<Card> cards)
        {
            if (!CanPlayCard(cards.First()))
                return CardPlayResult.Invalid;

            AddCardsToTable(cards);
            PickUpCards(minimumCardOnHands: 3);
            CheckIfPlayerIsWinner();

            var result = GetGameResult(cards.First());
            if (result == CardPlayResult.Succeded)
                ChangeCurrentPlayer();

            return result;
        }

        private CardPlayResult GetGameResult(Card cardFirst)
        {
            if (cardFirst.Number == _cardTwoNumber)
            {
                return CardPlayResult.CardTwoPlayed;
            }
            else if (ShouldFlipCardsOnTable())
            {
                return CardPlayResult.CardsFlipped;
            }
            else
            {
                return CardPlayResult.Succeded;
            }
        }

        private bool CheckIfPlayerIsWinner()
        {
            if (Player.CardsOnHand.Count == 0)
            {
                GameOver = true;
                Winner = Player.UserIdentifier;
                return true;
            }

            return false;
        }

        private void AddCardsToTable(List<Card> cards)
        {
            foreach (var card in cards)
            {
                if (!Player.CardsOnHand.Remove(card))
                    throw new ArgumentException($"Player '{Player.UserIdentifier}' cannot play '{card}' since it doesn't exist in players CardOnHand list.");

                CardsOnTable.Push(card);
            }
        }

        private bool ShouldFlipCardsOnTable()
        {
            if (CardsOnTable.Count < 1)
                return false;

            var topCardOnTale = CardsOnTable.Peek();
            var count = 0;
            foreach (var card in CardsOnTable)
            {
                if (card.Number != topCardOnTale.Number || count == 4)
                    break;

                count++;
            }

            return topCardOnTale.Number == _cardTenNumber || count == 4;
        }

        private void ChangeCurrentPlayer()
        {
            var currentPlayer = PlayerInformation.FirstOrDefault(p => p.IsPlayersTurn);
            if (currentPlayer == null)
            {
                throw new InvalidOperationException($"Failed to update player turn. 'IsPlayersTurn' is false for all players.");
            }
            currentPlayer.IsPlayersTurn = false;

            var currentPlayerIndex = PlayerInformation.IndexOf(currentPlayer);
            var newPlayersTurn = PlayerInformation[++currentPlayerIndex % PlayerInformation.Count];
            newPlayersTurn.IsPlayersTurn = true;

            Debug.WriteLine($"{DateTime.Now} - CardGame - Player turn updated from '{currentPlayer.Identifier}' (IsPlayersTurn: {currentPlayer.IsPlayersTurn}) to '{newPlayersTurn.Identifier}' (IsPlayersTurn: {newPlayersTurn.IsPlayersTurn})");
        }

        private void PickUpCards(int minimumCardOnHands)
        {
            if (Player.CardsOnHand.Count >= minimumCardOnHands)
                return;

            var numberOfCardsToPickUp = minimumCardOnHands - Player.CardsOnHand.Count;

            var cardsToPickup = new List<Card>();
            for (var i = 0; i < numberOfCardsToPickUp; i++)
            {
                if (DeckOfCards.Count == 0)
                    break;

                var card = DeckOfCards.Pop();
                cardsToPickup.Add(card);
            }

            Player.AddCardsToHand(cardsToPickup);
        }
    }
}
