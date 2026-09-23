using MEIUtil.Models;
using SQLite;

namespace MEIUtil.Services;

public sealed class DatabaseService
{
    private readonly SemaphoreSlim _initializeLock = new(1, 1);
    private SQLiteAsyncConnection? _database;
    private bool _initialized;

    private async Task<SQLiteAsyncConnection> GetDatabaseAsync()
    {
        if (_initialized && _database is not null)
            return _database;

        await _initializeLock.WaitAsync();
        try
        {
            if (_database is null)
            {
                var path = Path.Combine(FileSystem.AppDataDirectory, "meiutil.db3");
                _database = new SQLiteAsyncConnection(
                    path,
                    SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
            }

            if (!_initialized)
            {
                await _database.CreateTableAsync<Transaction>();
                await _database.CreateTableAsync<Client>();
                await _database.CreateTableAsync<Quote>();
                await _database.CreateTableAsync<ObligationRecord>();
                await _database.CreateTableAsync<MeiProfile>();
                _initialized = true;
            }

            return _database;
        }
        finally
        {
            _initializeLock.Release();
        }
    }

    public async Task<List<Transaction>> GetTransactionsAsync()
        => await (await GetDatabaseAsync()).Table<Transaction>().OrderByDescending(x => x.Date).ToListAsync();

    public async Task<List<Transaction>> GetTransactionsAsync(DateTime start, DateTime end)
        => await (await GetDatabaseAsync()).Table<Transaction>()
            .Where(x => x.Date >= start && x.Date < end)
            .OrderByDescending(x => x.Date)
            .ToListAsync();

    public async Task SaveTransactionAsync(Transaction item)
    {
        ArgumentNullException.ThrowIfNull(item);
        var db = await GetDatabaseAsync();
        if (item.Id == 0)
            await db.InsertAsync(item);
        else
            await db.UpdateAsync(item);
    }

    public async Task DeleteTransactionAsync(Transaction item)
    {
        ArgumentNullException.ThrowIfNull(item);
        await (await GetDatabaseAsync()).DeleteAsync(item);
    }

    public async Task<List<Client>> GetClientsAsync()
        => await (await GetDatabaseAsync()).Table<Client>().OrderBy(x => x.Name).ToListAsync();

    public async Task SaveClientAsync(Client item)
    {
        ArgumentNullException.ThrowIfNull(item);
        var db = await GetDatabaseAsync();
        if (item.Id == 0)
            await db.InsertAsync(item);
        else
            await db.UpdateAsync(item);
    }

    public async Task<List<Quote>> GetQuotesAsync()
        => await (await GetDatabaseAsync()).Table<Quote>().OrderByDescending(x => x.CreatedAt).ToListAsync();

    public async Task SaveQuoteAsync(Quote item)
    {
        ArgumentNullException.ThrowIfNull(item);
        var db = await GetDatabaseAsync();
        if (item.Id == 0)
            await db.InsertAsync(item);
        else
            await db.UpdateAsync(item);
    }

    public async Task<List<ObligationRecord>> GetObligationsAsync()
        => await (await GetDatabaseAsync()).Table<ObligationRecord>().OrderBy(x => x.DueDate).ToListAsync();

    public async Task SaveObligationAsync(ObligationRecord item)
    {
        ArgumentNullException.ThrowIfNull(item);
        var db = await GetDatabaseAsync();
        if (item.Id == 0)
            await db.InsertAsync(item);
        else
            await db.UpdateAsync(item);
    }

    public async Task<MeiProfile> GetProfileAsync()
    {
        var db = await GetDatabaseAsync();
        var profile = await db.FindAsync<MeiProfile>(1);
        if (profile is not null)
            return profile;

        profile = new MeiProfile();
        await db.InsertAsync(profile);
        return profile;
    }

    public async Task SaveProfileAsync(MeiProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);
        profile.Id = 1;
        await (await GetDatabaseAsync()).InsertOrReplaceAsync(profile);
    }
}
