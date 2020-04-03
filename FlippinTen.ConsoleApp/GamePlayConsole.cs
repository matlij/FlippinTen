using FlippinTen.Core;
using FlippinTen.Core.Entities;
using FlippinTen.Core.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FlippinTen.ConsoleApp
{
    public class GamePlayConsole
    {
        private CardGame _game;
        private readonly OnlineGameService _onlineService;
        private readonly string _userIdentifier;
        private readonly ManualResetEvent _waitForOtherPlayerEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent _waitForOpponentEvent = new ManualResetEvent(false);

        public GamePlayConsole(CardGame cardGame, OnlineGameService onlineService, string userIdentifier)
        {
            _game = cardGame;
            _onlineService = onlineService;
            _userIdentifier = userIdentifier;
            _onlineService.OnPlayerJoined += PlayerJoined;
            _onlineService.OnTurnedPlayed += TurnPlayed;
        }

        private void TurnPlayed(object sender, CardPlayedEventArgs e)
        {
            Console.WriteLine("Turned played: " + JsonConvert.SerializeObject(e.Game));

            _game = e.Game;

            if (IsPlayersTurn())
            {
                _waitForOtherPlayerEvent.Set();
            }
        }

        private void PlayerJoined(object sender, PlayerJoinedEventArgs e)
        {
            SetPlayerConnectedStatus(e.UserIdentifier, isConnected: true);

            if (_game.Players.All(p => p.IsConnected))
                _waitForOpponentEvent.Set();
        }

        public async Task StartGame()
        {
            if (!await ConnectToGame())
                return;

            Console.WriteLine("Spelare: ");
            foreach (var player in _game.Players)
            {
                Console.WriteLine($"{player.UserIdentifier} {player.IsConnected}");
            }

            var allPlayersConnected = _game.Players.All(p => p.IsConnected);
            if (!allPlayersConnected)
            {
                allPlayersConnected = WaitForOpponents(_game, allPlayersConnected);
            }

            if (allPlayersConnected)
            {
                PlayGame().Wait();
            }
        }

        private bool WaitForOpponents(CardGame game, bool allPlayersConnected)
        {
            while (!allPlayersConnected)
            {
                Console.WriteLine("Väntar på spelare att ansluta: " +
                    string.Concat(game.Players.Where(p => !p.IsConnected).Select(p => $"{p.UserIdentifier}\n")));

                if (_waitForOpponentEvent.WaitOne())
                {
                    allPlayersConnected = game.AllPlayersOnline();
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

                if (IsPlayersTurn())
                {
                    _waitForOtherPlayerEvent.Reset();

                    var input = GetPlayerInput();
                    var playSucceded = false;
                    try
                    {
                        if (input.ToUpper() == "P")
                        {
                            playSucceded = await _onlineService.Play(_game, g => g.PickUpCards());
                            if (!playSucceded)
                            {
                                lastMoveStatus = invalidMoveMessage;
                            }
                        }

                        else if (input.ToUpper() == "C")
                        {
                            var gamePlayResult = await _onlineService.Play(_game, g => g.PlayChanceCard());

                            lastMoveStatus = gamePlayResult == GamePlayResult.InvalidPlay ?
                                invalidMoveMessage :
                                gamePlayResult == GamePlayResult.ChanceSucceded ?
                                    "Chansningen lyckades! :D" :
                                    "Chansningen misslyckades... :(";
                        }

                        else
                        {
                            var inputIndex = int.Parse(input) - 1;
                            var cardToPlay = _game.GetPlayer(_userIdentifier).CardsOnHand[inputIndex];
                            playSucceded = await _onlineService.Play(_game, g => g.PlayCard(cardToPlay.CardNr));
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
            Console.WriteLine($"--------- Vändtia - {_game.GetPlayer(_userIdentifier).UserIdentifier} - {_game.Name} ---------");

            Console.WriteLine($"Kort kvar i kortlek: {_game.DeckOfCards.Count}");
            Console.WriteLine();

            var topCardOnTable = _game.CardsOnTable.TryPeek(out var card) ?
                card.CardName :
                "'table empty'";
            Console.WriteLine($"Kort på bord: { topCardOnTable} (totalt {_game.CardsOnTable.Count})");
            Console.WriteLine();

            Console.WriteLine("Kort på hand:");
            var cardsOnHand = _game.GetPlayer(_userIdentifier).CardsOnHand;
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
            var result = await _onlineService.ConnectToGame(_game.Identifier, _userIdentifier);
            if (!result)
            {
                return false;
            }

            SetPlayerConnectedStatus(_userIdentifier, result);
            return result;
        }

        private void SetPlayerConnectedStatus(string userIdentifier, bool isConnected)
        {
            var player = _game.GetPlayer(userIdentifier);
            player.IsConnected = isConnected;
        }

        private bool IsPlayersTurn()
        {
            var isPlayersTurn = _game.CurrentPlayer.UserIdentifier == _userIdentifier;
            return isPlayersTurn;
        }
    }
}
