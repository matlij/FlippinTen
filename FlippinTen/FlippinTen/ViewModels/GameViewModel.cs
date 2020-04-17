using FlippinTen.Core;
using FlippinTen.Core.Entities;
using FlippinTen.Core.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FlippinTen.ViewModels
{
    public class GameViewModel : BaseViewModel
    {
        private readonly OnlineGameService _onlineGameService;
        private const string _invalidMove = "Ogiltigt drag...";

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

        public ObservableCollection<CardCollection> CardsOnHand { get; set; } = new ObservableCollection<CardCollection>();

        public GameViewModel(OnlineGameService onlineGameService)
        {
            WaitingForPlayers = true;

            var gameName = onlineGameService.Game.Name;
            Title = $"Spel: {gameName}";
            _onlineGameService = onlineGameService;
            _onlineGameService.OnPlayerJoined += OnPlayerJoined;
            _onlineGameService.OnTurnedPlayed += OnTurnedPlayed;

            ItemTappedCommand = new Command((data) => OnCardOnHandTapped(data));
        }

        public Command ItemTappedCommand { get; }

        public async Task<bool> PlayCard(CardCollection card)
        {
            var result = await _onlineGameService.Play((c) => c.PlayCard(card.CardNr));

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

            var result = await _onlineGameService.Play(g => g.PlayChanceCard());

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

            var result = await _onlineGameService.Play(c => c.PickUpCards());

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

            Connected = await _onlineGameService.ConnectToGame();

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
        }

        private void UpdateCardCollections()
        {
            TopCardOnTable = _onlineGameService.Game.CardsOnTable.Count > 0
                ? _onlineGameService.Game.CardsOnTable.Peek()
                : null;

            CardsOnHand.Clear();
            var player = _onlineGameService.Game.Player;
            foreach (var cardOnHand in player.CardsOnHand)
                CardsOnHand.Add(cardOnHand);

            CardOnTableCount = _onlineGameService.Game.CardsOnTable.Count;
        }

        private void OnPlayerJoined(object sender, PlayerJoinedEventArgs e)
        {
            OnPlayerConnected();
        }

        private async void OnCardOnHandTapped(object card)
        {
            if (!(card is CardCollection cardCollection))
                return;

            await PlayCard(cardCollection);
        }

        private void OnPlayerConnected()
        {
            var allPlayersOnline = _onlineGameService.Game.AllPlayersOnline;
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
