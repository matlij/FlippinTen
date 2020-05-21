using FlippinTen.Core;
using FlippinTen.Core.Entities;
using FlippinTen.Core.Entities.Enums;
using FlippinTen.Core.Models.Information;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FlippinTen.ConsoleApp
{
    public class GamePlayConsole
    {
        private readonly OnlineGameService _onlineService;
        private readonly ManualResetEvent _waitForOtherPlayerEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent _waitForOpponentEvent = new ManualResetEvent(false);

        public GamePlayConsole(OnlineGameService onlineService)
        {
            _onlineService = onlineService;
            _onlineService.OnPlayerJoined += GameStarted;
            _onlineService.OnTurnedPlayed += TurnPlayed;
        }

        private void TurnPlayed(object sender, CardGameEventArgs e)
        {
            var lastMoveStatus = e.GameResult.GetResultInfo(_onlineService.Game.Player.UserIdentifier);
            PrintBoard(lastMoveStatus);

            if (_onlineService.Game.IsPlayersTurn())
            {
                _waitForOtherPlayerEvent.Set();
            }
        }

        private void GameStarted(object sender, PlayerJoinedEventArgs e)
        {
            _waitForOpponentEvent.Set();
        }

        public async Task StartGame()
        {
            if (!await ConnectToGame())
                return;

            Console.WriteLine("Motståndare: ");
            foreach (var player in _onlineService.Game.PlayerInformation)
            {
                Console.WriteLine($"{player}");
            }

            WaitForOpponents(_onlineService.Game);
            PlayGame().Wait();
        }

        public async Task EndGame()
        {
            await _onlineService.Disconnect();
        }

        private void WaitForOpponents(CardGame game)
        {
            while (!game.AllPlayersOnline)
            {
                Console.WriteLine("Väntar på spelare att ansluta");

                _waitForOpponentEvent.WaitOne();
            }
        }

        private async Task PlayGame()
        {
            var lastMoveStatus = string.Empty;

            do
            {
                if (_onlineService.Game.GameOver)
                {
                    Console.WriteLine($"Game Over! Winner is {_onlineService.Game.Winner}");
                    break;
                }

                PrintBoard(lastMoveStatus);

                if (_onlineService.Game.IsPlayersTurn())
                {
                    _waitForOtherPlayerEvent.Reset();

                    var input = GetPlayerInput();
                    GameResult gameResult = null;
                    try
                    {
                        if (input.ToUpper() == "P")
                        {
                            gameResult = await _onlineService.Play(g => g.PickUpCards());
                        }

                        else if (input.ToUpper() == "C")
                        {
                            gameResult = await _onlineService.Play(g => g.PlayChanceCard());
                        }
                        else
                        {
                            var cardIndex = int.Parse(input) - 1;
                            gameResult = await PlayCard(cardIndex);
                        }
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine(e);
                    }

                    lastMoveStatus = gameResult != null
                        ? gameResult.GetResultInfo(_onlineService.Game.Player.UserIdentifier)
                        : string.Empty;
                }
                else
                {
                    Console.WriteLine("Other players turn...");
                    _waitForOtherPlayerEvent.WaitOne();
                }
            } while (true);
        }

        private async Task<GameResult> PlayCard(int cardIndex)
        {
            var cardToPlay = _onlineService.Game.Player.CardsOnHand[cardIndex];
            var result = await _onlineService.Play(c => c.PlayCards(new List<Card> { cardToPlay }));
            if (result.Invalid())
            {
                foreach (var card in _onlineService.Game.Player.CardsOnHand)
                    card.Selected = false;
            }
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

        private void PrintBoard(string lastMoveStatus)
        {
            Console.Clear();
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine($"--------- Vändtia - {_onlineService.Game.Player.UserIdentifier} - {_onlineService.Game.Name} ---------");

            Console.WriteLine($"Kort kvar i kortlek: {_onlineService.Game.DeckOfCards.Count}");
            Console.WriteLine();

            var topCardOnTable = _onlineService.Game.CardsOnTable.TryPeek(out var card) ?
                card.ToString() :
                "'table empty'";
            Console.WriteLine($"Kort på bord: { topCardOnTable} (totalt {_onlineService.Game.CardsOnTable.Count})");
            Console.WriteLine();

            Console.WriteLine("Kort på hand:");
            var cardsOnHand = _onlineService.Game.Player.CardsOnHand;
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

        private async Task<bool> ConnectToGame()
        {
            return await _onlineService.ConnectToGame();
        }
    }
}
