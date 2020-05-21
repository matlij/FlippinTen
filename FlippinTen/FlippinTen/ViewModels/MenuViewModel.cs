using FlippinTen.Core.Entities;
using FlippinTen.Core.Interfaces;
using FlippinTen.Models.Constants;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FlippinTen.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        private readonly ICardGameService _cardGameService;

        public Command LoadGamesCommand { get; private set; }
        public ObservableCollection<CardGame> OnGoingGames { get; set; } = new ObservableCollection<CardGame>();

        public MenuViewModel(ICardGameService cardGameService)
        {
            _cardGameService = cardGameService;

            LoadGamesCommand = new Command(async () => await ExecuteLoadGamesCommand());

            Title = "Spelmeny";
        }

        private async Task ExecuteLoadGamesCommand()
        {
            IsBusy = true;
            Debug.WriteLine($"{DateTime.Now} - LoadGames starting. IsBusy: {IsBusy}.");

            try
            {
                await RefreshGames();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            finally
            {
                IsBusy = false;
                Debug.WriteLine($"{DateTime.Now} - LoadGames complete. IsBusy: {IsBusy}.");
            }
        }

        private async Task RefreshGames()
        {
            OnGoingGames.Clear();

            var games = await _cardGameService.GetByPlayer(DatabaseConstants.PlayerName);
            foreach (var game in games)
            {
                OnGoingGames.Add(game);
            }

            Debug.WriteLine($"{DateTime.Now} - Games refreshed, added '{games.Count}' games.");
        }
    }
}
