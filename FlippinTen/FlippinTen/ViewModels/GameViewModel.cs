using FlippinTen.Core;
using FlippinTen.Core.Entities;
using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FlippinTen.ViewModels
{
    public class GameViewModel : BaseViewModel
    {
        private readonly CardGame _cardGame;
        private readonly OnlineGameService _onlineGameService;
        private readonly string _userIdentifier;
        private const string _invalidMove = "Ogiltigt drag...";

        private bool _connected;
        public bool Connected
        {
            get { return _cardGame. GetPlayer(_userIdentifier).IsConnected; }
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

        public int CardOnTableCount
        {
            get { return _cardGame.CardsOnTable.Count; }
            //set { SetProperty(ref _cardOnTableCount, value); }
        }

        public ObservableCollection<CardCollection> CardsOnHand { get; set; } = new ObservableCollection<CardCollection>();

        public GameViewModel(CardGame cardGame, OnlineGameService onlineGameService, string userIdentifier)
        {
            WaitingForPlayers = true;

            Title = $"Spel: {cardGame.Name}";
            _cardGame = cardGame;
            _onlineGameService = onlineGameService;
            _userIdentifier = userIdentifier;
            _onlineGameService.OnPlayerJoined += OnPlayerJoined;
            _onlineGameService.OnTurnedPlayed += OnTurnedPlayed;
        }

        public async Task<bool> PlayCard(CardCollection card)
        {
            var result = await _onlineGameService.Play(_cardGame, (c) => c.PlayCard(card.CardNr));

            if (result)
            {
                UpdateCardCollections();

                GameStatus = $"Du la {string.Join(", ", card.Cards.Select(c => c.CardName))}";
            }
            else
            {
                GameStatus = "Ogiltigt drag :/";
            }

            UpdatePlayerTurnStatus();

            return result;
        }

        public async void PlayChanceCard()
        {
            IsBusy = true;

            var result = await _onlineGameService.Play(_cardGame, g => g.PlayChanceCard());

            IsBusy = false;

            switch (result)
            {
                case GamePlayResult.ChanceFailed: GameStatus = "Chansning misslyckades :("; break;
                case GamePlayResult.InvalidPlay: GameStatus = _invalidMove; break;
                case GamePlayResult.ChanceSucceded: GameStatus = "Chansning lyckades!"; break;
                default:
                    break;
            }

            UpdatePlayerTurnStatus();
            UpdateCardCollections();
        }

        public async void PickUpCards()
        {
            IsBusy = true;

            var result = await _onlineGameService.Play(_cardGame, c => c.PickUpCards());

            IsBusy = false;

            if (!result)
            {
                GameStatus = _invalidMove;
                return;
            }

            GameStatus = "Du plockade upp kort från bord";
            UpdatePlayerTurnStatus();
            UpdateCardCollections();
        }

        public async Task<bool> ConnectToGame()
        {
            IsBusy = true;

            Connected = await _onlineGameService.ConnectToGame(_cardGame.Identifier, _userIdentifier);

            IsBusy = false;

            if (Connected)
            {
                GameStatus = "Uppkopplad till spel!";
            }

            OnPlayerConnected();
            UpdatePlayerTurnStatus();

            return Connected;
        }

        private void UpdatePlayerTurnStatus()
        {
            PlayerTurnStatus = _cardGame.CurrentPlayer.UserIdentifier == _userIdentifier
                ? "Din tur att spela!"
                : "Väntar på moståndare.";
        }

        private void UpdateCardCollections()
        {
            TopCardOnTable = _cardGame.CardsOnTable.Count > 0
                ? _cardGame.CardsOnTable.Peek()
                : null;

            CardsOnHand.Clear();
            var player = _cardGame.GetPlayer(_userIdentifier);
            foreach (var cardOnHand in player.CardsOnHand)
                CardsOnHand.Add(cardOnHand);
        }

        private void OnPlayerJoined(object sender, PlayerJoinedEventArgs e)
        {
            OnPlayerConnected();
        }

        private void OnPlayerConnected()
        {
            var allPlayersOnline = _cardGame.AllPlayersOnline();
            WaitingForPlayers = !allPlayersOnline;

            if (allPlayersOnline)
            {
                UpdateCardCollections();
            }
        }

        private void OnTurnedPlayed(object sender, CardPlayedEventArgs e)
        {
            UpdatePlayerTurnStatus();
            UpdateCardCollections();
        }
    }
}
