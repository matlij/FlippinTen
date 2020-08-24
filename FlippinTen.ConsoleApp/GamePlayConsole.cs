using FlippinTen.Core;
using FlippinTen.Core.Entities;
using FlippinTen.Core.Models.Information;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FlippinTen.ConsoleApp
{
    public class GamePlayConsole
    {
        private readonly ICardGame _cardGame;
        private readonly ManualResetEvent _waitForOtherPlayerEvent = new ManualResetEvent(false);
        private string _lastMoveStatus;

        public GamePlayConsole(ICardGame cardGame)
        {
            _cardGame = cardGame;
            _cardGame.OnPlayerJoined += GameStarted;
            _cardGame.OnTurnedPlayed += TurnPlayed;
        }

        private void TurnPlayed(object sender, CardGameEventArgs e)
        {
            _lastMoveStatus = e.GameResult.GetResultInfo();
            _waitForOtherPlayerEvent.Set();
        }

        private void GameStarted(object sender, PlayerJoinedEventArgs e)
        {
            _lastMoveStatus = $"Spelare {e.UserIdentifier} anslut.";

            _waitForOtherPlayerEvent.Set();
        }

        public async Task StartGame()
        {
            if (!await _cardGame.ConnectToGame())
                return;

            PlayGame().Wait();
        }

        public async Task StartGameAgainstComputer(ComputerPlayer computerGame)
        {
            if (computerGame is null)
            {
                throw new ArgumentNullException(nameof(computerGame));
            }

            await computerGame.Start();
            await StartGame();
            computerGame.Dispose();
        }

        public void EndGame()
        {
            _cardGame.Disconnect();
        }

        private async Task PlayGame()
        {
            while (true)
            {
                var game = await WaitForPlayersTurn();
                if (game.GameOver)
                {
                    Console.WriteLine($"Game Over! Winner is {game.Winner}");
                    break;
                }

                var input = GetPlayerInput();
                if (input.ToUpper() == "Q")
                    break;

                var gameResult = await HandleUserInput(input, _cardGame, game);
                _lastMoveStatus = gameResult?.GetResultInfo(game.Player.UserIdentifier);
            }
        }

        private async Task<GameFlippinTen> WaitForPlayersTurn()
        {
            var game = await _cardGame.GetGame();
            PrintBoard(game, _lastMoveStatus);

            while (!game.GameOver && !game.IsPlayersTurn())
            {
                _waitForOtherPlayerEvent.WaitOne();
                _waitForOtherPlayerEvent.Reset();
                game = await _cardGame.GetGame();
                PrintBoard(game, _lastMoveStatus);
            }

            return game;
        }

        private async Task<GameResult> HandleUserInput(string input, ICardGame cardGame, Core.Entities.GameFlippinTen game)
        {
            try
            {
                switch (input.ToUpper())
                {
                    case "P":
                        return await cardGame.Play(g => g.PickUpCards());
                    case "C":
                        return await cardGame.Play(g => g.PlayChanceCard());
                    default:
                        if (!int.TryParse(input, out var cardIndex))
                            return new GameResult("Input must be of type integer");
                        return await PlayCard(game, cardIndex - 1);
                }
            }
            catch (ArgumentException e)
            {
                return new GameResult(e.Message);
            }
        }

        private async Task<GameResult> PlayCard(GameFlippinTen game, int cardIndex)
        {
            var cardToPlay = game.Player.CardsOnHand[cardIndex];
            var result = await _cardGame.Play(c => c.PlayCards(new List<Card> { cardToPlay }));
            return result;
        }

        private static string GetPlayerInput()
        {
            Console.WriteLine("Vilket kort vill du lägga?");

            do
            {
                var input = Console.ReadLine();

                if (int.TryParse(input, out _) ||
                    input.ToUpper() == "P" ||
                    input.ToUpper() == "C")
                {
                    return input;
                }
            } while (true);
        }

        private void PrintBoard(GameFlippinTen game, string lastMoveStatus)
        {
            Console.Clear();
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine($"--------- Vändtia - {game.Player.UserIdentifier} - {game.Name} ---------");

            Console.WriteLine($"Kort kvar i kortlek: {game.DeckOfCards.Count}");
            Console.WriteLine();

            var topCardOnTable = game.CardsOnTable.TryPeek(out var card) ?
                card.ToString() :
                "'table empty'";
            Console.WriteLine($"Kort på bord: { topCardOnTable} (totalt {game.CardsOnTable.Count})");
            Console.WriteLine();

            Console.WriteLine("Kort på hand:");
            var cardsOnHand = game.Player.CardsOnHand;
            for (var i = 0; i < cardsOnHand.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {cardsOnHand[i].ToString()}");
            }

            Console.WriteLine();
            Console.WriteLine("P. Plocka upp kort från bordet.");
            Console.WriteLine("C. Chansa.");

            if (!string.IsNullOrEmpty(lastMoveStatus))
            {
                Console.WriteLine();
                Console.WriteLine(lastMoveStatus);
                Console.WriteLine();
            }
        }
    }
}
