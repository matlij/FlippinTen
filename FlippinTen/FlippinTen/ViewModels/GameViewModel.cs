using FlippinTen.Core;
using FlippinTen.Core.Entities;
using FlippinTen.Core.Entities.Enums;
using FlippinTen.Core.Models.Information;
using FlippinTen.Models;
using Newtonsoft.Json;
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
        private bool _connected;
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

        public bool Connected
        {
            get { return OnlineGameService.Game.Player.IsConnected; }
            set { SetProperty(ref _connected, value); }
        }
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
        public OnlineGameService OnlineGameService { get; }

        public GameViewModel(OnlineGameService onlineGameService)
        {
            WaitingForPlayers = IsWaitingForPlayers(onlineGameService.Game);

            var gameName = onlineGameService.Game.Name;
            Title = $"Vändtia - {gameName}";
            OnlineGameService = onlineGameService;
            OnlineGameService.OnPlayerJoined += OnPlayerJoined;
            OnlineGameService.OnTurnedPlayed += OnTurnedPlayed;

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

            Connected = await OnlineGameService.ConnectToGame();

            IsBusy = false;

            if (Connected)
            {
                GameStatus = "Uppkopplad till spel!";
            }

            OnPlayerConnected();

            return Connected;
        }

        public async Task Disconnect()
        {
            await OnlineGameService.Disconnect();

            Connected = false;
        }

        public void UpdateGameBoard(GameResult result)
        {
            var game = OnlineGameService.Game;
            var user = game.Player.UserIdentifier;
            var resultInfo = result.GetResultInfo(user);
            GameStatus = result.GetResultInfo(user);

            Debug.WriteLine("Updating game board. ResultInfo: " + resultInfo);

            if (result.Invalid())
                return;

            UpdateGameBoard(game);
        }

        private void UpdateGameBoard(CardGame game)
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

        private void UpdatePlayerCards(CardGame game)
        {
            SelectedCards.Clear();
            CardsOnHand.Clear();
            foreach (var card in game.Player.CardsOnHand)
            {
                CardsOnHand.Add(card);
            }
        }

        private async Task Play(Func<CardGame, GameResult> play)
        {
            IsBusy = true;

            var result = await OnlineGameService.Play(play);

            IsBusy = false;

            UpdateGameBoard(result);
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

        private void OnTurnedPlayed(object sender, CardGameEventArgs e)
        {
            UpdateGameBoard(e.GameResult);
        }

        private void OnPlayerJoined(object sender, PlayerJoinedEventArgs e)
        {
            OnPlayerConnected();
        }

        private void OnPlayerConnected()
        {
            var game = OnlineGameService.Game;
            WaitingForPlayers = IsWaitingForPlayers(game);
            if (!WaitingForPlayers)
            {
                UpdateGameBoard(game);
            }
        }

        private static bool IsWaitingForPlayers(CardGame game)
        {
            var isWaitingForPlayers = game.GameOver || game.AllPlayersOnline
                ? false
                : true;
            return isWaitingForPlayers;
        }

        private static string GetCardBack(CardGame game)
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
