using FlippinTen.Core;
using FlippinTen.Core.Entities;
using FlippinTen.Core.Models.Information;
using FlippinTen.Extensions;
using FlippinTen.Models;
using FlippinTen.Translations;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FlippinTen.ViewModels
{
    public class GameViewModel : BaseViewModel
    {
        private bool _waitingForPlayers;
        private Models.Card _topCardOnTable;
        private string _gameStatus;
        private string _playerTurnStatus;
        private int _cardOnTableCount;
        private int _cardDeckCount;
        private string _cardBack;
        private bool _gameOver;
        private bool _showGame;
        private bool _isPlayersTurn;
        private GameResult _lastGameResult;

        private readonly ComputerPlayer _computerPlayer;

        public bool WaitingForPlayers
        {
            get { return _waitingForPlayers; }
            set { SetProperty(ref _waitingForPlayers, value); }
        }
        public Models.Card TopCardOnTable
        {
            get { return _topCardOnTable; }
            set { SetProperty(ref _topCardOnTable, value); }
        }
        public string GameStatus
        {
            get { return _gameStatus; }
            set { SetProperty(ref _gameStatus, value); }
        }
        public string PlayerTurnStatus
        {
            get { return _playerTurnStatus; }
            set { SetProperty(ref _playerTurnStatus, value); }
        }
        public int CardOnTableCount
        {
            get { return _cardOnTableCount; }
            set { SetProperty(ref _cardOnTableCount, value); }
        }
        public int CardDeckCount
        {
            get { return _cardDeckCount; }
            set { SetProperty(ref _cardDeckCount, value); }
        }
        public string CardBack
        {
            get { return _cardBack; }
            set { SetProperty(ref _cardBack, value); }
        }
        public bool GameOver
        {
            get { return _gameOver; }
            set { SetProperty(ref _gameOver, value); }
        }
        public bool ShowGame
        {
            get { return _showGame; }
            set { SetProperty(ref _showGame, value); }
        }
        public bool IsPlayersTurn
        {
            get { return _isPlayersTurn; }
            set { SetProperty(ref _isPlayersTurn, value); }
        }
        public GameResult LastGameResult
        {
            get { return _lastGameResult; }
            set { SetProperty(ref _lastGameResult, value); }
        }

        public ObservableCollection<Models.Card> CardsOnHand { get; set; } = new ObservableCollection<Models.Card>();
        public ObservableCollection<object> SelectedCards { get; set; } = new ObservableCollection<object>();

        public Command CardOnHandTappedCommand { get; }
        public Command CardOnTableTappedCommand { get; }
        public ICardGame CardGame { get; }

        public GameViewModel(ICardGame cardGame, GameFlippinTen game, ComputerPlayer computerPlayer)
            : this(cardGame, game)
        {
            _computerPlayer = computerPlayer;
        }

        public GameViewModel(ICardGame cardGame, GameFlippinTen game)
        {
            WaitingForPlayers = IsWaitingForPlayers(game);

            var gameName = game.Name;
            Title = $"Vändtia - {gameName}";
            CardGame = cardGame;
            CardGame.OnPlayerJoined += OnPlayerJoined;
            CardGame.OnTurnedPlayed += OnTurnedPlayed;

            CardOnHandTappedCommand = new Command(() => CardOnHandTapped());
            CardOnTableTappedCommand = new Command(async () => await CardOnTableTapped());
        }

        public async void PlayChanceCard()
        {
            await Play(g => g.PlayChanceCard());
        }

        public async Task PickUpCards()
        {
            await Play(g => g.PickUpCards());
        }

        public async Task<bool> ConnectToGame()
        {
            IsBusy = true;

            var connected = await CardGame.ConnectToGame();

            IsBusy = false;

            if (connected)
            {
                await OnPlayerConnected();
                if (_computerPlayer != null)
                    await _computerPlayer.Start();
            }

            return connected;
        }

        public void Disconnect()
        {
            CardGame.Disconnect();
            if (_computerPlayer != null)
                _computerPlayer.Dispose();
        }

        public async Task UpdateGameBoard(GameResult result)
        {
            var game = await CardGame.GetGame();
            var user = game.Player.UserIdentifier;
            var resultInfo = result.GetResultInfo(user);
            GameStatus = result.GetResultInfo(user);
            LastGameResult = result;

            Debug.WriteLine("Updating game board. ResultInfo: " + resultInfo);

            if (result.Invalid())
                return;

            UpdateGameBoard(game);
        }

        private void UpdateGameBoard(GameFlippinTen game)
        {
            UpdatePlayerCards(game);
            ShowGame = game.GameOver || WaitingForPlayers ? false : true;

            TopCardOnTable = game.CardsOnTable.Count > 0
                ? game.CardsOnTable.Peek().AsCard(false)
                : null;
            CardOnTableCount = game.CardsOnTable.Count;
            CardDeckCount = game.DeckOfCards.Count;
            CardBack = GetCardBack(game);

            if (game.GameOver)
            {
                GameOver = game.GameOver;
                GameStatus = game.Winner == game.Player.UserIdentifier
                    ? "Grattis du vann! :D"
                    : "Du förlorade :(";
            }

            var playersTurn = game.IsPlayersTurn();
            IsPlayersTurn = playersTurn;
            PlayerTurnStatus = playersTurn
                ? "Din tur!"
                : "Väntar på motståndare...";

            Debug.WriteLine($"Game properties updated. {nameof(game.IsPlayersTurn)}: {playersTurn}");
        }

        private void UpdatePlayerCards(GameFlippinTen game)
        {
            SelectedCards.Clear();

            if (game.Player.CardsOnHand.Count == 0)
            {
                var tableCards = game.Player.AsTableCards();
                CardsOnHand.Update(tableCards);
            }
            else
            {
                var cardsOnHand = game.Player.CardsOnHand
                    .Select(c => c.AsCard(false))
                    .ToList();
                CardsOnHand.Update(cardsOnHand);
            }

            CardsOnHand.Sort();
        }

        private async Task<GameResult> Play(Func<GameFlippinTen, GameResult> play)
        {
            IsBusy = true;

            var result = await CardGame.Play(play);

            IsBusy = false;

            await UpdateGameBoard(result);

            return result;
        }

        private void CardOnHandTapped()
        {
            //Do nothing now.
        }

        public async Task<GameResult> CardOnTableTapped()
        {
            var selectedCards = SelectedCards
                .Select(c => (c as Models.Card).AsCard())
                .ToList();

            return await Play(g => g.PlayCards(selectedCards));
        }

        private async void OnTurnedPlayed(object sender, CardGameEventArgs e)
        {
            await UpdateGameBoard(e.GameResult);
        }

        private async void OnPlayerJoined(object sender, PlayerJoinedEventArgs e)
        {
            await OnPlayerConnected();
        }

        private async Task OnPlayerConnected()
        {
            var game = await CardGame.GetGame();
            WaitingForPlayers = IsWaitingForPlayers(game);
            if (!WaitingForPlayers)
            {
                UpdateGameBoard(game);
            }
        }

        private static bool IsWaitingForPlayers(GameFlippinTen game)
        {
            return !game.GameOver && !game.AllPlayersOnline;
        }

        private static string GetCardBack(GameFlippinTen game)
        {
            var deckOfCardsCount = game.DeckOfCards.Count;
            if (deckOfCardsCount == 0)
            {
                return null;
            }
            else if (deckOfCardsCount == 1)
            {
                return ImageConstants.CardBack;
            }
            else
            {
                return ImageConstants.CardBackMultiple;
            }
        }
    }
}
