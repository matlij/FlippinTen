using FlippinTen.Core.Interfaces;
using FlippinTen.Core.Services;
using Models;
using Models.Constants;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FlippinTen.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        private readonly IGameMenuService _gameMenuService;

        public Command LoadGamesCommand { get; private set; }
        public ObservableCollection<GamePlay> OnGoingGames { get; set; } = new ObservableCollection<GamePlay>();

        public MenuViewModel()
        {
            OnGoingGames.Add(new GamePlay(new System.Collections.Generic.List<Player>()) { Name = "TestName" });

            Title = "Spelmeny";
        }

        public MenuViewModel(IGameMenuService gameMenuService)
        {
            _gameMenuService = gameMenuService;

            LoadGamesCommand = new Command(async () => await ExecuteLoadGamesCommand());

            Title = "Spelmeny";
        }

        private async Task ExecuteLoadGamesCommand()
        {
            IsBusy = true;

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
            }
        }

        private async Task RefreshGames()
        {
            OnGoingGames.Clear();

            var games = await _gameMenuService.GetGames(DatabaseConstants.PlayerName);
            if (games.Count > 0)
                games.ForEach(g => OnGoingGames.Add(g));
        }
    }
}
