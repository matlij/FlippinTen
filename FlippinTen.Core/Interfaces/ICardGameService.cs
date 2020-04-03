using FlippinTen.Core.Entities;
using FlippinTen.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlippinTen.Core.Interfaces
{
    public interface ICardGameService
    {
        Task<CardGame> Add(CardGame game);
        Task<List<CardGame>> Get(string playerName);
    }
}