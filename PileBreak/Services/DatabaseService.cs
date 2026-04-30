using PileBreak.Models;
using SQLite;

namespace PileBreak.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        private async Task Init()
        {
            if (_database is not null) return;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "PileBreak.db3");

            // 書き込み権限などを含めたフラグを指定するとより確実です
            _database = new SQLiteAsyncConnection(dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

            // 親クラスではなく、必ず「実際に保存する子クラス」を指定します
            await _database.CreateTableAsync<PileContentItem>();
        }

        public async Task<List<PileContentItem>> GetItemsAsync(bool isCleared)
        {
            await Init();
            int status = isCleared ? 1 : 0;
            return await _database.Table<PileContentItem>()
                                 .Where(i => i.IsCleared == status)
                                 .ToListAsync();
        }

        public async Task SaveItemAsync(PileContentItem item)
        {
            await Init();
            if (item.Id != 0) await _database.UpdateAsync(item);
            else await _database.InsertAsync(item);
        }
    }
}