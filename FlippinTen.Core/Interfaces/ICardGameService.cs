using FlippinTen.Core.Entities;
using FlippinTen.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlippinTen.Core.Interfaces
{
    public interface ICardGameService
    {
        Task<CardGame> Get(string gameIdentifier, string userIdentifier);
        Task<List<CardGame>> GetByPlayer(string playerName);
        Task<CardGame> Add(string gameName, string user, List<string> opponents);
        Task<bool> Update(CardGame game);
    }
}