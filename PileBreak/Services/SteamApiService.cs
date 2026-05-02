using PileBreak.Models;
using System.Net.Http.Json;

namespace PileBreak.Services
{
    public class SteamApiService
    {
        private readonly HttpClient _httpClient;
        private readonly DatabaseService _dbService;
        private string apiKey = Secrets.SteamApiKey;
        private string myId = Preferences.Default.Get("SavedSteamId", string.Empty);

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
            var threshold = Preferences.Default.Get("SavedThreshold", 0);
            foreach (var game in response.Response.Games)
            {
                if (game.PlaytimeForever > threshold) continue;
                // Steamからのデータを一応保存
                await _dbService.SaveSteamItemAsync(game);
                await _dbService.SyncFromSteamAsync(game);
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
        public List<SteamRawData> Games { get; set; }
    }
}