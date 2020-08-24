using System;
using System.Linq;
using System.Collections.Generic;
using FlippinTen.Core.Entities;
using FlippinTen.Core.Entities.Enums;

using dto = FlippinTen.Models.Entities;
using dtoInfo = FlippinTen.Models.Information;
using dtoEnum = FlippinTen.Models.Enums;
using FlippinTen.Core.Models.Information;
using System.ComponentModel;

namespace FlippinTen.Core.Translations
{
    public static class EntityTranslation
    {
        public static GameFlippinTen AsCardGame(this dto.CardGame game, string userIdentifier)
        {
            var deckOfCards = game.DeckOfCards.AsCardStack();
            var cardsOnTable = game.CardsOnTable.AsCardStack();
            var player = game.Players.FirstOrDefault(p => p.UserIdentifier == userIdentifier);
            var playerInformation = game.Players
                .Select(p => new PlayerInformation(p.UserIdentifier) { IsPlayersTurn = p.IsPlayersTurn, IsConnected = p.IsConnected })
                .ToList();
            return new GameFlippinTen(game.Identifier, game.Name, deckOfCards, cardsOnTable, player.AsPlayer(), playerInformation)
            {
                Winner = game.Winner,
                GameOver = game.GameOver
            };
        }

        public static Player AsPlayer(this dto.Player playerDto)
        {
            var player = new Player(playerDto.UserIdentifier)
            {
                IsConnected = playerDto.IsConnected
            };

            foreach (var item in playerDto.CardsOnHand)
                player.CardsOnHand.Add(item.AsCard());

            foreach (var item in playerDto.CardsHidden)
                player.CardsHidden.Add(item.AsCard());

            foreach (var item in playerDto.CardsVisible)
                player.CardsVisible.Add(item.AsCard());

            return player;
        }

        public static Stack<Card> AsCardStack(this Stack<dto.Card> cards)
        {
            var cardList = cards
                .Reverse()
                .Select(c => c.AsCard());
            return new Stack<Card>(cardList);
        }

        public static Card AsCard(this dto.Card card)
        {
            return new Card(card.Number, card.CardType.AsCardTypeDto());
        }

        public static CardType AsCardTypeDto(this dtoEnum.CardType cardType)
        {
            return CardType.FromValue((int)cardType);
        }

        public static dto.Player AsPlayerDto(this Player player, List<PlayerInformation> playerInformation)
        {
            var playerInfo = playerInformation.FirstOrDefault(p => p.Identifier == player.UserIdentifier);
            if (playerInfo == null)
            {
                throw new ArgumentException("Couldnt find player information");
            }

            return new dto.Player
            {
                CardsHidden = player.CardsHidden.Select(c => c.AsCardDto()).ToList(),
                CardsOnHand = player.CardsOnHand.Select(c => c.AsCardDto()).ToList(),
                CardsVisible = player.CardsVisible.Select(c => c.AsCardDto()).ToList(),
                IsConnected = player.IsConnected,
                IsPlayersTurn = playerInfo.IsPlayersTurn,
                UserIdentifier = player.UserIdentifier
            };
        }

        public static Stack<dto.Card> AsCardStackDto(this Stack<Card> cards)
        {
            var cardList = cards
                .Reverse()
                .Select(c => c.AsCardDto());
            return new Stack<dto.Card>(cardList);
        }

        public static dto.Card AsCardDto(this Card card)
        {
            return new dto.Card
            {
                CardType = card.CardType.AsCardTypeDto(),
                ID = card.ID,
                Number = card.Number
            };
        }

        public static dtoEnum.CardType AsCardTypeDto(this CardType cardType)
        {
                return (dtoEnum.CardType)Enum.Parse(typeof(dtoEnum.CardType), cardType.Name, true);
        }

        public static dtoInfo.GameResult AsGameResultDto(this GameResult gameResult)
        {
            var cards = gameResult.Cards.Select(c => c.AsCardDto());
            return new dtoInfo.GameResult
            {
                Cards = cards,
                Result = (int)gameResult.Result,
                GameIdentifier = gameResult.GameIdentifier,
                UserIdentifier = gameResult.UserIdentifier
            };
        }

        public static GameResult AsGameResult(this dtoInfo.GameResult gameResult)
        {
            if(!Enum.IsDefined(typeof(CardPlayResult), gameResult.Result))
                throw new InvalidEnumArgumentException(nameof(gameResult), gameResult.Result, typeof(CardPlayResult));

            var result = (CardPlayResult)gameResult.Result;
            var cards = gameResult.Cards.Select(c => c.AsCard());
            return new GameResult(gameResult.GameIdentifier, gameResult.UserIdentifier, result, cards);
        }
    }
}
