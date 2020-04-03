using FlippinTen.Utilities;
using FlippinTenWeb.DataAccess;
using Models;
using Models.Entities;
using Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlippinTenWeb.Services
{
    public class GameLogicLayer : IGameLogicLayer
    {
        private readonly IGameRepository _gameRepository;
        private readonly IGameCardUtilities _gameService;

        public GameLogicLayer(IGameRepository gameRepository, IGameCardUtilities gameService)
        {
            _gameRepository = gameRepository;
            _gameService = gameService;
        }

        public CardGame GetGame(string identifier)
        {
            return _gameRepository.Get(identifier);
        }

        public IEnumerable<CardGame> GetGames(string playerName)
        {
            return playerName is null ?
                _gameRepository.Get() :
                _gameRepository.GetFromPlayer(playerName);
        }

        public CardGame CreateGame(CardGame game)
        {
            if (game.DeckOfCards is null)
            {
                game.DeckOfCards = _gameService.GetDeckOfCards();
            }

            const int cardsToHandOut = 3;
            foreach (var player in game.Players)
            {
                var cardsOnHand = new List<Card>();
                for (var i = 0; i < cardsToHandOut; i++)
                {
                    player.CardsHidden.Add(game.DeckOfCards.Pop());
                    player.CardsVisible.Add(game.DeckOfCards.Pop());
                    cardsOnHand.Add(game.DeckOfCards.Pop());
                }
                player.AddCardsToHand(cardsOnHand);
            }

            game.CardsOnTable = new Stack<Card>();

            game.Identifier = Guid.NewGuid().ToString();

            if (!_gameRepository.Store(game))
                return null;

            return game;
        }

        public void UpdateGame(CardGame game)
        {
            _gameRepository.Update(game);
        }

        public bool JoinGame(string gameIdentifier, string playerName)
        {
            var game = _gameRepository.Get(gameIdentifier);

            if (game is null)
            {
                return false;
            }

            var joinedPlayer = game.Players.FirstOrDefault(p => p.Name == playerName);
            if (joinedPlayer != null)
            {
                joinedPlayer.IsConnected = true;
            }
            else
            {
                var player = new Player(Guid.NewGuid().ToString())
                {
                    Name = playerName,
                    IsConnected = true
                };

                game.Players.Add(player);
                _gameRepository.Update(game);
            }

            return true;
        }

        public void PlayerDisconnected(string playerName)
        {
            var games = GetGames(playerName);

            foreach (var game in games)
            {
                game.Players
                    .First(g => g.Name == playerName)
                    .IsConnected = false;
            }
        }
    }
}
