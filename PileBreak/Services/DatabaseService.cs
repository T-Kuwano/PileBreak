using PileBreak.Models;
using SQLite;

namespace PileBreak.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        private async Task Init()
        {
            if (_database is null)
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "PileBreak.db3");

                _database = new SQLiteAsyncConnection(dbPath,
                    SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
            }
            await _database.CreateTableAsync<PileContentItem>();
            await _database.CreateTableAsync<SteamRawData>();
        }


        public async Task<List<PileContentItem>> GetItemsAsync(bool isCleared)
        {
            await Init();
            int status = isCleared ? 1 : 0;
            return await _database.Table<PileContentItem>()
                                 .Where(i => i.IsCleared == status)
                                 .ToListAsync();
        }

        // Steam生データ登録処理
        public async Task SaveSteamItemAsync(SteamRawData item)
        {
            await Init();
            await _database.InsertOrReplaceAsync(item);
        }

        public async Task SyncFromSteamAsync(SteamRawData raw)
        {
            await Init();

            var appId = raw.AppId.ToString();
            // 1. 既にDBにあるか探す
            var existingItem = await _database.Table<PileContentItem>()
                                              .Where(x => x.SourceId == appId)
                                              .FirstOrDefaultAsync();

            if (existingItem == null)
            {
                // 初めて見るゲームなら新規登録
                var newItem = new PileContentItem
                {
                    Title = raw.Name,
                    Category = "Game",
                    SourceId = raw.AppId.ToString(),
                    Icon = $"https://cdn.akamai.steamstatic.com/steam/apps/{raw.AppId}/header.jpg",
                    IsCleared = 0
                };
                await _database.InsertAsync(newItem);
            }
            else
            {
                // 既に持っているゲームなら、コメント以外を最新にする
                existingItem.Title = raw.Name;

                await _database.UpdateAsync(existingItem);
            }
        }

        // 表示用テーブル登録、更新処理
        public async Task UpdateItemAsync(PileContentItem item)
        {
            await Init();
            await _database.UpdateAsync(item);
        }

        // 表示用テーブルリセット処理
        public async Task ResetItemAsync()
        {
            await Init();
            await _database.DropTableAsync<PileContentItem>();
            await _database.CreateTableAsync<PileContentItem>();
        }
    }
}