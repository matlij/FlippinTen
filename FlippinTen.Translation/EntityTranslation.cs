using System;
using System.Collections.Generic;
using System.Text;
using FlippinTen.Core.Entities;
using System.Linq;
using dto = Models.Entities;

namespace FlippinTen.Translation
{
    public static class EntityTranslation
    {
        public static CardGame AsCardGame(this dto.CardGame game)
        {
            var players = game.Players.Select(p => p.AsPlayer()).ToList();
            var deckOfCards = game.DeckOfCards.AsCardList();
            var cardsOnTable = game.CardsOnTable.AsCardList();

            return new CardGame(game.Identifier, game.Name, players, deckOfCards, cardsOnTable, game.CurrentPlayer.AsPlayer());
        }

        public static Player AsPlayer(this dto.Player playerDto)
        {
            var player = new Player(playerDto.UserIdentifier)
            {
                IsConnected = playerDto.IsConnected,
                IsPlayersTurn = playerDto.IsPlayersTurn,
            };

            foreach (var item in playerDto.CardsOnHand)
                player.CardsOnHand.Add(item.AsCardCollection());

            foreach (var item in playerDto.CardsHidden)
                player.CardsHidden.Add(item.AsCard());

            foreach (var item in playerDto.CardsVisible)
                player.CardsHidden.Add(item.AsCard());

            return player;
        }

        public static Stack<Card> AsCardList(this List<dto.Card> cards)
        {
            var cardsDto = new Stack<Card>();
            foreach (var card in cards)
            {
                cardsDto.Push(card.AsCard());
            }

            return cardsDto;
        }

        public static Card AsCard(this dto.Card card)
        {
            return new Card(card.Number, card.CardType);
        }

        public static CardCollection AsCardCollection(this dto.CardCollection card)
        {
            return new CardCollection
            {
                Cards = card.Cards.Select(c => c.AsCard()).ToList()
            };
        }

        public static dto.CardGame AsCardGameDto(this CardGame game)
        {
            return new dto.CardGame
            {
                CardsOnTable = game.CardsOnTable.AsCardListDto(),
                CurrentPlayer = game.CurrentPlayer.AsPlayerDto(),
                DeckOfCards = game.DeckOfCards.AsCardListDto(),
                Identifier = game.Identifier,
                Name = game.Name,
                Players = game.Players.Select(p => p.AsPlayerDto()).ToList()
            };
        }

        public static dto.Player AsPlayerDto(this Player player)
        {
            return new dto.Player
            {
                CardsHidden = player.CardsHidden.Select(c => c.AsCardDto()).ToList(),
                CardsOnHand = player.CardsOnHand.Select(c => c.AsCardCollectionDto()).ToList(),
                CardsVisible = player.CardsVisible.Select(c => c.AsCardDto()).ToList(),
                IsConnected = player.IsConnected,
                IsPlayersTurn = player.IsPlayersTurn,
                UserIdentifier = player.UserIdentifier
            };
        }

        public static List<dto.Card> AsCardListDto(this Stack<Card> cards)
        {
            return cards.ToList().Select(c => c.AsCardDto()).ToList();
        }

        public static dto.Card AsCardDto(this Card card)
        {
            return new dto.Card
            {
                CardType = card.CardType,
                ID = card.ID,
                ImageUrl = card.ImageUrl,
                Number = card.Number
            };
        }

        public static dto.CardCollection AsCardCollectionDto(this CardCollection card)
        {
            return new dto.CardCollection
            {
                ImageUrl = card.ImageUrl,
                Cards = card.Cards.Select(c => c.AsCardDto()).ToList()
            };
        }
    }
}
