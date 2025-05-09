using Microsoft.EntityFrameworkCore;
using WeatherApp.DataAccess.Postgres.Interfaces;
using WeatherApp.DataAccess.Postgres.Models;

namespace WeatherApp.DataAccess.Postgres.Repositories;
public class UsersRepository : IUsersRepository
{
    private readonly WeatherDbContext _dbContext;
    public UsersRepository(WeatherDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    /// <summary>
    /// Вернет всех пользователей
    /// </summary>
    /// <returns></returns>
    public async Task<List<UserEntity>> Get()
    {
        return await _dbContext.Users
            .AsNoTracking() //Отключает проверку изменения данных, ведь операция получения данных не предполагает изменение чего либо 
            .OrderBy(u => u.Id)
            .ToListAsync();
    }
    public async Task<List<UserEntity>> GetWithRequests()
    {
        return await _dbContext.Users
            .AsNoTracking() //Отключает проверку изменения данных, ведь операция получения данных не предполагает изменение чего либо 
            .Include(u => u.SearchRequests)
            .ToListAsync();
    }
    public async Task<UserEntity?> GetWithRequests(string email)
    {
        return await _dbContext
            .Users
            .AsNoTracking()
            .Include(u => u.SearchRequests)
            .FirstOrDefaultAsync(u => u.Email == email);
    }
    public async Task<UserEntity?> GetById(Guid id)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }
    public async Task<List<UserEntity>> GetByFilter(DateTime createdAt)
    {
        var query = _dbContext.Users.AsNoTracking();

        if (createdAt != default)
        {
            query = query.Where(u => u.CreatedAt > createdAt);
        }

        return await query.ToListAsync();
    }
    public async Task<UserEntity?> GetByEmail(string email)
    {
        return await _dbContext
                  .Users
                  .AsNoTracking()
                  .FirstOrDefaultAsync(u => u.Email.Equals(email));
    }
    public async Task<List<UserEntity>> GetByPage(int page, int pageSize)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<bool> Exists(string email)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == email);
    }
    public async Task Add(Guid id, string email, string password, DateTime createdAt)
    {
        var user = new UserEntity()
        {
            Id = id,
            Email = email,
            HashedPassword = password,
            CreatedAt = createdAt
        };
        await _dbContext.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }
    public async Task Update(Guid id, string email, string hashedPassword, DateTime createdAt)
    {
        await _dbContext.Users
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(u => u
                .SetProperty(p => p.Email, email)
                .SetProperty(p => p.HashedPassword, hashedPassword)
                .SetProperty(p => p.CreatedAt, createdAt));
    }
    public async Task Delete(Guid id)
    {
        await _dbContext.Users
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync();
    }
}