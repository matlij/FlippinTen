using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Models;
using Models;
using Models.Events;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FlippinTen.ConsoleApp
{
    public class GamePlayConsole
    {
        private readonly IGamePlayService _gamePlayService;
        private readonly ManualResetEvent _waitForOtherPlayerEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent _waitForOpponentEvent = new ManualResetEvent(false);

        public GamePlayConsole(IGamePlayService gamePlayService)
        {
            _gamePlayService = gamePlayService;

            _gamePlayService.OnPlayerJoined += PlayerJoined;
            _gamePlayService.OnTurnedPlayed += TurnPlayed;
        }

        private void TurnPlayed(object sender, CardPlayedEventArgs e)
        {
            _waitForOtherPlayerEvent.Set();
        }

        private void PlayerJoined(object sender, PlayerJoinedEventArgs e)
        {
            if (_gamePlayService.IsAllPlayersOnline())
                _waitForOpponentEvent.Set();
        }

        public async Task StartGame()
        {
            var game = _gamePlayService.Game;
            if (!await ConnectToGame())
                return;

            Console.WriteLine("Spelare: ");
            foreach (var player in game.Players)
            {
                Console.WriteLine($"{player.Name} {player.IsConnected}");
            }

            var allPlayersConnected = game.Players.All(p => p.IsConnected);
            if (!allPlayersConnected)
            {
                allPlayersConnected = WaitForOpponents(game, allPlayersConnected);
            }

            if (allPlayersConnected)
            {
                PlayGame().Wait();
            }
        }

        private bool WaitForOpponents(GamePlay game, bool allPlayersConnected)
        {
            while (!allPlayersConnected)
            {
                Console.WriteLine("Väntar på spelare att ansluta: " +
                    string.Concat(game.Players.Where(p => !p.IsConnected).Select(p => $"{p.Name}\n")));

                if (_waitForOpponentEvent.WaitOne())
                {
                    allPlayersConnected = game.Players.Any(p => p.IsConnected);
                }
                else
                {
                    Console.WriteLine("Vill du forsätta vänta på spelare (j/n)?");
                    var input = Console.ReadLine();

                    if (input.ToUpper() != "J")
                    {
                        allPlayersConnected = false;
                    }
                }

            }

            return allPlayersConnected;
        }

        private async Task PlayGame()
        {
            const string invalidMoveMessage = "Ogiltigt drag. Prova igen.";
            const string okMoveMessage = "Bra drag!";

            var lastMoveStatus = string.Empty;

            do
            {
                PrintBoard(lastMoveStatus);
                lastMoveStatus = string.Empty;

                if (_gamePlayService.IsPlayersTurn())
                {
                    _waitForOtherPlayerEvent.Reset();

                    var input = GetPlayerInput();
                    var playSucceded = false;
                    try
                    {
                        if (input.ToUpper() == "P")
                        {
                            playSucceded = await _gamePlayService.PickUpCards();
                            if (!playSucceded)
                            {
                                lastMoveStatus = invalidMoveMessage;
                            }
                        }

                        else if (input.ToUpper() == "C")
                        {
                            var gamePlayResult = await _gamePlayService.PlayChanceCard();

                            lastMoveStatus = gamePlayResult == GamePlayResult.InvalidPlay ?
                                invalidMoveMessage : 
                                gamePlayResult == GamePlayResult.ChanceSucceded ? 
                                    "Chansningen lyckades! :D" : 
                                    "Chansningen misslyckades... :(";
                        }

                        else
                        {
                            var inputIndex = int.Parse(input) - 1;
                            var cardToPlay = _gamePlayService.Player.CardsOnHand[inputIndex];
                            playSucceded = await _gamePlayService.PlayCard(cardToPlay);
                            lastMoveStatus = playSucceded ?
                                okMoveMessage :
                                invalidMoveMessage;
                        }
                    }
                    catch (ArgumentException)
                    {
                        lastMoveStatus = invalidMoveMessage;
                    }

                    if (playSucceded)
                    {
                        lastMoveStatus = "Bra drag!";
                    }
                }
                else
                {
                    Console.WriteLine("Other players turn...");
                    _waitForOtherPlayerEvent.WaitOne();
                }
            } while (true);
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
            Console.WriteLine($"--------- Vändtia - {_gamePlayService.Player.Name} - {_gamePlayService.Game.Name} ---------");

            Console.WriteLine($"Kort kvar i kortlek: {_gamePlayService.Game.DeckOfCards.Count}");
            Console.WriteLine();

            var topCardOnTable = _gamePlayService.Game.CardsOnTable.TryPeek(out var card) ?
                card.CardName :
                "'table empty'";
            Console.WriteLine($"Kort på bord: { topCardOnTable} (totalt {_gamePlayService.Game.CardsOnTable.Count})");
            Console.WriteLine();

            Console.WriteLine("Kort på hand:");
            var cardsOnHand = _gamePlayService.Player.CardsOnHand;
            for (var i = 0; i < cardsOnHand.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {string.Join(", ", cardsOnHand[i].Cards.Select(c => c.CardName))}");
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
            return await _gamePlayService.ConnectToGame();
        }
    }
}
