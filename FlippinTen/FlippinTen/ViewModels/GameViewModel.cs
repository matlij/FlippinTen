using FlippinTen.Core;
using FlippinTen.Core.Entities;
using FlippinTen.Core.Models;
using FlippinTen.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FlippinTen.ViewModels
{
    public class CardView
    {
        public int ID { get; set; }
        public int Number { get; set; }
        public string ImageUrl { get; set; }
        public bool Selected { get; set; }
        public string BackroundColor
        {
            get
            {
                return Selected
                    ? ColorPallet.Secondary
                    : ColorPallet.Transparent;
            }
        }
    }

    public class GameViewModel : BaseViewModel
    {
        private readonly OnlineGameService _onlineGameService;

        private bool _connected;
        public bool Connected
        {
            get { return _onlineGameService.Game.Player.IsConnected; }
            set { SetProperty(ref _connected, value); }
        }

        private bool _waitingForPlayers;
        public bool WaitingForPlayers
        {
            get { return _waitingForPlayers; }
            set { SetProperty(ref _waitingForPlayers, value); }
        }

        private Card _topCardOnTable;
        public Card TopCardOnTable
        {
            get { return _topCardOnTable; }
            set { SetProperty(ref _topCardOnTable, value); }
        }

        private string _gameStatus;
        public string GameStatus
        {
            get { return _gameStatus; }
            set { SetProperty(ref _gameStatus, value); }
        }

        private string _playerTurnStatus;
        public string PlayerTurnStatus
        {
            get { return _playerTurnStatus; }
            set { SetProperty(ref _playerTurnStatus, value); }
        }

        private int _cardOnTableCount;
        public int CardOnTableCount
        {
            get { return _cardOnTableCount; }
            set { SetProperty(ref _cardOnTableCount, value); }
        }

        private int _cardDeckCount;
        public int CardDeckCount
        {
            get { return _cardDeckCount; }
            set { SetProperty(ref _cardDeckCount, value); }
        }

        private string _cardBack;
        public string CardBack
        {
            get { return _cardBack; }
            set { SetProperty(ref _cardBack, value); }
        }

        public ObservableCollection<CardView> CardsOnHand { get; set; } = new ObservableCollection<CardView>();

        public Command CardOnHandTappedCommand { get; }
        public Command CardOnTableTappedCommand { get; }

        public GameViewModel(OnlineGameService onlineGameService)
        {
            WaitingForPlayers = true;

            var gameName = onlineGameService.Game.Name;
            Title = $"Vändtia - {gameName}";
            _onlineGameService = onlineGameService;
            _onlineGameService.OnPlayerJoined += OnPlayerJoined;
            _onlineGameService.OnTurnedPlayed += OnTurnedPlayed;

            CardOnHandTappedCommand = new Command((data) => OnCardOnHandTapped(data));
            CardOnTableTappedCommand = new Command(() => OnCardOnTableTapped());
        }

        private async void OnCardOnHandTapped(object data)
        {
            if (!(data is CardView card))
                return;

            await Play(g => g.SelectCard(card.ID));
        }

        private async void OnCardOnTableTapped()
        {
            await Play(g => g.PlaySelectedCards());
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

            Connected = await _onlineGameService.ConnectToGame();

            IsBusy = false;

            if (Connected)
            {
                GameStatus = "Uppkopplad till spel!";
            }

            OnPlayerConnected();

            return Connected;
        }

        private async Task Play(Func<CardGame, GamePlayResult> play)
        {
            IsBusy = true;

            var result = await _onlineGameService.Play(play);

            IsBusy = false;

            GameStatus = result.ToString();

            if (result != GamePlayResult.Invalid && result != GamePlayResult.Unknown)
            {
                UpdateGameBoard();
            }
        }

        private void UpdateGameBoard()
        {
            TopCardOnTable = _onlineGameService.Game.CardsOnTable.Count > 0
                ? _onlineGameService.Game.CardsOnTable.Peek()
                : null;

            CardsOnHand.Clear();
            var player = _onlineGameService.Game.Player;
            foreach (var card in player.CardsOnHand)
            {
                var cardView = card.AsCardView();
                CardsOnHand.Add(cardView);
            }

            CardOnTableCount = _onlineGameService.Game.CardsOnTable.Count;
            CardDeckCount = _onlineGameService.Game.DeckOfCards.Count;
            CardBack = _onlineGameService.Game.DeckOfCards.Count > 1
                ? ImageConstants.CardBackMultiple
                : ImageConstants.CardBack;

            PlayerTurnStatus = _onlineGameService.Game.IsPlayersTurn()
                ? "Din tur!"
                : "Väntar på motståndare...";
        }

        private void OnPlayerJoined(object sender, PlayerJoinedEventArgs e)
        {
            OnPlayerConnected();
        }

        private void OnPlayerConnected()
        {
            var allPlayersOnline = _onlineGameService.Game.AllPlayersOnline;
            WaitingForPlayers = !allPlayersOnline;

            if (allPlayersOnline)
            {
                UpdateGameBoard();
            }
        }

        private void OnTurnedPlayed(object sender, CardPlayedEventArgs e)
        {
            UpdateGameBoard();
        }
    }
}
