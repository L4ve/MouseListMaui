using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyszkaMauii
{
    public class MouseDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public MouseDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Mouse1>().Wait();
        }

        public Task<List<Mouse1>> GetMiceAsync()
        {
            return _database.Table<Mouse1>().ToListAsync();
        }

        public Task<int> SaveMouseAsync(Mouse1 mouse)
        {
            return _database.InsertAsync(mouse);
        }
    }
}
