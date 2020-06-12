using FlippinTen.Core;
using FlippinTen.Core.Entities;
using FlippinTen.Core.Models.Information;
using FlippinTen.Extensions;
using FlippinTen.Models;
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
        private Card _topCardOnTable;
        private string _gameStatus;
        private string _playerTurnStatus;
        private int _cardOnTableCount;
        private int _cardDeckCount;
        private string _cardBack;
        private bool _gameOver;
        private bool _showGame;
        private bool _isPlayersTurn;
        private readonly ComputerPlayer _computerPlayer;

        public bool WaitingForPlayers
        {
            get { return _waitingForPlayers; }
            set { SetProperty(ref _waitingForPlayers, value); }
        }
        public Card TopCardOnTable
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
        public ObservableCollection<Card> CardsOnHand { get; set; } = new ObservableCollection<Card>();
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
            CardOnTableTappedCommand = new Command(() => CardOnTableTapped());
        }

        public async void PlayChanceCard()
        {
            await Play(g => g.PlayChanceCard());
        }

        public async void PickUpCards()
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
                GameStatus = "Uppkopplad till spel!";
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
                ? game.CardsOnTable.Peek()
                : null;
            CardOnTableCount = game.CardsOnTable.Count;
            CardDeckCount = game.DeckOfCards.Count;
            CardBack = GetCardBack(game);

            if (game.GameOver)
            {
                GameOver = game.GameOver;
                GameStatus = game.Winner == game.Player.UserIdentifier
                    ? "Grattis du vann! :D"
                    : $"Du förlorade :(";
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

            var updatedCardsOnHand = game.Player.CardsOnHand;

            CardsOnHand
                .Except(updatedCardsOnHand)
                .ToList()
                .ForEach(c => CardsOnHand.Remove(c));

            updatedCardsOnHand
                .Except(CardsOnHand)
                .ToList()
                .ForEach(c => CardsOnHand.Add(c));

            CardsOnHand.Sort();
        }

        private async Task Play(Func<GameFlippinTen, GameResult> play)
        {
            IsBusy = true;

            var result = await CardGame.Play(play);

            IsBusy = false;

            await UpdateGameBoard(result);
        }

        private void CardOnHandTapped()
        {
            //Do nothing now.
        }

        private async void CardOnTableTapped()
        {
            var selectedCards = SelectedCards.Select(c => c as Card).ToList();

            await Play(g => g.PlayCards(selectedCards));
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
