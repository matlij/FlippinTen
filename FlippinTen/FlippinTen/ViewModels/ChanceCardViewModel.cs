using FlippinTen.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlippinTen.ViewModels
{
    public class ChanceCardViewModel : BaseViewModel
    {
        public string CardImageUrl { get; set; }
        public string CardBack { get; set; }

        public ChanceCardViewModel(string cardImageUrl)
        {
            CardBack = ImageConstants.CardBack;
            CardImageUrl = cardImageUrl;
        }
    }
}
