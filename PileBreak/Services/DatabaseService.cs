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
            await _database.CreateTableAsync<SteamGameInfo>();
        }

        public async Task<List<PileContentItem>> GetItemsAsync(bool isCleared)
        {
            await Init();
            int status = isCleared ? 1 : 0;
            return await _database.Table<PileContentItem>()
                                 .Where(i => i.IsCleared == status)
                                 .ToListAsync();
        }
        public async Task SaveSteamItemAsync(SteamGameInfo item)
        {
            await Init();
            await _database.InsertOrReplaceAsync(item);
        }
        public async Task SaveItemAsync(PileContentItem item)
        {
            await Init();
            if (item.Id != 0) await _database.UpdateAsync(item);
            else await _database.InsertAsync(item);
        }

        public async Task ResetItemAsync()
        {
            await Init();
            await _database.DropTableAsync<PileContentItem>();
            await _database.CreateTableAsync<PileContentItem>();
        }
    }
}