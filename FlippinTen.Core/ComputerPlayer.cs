using FlippinTen.Core.Entities;
using FlippinTen.Core.Models.Information;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FlippinTen.Core
{
    public class ComputerPlayer : IDisposable
    {
        private readonly Thread _playGameThread;
        private readonly ManualResetEvent _waitForOtherPlayerEvent = new ManualResetEvent(false);
        private readonly ICardGame _cardGame;
        private bool _runOpponent;

        public ComputerPlayer(ICardGame cardGame)
        {
            _cardGame = cardGame;
            _playGameThread = new Thread(PlayGame);
            _cardGame.OnTurnedPlayed += OnTurnedPlayed;
            _cardGame.OnPlayerJoined += OnPlayerJoined; ;
        }

        private void OnPlayerJoined(object sender, PlayerJoinedEventArgs e)
        {
            _waitForOtherPlayerEvent.Set();
        }

        private void OnTurnedPlayed(object sender, CardGameEventArgs e)
        {
            _waitForOtherPlayerEvent.Set();
        }

        public async Task Start()
        {
            if (!await _cardGame.ConnectToGame())
                return;

            _runOpponent = true;
            _playGameThread.Start();
        }

        public void Dispose()
        {
            _runOpponent = false;
            _waitForOtherPlayerEvent.Set();
        }

        private async void PlayGame()
        {
            while (_runOpponent)
            {
                var game = await WaitForPlayersTurn();
                if (game.GameOver)
                    break;
                Thread.Sleep(1000); //Simulate computer thinking

                var cardsToPlay = FindCardToPlay(game);
                var gameResult = await PlayCards(game, cardsToPlay);

                if (gameResult.Invalid())
                    throw new Exception($"Computer played invalid move. Cards '{string.Join(", ", cardsToPlay)}'. TopCardOnTable '{game.CardsOnTable.Peek()}'. Result '{gameResult.Result}'");
            }
        }

        private async Task<GameResult> PlayCards(GameFlippinTen game, List<Card> cardsToPlay)
        {
            GameResult result;
            if (cardsToPlay == null)
            {
                result = game.DeckOfCards.Any()
                    ? await _cardGame.Play(g => g.PlayChanceCard())
                    : await _cardGame.Play(g => g.PickUpCards());
            }
            else
            {
                result = await _cardGame.Play(c => c.PlayCards(cardsToPlay)); ;
            }

            return result;
        }

        private async Task<GameFlippinTen> WaitForPlayersTurn()
        {
            var game = await _cardGame.GetGame();
            while (_runOpponent && !game.GameOver && !game.IsPlayersTurn())
            {
                _waitForOtherPlayerEvent.WaitOne();
                _waitForOtherPlayerEvent.Reset();
                game = await _cardGame.GetGame();
            }

            return game;
        }

        private static List<Card> FindCardToPlay(GameFlippinTen game)
        {
            const int cardTwo = 2;
            const int cardTen = 10;

            var numberOnTable = game.CardsOnTable.Count > 0
                ? game.CardsOnTable.Peek().Number
                : 0;
            var cardsOnHand = game.Player.CardsOnHand;

            var nonSpecialCards = cardsOnHand
                .Where(c => c.Number != 2 && c.Number != 10)
                .OrderBy(c => c.Number);
            foreach (var card in nonSpecialCards)
            {
                if (card.Number >= numberOnTable || numberOnTable == cardTwo)
                {
                    return nonSpecialCards.Where(c => c.Number == card.Number).ToList();
                }
            }

            if (cardsOnHand.Any(c => c.Number == cardTwo))
            {
                return new List<Card> { cardsOnHand.First(c => c.Number == cardTwo) };
            }
            else if (cardsOnHand.Any(c => c.Number == cardTen))
            {
                return new List<Card> { cardsOnHand.First(c => c.Number == cardTen) };
            }

            return null;
        }
    }
}
