using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Models;
using Models.Constants;
using Models.Events;
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
        private readonly IGamePlayService _gamePlayService;
        private const string _invalidMove = "Ogiltigt drag...";

        private bool _connected;
        public bool Connected
        {
            get { return _gamePlayService.Player.IsConnected; }
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
            get { return _gamePlayService.Game.CardsOnTable.Count; }
            //set { SetProperty(ref _cardOnTableCount, value); }
        }

        public ObservableCollection<CardCollection> CardsOnHand { get; set; } = new ObservableCollection<CardCollection>();

        //public Command PlayCardCommand { get; set; }

        public GameViewModel(IGamePlayService playService)
        {
            WaitingForPlayers = true;

            Title = $"Spel: {playService.Game.Name}";

            _gamePlayService = playService;

            _gamePlayService.OnPlayerJoined += OnPlayerJoined;
            _gamePlayService.OnTurnedPlayed += OnTurnedPlayed;
        }

        public async Task<bool> PlayCard(CardCollection card)
        {
            var result = await _gamePlayService.PlayCard(card);

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

            var result = await _gamePlayService.PlayChanceCard();

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

            var result = await _gamePlayService.PickUpCards();

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

            Connected = await _gamePlayService.ConnectToGame();

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
            PlayerTurnStatus = _gamePlayService.IsPlayersTurn()
                ? "Din tur att spela!"
                : "Väntar på moståndare.";
        }

        private void UpdateCardCollections()
        {
            TopCardOnTable = _gamePlayService.Game.CardsOnTable.Count > 0
                ? _gamePlayService.Game.CardsOnTable.Peek()
                : null;

            CardsOnHand.Clear();
            foreach (var cardOnHand in _gamePlayService.Player.CardsOnHand)
                CardsOnHand.Add(cardOnHand);
        }

        private void OnPlayerJoined(object sender, PlayerJoinedEventArgs e)
        {
            OnPlayerConnected();
        }

        private void OnPlayerConnected()
        {
            var allPlayersOnline = _gamePlayService.IsAllPlayersOnline();
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
