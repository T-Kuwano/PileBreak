using PileBreak.Models;
using System.Net.Http.Json;

namespace PileBreak.Services
{
    public class SteamApiService
    {
        private readonly HttpClient _httpClient;
        private readonly DatabaseService _dbService;
        // TODO:サーバ経由で取得する
        //private string apiKey = Preferences.Default.Get("SteamApiKey", string.Empty);
        private string apiKey = Secrets.SteamApiKey;
        private string myId = Preferences.Default.Get("steam_id", string.Empty);

        public SteamApiService(DatabaseService dbService)
        {
            _httpClient = new HttpClient();
            _dbService = dbService;
        }

        public async Task FetchAndSyncSteamGamesAsync()
        {
            if (string.IsNullOrEmpty(myId))
            {
                return;
            }

            var url = $"https://api.steampowered.com/IPlayerService/GetOwnedGames/v1/?key={apiKey}&steamid={myId}&include_appinfo=true&format=json";

            var response = await _httpClient.GetFromJsonAsync<SteamOwnedGamesResponse>(url);

            if (response?.Response?.Games == null) return;

            foreach (var game in response.Response.Games)
            {
                var newItem = new PileContentItem
                {
                    Title = game.Name,
                    Category = "Game",
                    SourceId = game.Appid.ToString(),
                    Icon = $"https://cdn.akamai.steamstatic.com/steam/apps/{game.Appid}/header.jpg",
                    IsCleared = 0
                };

                await _dbService.SaveItemAsync(newItem);
            }
        }
    }

    // --- Steam API レスポンス用の型定義 ---
    public class SteamOwnedGamesResponse
    {
        public GamesContainer Response { get; set; }
    }

    public class GamesContainer
    {
        public List<SteamGameInfo> Games { get; set; }
    }

    public class SteamGameInfo
    {
        public int Appid { get; set; }
        public string Name { get; set; }
        public int Playtime_forever { get; set; }
    }
}