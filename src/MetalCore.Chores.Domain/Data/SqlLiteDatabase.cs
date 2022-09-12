using SQLite;

namespace MetalCore.Chores.Domain.Data
{
    public interface ISqlLiteDatabase
    {
        TableQuery<T> Query<T>()
            where T : class, IEntity, new();

        int Create<T>(T input)
            where T : class, IEntity, new();

        int Update<T>(T input)
            where T : class, IEntity, new();

        int Delete<T>(T input)
            where T : class, IEntity, new();
    }

    public class SqlLiteDatabase : ISqlLiteDatabase
    {
        private readonly SQLiteConnection _database;

        public SqlLiteDatabase()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ChoresData.db");
            _database = new SQLiteConnection(dbPath);
            CreateTables();
        }

        public TableQuery<T> Query<T>()
            where T : class, IEntity, new()
        {
            return _database.Table<T>();
        }

        public int Create<T>(T input)
            where T : class, IEntity, new()
        {
            return _database.Insert(input);
        }

        public int Update<T>(T input)
            where T : class, IEntity, new()
        {
            return _database.Update(input);
        }

        public int Delete<T>(T input)
            where T : class, IEntity, new()
        {
            return _database.Delete(input);
        }

        private void CreateTables()
        {
            _database.CreateTable<MyEntity>();
        }
    }

    public class MyEntity : IEntity
    {
        public string Name { get; set; }
    }
}